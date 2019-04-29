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
    m_playerManager.Animator.SetTrigger("Fall");
    m_playerManager.m_colliders.m_base.m_headColl.enabled = false;
  }

  public void Exit()
  {
    //Debug.LogFormat("{0} : Exit()", GetType().Name);
    m_playerManager.m_colliders.m_base.m_headColl.enabled = true;
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
    m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed);

    if(SwitchCamera.Instance.ThirdPersonMode){
      m_playerManager.RotatePlayer();
    }else{
      FirstPersonCamera.Instance.RotateCamera();
    }
  }

}
