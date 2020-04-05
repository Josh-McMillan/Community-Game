using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class SelectionWheelSlot : MonoBehaviour
{
    public Image background;
    [SerializeField]
    private UnityEvent onTabSelected;
    [SerializeField]
    private UnityEvent onTabDeselected;

    [SerializeField]
    public RectTransform iconRect;
    [SerializeField]
    private float iconDis = 150;

    [SerializeField]
    private float time = 0.3f;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
    }
    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }

    #region OnWheelAnimation
    //These are stored values for the slots animatons
    private float targetAngle;
    private float targetFill;

    public void SetNewPlacement(float NewSize, float NewRot)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, UpdateRot, targetAngle, NewRot, time).setIgnoreTimeScale(true);
        LeanTween.value(gameObject, UpdateFill, targetFill, NewSize, time).setIgnoreTimeScale(true);
        targetFill = NewSize;
        targetAngle = NewRot;

    }

    void UpdateFill(float Value)
    {
        background.fillAmount = Value;
        iconRect.localPosition = Quaternion.Euler(new Vector3(0, 0, Value * 180) ) * (Vector3.up * iconDis);
        iconRect.rotation = Quaternion.Euler(Vector3.up);
    }
    void UpdateRot(float Value)
    {
        background.rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, Value));

    }


    public void EnableSlot()
    {
        if (!gameObject.active)
        {
            UpdateFill(0);
            UpdateRot(0);
        }
        iconRect.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    public void DisableSlot(float LastAngle)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, UpdateFill, background.fillAmount, 0, time / 1.5f);
        LeanTween.value(gameObject, UpdateRot, targetAngle, LastAngle, time / 1.5f);//.setOnComplete(TurnOff);
        iconRect.gameObject.SetActive(false);
        targetFill = 0;
        targetAngle = LastAngle;
    }
    void TurnOff()
    {
        gameObject.SetActive(false);
    }

    #endregion OnWheelAnimation
}
