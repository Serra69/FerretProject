using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

#region Public [System.Serializable] Variables

	public StateMachine m_sM = new StateMachine(); 

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

			public Interpolation m_interpolation = new Interpolation();
			[System.Serializable] public class Interpolation {
				[Header("Curve")]
				public AnimationCurve m_snapCurve;

				[Header("Speeds")]
				public float m_changePositionSpeed = 5;
				public float m_changeRotationSpeed = 5;				
			}
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

	[Header("Cameras")]
	public EditCamera m_cameras = new EditCamera();
	[System.Serializable] public class EditCamera {
		public Camera m_firstPerson;
		public Camera m_thirdPerson;
	}

	[Header("Rotations")]
	public Rotations m_rotations = new Rotations();
	[System.Serializable] public class Rotations {
		public CameraSettings cameraSettings;
		public Transform m_pivot;
		public float minTurnSpeed = 400;
		public float maxTurnSpeed = 1200;
	}
	

#endregion Public [System.Serializable] Variables

#region Public Variables

	public GameObject m_ferretMesh;
	public LayerMask m_checkMask;

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

	float m_lastStateMoveSpeed = 0;
    public float LastStateMoveSpeed
    {
        get
        {
            return m_lastStateMoveSpeed;
        }
    }

    Animator m_animator;
    public Animator Animator
    {
        get
        {
            return m_animator;
        }
    }

    bool m_canMoveOnClimb = false;
    public bool CanMoveOnClimb
    {
        get
        {
            return m_canMoveOnClimb;
        }
    }

    bool m_isIn3rdPersonCamera = true;

	// ----------------------------
	// ----- FOR THE ROTATION -----
	const float k_InverseOneEighty = 1f / 180f;
	const float k_AirborneTurnSpeedProportion = 5.4f;
	protected float m_AngleDiff;
	Quaternion m_TargetRotation;
	// ----------------------------


#endregion Private Variables

#region Private functions

    void Awake(){
		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),		// 0 = Idle
			new PlayerWalkState(this),		// 1 = Walk
			new PlayerRunState(this),		// 2 = Run
			new PlayerJumpState(this),		// 3 = Jump
			new PlayerFallState(this),		// 4 = Fall
			new PlayerCrawlState(this), 	// 5 = Crawl
			new PlayerClimbState(this), 	// 6 = Climb		
		});

		m_rigidbody = GetComponent<Rigidbody>();
		m_animator = GetComponentInChildren<Animator>();
	}
	void OnEnable(){
		ChangeState(0);
	}

	void Update(){
		m_sM.Update();
		UpdateInputButtons();

		Debug.DrawRay(m_ferretMesh.transform.position, m_ferretMesh.transform.forward * 1, Color.white, .25f);

		/*RaycastHit hit;
		Physics.Raycast(transform.position, Vector3.down, out hit, 1, m_checkMask);
		Debug.Log("Normal = " + hit.normal);*/
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

	public bool m_iAmOnAClimbArea = false;
	void OnTriggerEnter(Collider col){
		if(col.GetComponent<ClimbArea>()){
			print("JE VAIS MONTER !!!");
			m_iAmOnAClimbArea = true;
		}
	}

#endregion Private functions

#region Public functions

	public void ChangeState(int index){
		m_sM.ChangeState(index);
		SetLastStateMoveSpeed();
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

	public void ClimbMove(float speed){
		//MoveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button); 	// Monter/descendre
		MoveDirection = new Vector3(0, 0, m_vAxis_Button);					 // Monter/descendre + déplacements latéraux
		MoveDirection = transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();
		m_moveDirection.x *= speed;
		m_moveDirection.z *= speed;
		m_moveDirection.y *= speed;
	}

	public void RotatePlayer(){
		// -------------------------------
		// ----- SET TARGET ROTATION -----
		// -------------------------------
		// Create three variables, move input local to the player, flattened forward direction of the camera and a local target rotation.
		Vector2 moveInput = new Vector2(m_hAxis_Button, m_vAxis_Button);
		Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
		
		Vector3 forward = Quaternion.Euler(0f, m_rotations.cameraSettings.Current.m_XAxis.Value, 0f) * Vector3.forward;
		forward.y = 0f;
		forward.Normalize();

		Quaternion targetRotation;
		
		// If the local movement direction is the opposite of forward then the target rotation should be towards the camera.
		if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
		{
			targetRotation = Quaternion.LookRotation(-forward);
		}
		else
		{
			// Otherwise the rotation should be the offset of the input from the camera's forward.
			Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
			targetRotation = Quaternion.LookRotation(cameraToInputOffset * forward);
		}

		// The desired forward direction of Ellen.
		Vector3 resultingForward = targetRotation * Vector3.forward;

		// Find the difference between the current rotation of the player and the desired rotation of the player in radians.
		float angleCurrent = Mathf.Atan2(m_ferretMesh.transform.forward.x, m_ferretMesh.transform.forward.z) * Mathf.Rad2Deg;
		float targetAngle = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

		m_AngleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);
		m_TargetRotation = targetRotation;

		// -------------------------------
		// ----- UPDATE ORIENTATION -----
		// -------------------------------
		Vector3 localInput = new Vector3(m_hAxis_Button, 0f, m_vAxis_Button);
		float groundedTurnSpeed = Mathf.Lerp(m_rotations.maxTurnSpeed, m_rotations.minTurnSpeed, m_states.m_walk.m_speed/* / m_DesiredForwardSpeed*/);
		float actualTurnSpeed = CheckCollider(false) ? groundedTurnSpeed : Vector3.Angle(m_ferretMesh.transform.forward, localInput) * k_InverseOneEighty * k_AirborneTurnSpeedProportion * groundedTurnSpeed;
		m_TargetRotation = Quaternion.RotateTowards(m_ferretMesh.transform.rotation, m_TargetRotation, actualTurnSpeed * Time.deltaTime);

		transform.rotation = Quaternion.Euler(0f, m_rotations.m_pivot.rotation.eulerAngles.y, 0f);
		m_ferretMesh.transform.rotation = m_TargetRotation;
	}

	public void ChangeCamera(){
		m_isIn3rdPersonCamera =! m_isIn3rdPersonCamera;

		if(m_isIn3rdPersonCamera){
			m_cameras.m_firstPerson.depth = 0;
			m_cameras.m_thirdPerson.depth = 1;
		}else{
			m_cameras.m_firstPerson.depth = 1;
			m_cameras.m_thirdPerson.depth = 0;
		}
	}

	public void SetLastStateMoveSpeed(){
		if(m_sM.IsLastStateIndex(0)){
			m_lastStateMoveSpeed = 0;
		}else if(m_sM.IsLastStateIndex(1)){
        	m_lastStateMoveSpeed = m_states.m_walk.m_speed;
		}else if(m_sM.IsLastStateIndex(2)){
			m_lastStateMoveSpeed = m_states.m_run.m_speed;
		}else if(m_sM.IsLastStateIndex(5)){
			m_lastStateMoveSpeed = m_states.m_crawl.m_speed;
		}
	}

	public void StartClimbInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		StartCoroutine(ClimbInterpolation(transformPosition, fromPosition, toPosition, transformRotation, fromRotation, toRotation));
	}
	IEnumerator ClimbInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		
		m_canMoveOnClimb = false;

		m_rigidbody.isKinematic = true;

		AnimationCurve animationCurve = m_states.m_climb.m_interpolation.m_snapCurve;
		float changePositionSpeed = m_states.m_climb.m_interpolation.m_changePositionSpeed;
		float changeRotationSpeed = m_states.m_climb.m_interpolation.m_changeRotationSpeed;

		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(transform.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * changePositionSpeed / moveJourneyLength;
			//transform.position = Vector3.Lerp(fromPosition, toPosition, m_moveFracJourney);
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, animationCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateJourneyLength = Vector3.Distance(fromPosition, toPosition);
			rotateFracJourney += (Time.deltaTime) * changeRotationSpeed / rotateJourneyLength;
			//transform.rotation = Quaternion.Lerp(fromRotation, toRotation, m_rotateFracJourney);
			transformRotation.rotation = Quaternion.Lerp(fromRotation, toRotation, animationCurve.Evaluate(moveFracJourney));

			yield return null;
		}

		m_rigidbody.isKinematic = false;

		m_canMoveOnClimb = true;

		//moveFracJourney = 0;
		//rotateFracJourney = 0;
	}

	public RaycastHit climbingHit;
	public bool RayCastForStartClimbing(){
		//Debug.Log("I touch " + hit.collider.gameObject.name);
		return Physics.Raycast(m_ferretMesh.transform.position, m_ferretMesh.transform.forward, out climbingHit, 1, m_checkMask);
	}

#endregion Public functions

}
