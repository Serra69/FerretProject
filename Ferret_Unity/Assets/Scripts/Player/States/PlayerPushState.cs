using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushState : IState
{
  bool doOnceAfterEnterInterpolation = true;
	PlayerManager m_playerManager;
	// Constructor (CTOR)
  public PlayerPushState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    doOnceAfterEnterInterpolation = true;
    m_playerManager.PushableObject = m_playerManager.m_states.m_push.m_hit.collider.GetComponent<PushableObject>();

    Transform toPos = m_playerManager.PushableObject.On_PlayerSnapToObject();
    m_playerManager.StartRotateToPushableObjectInterpolation(m_playerManager.transform, m_playerManager.transform.position, toPos.position, m_playerManager.transform, m_playerManager.transform.rotation, toPos.rotation, m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, toPos.rotation);
    // m_playerManager.StartRotateInterpolation(m_playerManager.m_meshes.m_rotateFerret.transform, m_playerManager.m_meshes.m_rotateFerret.transform.rotation, toPos.rotation);
  }

  public void Update()
  {
    if(m_playerManager.CanMoveOnPush && doOnceAfterEnterInterpolation){
      doOnceAfterEnterInterpolation = false;
      m_playerManager.SetObjectInChildrenOfFerret(m_playerManager.PushableObject.transform, m_playerManager.m_states.m_push.m_objectTrans);
      m_playerManager.PushableObject.On_ObjectIsPushing(true);
    }
    if(!m_playerManager.m_pushButton && m_playerManager.CanMoveOnPush){
      m_playerManager.ChangeState(0);
    }
  }

  public void FixedUpdate()
  {
    if(m_playerManager.CanMoveOnPush){
      m_playerManager.PushMove(m_playerManager.m_states.m_push.m_speed);
    }
  }

  public void LateUpdate(){
      
  }

  public void Exit()
  {
    m_playerManager.PushableObject.On_ObjectIsPushing(false);
    m_playerManager.SetObjectInChildrenOfFerret(m_playerManager.PushableObject.transform);
  }

}
