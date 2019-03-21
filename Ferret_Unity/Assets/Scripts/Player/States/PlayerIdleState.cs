using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerIdleState (PlayerManager playerManager)
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
		if(m_playerManager.PlayerInputIsMoving()){
			m_playerManager.ChangeState(1);
		}

		if(m_playerManager.m_crawlButton){
			if(!m_playerManager.CheckTopCollider()){
				m_playerManager.ChangeState(3);
			}
		}
    }

}
