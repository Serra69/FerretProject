using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotCollisionState : IState
{
	PlayerManager m_playerManager;

	// Constructor (CTOR)
  public PlayerRobotCollisionState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    //Debug.LogFormat("{0} : Enter()", GetType().Name);
    m_playerManager.SetPlayerParent(m_playerManager.m_chargedRobot);
    m_playerManager.Rigidbody.isKinematic = true;
    m_playerManager.Rigidbody.useGravity = false;
  }

  public void FixedUpdate()
  {
    
  }

  public void Update()
  {
    
  }

  public void LateUpdate(){
    
  }

  public void Exit()
  {
    //Debug.LogFormat("{0} : Exit()", GetType().Name);
    m_playerManager.Rigidbody.isKinematic = false;
    m_playerManager.Rigidbody.useGravity = true;
    m_playerManager.SetPlayerParent(m_playerManager.m_chargedRobot, true);
  }

}
