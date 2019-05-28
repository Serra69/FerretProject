using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonController : TrainPathsTypes {

	[Header("Type of piston")]
	public PistonTypes m_pistonType = PistonTypes.MoveAndRotate;
	public bool m_rotateBeforeMove = true;
	[Space]
	public bool m_resetTransformAfterMoving = false;
	[SerializeField] float m_timeToWaitBeforeReset = 2;

	[Header("Rotate")]
	public Rotate m_rotate = new Rotate();
	[System.Serializable] public class Rotate {
		public Transform m_transform;
		public float m_speed = 2;
		public Vector3 m_toRotation;
		public AnimationCurve m_curve;
	}

	[Header("Move")]
	public Move m_move = new Move();
	[System.Serializable] public class Move {
		public Transform m_transform;
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

	Vector3 m_startPosition;
	Vector3 m_startRotation;

	void Start(){
		m_startPosition = m_move.m_transform.localPosition;
		m_startRotation = m_rotate.m_transform.rotation.eulerAngles;
	}

    public void DoYourJob(){
		switch(m_pistonType){ 
			case PistonTypes.Rotate:
				StartCoroutine(RotateCorout());
			break;
			case PistonTypes.Move:
				StartCoroutine(MoveCorout());
			break;
			case PistonTypes.MoveAndRotate:
				if(m_rotateBeforeMove){
					StartCoroutine(RotateCorout());
				}else{
					StartCoroutine(MoveAndRotateCorout());
				}
			break;
		}
	}

	IEnumerator RotateCorout(){
		float rotateJourneyLength;
		float rotateFracJourney = new float();
		while(m_rotate.m_transform.eulerAngles != m_rotate.m_toRotation){
			rotateJourneyLength = Vector3.Distance(m_rotate.m_transform.rotation.eulerAngles, m_rotate.m_toRotation);
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / rotateJourneyLength;
			
			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);

			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));
			yield return null;
		}

		switch(m_pistonType){ 
			case PistonTypes.Rotate:
				m_trainController.ChoseNextTarget();
				m_trainController.ResetTrainParent();

				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
			case PistonTypes.MoveAndRotate:
				if(m_rotateBeforeMove){
					StartCoroutine(MoveCorout());
				}
			break;
		}
	}
	IEnumerator MoveCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_move.m_transform.localPosition != m_move.m_toLocalePosition){
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_move.m_toLocalePosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_move.m_toLocalePosition, m_move.m_curve.Evaluate(moveFracJourney));
			yield return null;
		}
		m_trainController.ChoseNextTarget();
		m_trainController.ResetTrainParent();

		switch(m_pistonType){ 
			case PistonTypes.Move:
				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
			case PistonTypes.MoveAndRotate:
				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
		}
	}
	IEnumerator MoveAndRotateCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();

		while(m_move.m_transform.localPosition != m_move.m_toLocalePosition){
			// Move
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_move.m_toLocalePosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_move.m_toLocalePosition, m_move.m_curve.Evaluate(moveFracJourney));

			// Rotate
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / moveJourneyLength;
			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);
			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));

			yield return null;
		}
		m_trainController.ChoseNextTarget();
		m_trainController.ResetTrainParent();

		if(m_resetTransformAfterMoving){
			StartCoroutine(ResetTransform());
		}
	}
	IEnumerator ResetTransform(){
		yield return new WaitForSeconds(m_timeToWaitBeforeReset);
		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();

		while(m_move.m_transform.localPosition != m_startPosition){
			// Move
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_startPosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_startPosition, m_move.m_curve.Evaluate(moveFracJourney));

			// Rotate
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / moveJourneyLength;
			Quaternion toRotation = Quaternion.Euler(m_startRotation);
			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));

			yield return null;
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

