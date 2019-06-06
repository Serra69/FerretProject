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
    m_playerManager.EndOfOrientationAfterClimb = false;
    m_playerManager.m_states.m_climb.m_canClimb = false;

    m_playerManager.Animator.SetBool("Climb", true);

    m_endOfClimbState = false;
    m_playerManager.Rigidbody.useGravity = false;

    // Calcul du millieu de la position à rejoindre entre les 2 pattes du furet
    Vector3 lerpPosition =  Vector3.Lerp(m_playerManager.topRightClimbHit.point, m_playerManager.topLeftClimbHit.point, 0.5f);
    Quaternion rotationNormal = Quaternion.LookRotation(- m_playerManager.topRightClimbHit.normal);
    // Debug.Log("Normal = " + rotationNormal);

    // Déplacement et rotation du Furret entier par rapport au type de climbArea
    switch(m_playerManager.CheckClimbAreaType()){ 
			case 0:
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, lerpPosition, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, 0, 0), m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 0, 0));
        // m_playerManager.StartRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 0, 0));
			break;
			case 1:
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, lerpPosition, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, 90, 0), m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 90, 0));
        // m_playerManager.StartRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 90, 0));
			break;
			case 2:
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, lerpPosition, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, 180, 0), m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 180, 0));
        // m_playerManager.StartRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, 180, 0));
			break;
			case 3:
        m_playerManager.StartClimbInterpolation(m_playerManager.transform, m_playerManager.transform.position, lerpPosition, m_playerManager.transform, m_playerManager.transform.rotation, Quaternion.Euler(-90, -90, 0), m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, -90, 0));
        // m_playerManager.StartRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, Quaternion.Euler(-90, -90, 0));
			break;
		}
    
    // Rotation du mesh pour qu'il soit bien droit
    // m_playerManager.StartRotateInterpolation(m_playerManager.m_ferretMesh.transform, m_playerManager.m_ferretMesh.transform.localRotation, Quaternion.identity);

    // m_playerManager.m_meshes.m_rotateFerret.transform.localRotation = Quaternion.identity;

    m_playerManager.IsInLerpRotation = false;
  }

  public void FixedUpdate()
  {
    if(m_playerManager.CanMoveOnClimb && !m_playerManager.IsInLerpRotation){
      if( (m_playerManager.RayCastDownToStopClimbing() == false) && (!m_endOfClimbState) && !m_playerManager.IsInLerpRotation && m_playerManager.ClimbArea.m_areaCanBeFinishedClimbable){
        // Debug.Log("Je part en montant comme un connard");
        m_endOfClimbState = true;
        m_playerManager.EndClimbAnimation();
        // m_playerManager.m_updates.m_followPlayer.EndClimbMoveCamera();

        // m_playerManager.m_updates.m_followPlayer.FollowLookAtPoint = false;
        m_playerManager.m_updates.m_followPlayer.On_PlayerEndClimb(false);

        m_playerManager.Animator.SetTrigger("EndClimb");
      }
      m_playerManager.RayCastDownToStopSideScrollingMovement();
      m_playerManager.ClimbMove(m_playerManager.m_states.m_climb.m_speed);
      m_playerManager.Animator.SetFloat("MoveInput", m_playerManager.GetPlayerInputValue());
    }

    if(!m_playerManager.SwitchCamera.CameraIsSwitching){
      m_playerManager.FirstPersonCamera.RotateCamera(true);
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

      m_playerManager.StartOrientationAfterClimb(m_playerManager.transform, 
                                                m_playerManager.transform.position, 
                                                m_playerManager.transform.position + m_playerManager.transform.up * m_playerManager.m_states.m_climb.m_interpolation.m_fallingDistance, 
                                                m_playerManager.transform, m_playerManager.transform.localRotation, 
                                                Quaternion.Euler(0, m_playerManager.transform.localRotation.eulerAngles.y, m_playerManager.transform.localRotation.eulerAngles.z));
      // m_playerManager.StartLocalRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.localRotation, Quaternion.identity);
      // m_playerManager.ChangeState(4);
    }
    if(m_playerManager.EndOfOrientationAfterClimb){
      // Debug.Log("FINI LA STATE EN TOMBANT");
      m_playerManager.ChangeState(4);
    }
  }

  public void LateUpdate(){
      
  }

  public void Exit()
  {
    m_playerManager.Rigidbody.useGravity = true;
    m_playerManager.Animator.SetBool("Climb", false);
  }

}
