using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : IState
{

	PlayerManager m_playerManager;

  bool m_isJumpButtonHeld = false;

	// Constructor (CTOR)
    public PlayerJumpState (PlayerManager playerManager)
    {
		  m_playerManager = playerManager;
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void FixedUpdate()
    {
      if(m_isJumpButtonHeld){
        m_playerManager.MovePlayer(m_playerManager.m_states.m_run.m_speed, m_playerManager.m_states.m_jump.m_jumpForce);
      }
      
      /*IEnumerator JumpRoutine()
      {
        rigidbody.velocity = Vector2.zero;
        float timer = 0;
        
        while(jumpButtonPressed && timer < jumpTime)
        {
          //Calculate how far through the jump we are as a percentage
          //apply the full jump force on the first frame, then apply less force
          //each consecutive frame
          
          float proportionCompleted = timer / jumpTime;
          Vector2 thisFrameJumpVector = Vector2.Lerp(jumpVector, Vector2.zero, proportionCompleted);
          rigidbody.AddForce(thisFrameJumpVector);
          timer += Time.deltaTime;
          yield return null;
        }
        
        jumping = false;
      }*/

    }

    public void Update()
    {
      m_isJumpButtonHeld = m_playerManager.m_jumpHeldButton;
    }

}
