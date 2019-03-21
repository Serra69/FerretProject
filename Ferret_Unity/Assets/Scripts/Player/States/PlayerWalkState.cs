using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : IState
{

	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerWalkState (PlayerManager playerManager)
    {
		m_playerManager = playerManager;
    }

    public void Enter()
    {
		Debug.LogFormat("{0} : Enter()", GetType().Name);
    }

    public void Exit()
    {
		Debug.LogFormat("{0} : Exit()", GetType().Name);
    }

    public void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
		if(!m_playerManager.PlayerInputIsMoving()){
			m_playerManager.ChangeState(0);
		}

		if(m_playerManager.m_runButton){
			m_playerManager.ChangeState(2);
		}

		m_playerManager.MovePlayer(m_playerManager.m_states.m_walk.m_speed);
		m_playerManager.RotatePlayer();
    }

}
