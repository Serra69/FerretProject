using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : IState
{
	PlayerManager m_playerManager;

  float m_timer;
  bool[] m_timerIsPassed = new bool[10];

	// Constructor (CTOR)
  public PlayerDeathState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    m_timer = 0;
    m_playerManager.PlayerIsDead = true;
    // m_playerManager.Animator.SetTrigger("Idle");
    m_playerManager.Rigidbody.Sleep();
  }

  public void Update()
  {
    m_timer += Time.deltaTime;

    if(m_timer > m_playerManager.m_states.m_death.m_timeToMoveInFirstPerson && !m_timerIsPassed[0]){
      m_timerIsPassed[0] = true;
      if(m_playerManager.SwitchCamera.ThirdPersonMode){
        m_playerManager.SwitchCamera.SwitchCameraType(true);
      }
    }

    if(m_timer > m_playerManager.m_states.m_death.m_timeToCloseEyes && !m_timerIsPassed[1]){
      m_timerIsPassed[1] = true;
      m_playerManager.m_states.m_death.m_eyesAnimator.SetTrigger("Close");
    }

    if(m_timer > m_playerManager.m_states.m_death.m_timeToResetPosition && !m_timerIsPassed[2]){
      m_timerIsPassed[2] = true;
      m_playerManager.ResetCheckPointPosition();
    }

    if(m_timer > m_playerManager.m_states.m_death.m_timeToOpenEyes && !m_timerIsPassed[3]){
      m_timerIsPassed[3] = true;
      m_playerManager.m_states.m_death.m_eyesAnimator.SetTrigger("Open");
    }

    if(m_timer > m_playerManager.m_states.m_death.m_timeToCanMove && !m_timerIsPassed[4]){
      m_timerIsPassed[4] = true;
      m_playerManager.ChangeState(0);
    }
    
  }

  public void FixedUpdate()
  {
    
  }

  public void Exit()
  {
    m_playerManager.PlayerIsDead = false;
  }

}
