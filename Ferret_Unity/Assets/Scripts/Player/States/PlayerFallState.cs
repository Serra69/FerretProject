using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : IState
{
    float m_currentime = 0;

    bool m_isFalling = false;
    float m_currentTimeOfFall = 0;
    PlayerManager m_playerManager;

    // Constructor (CTOR)
    public PlayerFallState(PlayerManager playerManager)
    {
        m_playerManager = playerManager;
    }

    public void Enter()
    {
        //Debug.LogFormat("{0} : Enter()", GetType().Name);
        m_currentime = 1 - m_playerManager.TimerOfPressSpace;
        m_currentTimeOfFall = 0;
    }

    public void Exit()
    {
        //Debug.LogFormat("{0} : Exit()", GetType().Name);
    }

    public void FixedUpdate()
    {
        Move();
        m_currentime -= Time.deltaTime;
        // if (m_currentime > m_playerManager.m_states.m_fall.m_duration)
        if (m_currentime < 0)
        {
            m_currentime = m_playerManager.m_states.m_fall.m_duration;
        }
    }

    public void Update()
    {
        if (m_playerManager.CheckCollider(false))
        {
            m_playerManager.ChangeState(0);
        }
        if (m_playerManager.RayCastForwardToStartClimbing() && m_playerManager.m_states.m_climb.m_canClimb)
        {
            m_playerManager.ChangeState(6);
        }
    }

    public void LateUpdate(){
      
    }

    void Move()
    {

        if (m_playerManager.SwitchCamera.ThirdPersonMode)
        {
            m_playerManager.MovePlayer(
                m_playerManager.LastStateMoveSpeed,
                m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(m_currentime / m_playerManager.m_states.m_fall.m_duration) *
                Physics.gravity.y,
                1
            );

            if (m_playerManager.PlayerInputIsMoving())
            {
                m_playerManager.RotatePlayer();
            }
        }
        else
        {
            // m_playerManager.MoveFirstPersonPlayer(m_playerManager.LastStateMoveSpeed);
            m_playerManager.MoveFirstPersonPlayer(
                m_playerManager.LastStateMoveSpeed,
                m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(m_currentime / m_playerManager.m_states.m_fall.m_duration) *
                Physics.gravity.y,
                1
            );
            if (!m_playerManager.SwitchCamera.CameraIsSwitching)
            {
                m_playerManager.FirstPersonCamera.RotateCamera();
            }
        }

    }

}
