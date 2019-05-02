using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathMove) ,typeof(FieldOfView), typeof(NavMeshAgent))]
public class RobotDoctor : MonoBehaviour {

    bool m_followPlayer = false;
    public bool FollowPlayer
    {
        get
        {
            return m_followPlayer;
        }
    }

    FieldOfView m_fov;
    NavMeshAgent m_agent;

    void Awake(){
        m_fov = GetComponent<FieldOfView>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    void Update(){
        if(m_fov.m_playerTarget != null){
            m_followPlayer = true;
            m_agent.SetDestination(m_fov.m_playerTarget.transform.position);
        }else{
            m_followPlayer = false;
        }
    }

}
