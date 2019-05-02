using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathMove), typeof(Rigidbody))]
public class RobotPusher : MonoBehaviour {

	public float m_moveSpeed = 10;

    Rigidbody m_rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            return m_rigidbody;
        }

        set
        {
            m_rigidbody = value;
        }
    }

    void Awake(){
        m_rigidbody = GetComponent<Rigidbody>();
    }

}
