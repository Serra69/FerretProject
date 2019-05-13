using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonController : TrainPathsTypes {

	[Header("Type of piston")]
	public PistonTypes m_pistonType = PistonTypes.MoveAndRotate;

	[Header("Rotate")]
	public Rotate m_rotate = new Rotate();
	[System.Serializable] public class Rotate {
		public Transform m_transforme;
		public float m_speed = 2;
		public Vector3 m_toRotation;
		public AnimationCurve m_curve;
	}

	[Header("Move")]
	public Move m_move = new Move();
	[System.Serializable] public class Move {
		public Transform m_transforme;
		public float m_speed = 2;
		public Vector3 m_toLocalePosition;
		public AnimationCurve m_curve;
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

	bool m_rotateIsFinished = false;
	bool m_moveIsFinished = false;

    public void DoYourJob(){
		switch(m_pistonType){ 
			case PistonTypes.Rotate:
				StartCoroutine(RotateCorout());
			break;
			case PistonTypes.Move:
				StartCoroutine(MoveCorout());
			break;
			case PistonTypes.MoveAndRotate:
				StartCoroutine(RotateCorout());
				StartCoroutine(MoveCorout());
			break;
		}
	}

	IEnumerator RotateCorout(){
		float rotateJourneyLength;
		float rotateFracJourney = new float();
		while(m_rotate.m_transforme.eulerAngles != m_rotate.m_toRotation){
			rotateJourneyLength = Vector3.Distance(m_rotate.m_transforme.rotation.eulerAngles, m_rotate.m_toRotation);
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / rotateJourneyLength;
			
			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);

			m_rotate.m_transforme.rotation = Quaternion.Lerp(m_rotate.m_transforme.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));
			yield return null;
		}

		if(m_pistonType != PistonTypes.MoveAndRotate){
			m_trainController.ChoseNextTarget();
		}else{
			m_rotateIsFinished = true;
			CheckPositionAndRotation();
		}
	}

	IEnumerator MoveCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_move.m_transforme.localPosition != m_move.m_toLocalePosition){
			moveJourneyLength = Vector3.Distance(m_move.m_transforme.localPosition, m_move.m_toLocalePosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transforme.localPosition = Vector3.Lerp(m_move.m_transforme.localPosition, m_move.m_toLocalePosition, m_move.m_curve.Evaluate(moveFracJourney));
			yield return null;
		}

		if(m_pistonType != PistonTypes.MoveAndRotate){
			m_trainController.ChoseNextTarget();
		}else{
			m_moveIsFinished = true;
			CheckPositionAndRotation();
		}
	}

	void CheckPositionAndRotation(){
		if(m_rotateIsFinished && m_moveIsFinished){
			m_trainController.ChoseNextTarget();
			m_rotateIsFinished = false;
			m_moveIsFinished = false;
		}
	}

}
