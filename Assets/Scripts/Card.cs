using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int cardID;
    public Image frontImage;
    public Image backImage;
    public float cardFlipDuration = 0.4f;

    public bool isMatched { get; private set; }
    public bool isFaceUp { get; private set; }

    [HideInInspector] public GameManager gameManager;

    private bool animating = false;

    public void Initialize(int id, Sprite faceSprite, Sprite backSprite)
    {
        cardID = id;
        if (frontImage != null) frontImage.sprite = faceSprite;
        if (backImage != null) backImage.sprite = backSprite;
        SetFace(false, true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animating || isMatched) return;

        if (gameManager != null)
            gameManager.RequestFlip(this);
        else
            StartCoroutine(FlipToFaceCoroutine()); // fallback
    }

    public void ForceSetMatched()
    {
        isMatched = true;
    }

    public void SetFace(bool faceUp, bool instant = false)
    {
        isFaceUp = faceUp;
        float y = faceUp ? 180f : 0f;

        if (instant)
        {
            transform.localEulerAngles = new Vector3(0, y, 0);
            frontImage.gameObject.SetActive(faceUp);
            backImage.gameObject.SetActive(!faceUp);
        }
        else
        {
            if (faceUp) StartCoroutine(FlipToFaceCoroutine());
            else StartCoroutine(FlipToBackCoroutine());
        }
    }

    public IEnumerator FlipToFaceCoroutine()
    {
        if (animating || isFaceUp) yield break;
        yield return FlipCoroutine(true);
    }

    public IEnumerator FlipToBackCoroutine()
    {
        if (animating || !isFaceUp) yield break;
        yield return FlipCoroutine(false);
    }

    IEnumerator FlipCoroutine(bool toFace)
    {
        animating = true;
        float half = cardFlipDuration * 0.5f;
        float t = 0f;
        float start = transform.localEulerAngles.y;
        start = Mathf.Repeat(start, 360f);
        float end = toFace ? 180f : 0f;

        // First half rotation
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            float y = Mathf.LerpAngle(start, (start + (end - start) / 2f), p);
            transform.localEulerAngles = new Vector3(0, y, 0);
            yield return null;
        }

        // Toggle visuals
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
    }
}
