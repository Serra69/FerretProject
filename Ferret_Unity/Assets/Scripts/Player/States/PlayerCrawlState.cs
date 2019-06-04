using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrawlState : IState {

    PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerCrawlState (PlayerManager playerManager)
    {
		  m_playerManager = playerManager;
    }

    public void Enter()
    {
      //Debug.LogFormat("{0} : Enter()", GetType().Name);
      m_playerManager.Crawl(true);
      m_playerManager.Animator.SetBool("Crawl", true);
    }

    public void Exit()
    {
      m_playerManager.Crawl(false);
      //Debug.LogFormat("{0} : Exit()", GetType().Name);
      m_playerManager.Animator.SetBool("Crawl", false);
    }

    public void FixedUpdate()
    {
        Move();
        /*if(m_playerManager.PlayerInputIsMoving()){
          m_playerManager.Animator.SetFloat("MoveInput", m_playerManager.GetPlayerInputValue());
        }else{
          m_playerManager.Animator.SetFloat("MoveInput", 0);
        }*/
        m_playerManager.Animator.SetFloat("MoveInput", m_playerManager.GetPlayerInputValue());
    }

    public void Update()
    {
      if(m_playerManager.m_crawlButton){
        if(!m_playerManager.CheckCollider(true)){
          m_playerManager.ChangeState(0);
        }
      }
      if(m_playerManager.m_jumpButton){
        if(!m_playerManager.CheckCollider(true)){
          m_playerManager.ChangeState(3);
        }
      }

      if(!m_playerManager.CheckCollider(false)){
        m_playerManager.ChangeState(4);
      }

      if(m_playerManager.m_pushButton){
        if(m_playerManager.RayCastToCanPush()){
          m_playerManager.ChangeState(7);
        }
      }
    }

    public void LateUpdate(){
      
    }

    void Move(){

      if(m_playerManager.SwitchCamera.ThirdPersonMode){
        if(m_playerManager.PlayerInputIsMoving()){
        m_playerManager.MovePlayer(m_playerManager.m_states.m_crawl.m_speed);
          m_playerManager.RotatePlayer();
        }
      }else{
        m_playerManager.MoveFirstPersonPlayer(m_playerManager.m_states.m_crawl.m_speed);
        if(!m_playerManager.SwitchCamera.CameraIsSwitching){
          m_playerManager.FirstPersonCamera.RotateCamera();
        }
      }

    }
	
}
