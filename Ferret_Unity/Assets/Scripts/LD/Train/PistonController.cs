using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonController : TrainPathsTypes {

	[Header("Type of piston")]
	public PistonTypes m_pistonType = PistonTypes.MoveAndRotate;

	[Header("Rotate")]
	public Rotate m_rotate = new Rotate();
	[System.Serializable] public class Rotate {
		public Transform m_transformToRotate;
		public float m_rotateSpeed = 2;
		public Vector3 m_toRotation;
		public AnimationCurve m_rotateCurve;
	}

	[Header("Move")]
	public Move m_move = new Move();
	[System.Serializable] public class Move {
		public Transform m_transformToMove;
		public float m_moveSpeed = 2;
		public Vector3 m_toPosition;
		public AnimationCurve m_positionCurve;
	}

	TrainController m_trainController;
    public TrainController TrainController
    {
        get
        {
            return m_trainController;
        }

        set
        {
            m_trainController = value;
        }
    }

    public void RotatePiston(){
		StartCoroutine(RotateCorout());
	}

	IEnumerator RotateCorout(){
		float rotateJourneyLength;
		float rotateFracJourney = new float();
		while(m_rotate.m_transformToRotate.eulerAngles != m_rotate.m_toRotation){
			rotateJourneyLength = Vector3.Distance(m_rotate.m_transformToRotate.rotation.eulerAngles, m_rotate.m_toRotation);
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_rotateSpeed / rotateJourneyLength;
			
			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);

			m_rotate.m_transformToRotate.rotation = Quaternion.Lerp(m_rotate.m_transformToRotate.rotation, toRotation, m_rotate.m_rotateCurve.Evaluate(rotateFracJourney));
			yield return null;
		}
		m_trainController.ChoseNextTarget();
	}

}
