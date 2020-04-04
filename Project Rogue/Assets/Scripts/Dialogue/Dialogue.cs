using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//XML
using System.Linq;
using System.Xml.Linq;

//UI
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {

    //XML
    XDocument m_Doc; //Dialogue text
    XElement m_Root; //Root node

    //UI
    [Header("UI")]
    [SerializeField] private GameObject m_DialoguePopUp; //UI popup
    [SerializeField] private Text m_DialogueText; //Text to display
    [Space(0.1f)]
    [SerializeField] private float m_fDisplaySpeed = 10; //Number fo characters to display per second

    //Management
    List<QueuedDialogue> m_QueuedDialogue = new List<QueuedDialogue>(); //List of dialogue to display

    /// <summary>
    /// Queued Dialogue struct
    /// Holds infomation about the stored dialogue inluding how long it should be displayed for
    /// </summary>
    struct QueuedDialogue
    {
        public XName sceneName;
        public uint id;
        public float delay;

        public QueuedDialogue(XName sceneName, uint id, float delay) //Constructor
        {
            this.sceneName = sceneName;
            this.id = id;

            this.delay = delay;
        }
    };

    public delegate void Event(); //Dialogue event delegate (used for storing function pointer)
    private Event m_Event;
	// Use this for initialization
	void Awake () {
        TextAsset doc = Resources.Load("Dialogue") as TextAsset; //Load in text from xml file
        if (!doc) { Debug.LogWarning("Dialgue: Start: Failed to load file."); return; } //Error check
        Assert.IsTrue(doc.text.Length > 0);  //Make sure we have text not an empty or incorrectly loaded file
        m_Doc = XDocument.Parse(doc.text); //Pass the text into a xml document format
        m_Root = m_Doc.Root; //Get the root node
    }
	
    public void Push(XName sceneName, uint id, float delay) //Push dialogue to the list
    {
        m_QueuedDialogue.Add(new QueuedDialogue(sceneName, id, delay));
    }

    public void Flush(bool close, Event trigger = null) //Flush and call a optional function once it is compelte
    {
        StartCoroutine(FlushDialogue(trigger, close));
    }

    private IEnumerator FlushDialogue(Event trigger, bool close)
    {
        m_DialoguePopUp.SetActive(true); //Display dialogue popup
        while (m_QueuedDialogue.Count() > 0) //Loop through the list
        {
            QueuedDialogue queuedDialogue = m_QueuedDialogue[0]; //Get first element
            yield return DisplayText(queuedDialogue.sceneName, queuedDialogue.id); //Call display
            m_QueuedDialogue.RemoveAt(0); //Remove first element
            yield return new WaitForSeconds(queuedDialogue.delay); //Wait
        }
        m_DialoguePopUp.SetActive(!close); //Close the dialogue box if it close is true
        if (trigger != null) //If there is a function event call it
            trigger();
    }

    private IEnumerator DisplayText(XName sceneName, uint id)
    {
        string text = ""; //Set empty string
        XElement scene = m_Root.Element(sceneName); //Get scene node
        Assert.IsTrue(scene != null);
        foreach (XElement ele in scene.Elements()) //Loop through and find correct dialogue based on id
        {
            if (ele.FirstAttribute.Value == id.ToString())
            {
                text = ele.Value;
            }
        }

        if (text.Length <= 0) //check if the dialogue exsists
        {
            Debug.LogWarning(string.Format("Dialogue: getDialogue: Failed to find element in scene {0} with id {1}", sceneName, id));
            yield break;
        }

        WaitForSeconds wait = new WaitForSeconds(1.0f / m_fDisplaySpeed); //Set up wait

        text = text.Replace("\\n", "\n"); //Replace default new line character with one recognised by unity
        m_DialogueText.text = ""; //Set dialogue text to be empty

        foreach(char c in text) //loop through each character and print them out
        {
            m_DialogueText.text += c;
            yield return wait;
        }
    }
}
