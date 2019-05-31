using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathMove), typeof(Rigidbody))]
public class RobotPusher : MonoBehaviour {

    [Header("Robot type")]
    public bool m_isMovingInLine = false;

    [Header("Move")]
	public float m_moveSpeed = 10;
    public AnimationCurve m_moveCurve;

    [Header("Rotate")]
	public float m_rotateSpeed = 10;
    public AnimationCurve m_rotateCurve;

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
