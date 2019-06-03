using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PathMove : MonoBehaviour {

	[SerializeField] float m_timeToStartMoving = 0;
	[Space]
	[SerializeField] Color m_pathColor = Color.yellow;
	[SerializeField] float m_timeToWaitOnAWaitPath = 2;
	[SerializeField] Transform[] m_pathList;

	RobotPusher m_robotPusher;
	RobotDoctor m_robotDoctor;

	NavMeshAgent m_agent;
	Transform m_pathTarget;
	int m_currenttarget;
	int m_pathType;
	bool m_waitingOnAPath;

	void Awake(){
		if(m_pathList.Length < 2){
			Debug.LogError("Difficle de bouger un ennemi avec moins de 2 points !");
		}
		foreach(Transform trans in m_pathList){
			if(trans == null){
				Debug.LogError("Il manque un path dans le path root d'un ennemi !");
			}
		}
		transform.position = m_pathList[0].position;
		transform.LookAt(m_pathList[1]);

		m_robotDoctor = GetComponent<RobotDoctor>();
		m_robotPusher = GetComponent<RobotPusher>();
	}

	void Start(){
		m_agent = GetComponent<NavMeshAgent>();

		StartCoroutine(InitializePath());
	}

	IEnumerator InitializePath(){
		yield return new WaitForSeconds(m_timeToStartMoving);
		ChoseNextTarget();

		if(GetComponent<RobotPusher>()){
			if(m_pathTarget != null){
				StartCoroutine(MoveWithRigidbody(transform, transform.position, m_pathTarget.position, transform, transform.rotation, m_pathTarget.rotation));
			}
		}
	}

	void Update(){

		if(GetComponent<RobotDoctor>()){
			if(!m_robotDoctor.FollowPlayer){
				MoveWithNavMesh();
			}
		}
	}

	IEnumerator MoveWithRigidbody(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		
		m_robotPusher.m_wheel.Move = true;

		float moveJourneyLength;
		float moveFracJourney = new float();

		// print("Start move");

		while(transformPosition.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * m_robotPusher.m_moveSpeed / moveJourneyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, m_robotPusher.m_moveCurve.Evaluate(moveFracJourney));

			yield return null;
		}
		// print("End move");

		m_robotPusher.StartAnimationBroomEnter(true);

		m_robotPusher.m_wheel.Move = false;

	}

	public void StartRotateRigidbody(){
		if(m_currenttarget == m_pathList.Length -1){
				// print("m_pathList[0] = " + m_pathList[0]);
			StartCoroutine(RotateWithRigidbody(transform.rotation, m_pathList[0].rotation));
		}else{
			if(m_pathList[m_currenttarget + 1] != null){
				// print("m_pathList[m_currenttarget + 1] = " + m_pathList[m_currenttarget + 1]);
				StartCoroutine(RotateWithRigidbody(transform.rotation, m_pathList[m_currenttarget + 1].rotation));
			}
		}
	}

	IEnumerator RotateWithRigidbody(Quaternion fromRotation, Quaternion toRotation){

		float rotateJourneyLength = new float();
		float rotateFracJourney = new float();

		// print("Start rotate");
		// Debug.Log("transformRotation.rotation = " + transform.rotation.eulerAngles);
		// Debug.Log("toRotation = " + toRotation.eulerAngles);

		while(transform.rotation.eulerAngles != toRotation.eulerAngles){

			// MoveRotation

			if(!m_robotPusher.m_isMovingInLine){
				rotateJourneyLength = Quaternion.Dot(fromRotation, toRotation);
			}else{
				rotateJourneyLength = Vector3.Distance(fromRotation.eulerAngles, toRotation.eulerAngles);
			}

			// Debug.Log("rotateJourneyLength = " + rotateJourneyLength);
			if(rotateJourneyLength < 0){
				rotateJourneyLength = - rotateJourneyLength;
				// Debug.Log("after calcul : rotateJourneyLength = " + rotateJourneyLength);
			}

			rotateFracJourney += (Time.deltaTime) * m_robotPusher.m_rotateSpeed / rotateJourneyLength;

			if(!m_robotPusher.m_isMovingInLine){
				transform.rotation = Quaternion.Slerp(fromRotation, toRotation, m_robotPusher.m_rotateCurve.Evaluate(rotateFracJourney));
			}else{
				transform.rotation = Quaternion.Lerp(fromRotation, toRotation, m_robotPusher.m_rotateCurve.Evaluate(rotateFracJourney));
			}

			// Debug.Log("transformRotation.rotation = " + transform.rotation.eulerAngles);
			// Debug.Log("toRotation.eulerAngles = " + toRotation.eulerAngles);
			// Debug.Log("rotateFracJourney = " + rotateFracJourney);
			// Debug.Log("Quaternion.Slerp = " + Quaternion.Slerp(fromRotation, toRotation, m_robotPusher.m_rotateCurve.Evaluate(rotateFracJourney)).eulerAngles);

			yield return null;
		}
		// print("End rotate");
		ChoseNextTarget();
		StartCoroutine(MoveWithRigidbody(transform, transform.position, m_pathList[m_currenttarget].position, transform, transform.rotation, m_pathList[m_currenttarget].rotation));
	}

	void MoveWithNavMesh(){
		if(!m_waitingOnAPath){
			m_agent.SetDestination(m_pathTarget.position);
		}else{
			m_agent.SetDestination(transform.position);
		}
	}

	void ChoseNextTarget(){
		if(m_currenttarget == m_pathList.Length -1){
			m_pathTarget = m_pathList[0].transform;
			m_currenttarget = 0;
		}else{
			if(m_pathList[m_currenttarget + 1] != null){
				m_pathTarget = m_pathList[m_currenttarget + 1].transform;
				m_currenttarget ++;
			}
		}
	}

	/*void OnTriggerEnter(Collider col){
		if(col.tag == "Path"){
			if(col.gameObject == m_pathTarget.gameObject){

				col.SendMessage("GetPathType", gameObject, SendMessageOptions.RequireReceiver);

				if(m_pathType == 1){			// A simple path
					ChoseNextTarget();
				}else if(m_pathType == 2){		// A wait path
					StartCoroutine(WaitInAPath());
				}
			}
		}
	}*/

	void GetPathType(int i){
		m_pathType = i;
	}

	IEnumerator WaitInAPath(){
		m_waitingOnAPath = true;
		yield return new WaitForSeconds(m_timeToWaitOnAWaitPath);
		m_waitingOnAPath = false;
		ChoseNextTarget();
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = m_pathColor;

		if(m_pathList == null){
			return;
		}

		for(int i = 0; i < m_pathList.Length; i++){
			if(i+1 != m_pathList.Length){
				if(m_pathList[i] != null && m_pathList[i + 1] != null){
					Gizmos.DrawLine(m_pathList[i].transform.position, m_pathList[i + 1].transform.position);
				}
			}
			if(i == m_pathList.Length -1){
				if(m_pathList[i] != null && m_pathList[0] != null){
					Gizmos.DrawLine(m_pathList[i].transform.position, m_pathList[0].transform.position);
				}
			}
			
		}
	}

}

/*
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class PathMove : MonoBehaviour {

	[SerializeField] private float m_moveSpeed = 5;
	//[SerializeField] private float m_rotationSpeed = 90;
	[SerializeField] private Color m_pathColor = Color.yellow;
	[SerializeField] private float m_timeToWaitOnAWaitPath = 2;
	[SerializeField] private Transform[] m_pathList;

	private Rigidbody m_rbody;
	private Transform m_target;
	private int m_currenttarget;
	private int m_pathType;
	private bool m_waitingOnAPath;

	void Awake(){
		if(m_pathList.Length < 2){
			Debug.LogError("Difficle de bouger un ennemi avec moins de 2 points !");
		}
		foreach(Transform trans in m_pathList){
			if(trans == null){
				Debug.LogError("Il manque un path dans le path root d'un ennemi !");
			}
		}
		transform.position = m_pathList[0].position;
	}

	void Start(){
		m_rbody = GetComponent<Rigidbody>();
		ChoseNextTarget();
	}

	void Update(){
		Move();
	}

	void Move(){
		Vector3 step;

		Vector3 targetPos = m_target.position;
		targetPos.y = transform.position.y;
		
		transform.LookAt(targetPos);

		/*Vector3 direction = (targetPos - transform.position).normalized;	// Soustraction entre 2 vecteurs (".normalized" : stocke la direction normalisée)
		Quaternion targetRot = Quaternion.LookRotation(direction);			// Quaternion (pour la rotation
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_rotationSpeed * Time.deltaTime);*

		step = transform.forward * (m_moveSpeed * Time.deltaTime);

		if(!m_waitingOnAPath){
			m_rbody.velocity = step;
		}else{
			m_rbody.velocity = new Vector3(0, 0, 0);
		}

	}

	void ChoseNextTarget(){
		if(m_currenttarget == m_pathList.Length -1){
			m_target = m_pathList[0].transform;
			m_currenttarget = 0;
		}else{
			if(m_pathList[m_currenttarget + 1] != null){
				m_target = m_pathList[m_currenttarget + 1].transform;
				m_currenttarget ++;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.tag == "Path"){
			if(col.gameObject == m_target.gameObject){

				col.SendMessage("GetPathType", gameObject, SendMessageOptions.RequireReceiver);

				if(m_pathType == 1){			// A simple path
					ChoseNextTarget();
				}else if(m_pathType == 2){		// A wait path
					StartCoroutine(WaitInAPath());
				}
			}
		}
	}

	void GetPathType(int i){
		m_pathType = i;
	}

	IEnumerator WaitInAPath(){
		m_waitingOnAPath = true;
		yield return new WaitForSeconds(m_timeToWaitOnAWaitPath);
		m_waitingOnAPath = false;
		ChoseNextTarget();
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = m_pathColor;
		for(int i = 0; i < m_pathList.Length; i++){
			if(i+1 != m_pathList.Length){
				if(m_pathList[i] != null && m_pathList[i + 1] != null){
					Gizmos.DrawLine(m_pathList[i].transform.position, m_pathList[i + 1].transform.position);
				}
			}
			if(i == m_pathList.Length -1){
				if(m_pathList[i] != null && m_pathList[0] != null){
					Gizmos.DrawLine(m_pathList[i].transform.position, m_pathList[0].transform.position);
				}
			}
			
		}
	}

}
*/