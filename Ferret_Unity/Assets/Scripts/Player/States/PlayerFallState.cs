using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : IState
{

	PlayerManager m_playerManager;

	// Constructor (CTOR)
  public PlayerFallState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    //Debug.LogFormat("{0} : Enter()", GetType().Name);
  }

  public void Exit()
  {
    //Debug.LogFormat("{0} : Exit()", GetType().Name);
  }

  public void FixedUpdate()
  {
    Move();
  }

  public void Update()
  {
    if(m_playerManager.CheckCollider(false)){
      m_playerManager.ChangeState(0);
    }
  }

  void Move(){
    m_playerManager.MovePlayer(m_playerManager.m_states.m_walk.m_speed);
    m_playerManager.RotatePlayer();
  }

}
