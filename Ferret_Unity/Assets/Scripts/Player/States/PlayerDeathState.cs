using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : IState
{
	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerDeathState (PlayerManager playerManager)
    {
		  m_playerManager = playerManager;
    }

    public void Enter()
    {
      Debug.Log("YOU DIE!");
      m_playerManager.PlayerIsDead = true;
      // m_playerManager.Animator.SetTrigger("Idle");
      m_playerManager.Rigidbody.Sleep();
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
      
    }

    public void Exit()
    {
      m_playerManager.PlayerIsDead = false;
    }

}
