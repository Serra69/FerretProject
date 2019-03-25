using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : IState
{

	PlayerManager m_playerManager;

  float m_timer = 0;

  bool m_isJumpButtonHeld = false;

	// Constructor (CTOR)
    public PlayerJumpState (PlayerManager playerManager)
    {
		  m_playerManager = playerManager;
    }

    public void Enter()
    {
      m_timer = 0;
    }

    public void Exit()
    {

    }

    public void FixedUpdate()
    {
      if (isJumpContinue() && m_timer < m_playerManager.m_states.m_jump.m_jumpTime)
      {   
        float proportionCompleted = m_timer / m_playerManager.m_states.m_jump.m_jumpTime;
        float yDirection = m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(proportionCompleted);
        m_playerManager.MovePlayer(1, yDirection, m_playerManager.m_states.m_jump.m_jumpForce);
        m_timer += Time.deltaTime;
      }
      else
      {
        m_playerManager.ChangeState(4);
      }
    }

    bool isJumpContinue(){
      return m_isJumpButtonHeld || m_timer < m_playerManager.m_states.m_jump.m_minJumpTime;
    }

    public void Update()
    {
      m_isJumpButtonHeld = m_playerManager.m_jumpHeldButton;
    }

}
