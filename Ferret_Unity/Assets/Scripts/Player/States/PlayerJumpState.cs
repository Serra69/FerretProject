using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : IState
{

    PlayerManager m_playerManager;
    float m_timer = 0;
    float m_jumpTimer = 0;
    bool m_isJumpButtonHeld = false;

    // Constructor (CTOR)
    public PlayerJumpState(PlayerManager playerManager)
    {
        m_playerManager = playerManager;
    }

    public void Enter()
    {
        m_playerManager.Animator.SetTrigger("Jump");
        m_timer = 0;
        m_jumpTimer = 0;
    }

    public void Exit()
    {

    }

    public void FixedUpdate()
    {
        m_jumpTimer += Time.deltaTime;
        
        if(m_jumpTimer < m_playerManager.m_states.m_jump.m_timeToJumpImpulse){
            m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed);
        }else{
            Jump();
        }

        MoveCamera();
    }

    void Jump(){
        if (isJumpContinue() && m_timer < m_playerManager.m_states.m_jump.m_jumpTime)
        {
            float proportionCompleted = m_timer / m_playerManager.m_states.m_jump.m_jumpTime;
            if (proportionCompleted > 1)
            {
                proportionCompleted = 1;
            }
            float yDirection = m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(proportionCompleted);
            // Debug.Log("proportionCompleted=" + proportionCompleted);

            m_playerManager.TimerOfPressSpace = proportionCompleted;

            if (m_playerManager.SwitchCamera.ThirdPersonMode)
            {
                m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed, yDirection, m_playerManager.m_states.m_jump.m_jumpForce);
            }
            else
            {
                m_playerManager.MoveFirstPersonPlayer(m_playerManager.LastStateMoveSpeed, yDirection, m_playerManager.m_states.m_jump.m_jumpForce);
            }

            m_timer += Time.deltaTime;
        }
        else
        {
            m_playerManager.ChangeState(4);
        }
    }

    bool isJumpContinue()
    {
        return m_isJumpButtonHeld || m_timer < m_playerManager.m_states.m_jump.m_minJumpTime;
    }

    public void Update()
    {
        m_isJumpButtonHeld = m_playerManager.m_jumpHeldButton;
    }

    void MoveCamera()
    {

        if (m_playerManager.SwitchCamera.ThirdPersonMode)
        {
            if (m_playerManager.PlayerInputIsMoving())
            {
                m_playerManager.RotatePlayer();
            }
        }
        else
        {
            if (!m_playerManager.SwitchCamera.CameraIsSwitching)
            {
                m_playerManager.FirstPersonCamera.RotateCamera();
            }
        }

    }

    public void LateUpdate(){
      
    }

}
