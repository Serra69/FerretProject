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
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_rotationSpeed * Time.deltaTime);*/

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