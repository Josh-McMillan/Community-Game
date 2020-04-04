using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerDialogue : MonoBehaviour {

    [SerializeField] private Dialogue m_Dialogue;
    [SerializeField] private string m_sSceneName = "Test";
    [SerializeField] private uint m_iID = 1;
    [SerializeField] private float m_fDisplayTime = 2.0f;
    private bool m_bTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_bTriggered) return;
        if (other.tag == "Player")
        {
            m_bTriggered = true;
            m_Dialogue.Push(m_sSceneName, m_iID, m_fDisplayTime);
            m_Dialogue.Flush(true);
        }
    }
}
