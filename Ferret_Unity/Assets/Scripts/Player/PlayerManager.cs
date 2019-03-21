using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

#region Public [System.Serializable] Variables

	[Header("States")]
	public States m_states = new States();
	[System.Serializable] public class States {

		[Range(0, 1)] public float m_forceInputToMove = 0.2f;

		public Walk m_walk = new Walk();
		[System.Serializable] public class Walk {
			public float m_speed = 3f;
		}

		public Run m_run = new Run();
		[System.Serializable] public class Run {
			public float m_speed = 6f;
		}

		public Climb m_climb = new Climb();
		[System.Serializable] public class Climb {
			public float m_speed = 2.25f;
		}

		public Crawl m_crawl = new Crawl();
		[System.Serializable] public class Crawl {
			public float m_speed = 1.5f;
		}

		public Jump m_jump = new Jump();
		[System.Serializable] public class Jump {
			public float m_jumpForce = 10f;
		}

	}

	[Header("Physics")]
	public PhysicsVar m_physics = new PhysicsVar();
	[System.Serializable] public class PhysicsVar {
		public bool m_useGravity = true;
		public float m_gravity = 9.81f;
	}	

	[Header("Colliders")]
	public Colliders m_colliders = new Colliders();
	[System.Serializable] public class Colliders {

		public Base m_base = new Base();
		[System.Serializable] public class Base {
			public SphereCollider m_headColl;
			public CapsuleCollider m_bodyColl;
		}

		public Crawl m_crawl = new Crawl();
		[System.Serializable] public class Crawl {
			public SphereCollider m_headColl;
			public CapsuleCollider m_bodyColl;
		}

	}

	[Header("Meshes")]
	public Meshes m_meshes = new Meshes();
	[System.Serializable] public class Meshes {
		public GameObject m_base;
		public GameObject m_crawl;
	}

	[Header("Camera")]
	public Camera m_camera = new Camera();
	[System.Serializable] public class Camera {
		public Transform m_pivot;
		public float m_rotateSpeed = 5;
		public Camera m_firstPerson;
		public Camera m_thirdPerson;
	}

#endregion Public [System.Serializable] Variables

#region Public Variables

	public GameObject m_ferretMesh;
	public LayerMask m_checkTopMask;

	[HideInInspector] public Rigidbody m_rigidbody;

#endregion Public Variables

#region Input Buttons

	[HideInInspector] public float m_hAxis_Button;
	[HideInInspector] public float m_vAxis_Button;
	[HideInInspector] public bool m_runButton;
	[HideInInspector] public bool m_jumpButton;
	[HideInInspector] public bool m_crawlButton;
	[HideInInspector] public bool m_takeButton;

#endregion Input Buttons

#region Private Variables

	StateMachine m_sM = new StateMachine(); 
	Vector3 m_moveDirection = Vector3.zero;

#endregion Private Variables

#region Private functions

	void Awake(){
		m_sM.AddStates(new List<IState> {
				new PlayerIdleState(this),	// 0 = Idle
				new PlayerWalkState(this),	// 1 = Walk
				new PlayerRunState(this),	// 2 = Run
				new PlayerCrawlState(this)	// 3 = Crawl
		});

		m_rigidbody = GetComponent<Rigidbody>();
	}
	void OnEnable(){
		ChangeState(0);
	}

	void Update(){
		m_sM.Update();
		UpdateInputButtons();
	}

	void UpdateInputButtons(){
		m_hAxis_Button = Input.GetAxis("Horizontal");
		m_vAxis_Button = Input.GetAxis("Vertical");
		m_runButton = Input.GetButton("Run");
		m_jumpButton = Input.GetButtonDown("Jump");
		m_crawlButton = Input.GetButtonDown("Crawl");
		m_takeButton = Input.GetButtonDown("Take");
	}

#endregion Private functions

#region Public functions

	public void ChangeState(int index){
		m_sM.ChangeState(index);
	}

	public bool PlayerInputIsMoving(){
		if( (m_hAxis_Button <= - m_states.m_forceInputToMove) || (m_hAxis_Button >= m_states.m_forceInputToMove) || (m_vAxis_Button <= - m_states.m_forceInputToMove) || (m_vAxis_Button >= m_states.m_forceInputToMove) ){
			return true;
		}else{
			return false;
		}
	}

	public bool CheckTopCollider(){
		Vector3 center = transform.position + new Vector3(0, 0, 0.075f);
		Vector3 halfExtends = new Vector3(0.3f, 0.5f, 1.25f) / 2;
		Vector3 direction = Vector3.up;
		Quaternion orientation = m_ferretMesh.transform.rotation;
		float maxDistance = 0.75f;
		int layerMask = m_checkTopMask;
		
		if(Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)){
			//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			return true;
		}else{
			//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			return false;
		}
	}
	public void Crawl(bool b){
		if(b){
			m_colliders.m_base.m_headColl.enabled = false;
			m_colliders.m_base.m_bodyColl.enabled = false;

			m_colliders.m_crawl.m_headColl.enabled = true;
			m_colliders.m_crawl.m_bodyColl.enabled = true;

			m_meshes.m_base.SetActive(false);
			m_meshes.m_crawl.SetActive(true);
		}else{
			m_colliders.m_base.m_headColl.enabled = true;
			m_colliders.m_base.m_bodyColl.enabled = true;

			m_colliders.m_crawl.m_headColl.enabled = false;
			m_colliders.m_crawl.m_bodyColl.enabled = false;

			m_meshes.m_base.SetActive(true);
			m_meshes.m_crawl.SetActive(false);
		}
	}

	public void MovePlayer(float speed){
		m_moveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button);
		m_moveDirection = transform.TransformDirection(m_moveDirection);
		m_moveDirection = m_moveDirection.normalized;
		m_moveDirection *= speed;

		m_rigidbody.MovePosition(transform.position + m_moveDirection * Time.deltaTime);
	}

	public void ClimbMove(){

	}

	public void RotatePlayer(){
		// Rotate the player in different directions based on camera look direction
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			transform.rotation = Quaternion.Euler(0f, m_camera.m_pivot.rotation.eulerAngles.y, 0f);
			Quaternion newRotation = Quaternion.LookRotation(new Vector3(m_moveDirection.x, 0f, m_moveDirection.z));
			m_ferretMesh.transform.rotation = Quaternion.Slerp(m_ferretMesh.transform.rotation, newRotation, m_camera.m_rotateSpeed * Time.deltaTime);
		}
	}

#endregion Public functions

}
