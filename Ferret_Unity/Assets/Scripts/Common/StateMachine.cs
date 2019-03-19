using System.Collections.Generic;

public class StateMachine
{
    List<IState> m_states = null;

    IState m_currentState = null; // = null car elle n'a pas d'état courant, elle n'est pas initialisée

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
        m_currentState = m_states[index];
        m_currentState.Enter();
    }
    #endregion // Methods

}
