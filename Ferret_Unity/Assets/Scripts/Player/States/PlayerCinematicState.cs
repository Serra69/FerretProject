using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCinematicState : IState {

	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerCinematicState (PlayerManager playerManager)
    {
		m_playerManager = playerManager;
    }

    public void Enter()
    {
    	m_playerManager.Animator.SetTrigger("Idle");
    }

    public void Exit()
    {

    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {

    }

    public void LateUpdate(){
      
    }

}
