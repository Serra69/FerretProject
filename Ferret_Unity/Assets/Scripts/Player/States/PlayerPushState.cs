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
    Transform toPos = m_playerManager.m_states.m_push.m_hit.collider.GetComponent<PushableObject>().On_PlayerSnapToObject();
    m_playerManager.transform.position = toPos.position;
    m_playerManager.transform.rotation = toPos.rotation;
    m_playerManager.m_ferretMesh.transform.localRotation = Quaternion.identity;
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
    
  }

}
