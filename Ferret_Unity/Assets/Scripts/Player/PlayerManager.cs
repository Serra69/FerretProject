using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;

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
			public bool m_canClimb = true;
			public float m_timeToCanReClimb = 0.1f;
			[Space]
			public LayerMask m_climbCollision;
			public float m_speed = 2.25f;

			public Interpolation m_interpolation = new Interpolation();
			[System.Serializable] public class Interpolation {
				[Header("Curve")]
				public AnimationCurve m_snapCurve;

				[Header("Enter Speeds")]
				public float m_enterChangePositionSpeed = 5;
				public float m_enterChangeRotationSpeed = 5;

				[Header("Exit Speeds")]
				public float m_exitChangePositionSpeed = 5;
				public float m_exitChangeRotationSpeed = 5;					
			}

			public CheckCollision m_checkCollision = new CheckCollision();
			[System.Serializable] public class CheckCollision {
				public bool m_outOfClibingAreaInTopLeft = false;
				public bool m_outOfClibingAreaInBotLeft = false;
				public bool m_outOfClibingAreaInTopRight = false;
				public bool m_outOfClibingAreaInBotRight = false;
				public bool m_outOfClibingAreaInBot = false;
			}
		}

		public Crawl m_crawl = new Crawl();
		[System.Serializable] public class Crawl {
			[HideInInspector] public bool m_isCrawling = false;
			public float m_speed = 1.5f;
		}

		public Jump m_jump = new Jump();
		[System.Serializable] public class Jump {

			public float m_jumpForce = 10f;
			public float m_minJumpTime = 0.1f;
			public float m_jumpTime = 0.5f;
			public AnimationCurve m_jumpHeightCurve = null;

			public MovementSpeed m_movementSpeed = new MovementSpeed();
			[System.Serializable] public class MovementSpeed {
				public float m_fromIdle = 1.5f;
				public float m_fromWalk = 3;
				public float m_fromRun = 6;
			}
		}

		public TakeObject m_takeObject = new TakeObject();
		[System.Serializable] public class TakeObject {
			public float m_delayToTakeAnObject = 0.15f;
			public bool m_canITakeAnObject = true;
			public bool m_iHaveAnObject = false;
			public Transform m_objectPosition;
			public ObjectToBeGrapped m_actualGrappedObject;
			public ObjectToBeGrapped m_actualClosedObjectToBeGrapped;
		}

		public Push m_push = new Push();
		[System.Serializable] public class Push {
			public float m_speed = 1.5f;
			public LayerMask m_pushLayer;
			public RaycastHit m_hit;
			public Transform m_objectTrans;
		}
	}

	[Header("Physics")]
	public PhysicsVar m_physics = new PhysicsVar();
	[System.Serializable] public class PhysicsVar {
		// public bool m_useGravity = true;
		// public float m_gravity = 9.81f;
		public Transform castCenter = null;
		public float m_topMaxDistance = 1;
		public float m_botMaxDistance = 1;

		[Space]
		public float m_maxCenterDistance = 2;

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
	public GameObject m_mesh;

	[Header("Cameras")]
	public EditCamera m_cameras = new EditCamera();
	[System.Serializable] public class EditCamera {
		public Camera m_firstPerson;
		public Camera m_thirdPerson;

		[Header("Distance")]
		public float m_miniDistanceToSeeFurret = 1;
		public bool m_showDistance = false;
		public Color m_distanceColor = Color.white;
	}

	[Header("Rotations")]
	public Rotations m_rotations = new Rotations();
	[System.Serializable] public class Rotations {
		public CameraSettings cameraSettings;
		public Transform m_pivot;
		public float minTurnSpeed = 400;
		public float maxTurnSpeed = 1200;
	}
	
	[Header("Raycasts")]
	public Raycasts m_raycasts = new Raycasts();
	[System.Serializable] public class Raycasts {
		public Transform m_topRightLeg;
		public Transform m_topLeftLeg;
		public Transform m_botRightLeg;
		public Transform m_botLeftLeg;
		public Transform m_middleAss;
		public float m_maxCastDistance = 0.5f;
		public Color m_color = Color.white;
	}

	[Space]

#endregion Public [System.Serializable] Variables

#region Public Variables

	public GameObject m_ferretMesh;
	public LayerMask m_checkLayer;

#endregion Public Variables

#region Input Buttons

	[HideInInspector] public float m_hAxis_Button;
	[HideInInspector] public float m_vAxis_Button;
	[HideInInspector] public bool m_runButton;
	[HideInInspector] public bool m_jumpButton;
	[HideInInspector] public bool m_jumpHeldButton;
	[HideInInspector] public bool m_crawlButton;
	[HideInInspector] public bool m_takeButton;
	[HideInInspector] public bool m_pushButton;

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

	bool m_endOfClimbInterpolation = false;
    public bool EndOfClimbInterpolation
    {
        get
        {
            return m_endOfClimbInterpolation;
        }
    }
  
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
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of PlayerManager");
		}

		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),		// 0 = Idle
			new PlayerWalkState(this),		// 1 = Walk
			new PlayerRunState(this),		// 2 = Run
			new PlayerJumpState(this),		// 3 = Jump
			new PlayerFallState(this),		// 4 = Fall
			new PlayerCrawlState(this), 	// 5 = Crawl
			new PlayerClimbState(this), 	// 6 = Climb
			new PlayerPushState(this),		// 7 = Push
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
		if(m_takeButton){
			GrappedObject();
		}
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
		m_takeButton = Input.GetButtonDown("Action");
		m_pushButton = Input.GetButton("Action");
	}

	/*public bool m_iAmOnAClimbArea = false;
	void OnTriggerEnter(Collider col){
		if(col.GetComponent<ClimbArea>()){
			// print("JE VAIS MONTER !!!");
			m_iAmOnAClimbArea = true;
		}
	}
	void OnTriggerExit(Collider col){
		if(col.GetComponent<ClimbArea>()){
			m_iAmOnAClimbArea = false;
		}*
	}*/

	void OnDrawGizmos(){
		if (m_physics.castCenter != null){
			Vector3 center = m_physics.castCenter.position;
			Vector3 halfExtends = new Vector3(0.3f, 0.25f, 1.25f) / 2;
			Quaternion orientation = m_ferretMesh.transform.rotation;

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(center, halfExtends);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(center + (Vector3.down * m_physics.m_botMaxDistance), halfExtends);
			Gizmos.DrawWireCube(center + (Vector3.up * m_physics.m_topMaxDistance), halfExtends);

			Debug.DrawRay(center, m_physics.castCenter.forward * m_physics.m_maxCenterDistance, Color.black, 0.01f);
		}
		
		if(m_raycasts.m_topRightLeg != null && m_raycasts.m_topLeftLeg != null && m_raycasts.m_botRightLeg != null && m_raycasts.m_botLeftLeg != null){
			// Forward
			Debug.DrawRay(m_raycasts.m_topRightLeg.position, m_raycasts.m_topRightLeg.transform.forward * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_topLeftLeg.position, m_raycasts.m_topLeftLeg.transform.forward * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			// Down
			Debug.DrawRay(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);

			Debug.DrawRay(m_raycasts.m_botRightLeg.position, - m_raycasts.m_botRightLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_botLeftLeg.position, - m_raycasts.m_botLeftLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
		}

		// Middle ass
		if(m_raycasts.m_middleAss != null){
			Debug.DrawRay(m_raycasts.m_middleAss.position, - m_raycasts.m_middleAss.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
		}

		if(m_cameras.m_showDistance){
			Gizmos.color = m_cameras.m_distanceColor;
			Gizmos.DrawWireSphere(transform.position, m_cameras.m_miniDistanceToSeeFurret);
		}
	}

#endregion Private functions

#region Public functions

	public void ChangeState(int index){
		m_sM.ChangeState(index);
		SetLastStateMoveSpeedForJump();
	}

	public bool PlayerInputIsMoving(){
		if( (m_hAxis_Button <= - m_states.m_forceInputToMove) || (m_hAxis_Button >= m_states.m_forceInputToMove) || (m_vAxis_Button <= - m_states.m_forceInputToMove) || (m_vAxis_Button >= m_states.m_forceInputToMove) ){
			return true;
		}else{
			return false;
		}
	}

	/*public bool m_colliderOnTop = false;
	public bool m_colliderOnBot = false;
	void LateUpdate(){
		m_colliderOnTop = CheckCollider(true);
		m_colliderOnBot = CheckCollider(false);
	}*/

	public bool CheckCollider(bool top){
		// Vector3 center = transform.position + new Vector3(0, top == true ? 0 : 0.1f , 0.075f);
		Vector3 center = m_physics.castCenter.position;
		Vector3 halfExtends = new Vector3(0.3f, 0.25f, 1.25f) / 2;
		
		Vector3 direction = top == true ? Vector3.up : Vector3.down;

		Quaternion orientation = m_ferretMesh.transform.rotation;
		
		if(top){
			if(Physics.BoxCast(center, halfExtends, direction, orientation, m_physics.m_topMaxDistance, m_checkLayer)){
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return true;
			}else{
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return false;
			}
		}else{
			if(Physics.BoxCast(center, halfExtends, direction, orientation, m_physics.m_botMaxDistance, m_checkLayer)){
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return true;
			}else{
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return false;
			}
		}

	}

	public void Crawl(bool isCrawling){
			m_states.m_crawl.m_isCrawling = isCrawling;

		if(isCrawling){
			m_colliders.m_base.m_headColl.enabled = false;
			m_colliders.m_base.m_bodyColl.enabled = false;

			m_colliders.m_crawl.m_headColl.enabled = true;
			m_colliders.m_crawl.m_bodyColl.enabled = true;

			m_mesh.transform.localScale = new Vector3(m_mesh.transform.localScale.x, m_mesh.transform.localScale.y / 2, m_mesh.transform.localScale.z);
		}else{
			m_colliders.m_base.m_headColl.enabled = true;
			m_colliders.m_base.m_bodyColl.enabled = true;

			m_colliders.m_crawl.m_headColl.enabled = false;
			m_colliders.m_crawl.m_bodyColl.enabled = false;

			m_mesh.transform.localScale = new Vector3(m_mesh.transform.localScale.x, m_mesh.transform.localScale.y * 2, m_mesh.transform.localScale.z);
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

	Quaternion m_normal;
	public void InclinePlayer(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, - transform.up, out hit, /*m_raycasts.m_maxCastDistance*/ Mathf.Infinity, m_checkLayer)){
			m_normal = Quaternion.Euler(Quaternion.Euler(hit.normal).x, m_ferretMesh.transform.rotation.y, m_ferretMesh.transform.rotation.z);
			// m_ferretMesh.transform.rotation = m_normal;

			// Debug.Log("Normal map = " + m_normal.eulerAngles);
		}
	}

	public void DoMove(){
		if(MoveDirection != Vector3.zero){
			m_rigidbody.MovePosition(transform.position + MoveDirection * Time.fixedDeltaTime);
		}
	}

	public void ClimbMove(float speed){
		MoveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button);		// Monter/descendre + déplacements latéraux
		MoveDirection = transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();

		// X
		if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight){
			if(m_moveDirection.x < 0){
				m_moveDirection.x *= speed;
			}else{
				m_moveDirection.x = 0;
			}
		}else if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft){
			if(m_moveDirection.x > 0){
				m_moveDirection.x *= speed;
			}else{
				m_moveDirection.x = 0;
			}
		}else if((m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight) && (m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft)){
			m_moveDirection.x *= speed;
		}

		// Y
		m_moveDirection.z *= speed;

		// Z
		if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot){
			if(m_moveDirection.y > 0){
				m_moveDirection.y *= speed;
			}else{
				m_moveDirection.y = 0;
			}
		}

	}

	public void PushMove(float speed){
		MoveDirection = new Vector3(0, 0, m_vAxis_Button);
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

	public void SetLastStateMoveSpeedForJump(){
		if(m_sM.IsLastStateIndex(0)){
			m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromIdle;
		}else if(m_sM.IsLastStateIndex(1)){
        	m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
		}else if(m_sM.IsLastStateIndex(2)){
			m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
		}
	}

	public void StartClimbInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation, bool enter = true){
		StartCoroutine(ClimbInterpolation(enter, transformPosition, fromPosition, toPosition, transformRotation, fromRotation, toRotation));
	}
	IEnumerator ClimbInterpolation(bool enter, Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		
		m_canMoveOnClimb = false;

		m_rigidbody.isKinematic = true;

		AnimationCurve animationCurve = m_states.m_climb.m_interpolation.m_snapCurve;

		float changePositionSpeed;
		float changeRotationSpeed;

		if(enter){
			changePositionSpeed = m_states.m_climb.m_interpolation.m_enterChangePositionSpeed;
			changeRotationSpeed = m_states.m_climb.m_interpolation.m_enterChangeRotationSpeed;
		}else{
			changePositionSpeed = m_states.m_climb.m_interpolation.m_exitChangePositionSpeed;
			changeRotationSpeed = m_states.m_climb.m_interpolation.m_exitChangeRotationSpeed;
		}

		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(transform.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * changePositionSpeed / moveJourneyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, animationCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateJourneyLength = Vector3.Distance(fromPosition, toPosition);
			rotateFracJourney += (Time.deltaTime) * changeRotationSpeed / rotateJourneyLength;
			transformRotation.rotation = Quaternion.Slerp(fromRotation, toRotation, animationCurve.Evaluate(rotateFracJourney));

			yield return null;
		}

		m_rigidbody.isKinematic = false;

		m_canMoveOnClimb = true;

		m_endOfClimbInterpolation = true;
		yield return new WaitForSeconds(0.5f);
		m_endOfClimbInterpolation = false;

		//moveFracJourney = 0;
		//rotateFracJourney = 0;
	}

	public void StartClimbCooldown(){
		StartCoroutine(ClimbCooldownCorout());
	}
	IEnumerator ClimbCooldownCorout(){
		m_states.m_climb.m_canClimb = false;
		yield return new WaitForSeconds(m_states.m_climb.m_timeToCanReClimb);
		m_states.m_climb.m_canClimb = true;
	}
	
	public void StartRotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){
		StartCoroutine(RotateInterpolation(trans, fromRotation, toRotation));
	}
	IEnumerator RotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){

		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(transform.rotation != toRotation){
			// MoveRotation
			rotateJourneyLength = Quaternion.Dot(fromRotation, toRotation);
			rotateFracJourney += (Time.deltaTime) * m_states.m_climb.m_interpolation.m_enterChangeRotationSpeed / rotateJourneyLength;
			trans.rotation = Quaternion.Slerp(fromRotation, toRotation, m_states.m_climb.m_interpolation.m_snapCurve.Evaluate(rotateFracJourney));

			yield return null;
		}
	}

	public RaycastHit topRightClimbHit;
	public RaycastHit topLeftClimbHit;
	public RaycastHit botRightClimbHit;
	public RaycastHit boteftClimbHit;
	public bool RayCastForwardToStartClimbing(){
		//Debug.Log("I touch " + hit.collider.gameObject.name);
		
		if(Physics.Raycast(m_raycasts.m_topRightLeg.position, m_raycasts.m_topRightLeg.transform.forward, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)
		&&
		Physics.Raycast(m_raycasts.m_topLeftLeg.position, m_raycasts.m_topLeftLeg.transform.forward, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			return true;
		}else{
			return false;
		}
		/*Physics.Raycast(m_raycasts.m_topRightLeg.position, m_raycasts.m_topRightLeg.transform.forward, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		Physics.Raycast(m_raycasts.m_topLeftLeg.position, m_raycasts.m_topLeftLeg.transform.forward, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		if(topRightClimbHit.collider.CompareTag("ClimbArea") && topLeftClimbHit.collider.CompareTag("ClimbArea")){
			return true;
		}else{
			return false;
		}*/
	}
	public bool RayCastDownToStopClimbing(){
		//Debug.Log("I touch " + hit.collider.gameObject.name);
		if(Physics.Raycast(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)
		||
		Physics.Raycast(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			return true;
		}else{
			return false;
		}
		/*Physics.Raycast(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		Physics.Raycast(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		
		if(topRightClimbHit.collider.CompareTag("ClimbArea") || topLeftClimbHit.collider.CompareTag("ClimbArea")){
			return true;
		}else{
			return false;
		}*/
	}

	public void RayCastDownToStopSideScrollingMovement(){

		// RIGHT check
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight = !Physics.Raycast(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight = !Physics.Raycast(m_raycasts.m_botRightLeg.position, - m_raycasts.m_botRightLeg.transform.up, out botRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		/*if(Physics.Raycast(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			if(topRightClimbHit.collider.CompareTag("ClimbArea")){
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInRight = false;
			}else{
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInRight = true;
			}
		}*/

		// LEFT check
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft = !Physics.Raycast(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft = !Physics.Raycast(m_raycasts.m_botLeftLeg.position, - m_raycasts.m_botLeftLeg.transform.up, out boteftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		/*if(Physics.Raycast(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			if(topLeftClimbHit.collider.CompareTag("ClimbArea")){
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInLeft = false;
			}else{
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInLeft = true;
			}
		}*/

		// BOT check
		RaycastHit hit;
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot = !Physics.Raycast(m_raycasts.m_middleAss.position, - m_raycasts.m_middleAss.transform.up, out hit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		/*if(Physics.Raycast(m_raycasts.m_middleAss.position, - m_raycasts.m_middleAss.transform.up, out hit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			if(hit.collider.CompareTag("ClimbArea")){
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot = false;
			}else{
				m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot = true;
			}
		}*/
	}

	public bool RayCastToCanPush(){
		return Physics.Raycast(m_physics.castCenter.position, m_physics.castCenter.forward, out m_states.m_push.m_hit, m_physics.m_maxCenterDistance, m_states.m_push.m_pushLayer);
	}

	public void WhenCameraIsCloseToTheFerret(float distance){
		if(distance < m_cameras.m_miniDistanceToSeeFurret){
			m_mesh.SetActive(false);
		}else{
			m_mesh.SetActive(true);
		}
	}

	public void WhenCameraGoToFirstPlayerMode(){
		transform.rotation = Quaternion.Euler(0f, m_rotations.m_pivot.rotation.eulerAngles.y, 0f);
	}

	public void SetClosedObjectToBeGrapped(bool isClosedObject, ObjectToBeGrapped obj){
		if(m_states.m_takeObject.m_actualGrappedObject != obj && isClosedObject){
			m_states.m_takeObject.m_actualClosedObjectToBeGrapped = obj;
		}else if(!isClosedObject && m_states.m_takeObject.m_actualClosedObjectToBeGrapped == obj){
			m_states.m_takeObject.m_actualClosedObjectToBeGrapped = null;
		}
	}

	public void GrappedObject()
	{
		if(!m_states.m_takeObject.m_canITakeAnObject){
			return;
		}
		StartCoroutine(DelayToTakeAnObject());

		if(m_states.m_takeObject.m_actualGrappedObject != null){
			m_states.m_takeObject.m_actualGrappedObject.On_ObjectIsTake(false);
			m_states.m_takeObject.m_actualGrappedObject = null;
		}
		if(m_states.m_takeObject.m_actualClosedObjectToBeGrapped != null){
			m_states.m_takeObject.m_actualGrappedObject = m_states.m_takeObject.m_actualClosedObjectToBeGrapped;
			m_states.m_takeObject.m_actualGrappedObject.On_ObjectIsTake(true);
			m_states.m_takeObject.m_actualClosedObjectToBeGrapped = null;
		}
	}
	IEnumerator DelayToTakeAnObject(){
		m_states.m_takeObject.m_canITakeAnObject = false;
		yield return new WaitForSeconds(m_states.m_takeObject.m_delayToTakeAnObject);
		m_states.m_takeObject.m_canITakeAnObject = true;
	}

	public void SetObjectInChildrenOfFerret(Transform fromTrans, Transform toTrans = null){
		fromTrans.SetParent(toTrans);
	}

#endregion Public functions
public GameObject m_rightHit;
public GameObject m_leftHit;
}
