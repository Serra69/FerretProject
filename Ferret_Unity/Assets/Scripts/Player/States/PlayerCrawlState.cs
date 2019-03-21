using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrawlState : IState {

    PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerCrawlState (PlayerManager playerManager)
    {
		m_playerManager = playerManager;
    }

    public void Enter()
    {
		Debug.LogFormat("{0} : Enter()", GetType().Name);
		m_playerManager.Crawl(true);
    }

    public void Exit()
    {
		m_playerManager.Crawl(false);
		Debug.LogFormat("{0} : Exit()", GetType().Name);
    }

    public void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        if(m_playerManager.m_crawlButton){
			if(!m_playerManager.CheckTopCollider()){
				m_playerManager.ChangeState(0);
			}
		}

		m_playerManager.MovePlayer(m_playerManager.m_states.m_crawl.m_speed);
		m_playerManager.RotatePlayer();
    }
	
}
