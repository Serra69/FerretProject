using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

#region Public [System.Serializable] Variables

	[SerializeField] StateMachine m_sM = new StateMachine(); 

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
			public float m_minJumpTime = 0.1f;
			public float m_jumpTime = 0.5f;

			public AnimationCurve m_jumpHeightCurve = null;
		}

	}

	[Header("Physics")]
	public PhysicsVar m_physics = new PhysicsVar();
	[System.Serializable] public class PhysicsVar {
		public bool m_useGravity = true;
		public float m_gravity = 9.81f;

		public Transform castCenter = null;
		public float m_maxDistance = 1;
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
	public EditCamera m_camera = new EditCamera();
	[System.Serializable] public class EditCamera {

		public Cameras m_cameras = new Cameras();
		[System.Serializable] public class Cameras {
			public Camera m_firstPerson;
			public Camera m_thirdPerson;
		}

		public Transform m_pivot;
		public float m_rotateSpeed = 5;
	}

#endregion Public [System.Serializable] Variables

#region Public Variables

	public GameObject m_ferretMesh;
	public LayerMask m_checkMask;

    [HideInInspector] public bool m_isIn3rdPersonCamera = true;

#endregion Public Variables

#region Input Buttons

	[HideInInspector] public float m_hAxis_Button;
	[HideInInspector] public float m_vAxis_Button;
	[HideInInspector] public bool m_runButton;
	[HideInInspector] public bool m_jumpButton;
	[HideInInspector] public bool m_jumpHeldButton;
	[HideInInspector] public bool m_crawlButton;
	[HideInInspector] public bool m_takeButton;

#endregion Input Buttons

#region Private Variables

	Vector3 m_moveDirection = Vector3.zero;
	public Vector3 MoveDirection
    {
        get
        {
            return m_moveDirection;
        }
        set
        {
            m_moveDirection = value;
        }
    }

	Rigidbody m_rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            return m_rigidbody;
        }
    }

    #endregion Private Variables

    #region Private functions

    void Awake(){
		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),	// 0 = Idle
			new PlayerWalkState(this),	// 1 = Walk
			new PlayerRunState(this),	// 2 = Run
			new PlayerJumpState(this),	// 3 = Jump
			new PlayerFallState(this),	// 4 = Fall
			new PlayerCrawlState(this)	// 5 = Crawl	
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

	void FixedUpdate(){
		MoveDirection = Vector3.zero;
		m_sM.FixedUpdate();
		DoMove();
	}

	void UpdateInputButtons(){
		m_hAxis_Button = Input.GetAxis("Horizontal");
		m_vAxis_Button = Input.GetAxis("Vertical");
		m_runButton = Input.GetButton("Run");
		m_jumpButton = Input.GetButtonDown("Jump");
		m_jumpHeldButton = Input.GetButton("Jump");
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

	public bool CheckCollider(bool top){
		// Vector3 center = transform.position + new Vector3(0, top == true ? 0 : 0.1f , 0.075f);
		Vector3 center = m_physics.castCenter.position;
		Vector3 halfExtends = new Vector3(0.3f, 0.5f, 1.25f) / 2;
		
		Vector3 direction = top == true ? Vector3.up : Vector3.down;

		Quaternion orientation = m_ferretMesh.transform.rotation;
		int layerMask = m_checkMask;
		
		if(Physics.BoxCast(center, halfExtends, direction, orientation, m_physics.m_maxDistance, layerMask)){
			//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			return true;
		}else{
			//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			return false;
		}
	}

	void OnDrawGizmos(){
		if (m_physics.castCenter == null)
		{
			return;
		}
		Vector3 center = m_physics.castCenter.position;
		Vector3 halfExtends = new Vector3(0.3f, 0.5f, 1.25f) / 2;
		Quaternion orientation = m_ferretMesh.transform.rotation;

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(center, halfExtends);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(center + (Vector3.down * m_physics.m_maxDistance), halfExtends);
		Gizmos.DrawWireCube(center + (Vector3.up * m_physics.m_maxDistance), halfExtends);
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

	public void MovePlayer(float speed, float y = 0, float jumpSpeed = 0){
		MoveDirection = new Vector3(m_hAxis_Button, y, m_vAxis_Button);
		MoveDirection = transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();
		m_moveDirection.x *= speed;
		m_moveDirection.z *= speed;
		m_moveDirection.y *= jumpSpeed;
	}

	public void DoMove(){
		if(MoveDirection != Vector3.zero){
			m_rigidbody.MovePosition(transform.position + MoveDirection * Time.fixedDeltaTime);
		}
	}

	public void ClimbMove(){

	}

	public void RotatePlayer(){
		// Rotate the player in different directions based on camera look direction
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			transform.rotation = Quaternion.Euler(0f, m_camera.m_pivot.rotation.eulerAngles.y, 0f);
			Quaternion newRotation = Quaternion.LookRotation(new Vector3(MoveDirection.x, 0f, MoveDirection.z));
			m_ferretMesh.transform.rotation = Quaternion.Slerp(m_ferretMesh.transform.rotation, newRotation, m_camera.m_rotateSpeed * Time.deltaTime);
		}
	}

	public void ChangeCamera(){
		m_isIn3rdPersonCamera =! m_isIn3rdPersonCamera;

		if(m_isIn3rdPersonCamera){
			m_camera.m_cameras.m_firstPerson.depth = 0;
			m_camera.m_cameras.m_thirdPerson.depth = 1;
		}else{
			m_camera.m_cameras.m_firstPerson.depth = 1;
			m_camera.m_cameras.m_thirdPerson.depth = 0;
		}
	}

#endregion Public functions

}
