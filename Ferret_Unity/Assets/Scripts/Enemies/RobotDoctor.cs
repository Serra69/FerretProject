using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathMove) ,typeof(FieldOfView), typeof(NavMeshAgent))]
public class RobotDoctor : MonoBehaviour {

    FieldOfView m_fov;
    NavMeshAgent m_agent;

    void Awake(){
        m_fov = GetComponent<FieldOfView>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    void Update(){
        if(m_fov.m_target != null){
            m_agent.SetDestination(m_fov.m_target.transform.position);
        }
    }

}
