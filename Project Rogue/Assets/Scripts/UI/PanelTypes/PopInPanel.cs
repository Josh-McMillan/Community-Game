using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
//[AddComponentMenu("UI/Blur Panel")]
public class PopInPanel : Image
{
   
    [SerializeField]
    private float fadeTime = 0.5f;
    [SerializeField]
    private float popTime = 0.2f;
    [SerializeField]
    private float fadeInDelay = 0f;
    [SerializeField]
    private float popInDelay = 0f;
    [SerializeField]
    private Vector3 startScale;


    [SerializeField]
    private LeanTweenType popScaleTween;

    [SerializeField]
    private float closeTimeMult = 2;

    [SerializeField]
    private bool ignorTimeScale;

    CanvasGroup canvas;


    protected override void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    protected override void OnEnable()
    {
        if (Application.isPlaying)
        {
            material.SetFloat("_Size", 0);
            canvas.alpha = 0;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
            transform.localScale = startScale;

            LeanTween.value(gameObject, UpdateAlpha, 0, 1, fadeTime).setDelay(fadeInDelay).setIgnoreTimeScale(ignorTimeScale);
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), popTime).setDelay(popInDelay).setEase(popScaleTween).setIgnoreTimeScale(ignorTimeScale) ;//.setOnComplete();
        }
    }

    protected override void OnDisable()
    {
        if (Application.isPlaying)
        {
            canvas.interactable = false;
            canvas.blocksRaycasts = false;

            LeanTween.scale(gameObject, startScale, popTime * closeTimeMult).setEase(popScaleTween).setIgnoreTimeScale(ignorTimeScale);//.setOnComplete();
            LeanTween.value(gameObject, UpdateAlpha, 1, 0, fadeTime * closeTimeMult).setIgnoreTimeScale(ignorTimeScale);
        }
    }

    void UpdateAlpha(float value)
    {
        canvas.alpha = value;
    }
}
