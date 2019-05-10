using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : IState
{

	PlayerManager m_playerManager;

  bool m_endOfClimbState = false;

	// Constructor (CTOR)
	public PlayerClimbState (PlayerManager playerManager)
	{
		m_playerManager = playerManager;
	}

  public void Enter()
  {
    m_playerManager.Animator.SetTrigger("Climb");

    m_endOfClimbState = false;
    m_playerManager.Rigidbody.useGravity = false;

    // Calcul du millieu de la position à rejoindre entre les 2 pattes du furet
    Vector3 lerpPosition =  Vector3.Lerp(m_playerManager.topRightClimbHit.point, m_playerManager.topLeftClimbHit.point, 0.5f);
    Quaternion rotationNormal = Quaternion.LookRotation(- m_playerManager.topRightClimbHit.normal);
    // Debug.Log("Normal = " + rotationNormal);

    // Déplacement et rotation du Furret entier
    m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, lerpPosition, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, 0, 0));
    
    // Rotation du mesh pour qu'il soit bien droit
    m_playerManager.StartRotateInterpolation(m_playerManager.m_ferretMesh.transform, m_playerManager.m_ferretMesh.transform.rotation, Quaternion.Euler(-90, 0, 0));

    // For debug
    m_playerManager.m_rightHit.transform.position = m_playerManager.topRightClimbHit.point;
    m_playerManager.m_leftHit.transform.position = m_playerManager.topLeftClimbHit.point;

    m_playerManager.IsInLerpRotation = false;
  }

  public void FixedUpdate()
  {
    if(m_playerManager.CanMoveOnClimb && !m_playerManager.IsInLerpRotation){
      if( (m_playerManager.RayCastDownToStopClimbing() == false) && (!m_endOfClimbState) && !m_playerManager.IsInLerpRotation){
        // Debug.Log("j'arrive au bout");
        Debug.Log("Je part en montant comme un connard");
        m_endOfClimbState = true;
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, m_playerManager.transform.position + Vector3.down * 2 + Vector3.up * 3, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(0, 0, 0), false);
      }
      m_playerManager.RayCastDownToStopSideScrollingMovement();
      m_playerManager.ClimbMove(m_playerManager.m_states.m_climb.m_speed);
    }
  }

  public void Update()
  {
    if(m_endOfClimbState){
      if(m_playerManager.EndOfClimbInterpolation){
        m_playerManager.ChangeState(0);
      }
    }

    /*if(m_playerManager.m_jumpButton){
      m_playerManager.StartClimbCooldown();

      m_playerManager.transform.rotation = Quaternion.Euler(0, m_playerManager.transform.rotation.y, m_playerManager.transform.rotation.z);
      m_playerManager.m_ferretMesh.transform.localRotation = Quaternion.Euler(0, m_playerManager.m_ferretMesh.transform.rotation.y, m_playerManager.m_ferretMesh.transform.rotation.z);
      
      Vector3 newPosition = m_playerManager.transform.position + Vector3.back * 1f;
      m_playerManager.transform.position = newPosition;
      
      m_playerManager.ChangeState(3);
    }*/

    if(m_playerManager.CanMoveOnClimb && m_playerManager.m_jumpButton && !m_playerManager.IsInLerpRotation){
      m_playerManager.StartOrientationAfterClimb(m_playerManager.transform, m_playerManager.transform.position, m_playerManager.transform.position + m_playerManager.transform.up * m_playerManager.m_states.m_climb.m_interpolation.m_fallingDistance, m_playerManager.transform, m_playerManager.transform.localRotation, Quaternion.Euler(0, m_playerManager.transform.localRotation.y, m_playerManager.transform.localRotation.z));
    }
    if(m_playerManager.EndOfOrientationAfterClimb){
      Debug.Log("FINI LA STATE EN TOMBANT");
      m_playerManager.ChangeState(4);
    }
  }

  public void Exit()
  {
    m_playerManager.Rigidbody.useGravity = true;
    m_playerManager.StartClimbCooldown();
  }

}
