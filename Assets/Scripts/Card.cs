using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int cardID; // To Identify Pairs
    public Image frontImage;
    public Image backImage;
    public float cardFlipDuration;

    public bool isMatched { get; private set; }
    public bool isFaceUp { get; private set; }

    // Lock so that the card will not flip during animation
    bool animating = false;

    public event Action<Card> OnFlippedFaceUp; // when flip ends and card is face up
    public event Action<Card> OnFlippedFaceDown; // when front is hidden again

    void Awake()
    {
        // Ensures the intial state
        SetFace(false, instant: true);
    }

    public void Initialize(int id, Sprite faceSprite, Sprite backSprite)
    {
        cardID = id;

        if(frontImage != null)
        {
            frontImage.sprite = faceSprite;
        }
        if(backImage != null)
        {
            backImage.sprite = backSprite;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMatched || animating) return;
        FlipToFace();
    }

    public void ForceSetMatched()
    {
        isMatched = true;
        // optionally disable interactivity
    }

    public void FlipToFace()
    {
        if (isFaceUp || animating || isMatched) return;
        StartCoroutine(Flip(true));
    }

    public void FlipToBack()
    {
        if (!isFaceUp || animating || isMatched) return;
        StartCoroutine(Flip(false));
    }

    public void SetFace(bool faceUp, bool instant = false)
    {
        isFaceUp = faceUp;
        if (instant)
        {
            float rotY = faceUp ? 180f : 0f;
            transform.localEulerAngles = new Vector3(0, rotY, 0);
            frontImage.gameObject.SetActive(faceUp);
            backImage.gameObject.SetActive(!faceUp);
        }
        else
        {
            if (faceUp) FlipToFace(); else FlipToBack();
        }
    }

    IEnumerator Flip(bool toFace)
    {
        animating = true;
        float half = cardFlipDuration * 0.5f;
        float t = 0f;
        float start = transform.localEulerAngles.y;
        // Normalize start to 0..360
        start = Mathf.Repeat(start, 360f);
        float end = toFace ? 180f : 0f;

        // rotate first half
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            float y = Mathf.LerpAngle(start, (start + (end - start) / 2f), p);
            transform.localEulerAngles = new Vector3(0, y, 0);
            yield return null;
        }

        // toggle visuals at flip midpoint
        frontImage.gameObject.SetActive(toFace);
        backImage.gameObject.SetActive(!toFace);

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            float y = Mathf.LerpAngle((start + (end - start) / 2f), end, p);
            transform.localEulerAngles = new Vector3(0, y, 0);
            yield return null;
        }

        isFaceUp = toFace;
        animating = false;

        if (isFaceUp) OnFlippedFaceUp?.Invoke(this);
        else OnFlippedFaceDown?.Invoke(this);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
