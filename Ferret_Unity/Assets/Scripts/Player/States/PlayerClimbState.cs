using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : IState
{

	PlayerManager m_playerManager;

	// Constructor (CTOR)
	public PlayerClimbState (PlayerManager playerManager)
	{
		m_playerManager = playerManager;
	}

    public void Enter()
    {
      m_playerManager.Rigidbody.useGravity = false;

      //RaycastHit hit;
      if(m_playerManager.RayCastForStartClimbing()){
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, m_playerManager.climbingHit.point, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, 0, 0));
      }
    }

    public void Exit()
    {
		  m_playerManager.Rigidbody.useGravity = true;
    }

    public void FixedUpdate()
    {
      if(m_playerManager.CanMoveOnClimb){
		    m_playerManager.ClimbMove(m_playerManager.m_states.m_climb.m_speed);
      }
    }

    public void Update()
    {

    }

}
