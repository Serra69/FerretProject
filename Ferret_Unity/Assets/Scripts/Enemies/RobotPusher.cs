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

    [Header("Animations")]
    public Wheel m_wheel = new Wheel();
	[System.Serializable] public class Wheel {
        public Transform[] m_toAnimate;
        public float m_speed = 3;
        bool m_move = false;
        public bool Move{
            get{
                return m_move;
            }
            set{
                m_move = value;
            }
        }
    }

    public Broom m_broom = new Broom();
	[System.Serializable] public class Broom {
        public Transform m_toAnimate;
        public float m_timeToWaitToEndRotation = 0.25f;

        [Header("Enter")]
        public float m_enterSpeed = 150;
        public Vector3 m_startRotation = new Vector3(0, 0, 0);
        public AnimationCurve m_enterCurve;

        [Header("Exit")]
        public float m_exitSpeed = 150;
        public Vector3 m_endRotation = new Vector3(90, 0, 0);
        public AnimationCurve m_exitCurve;
    }

    [Header("FX")]
    public GameObject m_rotateFx;
    [SerializeField] GameObject m_sweepFx;

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

    PathMove m_pathMove;

    void Awake(){
        m_rigidbody = GetComponent<Rigidbody>();
        m_pathMove = GetComponentInChildren<PathMove>();
    }

    void FixedUpdate(){
        if(m_wheel.Move){
            for (int i = 0, l = m_wheel.m_toAnimate.Length; i < l; ++i){
                m_wheel.m_toAnimate[i].Rotate(- m_wheel.m_speed, 0, 0);
            }
        }
    }

    public void StartAnimationBroomEnter(bool enter){
        if(enter){
            StartCoroutine(StartAnimationBroomCorout(enter, m_broom.m_toAnimate, m_broom.m_startRotation, m_broom.m_endRotation, m_broom.m_enterSpeed, m_broom.m_enterCurve));
        }else{
            StartCoroutine(StartAnimationBroomCorout(enter, m_broom.m_toAnimate, m_broom.m_endRotation, m_broom.m_startRotation, m_broom.m_exitSpeed, m_broom.m_exitCurve));
        }
    }
    IEnumerator StartAnimationBroomCorout(bool isEnter, Transform trans, Vector3 fromRot, Vector3 toRot, float speed, AnimationCurve curve){

		float journeyLength = new float();
		float fracJourney = new float();

        Level.AddFX(m_sweepFx, transform.position, Quaternion.identity);

		while(trans.localEulerAngles != toRot){
			// MoveRotation
			journeyLength = Vector3.Distance(fromRot, toRot);
			fracJourney += (Time.deltaTime) * speed / journeyLength;
			trans.localEulerAngles = Vector3.Lerp(fromRot, toRot, curve.Evaluate(fracJourney));
			yield return null;
		}

        if(isEnter){
            StartAnimationBroomEnter(false);
            StartCoroutine(WaitToEndRotation());
        }
    }

    IEnumerator WaitToEndRotation(){
        yield return new WaitForSeconds(m_broom.m_timeToWaitToEndRotation);
        m_pathMove.StartRotateRigidbody();
    }

}
