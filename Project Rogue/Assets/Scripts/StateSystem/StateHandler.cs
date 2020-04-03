using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateHandler : MonoBehaviour
{
#region STATES
    [Flags]
    public enum State : byte
    {
        Active = 1 << 0,
        Downed = 1 << 1,
        Dead = 1 << 2,
        Null = 1 << 3 //This should always be the last state, we use this as a default, but it should be set to correct state onload, if not there is an error
    }

    private State m_CurrentState = State.Null;
    public bool CheckState(State stateFlag)
    {
        if ((m_CurrentState & stateFlag) != 0)
        {
            return true;
        }
        return false;
    }
    private void SetState(State stateFlag)
    {
        m_CurrentState = stateFlag;
        OnStateChangeEvent();
    }
#endregion
#region DELEGATE_EVENTS
    public delegate void OnStateChange(State newState);
    private OnStateChange m_MethodsToCall;

    public void SubscribeToEvents(OnStateChange methodToCall)
    {
        m_MethodsToCall += methodToCall;
    }
    public void UnsubscribeToEvents(OnStateChange methodToCall)
    {
        m_MethodsToCall -= methodToCall;
    }

    private void OnStateChangeEvent()
    {
        if (m_MethodsToCall != null)
        {
            m_MethodsToCall(m_CurrentState);
        }
    }
#endregion

}
