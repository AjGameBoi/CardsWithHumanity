using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public RectTransform cardParent;
    public GameObject cardPrefab;
    public Sprite[] faceSprites;
    public Sprite backSprite;

    public int columns;
    public int rows;
    public int pointsPerMatch;
    public int penaltyForMismatch;

    public TextMeshProUGUI scoreText;

    public float mismatchRevealTime;
    public float previewTime;

    public bool isPreviewing { get; private set; } = false;

    int totalCards;
    int totalPairs;
    int score;

    List<Card> allCards = new List<Card>();

    // Queue for cards waiting to be compared
    Queue<Card> faceUpQueue = new Queue<Card>();
    HashSet<Card> lockedForCompare = new HashSet<Card>();

    // Lock to prevent flipping other cards during a pair flip/compare
    private bool _isComparing = false;
    public bool IsComparing => _isComparing;

    private void Start()
    {
        SetupBoard(columns, rows);
        UpdateScoreUI();
    }

    public void SetupBoard(int cols, int rows)
    {
        columns = cols;
        this.rows = rows;
        boardManager.SetupGrid(cols, rows);

        totalCards = cols * rows;
        if (totalCards % 2 != 0) totalCards--;
        totalPairs = totalCards / 2;

        foreach (Transform t in cardParent) Destroy(t.gameObject);
        allCards.Clear();
        faceUpQueue.Clear();
        lockedForCompare.Clear();

        // Generate pair IDs
        List<int> pairIds = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            pairIds.Add(i);
            pairIds.Add(i);
        }
        pairIds = pairIds.OrderBy(x => Random.value).ToList();

        // Instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardParent);
            Card c = go.GetComponent<Card>();
            c.gameManager = this; // Letting the card access the manager
            int pid = pairIds[i];
            Sprite faceSprite = faceSprites[pid % faceSprites.Length];
            c.Initialize(pid, faceSprite, backSprite);
            allCards.Add(c);
        }

        boardManager.UpdateGrid();
        StartCoroutine(PreviewCards());
    }

    // For showing the cards at the beginning
    IEnumerator PreviewCards()
    {
        isPreviewing = true;

        // Instantly show all cards face up
        foreach (Card c in allCards)
        c.SetFace(true, instant: true);

        yield return new WaitForSeconds(previewTime);

        // Smoothly flip them all back
        foreach (Card c in allCards)
        c.SetFace(false, instant: false);

        isPreviewing = false;
    }

    // Called by a card when it is clicked and wants to flip
    public void RequestFlip(Card card)
    {
        if (_isComparing || card.isMatched || card.isFaceUp || lockedForCompare.Contains(card)) return;

        // Start the flip
        StartCoroutine(HandleCardFlip(card));
    }

    IEnumerator HandleCardFlip(Card card)
    {
        SceneLoader.Instance.PlayFlip();
        // Flip the card
        yield return card.FlipToFaceCoroutine();

        // Add to queue
        faceUpQueue.Enqueue(card);

        // Start comparison if 2 cards are face up
        if (faceUpQueue.Count >= 2)
        {
            Card a = faceUpQueue.Dequeue();
            Card b = faceUpQueue.Dequeue();

            lockedForCompare.Add(a);
            lockedForCompare.Add(b);

            _isComparing = true;

            yield return ComparePair(a, b);

            lockedForCompare.Remove(a);
            lockedForCompare.Remove(b);
            _isComparing = false;

            // Check for game over
            if (allCards.All(c => c.isMatched))
            {
                OnGameOver();
            }
        }
    }

    IEnumerator ComparePair(Card a, Card b)
    {
        if (a.cardID == b.cardID)
        {
            // Match
            a.ForceSetMatched();
            b.ForceSetMatched();
            score += pointsPerMatch;
            UpdateScoreUI();

            SceneLoader.Instance.PlayMatch();
        }
        else
        {
            SceneLoader.Instance.PlayMismatch();
            // Wait a bit so player can see the mismatch
            yield return new WaitForSeconds(mismatchRevealTime);

            if (!a.isMatched) yield return a.FlipToBackCoroutine();
            if (!b.isMatched) yield return b.FlipToBackCoroutine();

            score = Mathf.Max(0, score - penaltyForMismatch);
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) 
        {
            scoreText.text = $"Score: {score}";
        }
    }

    void OnGameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        SceneLoader.Instance.PlayGameOver();
        // Store the final score in SceneLoader
        SceneLoader.SetFinalScore(score);
        SceneLoader.Instance.LoadGameOver();
    }
}

