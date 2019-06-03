using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : TrainPathsTypes {

	[Header("SETUP")]
	[SerializeField] bool m_startTPAtFirstPoint = true;

	[Header("Objects to move")]
	[SerializeField] Transform m_myParent;
	[SerializeField] Transform m_train;

	[Header("Points")]
	[SerializeField] Transform m_startPoint;
	[SerializeField] TrainPoint[] m_points;

	[Header("Move")]
	[SerializeField] float[] m_moveSpeeds;
	[SerializeField] AnimationCurve m_moveCurve;

	[Header("FX")]
	[SerializeField] GameObject m_startMoveFX;
	[SerializeField] GameObject m_moveFX;

	[Header("Gizmos")]
	[SerializeField] Color m_gizmosColor = Color.magenta;

	TrainPoint m_trainPoint;
	int m_nextPoint = 0;

	Rigidbody m_rbody;
	FX m_saveMoveFX;

	void Start(){

		if(m_moveSpeeds.Length != m_points.Length){
			Debug.LogError("You need to have the same length of array in Points & Move Speeds");
		}

		m_rbody = m_train.GetComponent<Rigidbody>();

		if(m_startTPAtFirstPoint){
			m_train.position = m_startPoint.position;
		}


		Vector3 targetPos = m_points[0].transform.position;
		targetPos.y = m_train.position.y;
		m_train.LookAt(targetPos);

		// StartCoroutine(Move(m_train, m_train.position, m_points[m_nextPoint].transform.position));
	}

	void Update(){
		/*if(Input.GetKeyDown(KeyCode.P)){
			ChoseNextTarget();
		}*/
	}

	float GetTrainSpeed(){
		if(m_nextPoint - 1 >= 0){
			return m_moveSpeeds[m_nextPoint - 1];
		}else{
			return m_moveSpeeds[m_nextPoint];
		}
	}

	IEnumerator Move(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition){

		if(m_moveFX != null){
			m_saveMoveFX = Level.AddFX(m_moveFX, transform.position, transform.rotation);
		}

		float moveJourneyLength;
		float moveFracJourney = new float();

		while(transformPosition.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * GetTrainSpeed() / moveJourneyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, m_moveCurve.Evaluate(moveFracJourney));
			yield return null;
		}

		switch(m_trainPoint.m_pointTypes){ 
			case PointTypes.WaitSeconds:
				PistonController m_pistonController = m_trainPoint.GetComponent<PistonController>();

				transform.SetParent(m_pistonController.m_rotate.m_transform);

				m_pistonController.DoYourJob();
				m_pistonController.TrainController = this;

			break;
			case PointTypes.Wait:
				
			break;
		}
		if(m_moveFX != null){
			if(m_saveMoveFX != null){
		print("J'ai fini");
				Destroy(m_saveMoveFX.gameObject);
			}
		}
	}

	public void ChoseNextTarget(){

		StartCoroutine(Move(m_train, m_train.position, m_points[m_nextPoint].transform.position));

		m_trainPoint = m_points[m_nextPoint];
		m_nextPoint ++;
	}

	public void ResetTrainParent(){
		transform.SetParent(m_myParent.transform);
	}

	public void EnVoiture(){
		if(m_startMoveFX != null){
			Level.AddFX(m_startMoveFX, transform.position, transform.rotation);
		}
	}
	
	void OnDrawGizmosSelected(){
		Gizmos.color = m_gizmosColor;

		if(m_points == null){
			return;
		}

		for(int i = 0; i < m_points.Length; i++){

			if(m_startPoint != null && m_points[0] != null){
				Gizmos.DrawLine(m_startPoint.transform.position, m_points[0].transform.position);
			}

			if(i+1 != m_points.Length){
				if(m_points[i] != null && m_points[i + 1] != null){
					Gizmos.DrawLine(m_points[i].transform.position, m_points[i + 1].transform.position);
				}
			}
		}
	}

}
 