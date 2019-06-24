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
      m_playerManager.Rigidbody.useGravity = false;
      m_playerManager.Rigidbody.isKinematic = true;
    }

    public void Exit()
    {
      m_playerManager.Rigidbody.useGravity = true;
      m_playerManager.Rigidbody.isKinematic = false;
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
