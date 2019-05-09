using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushState : IState
{

	PlayerManager m_playerManager;
	// Constructor (CTOR)
  public PlayerPushState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    m_playerManager.PushableObject = m_playerManager.m_states.m_push.m_hit.collider.GetComponent<PushableObject>();
    m_playerManager.PushableObject.On_ObjectIsPushing(true);
    Transform toPos = m_playerManager.PushableObject.On_PlayerSnapToObject();
    m_playerManager.transform.position = toPos.position;
    m_playerManager.transform.rotation = toPos.rotation;
    m_playerManager.m_ferretMesh.transform.localRotation = Quaternion.identity;

    m_playerManager.SetObjectInChildrenOfFerret(m_playerManager.PushableObject.transform, m_playerManager.m_states.m_push.m_objectTrans);
  }

  public void Update()
  {
    if(!m_playerManager.m_pushButton){
      m_playerManager.ChangeState(0);
    }
  }

  public void FixedUpdate()
  {
    m_playerManager.PushMove(m_playerManager.m_states.m_push.m_speed);
  }


  public void Exit()
  {
    m_playerManager.PushableObject.On_ObjectIsPushing(false);
    m_playerManager.SetObjectInChildrenOfFerret(m_playerManager.PushableObject.transform);
  }

}
