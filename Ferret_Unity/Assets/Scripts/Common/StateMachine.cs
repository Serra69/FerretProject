using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine
{
    [SerializeField] string m_currentStateString;

    List<IState> m_states = null;
    public List<IState> States{
        get{
            return m_states;
        }
    }

    IState m_currentState = null; // = null car elle n'a pas d'état courant, elle n'est pas initialisée
    public IState CurrentState
    {
        get
        {
            return m_currentState;
        }
    }

    IState m_lastState = null;
    public IState LastState
    {
        get
        {
            return m_lastState;
        }
    }


    #region Methods

    public void AddStates(List<IState> statesAdded)
    {
        if (m_states == null)
        {
            m_states = new List<IState>();
        }
        m_states.AddRange(statesAdded);
    }

    public void Start()
    {
        if (m_states != null && m_states.Count != 0)
        {
            ChangeState(0);
        }
    }

    public void Stop()
    {
        if (m_currentState != null)
        {
            m_currentState.Exit();
            m_currentState = null;
        }
    }

    public void Update()
    {
        if (m_currentState != null)
        {
            m_currentState.Update();
        }
    }

    public void FixedUpdate()
    {
        if (m_currentState != null)
        {
            m_currentState.FixedUpdate();
        }
    }

    public void ChangeState(int index)
    {
        if (index > m_states.Count - 1)
        {
            return;
        }
        if (m_currentState != null)
        {
            m_currentState.Exit();
        }

        m_lastState = m_currentState;
        
        m_currentState = m_states[index];
        m_currentState.Enter();
        m_currentStateString = m_currentState.GetType().Name;
    }

    public bool CompareState(int stateIndex){
        return m_states[stateIndex] == CurrentState;
    }

    public bool IsLastStateIndex(int index){
        return m_lastState == m_states[index];
    }

    /*public int ReturnLastState(){
        /*if(IsLastStateIndex(0)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromIdle;
        }else if(IsLastStateIndex(1)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
        }else if(IsLastStateIndex(2)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
        }else if(IsLastStateIndex(3)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
        }else if(IsLastStateIndex(4)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
        }else if(IsLastStateIndex(5)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromIdle;
        }else if(IsLastStateIndex(6)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
        }else if(IsLastStateIndex(7)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
        }else if(IsLastStateIndex(8)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
        }else if(IsLastStateIndex(9)){
            m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
        }*

        for (int i = 0, l = m_states.Count; i < l; ++i){
            if(IsLastStateIndex(i)){
                return i;
            }else{
                return 0;
            }
        }
    }*/

    #endregion // Methods

}
