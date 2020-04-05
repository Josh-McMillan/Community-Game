using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{
    private SelectionWheelSlot[] tabButtons;
    [SerializeField]
    private bool windowActive;
    private PopInPanel DisplayPanel;
    [Header("Intraction Colours")]
    [SerializeField]
    private Color tabIdle = Color.grey;
    [SerializeField]
    private Color tabHovered = Color.white;
    [SerializeField]
    private Color tabSelected = Color.cyan;
    [SerializeField]
    [Range(0, 2)]
    private float hoverSize = 1.05f;
    [Header("Intraction Audio")]
    private AudioSource audio;
    [SerializeField]
    private AudioClip soundSelect;
    [SerializeField]
    private AudioClip soundMouseOver;
    [SerializeField]
    private AudioClip soundOpenWheel;

    [Header("Active Tab")]
    public SelectionWheelSlot selectedTab;
    //the portion the mouse was last above
    private SelectionWheelSlot mouseOverTab;

    void Start()
    {
        tabButtons = GetComponentsInChildren<SelectionWheelSlot>();
        DisplayPanel = GetComponent<PopInPanel>();
        selectedTab = tabButtons[0];
        OnTabSelected(selectedTab);
       // UpdateWheelDisplays(0);
        ResetTabs();
        SetDisplayAction();
        if (GetComponent<AudioSource>())
        {
            audio = GetComponent<AudioSource>();
        }
    }

    [Header("Wheel Display Settings")]
    [SerializeField]
    [Range(0, 9)]
    private int displayAmount = 6;
    [SerializeField]
    [Range(0, .3f)]
    [Tooltip("How much space do you want between slots")]
    private float portionSpacing = .02f;

    //Used so changes in display amount can be update in editor, this can be removed
    private int lastAmount;

    public void SetDisplayAction(bool SetTo)
    {
        windowActive = SetTo;
        SetDisplayAction();
    }
    void SetDisplayAction()
    {
        if (windowActive)
        {
            UpdateWheelDisplays();
            if (audio != null)
            {
                audio.clip = soundOpenWheel;
                audio.pitch = 1;
                audio.Play();
            }
            DisplayPanel.enabled = true;
        }
        else
        {
            UpdateWheelDisplays(0);
            if (audio != null)
            {
                audio.clip = soundOpenWheel;
                audio.pitch = .8f;
                audio.Play();
            }
            DisplayPanel.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetDisplayAction(!windowActive);
        }
        if (windowActive)
        {
            UpdateCurrentSelection();
            if (Input.GetMouseButtonDown(0))
            {
                OnTabSelected(mouseOverTab);
            }

            //This is so the amount of slots can be change dynamicly from the editor
            if (lastAmount != displayAmount)
            {
                UpdateWheelDisplays();
                lastAmount = displayAmount;
            }
        }

    }


    #region DisplayControls


    //Determines which slot the mouse is over
    void UpdateCurrentSelection()
    {
        //Get diffrance between rectTransform center and mouse position as angle
        float mouseAngle = Vector3.Angle(Vector3.up, Input.mousePosition - transform.position);
        if (Input.mousePosition.x > transform.position.x) { mouseAngle = 360 - mouseAngle; }

        int selected = Mathf.CeilToInt( mouseAngle / (360 / (float)displayAmount));

        //Get mouse over and check if it's new
        SelectionWheelSlot NewMouseOver = tabButtons[Mathf.Clamp( selected - 1, 0, tabButtons.Length)];
        if(NewMouseOver != mouseOverTab)
        {
            if(mouseOverTab != null)
            {
                OnTabExit(mouseOverTab);
            }
            mouseOverTab = NewMouseOver;
            OnTabEnter(mouseOverTab);
        }
        
    }

    //If you want to change how many slots are shown
    public void SetAmountOfSlotsDisplayed(int NewAmount)
    {
        displayAmount = NewAmount;
        UpdateWheelDisplays();
    }
    void UpdateWheelDisplays()
    {
        UpdateWheelDisplays(displayAmount);
    }
    void UpdateWheelDisplays(int TargetDisplay)
    {
        if (TargetDisplay == 0)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                
                tabButtons[i].DisableSlot(360);
            }
            return;
        }

        TargetDisplay = Mathf.Clamp(TargetDisplay, 1, tabButtons.Length);
        float RadialFill = (1f / (float)TargetDisplay) - portionSpacing;
        float AngleGap = 360 / (float)TargetDisplay;
        float StartOffset = 360 * portionSpacing / 2;

        float lastAngle = AngleGap;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (i < TargetDisplay)
            {
                lastAngle = ( AngleGap * (i + 1));
                tabButtons[i].EnableSlot();
                tabButtons[i].SetNewPlacement(RadialFill, (StartOffset + AngleGap * ( i)));
            }
            else
            {
                tabButtons[i].DisableSlot(lastAngle);
            }
        }
    }


    #endregion DisplayControls

    #region TabSelections&Actions
    public void OnTabEnter(SelectionWheelSlot button)
    {
        if (selectedTab == null || button != selectedTab)
        {
            ResetTabs();
            button.background.color = tabHovered;
            button.background.rectTransform.localScale = Vector3.one * hoverSize;
            if (audio != null)
            {
                audio.clip = soundMouseOver;
                audio.Play();
            }
        }
    }
    public void OnTabExit(SelectionWheelSlot button)
    {
        ResetTabs();
    }
    public void OnTabSelected(SelectionWheelSlot button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;

        selectedTab.Select();

        if (audio != null)
        {
            audio.clip = soundSelect;
            audio.Play();
        }
        ResetTabs();
        button.background.color = tabSelected;

        //If you want to apply a check, based on which tag is selected, here is where to do it
        //You can use
    }
    public void ResetTabs()
    {
        foreach (SelectionWheelSlot button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.rectTransform.localScale = Vector3.one;
            button.background.color = tabIdle;
        }
    }
    #endregion TabSelections&Actions
}
