﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : ClimbTypesArea {

	public static PlayerManager Instance;

	// Utiliser ça pour faire tout les changements de "transform.position"
	/*Vector3 TransformPosition{
		get{
			return transform.position;
		}
		set{
			transform.position = value;
		}
	}*/

#region Public [System.Serializable] Variables

	public PlayerDebugs m_playerDebugs = new PlayerDebugs();
	[System.Serializable] public class PlayerDebugs {
		[Range(0, 9)] public int m_startPlayerState = 0;
		public bool m_playerCanMove = true;
		public bool m_playerCanDie = true;
		public bool m_useAudioListenerOnFerret = true;
		public PauseGame m_pauseGame;
		public float m_timeToJumpAfterEndPause = 0.1f;
	}

	public StateMachine m_sM = new StateMachine(); 

	[Header("States")]
	public States m_states = new States();
	[System.Serializable] public class States {

		public Iddle m_iddle = new Iddle();
		[System.Serializable] public class Iddle {
			public float m_minTimeToSwitchIddle2 = 5;
			public float m_maxTimeToSwitchIddle2 = 10;
		}

		public Walk m_walk = new Walk();
		[System.Serializable] public class Walk {
			[Range(0, 1)] public float m_forceInputToMove = 0.1f;
			public float m_speed = 3f;
		}

		public Run m_run = new Run();
		[System.Serializable] public class Run {
			public float m_changeRunSpeed = 3;
			[Range(0, 1)] public float m_forceInputToRun = 0.5f;
			public float m_speed = 6f;
		}

		public Climb m_climb = new Climb();
		[System.Serializable] public class Climb {
			public bool m_canClimb = true;
			public float m_waitClimbCameraMove = 0.25f;

			// public float m_timeToCanReClimb = 0.1f;
			[Space]
			public LayerMask m_climbCollision;
			public float m_speed = 2.25f;
			[Space]
			public float m_timeToEndClimbAnim = 1;
			public Transform m_endClimbAnimPos;

			public Interpolation m_interpolation = new Interpolation();
			[System.Serializable] public class Interpolation {
				[Header("Curve")]
				public AnimationCurve m_snapCurve;

				[Header("Speeds")]
				public float m_enterChangePositionSpeed = 5;
				public float m_enterChangeRotationSpeed = 5;

				[Header("Fall Speeds")]
				public float m_fallPositionSpeed = 5;
				public float m_fallRotationSpeed = 5;
				[Space]
				public float m_fallingDistance = 1.5f;
				public float m_fallingInY = 1f;
			}

			public CheckCollision m_checkCollision = new CheckCollision();
			[System.Serializable] public class CheckCollision {
				public bool m_outOfClibingAreaInTopMiddle = false;
				public bool m_outOfClibingAreaInTopLeft = false;
				public bool m_outOfClibingAreaInBotLeft = false;
				public bool m_outOfClibingAreaInTopRight = false;
				public bool m_outOfClibingAreaInBotRight = false;
				public bool m_outOfClibingAreaInBot = false;
			}

			public FX m_fx = new FX();
			[System.Serializable] public class FX {
				public GameObject[] m_climbMoveFX;
				[Range(0, 1)] public float m_inputToAddFx = 0.25f;
				public float m_climbDelay = 0.75f;
			}
		}

		public Crawl m_crawl = new Crawl();
		[System.Serializable] public class Crawl {
			[HideInInspector] public bool m_isCrawling = false;
			public float m_speed = 1.5f;
		}

		public Jump m_jump = new Jump();
		[System.Serializable] public class Jump {
			public float m_timeToJumpImpulse = 0.2f;
			public float m_jumpForce = 10f;
			public float m_minJumpTime = 0.1f;
			public float m_jumpTime = 0.5f;
			public AnimationCurve m_jumpHeightCurve = null;

			public MovementSpeed m_movementSpeed = new MovementSpeed();
			[System.Serializable] public class MovementSpeed {
				public float m_fromIdle = 1.5f;
				public float m_fromWalk = 3;
				public float m_fromRun = 6;
				public float m_fromCrawl = 1.5f;
			}
			public AirControl m_airControl = new AirControl();
			[System.Serializable] public class AirControl {
				public float m_fullAirControlAngle = 90;
				public float m_moveCoef = 0.5f;
			}
		}

		public Fall m_fall = new Fall();
		[System.Serializable] public class Fall {
  			public float m_duration = 3;

			[Header("FX")]
			public float m_landingFxCooldown = 0.25f;
			public GameObject m_landingFx;
			public Transform m_landingPos;
			[Space]
			public float m_timeToDoMinSound = 0.25f;
			public float m_timeToDoMaxSound = 1f;
			public AnimationCurve m_soundCurve;
			[Range(0,1)] public float m_minSoundVolume = 0.25f;
			[Range(0,1)] public float m_maxSoundVolume = 1f;
		}

		public TakeObject m_takeObject = new TakeObject();
		[System.Serializable] public class TakeObject {
			public float m_timeToTakeObject = 0.15f;
			public float m_delayToTakeAnObject = 0.15f;
			public bool m_canITakeAnObject = true;
			public bool m_iHaveAnObject = false;
			public Transform m_objectPosition;
			public ObjectToBeGrapped m_actualGrappedObject;
			public ObjectToBeGrapped m_actualClosedObjectToBeGrapped;

			[Header("FX")]
			public GameObject m_takeObjectFx;
		}

		public Push m_push = new Push();
		[System.Serializable] public class Push {
			[Header("Push")]
			public float m_speed = 1.5f;
			public LayerMask m_pushLayer;
			public RaycastHit m_hit;
			public Transform m_objectTrans;

			[Header("Snap interpolation")]
			public float m_snapSpeed = 3;
			public AnimationCurve m_snapCurve;
		}

		public Death m_death = new Death();
		[System.Serializable] public class Death {
			[Header("References")]
			[Tooltip("Reference a Start transform for death. If empty use start transform of player")]
			public Transform m_startTransform;
			public Animator m_eyesAnimator;

			[Header("Speeds")]
			public float m_changePositionSpeed = 5;
			public float m_changeRotationSpeed = 5;

			[Header("Timers")]
			public float m_timeToMoveInFirstPerson = 1;
			public float m_timeToCloseEyes = 2;
			public float m_timeToResetPosition = 3;
			public float m_timeToOpenEyes = 4;
			public float m_timeToCanMove = 5;
		}

	}

	[Header("Physics")]
	public PhysicsVar m_physics = new PhysicsVar();
	[System.Serializable] public class PhysicsVar {
		public Transform castCenter = null;
		public float m_topMaxDistance = 1;
		public float m_botMaxDistance = 1;
		public float m_maxCenterDistance = 2;
		[Space]
		public LayerMask m_groundLayer;
	}	

	[Header("Colliders")]
	public Colliders m_colliders = new Colliders();
	[System.Serializable] public class Colliders {
		public CapsuleCollider m_coll;

		public BaseCollider m_baseCollider = new BaseCollider();
		[System.Serializable] public class BaseCollider {
			public Vector3 m_center;
			public float m_radius;
			public float m_height;
		}

		public CrawlCollider m_crawlCollider = new CrawlCollider();
		[System.Serializable] public class CrawlCollider {
			public Vector3 m_center;
			public float m_radius;
			public float m_height;
		}
	}

	[Header("Meshes")]
	public Meshes m_meshes = new Meshes();
	[System.Serializable] public class Meshes {
		public GameObject m_rotateFerret;
		public GameObject m_ferretMesh;
		public GameObject m_meshSkin;
	}

	[Header("Updates")]
	public Updates m_updates = new Updates();
	[System.Serializable] public class Updates {
		public CameraPivot m_pivotCamera;
		public CameraFollowLookAt m_cameraFollowLookAt;
		public FollowPlayer m_followPlayer;
	}

	[Header("Cameras")]
	public EditCamera m_cameras = new EditCamera();
	[System.Serializable] public class EditCamera {
		public Transform m_pivotCamera;
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

		[Header("Turn speed when grounded")]
		// [Range(0, 1)] public float m_turnSpeedGrounded = 0.5f;
		// public float m_minTurnSpeedGrounded = 800;
		public float m_turnSpeedGrounded = 500;

		[Header("Turn speed when is not grounded")]
		// [Range(0, 1)] public float m_turnSpeedWithoutGrounded = 0.5f;
		// public float m_minTurnSpeedWithoutGronuded = 400;
		public float m_turnSpeedWithoutGrounded = 100;
	}
	
	[Header("Raycasts")]
	public Raycasts m_raycasts = new Raycasts();
	[System.Serializable] public class Raycasts {
		public Transform m_forwardTopRightLeg;
		public Transform m_forwardTopLeftLeg;
		public float m_forwardDistance = 0.5f;
		[Space]
		public Transform m_topMiddleLeg;
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

#region Encapsulate

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
		set
        {
            m_rigidbody = value;
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

	bool m_isInLerpRotation = false;
	public bool IsInLerpRotation
    {
        get
        {
            return m_isInLerpRotation;
        }
		set
        {
            m_isInLerpRotation = value;
        }
    }

	bool m_endOfOrientationAfterClimb = false;
	public bool EndOfOrientationAfterClimb{
        get{
            return m_endOfOrientationAfterClimb;
        }
		set{
            m_endOfOrientationAfterClimb = value;
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

    PushableObject m_pushableObject;
    public PushableObject PushableObject
    {
        get
        {
            return m_pushableObject;
        }
        set
        {
            m_pushableObject = value;
        }
    }

    bool m_playerIsDead = false;
    public bool PlayerIsDead
    {
        get
        {
            return m_playerIsDead;
        }
		set
        {
            m_playerIsDead = value;
        }
    }

    SwitchCamera m_switchCamera;
    public SwitchCamera SwitchCamera
    {
        get
        {
            return m_switchCamera;
        }
    }

	FirstPersonCamera m_firstPersonCamera;
    public FirstPersonCamera FirstPersonCamera
    {
        get
        {
            return m_firstPersonCamera;
        }
    }

	ClimbArea m_climbArea;
    public ClimbArea ClimbArea
    {
        get
        {
            return m_climbArea;
        }
    }

    float m_timerOfPressSpace = 0;
    public float TimerOfPressSpace
    {
        get
        {
            return m_timerOfPressSpace;
        }

        set
        {
            m_timerOfPressSpace = value;
        }
    }

    bool m_canHideFerret = true;
    public bool CanHideFerret{
        get{
            return m_canHideFerret;
        }
        set{
            m_canHideFerret = value;
        }
    }

	bool m_canMoveOnPush = false;
    public bool CanMoveOnPush
    {
        get
        {
            return m_canMoveOnPush;
        }
    }

    bool m_canJumpAfterEndPause = true;
    public bool CanJumpAfterEndPause{
        get{
            return m_canJumpAfterEndPause;
        }
    }

#endregion Encapsulate

#region Private Variables

	// ----------------------------
    // ----- FOR THE ROTATION -----
    const float k_InverseOneEighty = 1f / 180f;
	const float k_AirborneTurnSpeedProportion = 5.4f;
	protected float m_AngleDiff;
	Quaternion m_TargetRotation;
	// ----------------------------

	Vector3 m_savePosition;
	Quaternion m_saveRotation;

	bool m_canHadeLandingFx = true;

#endregion Private Variables

#region Private functions

    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of PlayerManager");
		}

		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),				// 0 = Idle
			new PlayerWalkState(this),				// 1 = Walk
			new PlayerRunState(this),				// 2 = Run
			new PlayerJumpState(this),				// 3 = Jump
			new PlayerFallState(this),				// 4 = Fall
			new PlayerCrawlState(this), 			// 5 = Crawl
			new PlayerClimbState(this), 			// 6 = Climb
			new PlayerPushState(this),				// 7 = Push
			new PlayerDeathState(this),				// 8 = Death
			new PlayerCinematicState(this),			// 9 = Cinematic
		});

		m_rigidbody = GetComponent<Rigidbody>();
		m_animator = m_meshes.m_ferretMesh.GetComponent<Animator>();
	}
	void Start(){
		m_switchCamera = SwitchCamera.Instance;
		m_firstPersonCamera = FirstPersonCamera.Instance;

		if(m_states.m_death.m_startTransform != null){
			m_savePosition = m_states.m_death.m_startTransform.position;
			m_saveRotation = m_states.m_death.m_startTransform.rotation;
		}else{
			m_savePosition = transform.position;
			m_saveRotation = transform.rotation;
		}
	}
	void OnEnable(){
		ChangeState(m_playerDebugs.m_startPlayerState);
	}

	void FixedUpdate(){
		if(!m_playerDebugs.m_playerCanMove)
			return;

		MoveDirection = Vector3.zero;

		m_updates.m_pivotCamera.UpdateCameraPivot();
		// m_updates.m_cameraFollowLookAt.UpdateCameraFollowLookAt();
		// m_updates.m_followPlayer.UpdateFollowPlayerPosition();
		// m_updates.m_followPlayer.UpdateFollowPlayerRotation();

		m_sM.FixedUpdate();
		CheckAirControl();
		DoMove();
	}

	void Update(){
		if(!m_playerDebugs.m_playerCanMove)
			return;

		m_sM.Update();
		UpdateInputButtons();
		if(m_takeButton && !RayCastToCanPush()){
			Animator.SetTrigger("Take");
			GrappedObject();
			if(m_closedInteractiveObject != null){
				m_closedInteractiveObject.On_ObjectIsTake();
			}
		}
		// if(Input.GetKeyDown(KeyCode.A)){
		// 	m_switchCamera.SwitchCameraType();
		// }
		if(m_sM.CompareState(0)){
			SetIddleTimer();
		}

		// m_updates.m_pivotCamera.UpdateCameraPivot();
		// m_updates.m_cameraFollowLookAt.UpdateCameraFollowLookAt();
		// m_updates.m_followPlayer.UpdateFollowPlayerPosition();
		// m_updates.m_followPlayer.UpdateFollowPlayerRotation();
	}

	void LateUpdate(){
		if(!m_playerDebugs.m_playerCanMove)
			return;

		m_sM.LateUpdate();

		// m_updates.m_pivotCamera.UpdateCameraPivot();
		m_updates.m_cameraFollowLookAt.UpdateCameraFollowLookAt();
		// m_updates.m_followPlayer.UpdateFollowPlayerPosition();
		// m_updates.m_followPlayer.UpdateFollowPlayerRotation();
	}

	void UpdateInputButtons(){
		m_hAxis_Button = Input.GetAxis("Horizontal");
		m_vAxis_Button = Input.GetAxis("Vertical");
		// m_runButton = Input.GetButton("Run");

		float f = Input.GetAxisRaw("Run");

		if(f != 0){
			m_runButton = true;	
		}else{
			m_runButton = false;
		}

		if(!m_playerDebugs.m_pauseGame.m_pause){
			m_jumpButton = Input.GetButtonDown("Jump");
		}

		m_jumpHeldButton = Input.GetButton("Jump");
		m_crawlButton = Input.GetButtonDown("Crawl");
		m_takeButton = Input.GetButtonDown("Action");
		m_pushButton = Input.GetButton("Action");
	}

	void OnDrawGizmos(){
		if (m_physics.castCenter != null){
			Vector3 center = m_physics.castCenter.position;
			Vector3 halfExtends = new Vector3(0.3f, 0.25f, 1.25f) / 2;
			Quaternion orientation = m_meshes.m_rotateFerret.transform.rotation;

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(center, halfExtends);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(center + (Vector3.down * m_physics.m_botMaxDistance), halfExtends);
			Gizmos.DrawWireCube(center + (Vector3.up * m_physics.m_topMaxDistance), halfExtends);

			Debug.DrawRay(center, m_physics.castCenter.forward * m_physics.m_maxCenterDistance, Color.black, 0.01f);
		}
		
		if(m_raycasts.m_forwardTopRightLeg != null && m_raycasts.m_forwardTopLeftLeg != null){
			// Forward
			Debug.DrawRay(m_raycasts.m_forwardTopRightLeg.position, m_raycasts.m_forwardTopRightLeg.transform.forward * m_raycasts.m_forwardDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_forwardTopLeftLeg.position, m_raycasts.m_forwardTopLeftLeg.transform.forward * m_raycasts.m_forwardDistance, m_raycasts.m_color, .05f);
		}

		if(m_raycasts.m_topRightLeg != null && m_raycasts.m_topLeftLeg != null && m_raycasts.m_botRightLeg != null && m_raycasts.m_botLeftLeg != null && m_raycasts.m_topMiddleLeg != null){
			// Down
			Debug.DrawRay(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);

			Debug.DrawRay(m_raycasts.m_botRightLeg.position, - m_raycasts.m_botRightLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
			Debug.DrawRay(m_raycasts.m_botLeftLeg.position, - m_raycasts.m_botLeftLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);

			Debug.DrawRay(m_raycasts.m_topMiddleLeg.position, - m_raycasts.m_topMiddleLeg.transform.up * m_raycasts.m_maxCastDistance, m_raycasts.m_color, .05f);
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
		if((m_hAxis_Button <= - m_states.m_walk.m_forceInputToMove) || (m_hAxis_Button >= m_states.m_walk.m_forceInputToMove) || (m_vAxis_Button <= - m_states.m_walk.m_forceInputToMove) || (m_vAxis_Button >= m_states.m_walk.m_forceInputToMove)){
			return true;
		}else{
			return false;
		}
	}
	public bool PlayerInputIsRuning(){
		if((m_hAxis_Button <= - m_states.m_run.m_forceInputToRun) || (m_hAxis_Button >= m_states.m_run.m_forceInputToRun) || (m_vAxis_Button <= - m_states.m_run.m_forceInputToRun) || (m_vAxis_Button >= m_states.m_run.m_forceInputToRun)){
			return true;
		}else{
			return false;
		}
	}

	public float GetPlayerInputValue(){
		float h = m_hAxis_Button;
		float v = m_vAxis_Button;

		if(h < 0){
			h = -h;
		}
		if(v < 0){
			v = -v;
		}

		float total;
		total = h + v;

		float finalyTotal = Mathf.InverseLerp(0, 1, total);
		// Debug.Log("finalyTotal = " + finalyTotal);

		return finalyTotal;
	}

	public bool CheckCollider(bool top){
		// Vector3 center = transform.position + new Vector3(0, top == true ? 0 : 0.1f , 0.075f);
		Vector3 center = m_physics.castCenter.position;
		Vector3 halfExtends = new Vector3(0.3f, 0.25f, 1.25f) / 2;
		
		Vector3 direction = top == true ? Vector3.up : Vector3.down;

		Quaternion orientation = m_meshes.m_rotateFerret.transform.rotation;
		
		if(top){
			if(Physics.BoxCast(center, halfExtends, direction, orientation, m_physics.m_topMaxDistance, m_physics.m_groundLayer)){
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return true;
			}else{
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				return false;
			}
		}else{
			if(Physics.BoxCast(center, halfExtends, direction, orientation, m_physics.m_botMaxDistance, m_physics.m_groundLayer)){
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
				m_states.m_climb.m_canClimb = true;
      			Animator.SetBool("isGrounded", true);
				return true;
			}else{
				//Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, m_checkLayer)));
      			Animator.SetBool("isGrounded", false);
				return false;
			}
		}

	}

	public void Crawl(bool isCrawling){
		m_states.m_crawl.m_isCrawling = isCrawling;

		if(isCrawling){
			m_colliders.m_coll.center = m_colliders.m_crawlCollider.m_center;
			m_colliders.m_coll.radius = m_colliders.m_crawlCollider.m_radius;
			m_colliders.m_coll.height = m_colliders.m_crawlCollider.m_height;

			// m_meshes.m_ferretMesh.transform.localScale = new Vector3(m_meshes.m_ferretMesh.transform.localScale.x, m_meshes.m_ferretMesh.transform.localScale.y / 2, m_meshes.m_ferretMesh.transform.localScale.z);
		}else{
			m_colliders.m_coll.center = m_colliders.m_baseCollider.m_center;
			m_colliders.m_coll.radius = m_colliders.m_baseCollider.m_radius;
			m_colliders.m_coll.height = m_colliders.m_baseCollider.m_height;

			// m_meshes.m_ferretMesh.transform.localScale = new Vector3(m_meshes.m_ferretMesh.transform.localScale.x, m_meshes.m_ferretMesh.transform.localScale.y * 2, m_meshes.m_ferretMesh.transform.localScale.z);
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
	public void MoveFirstPersonPlayer(float speed, float y = 0, float jumpSpeed = 0){
		MoveDirection = new Vector3(m_hAxis_Button, y, m_vAxis_Button);
		MoveDirection = m_firstPersonCamera.transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();
		m_moveDirection.x *= speed;
		m_moveDirection.z *= speed;
		m_moveDirection.y *= jumpSpeed;
	}

	/*public void RotatePlayerWithSlope(){
		Vector3 moveInput = new Vector2(m_hAxis_Button, m_vAxis_Button).normalized;
		moveInput.z = moveInput.y;
		moveInput.y =0;

		Vector3 moveDirection = m_rotations.m_pivot.TransformDirection(moveInput);
		Quaternion rotation = Quaternion.LookRotation(moveDirection, transform.up);
		rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);

		RaycastHit hit;
		Vector3 desiredOrigin = transform.position + (transform.up * 3);
		if(Physics.Raycast(desiredOrigin, - transform.up, out hit, /*m_raycasts.m_maxCastDistance* Mathf.Infinity, m_physics.m_groundLayer)){
			// m_normal = Quaternion.Euler(Quaternion.Euler(hit.normal).x, m_ferretMesh.transform.rotation.y, m_ferretMesh.transform.rotation.z);
			
			Debug.Log("Normal map = " + hit.normal);
			// transform.rotation = Quaternion.Euler(hit.normal);
			// m_desiredRotation *= Quaternion.FromToRotation(transform.up, hit.normal) * m_rigidbody.rotation;
			// Debug.Log("m_desiredRotation  = " + m_desiredRotation.eulerAngles );
			// Debug.DrawLine(desiredOrigin, transform.position + (m_desiredRotation * hit.normal) * 10, Color.magenta, 10f);
			// float angle = Vector3.Angle(hit.normal, Vector3.up);
			// transform.rotation = Quaternion.Euler(angle, transform.rotation.y, transform.rotation.z);
		}
		else
		{
			Debug.LogError("Raycast doesnt work");
		}
	}*/

	void CheckAirControl(){
		if(!CheckCollider(false)){
			float angle = Vector3.Angle(MoveDirection, m_meshes.m_rotateFerret.transform.forward);
			if(angle > m_states.m_jump.m_airControl.m_fullAirControlAngle){
				MoveDirection = new Vector3(MoveDirection.x * m_states.m_jump.m_airControl.m_moveCoef, MoveDirection.y, MoveDirection.z * m_states.m_jump.m_airControl.m_moveCoef);
			}
		}
	}

	public void DoMove(){
		m_rigidbody.velocity = MoveDirection;
	}

	public void ClimbMove(float speed){
		MoveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button);		// Monter/descendre + déplacements latéraux
		MoveDirection = transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();

		Animator.SetFloat("XClimb", m_hAxis_Button);
		Animator.SetFloat("YClimb", m_vAxis_Button);

		if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight){
			
			if(ClimbArea.m_climbType == ClimbTypes.forward || ClimbArea.m_climbType == ClimbTypes.right){
				if(m_moveDirection.x < 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}

				if(m_moveDirection.z < 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
			}else{
				if(m_moveDirection.x > 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}

				if(m_moveDirection.z > 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
			}
		}else if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft){

			if(ClimbArea.m_climbType == ClimbTypes.backward || ClimbArea.m_climbType == ClimbTypes.left){
				if(m_moveDirection.x < 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}

				if(m_moveDirection.z < 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
			}else{
				if(m_moveDirection.x > 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}

				if(m_moveDirection.z > 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
			}
		}else if((m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight) && (m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft || m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft)){
			m_moveDirection.x *= speed;
			m_moveDirection.z *= speed;
		}

		if(m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot){
			if(m_moveDirection.y > 0){
				m_moveDirection.y *= speed;
			}else{
				m_moveDirection.y = 0;
			}
		}else if(!m_climbArea.m_areaCanBeFinishedClimbable && m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopMiddle){
			if(m_moveDirection.y > 0){
				m_moveDirection.y = 0;
			}else{
				m_moveDirection.y *= speed;
			}
		}
		m_moveDirection *= speed;

		if(GetPlayerInputValue() > m_states.m_climb.m_fx.m_inputToAddFx){
			ClimbSound();
		}
	}
	float m_nextPlay = 0;
	void ClimbSound(){
		if(Time.time > m_nextPlay){
			m_nextPlay = Time.time + m_states.m_climb.m_fx.m_climbDelay;
			GameObject step = m_states.m_climb.m_fx.m_climbMoveFX[Random.Range(0, m_states.m_climb.m_fx.m_climbMoveFX.Length)];
			Level.AddFX(step, transform.position, transform.rotation);
		}
	}

	void PushSound(){
		if(GetPlayerInputValue() > 0.1f){
			PushableObject.AddSound();
		}
	}
	public void PushMove(float speed){

		Vector3 worldDirection = new Vector3(0, 0, m_vAxis_Button);
		worldDirection.Normalize();

		MoveDirection = new Vector3(0, 0, m_vAxis_Button);
		MoveDirection = transform.TransformDirection(MoveDirection);
		MoveDirection.Normalize();

		if(!PushableObject.BoxCollForward && !PushableObject.BoxCollBackward){
			PushSound();
		}

		if(RaycastFromFerretAss() && !PushableObject.BoxCollBackward){
			m_moveDirection.x *= speed;
			m_moveDirection.z *= speed;
		}else{

			if(m_pushableObject.ClosedPosition == 2 || m_pushableObject.ClosedPosition == 3){
				if(m_moveDirection.z > 0){
					m_moveDirection.z *= speed;
				}else{
					m_moveDirection.z = 0;
				}
				if(m_moveDirection.x > 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}
			}else{
				if(m_moveDirection.z < 0){
					m_moveDirection.z *= speed;
				}else{
					m_moveDirection.z = 0;
				}
				if(m_moveDirection.x < 0){
					m_moveDirection.x *= speed;
				}else{
					m_moveDirection.x = 0;
				}
			}
		}

		if(!PushableObject.CanMove || PushableObject.BoxCollForward){

			if(m_pushableObject.ClosedPosition == 2 || m_pushableObject.ClosedPosition == 3){
				if(m_moveDirection.z > 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
				if(m_moveDirection.x > 0){
					m_moveDirection.x = 0;
				}else{
					m_moveDirection.x *= speed;
				}
			}else{
				if(m_moveDirection.z < 0){
					m_moveDirection.z = 0;
				}else{
					m_moveDirection.z *= speed;
				}
				if(m_moveDirection.x < 0){
					m_moveDirection.x = 0;
				}else{
					m_moveDirection.x *= speed;
				}
			}
		}
	}

	public void RotatePlayer(){
		// SET TARGET ROTATION
		// Create three variables, move input local to the player, flattened forward direction of the camera and a local target rotation.
		Vector2 moveInput = new Vector2(m_hAxis_Button, m_vAxis_Button);
		Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
		
		// Vector3 forward = Quaternion.Euler(0f, m_rotations.cameraSettings.Current.m_XAxis.Value, 0f) * Vector3.forward;
		Vector3 forward = Quaternion.Euler(0f, m_cameras.m_pivotCamera.transform.rotation.eulerAngles.y, 0f) * Vector3.forward;
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

		m_TargetRotation = targetRotation;

		// UPDATE ORIENTATION
		Vector3 localInput = new Vector3(m_hAxis_Button, 0f, m_vAxis_Button);

		float actualTurnSpeed = CheckCollider(false) == true ? m_rotations.m_turnSpeedGrounded : m_rotations.m_turnSpeedWithoutGrounded;
		m_TargetRotation = Quaternion.RotateTowards(m_meshes.m_rotateFerret.transform.rotation, m_TargetRotation, actualTurnSpeed * Time.deltaTime);

		m_rigidbody.rotation = Quaternion.Euler(0f, m_rotations.m_pivot.rotation.eulerAngles.y, 0f);
		m_meshes.m_rotateFerret.transform.rotation = m_TargetRotation;
	}

	public void SetLastStateMoveSpeedForJump(){
		if(m_sM.IsLastStateIndex(0)){
			m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromIdle;
		}else if(m_sM.IsLastStateIndex(1)){
        	m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromWalk;
		}else if(m_sM.IsLastStateIndex(2)){
			m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromRun;
		}else if(m_sM.IsLastStateIndex(5)){
			m_lastStateMoveSpeed = m_states.m_jump.m_movementSpeed.m_fromCrawl;
		}
	}

	public void StartClimbInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation, Transform secondTransformRotation, Quaternion secondFromRotation, Quaternion secondToRotation){
		StartCoroutine(ClimbInterpolation(transformPosition, fromPosition, toPosition, transformRotation, fromRotation, toRotation, secondTransformRotation, secondFromRotation, secondToRotation));
	}
	IEnumerator ClimbInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation, Transform secondTransformRotation, Quaternion secondFromRotation, Quaternion secondToRotation){
		
		m_canMoveOnClimb = false;

		m_rigidbody.isKinematic = true;

		AnimationCurve animationCurve = m_states.m_climb.m_interpolation.m_snapCurve;

		float changePositionSpeed;
		float changeRotationSpeed;

		changePositionSpeed = m_states.m_climb.m_interpolation.m_enterChangePositionSpeed;
		changeRotationSpeed = m_states.m_climb.m_interpolation.m_enterChangeRotationSpeed;

		float journeyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();
		float rotateSecondFracJourney = new float();

		while(transform.position != toPosition){
			// MovePosition
			journeyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * changePositionSpeed / journeyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, animationCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateFracJourney += (Time.deltaTime) * changeRotationSpeed / journeyLength;
			transformRotation.rotation = Quaternion.Slerp(fromRotation, toRotation, animationCurve.Evaluate(rotateFracJourney));
			
			// MoveSecondRotation
			rotateSecondFracJourney += (Time.deltaTime) * changeRotationSpeed  / journeyLength;
			secondTransformRotation.rotation = Quaternion.Slerp(secondFromRotation, secondToRotation, animationCurve.Evaluate(rotateSecondFracJourney));

			yield return null;
		}

		m_rigidbody.isKinematic = false;

		m_canMoveOnClimb = true;

		m_endOfClimbInterpolation = true;
		yield return new WaitForSeconds(0.5f);
		m_endOfClimbInterpolation = false;
	}

	public void EndClimbAnimation(){
		StartCoroutine(ClimbAnimation());
	}
	IEnumerator ClimbAnimation(){
		
		// Debug.Log("Start position = " + m_states.m_climb.m_endClimbAnimPos.position);

		m_canMoveOnClimb = false;

		m_rigidbody.isKinematic = true;

		yield return new WaitForSeconds(m_states.m_climb.m_timeToEndClimbAnim);

		m_rigidbody.isKinematic = false;

		m_canMoveOnClimb = true;

		m_endOfClimbInterpolation = true;
		yield return new WaitForSeconds(0.5f);
		m_endOfClimbInterpolation = false;
	}

	/*public void StartClimbCooldown(){
		StartCoroutine(ClimbCooldownCorout());
	}
	IEnumerator ClimbCooldownCorout(){
		m_states.m_climb.m_canClimb = false;
		yield return new WaitForSeconds(m_states.m_climb.m_timeToCanReClimb);
		m_states.m_climb.m_canClimb = true;
	}*/
	
	/*public void StartRotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){
		StartCoroutine(RotateInterpolation(trans, fromRotation, toRotation));
	}
	IEnumerator RotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){

		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(trans.rotation != toRotation){
			// MoveRotation
			rotateJourneyLength = Quaternion.Dot(fromRotation, toRotation);
				
			Debug.Log("ça fini quand ?");

			if(rotateJourneyLength < 0){
				rotateJourneyLength = - rotateJourneyLength;
				// Debug.Log("new rotateJourneyLength = " + rotateJourneyLength);
			}

			rotateFracJourney += (Time.deltaTime) * m_states.m_climb.m_interpolation.m_enterChangeRotationSpeed / rotateJourneyLength;
			trans.rotation = Quaternion.Slerp(fromRotation, toRotation, m_states.m_climb.m_interpolation.m_snapCurve.Evaluate(rotateFracJourney));

			yield return null;
		}
	}*/

	bool m_exitAction = false;
	public void StartLocalRotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){
		StartCoroutine(LocalRotateInterpolation(trans, fromRotation, toRotation));
	}
	IEnumerator LocalRotateInterpolation(Transform trans, Quaternion fromRotation, Quaternion toRotation){

		m_exitAction = false;

		StartCoroutine(TimeToExitAction());

		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(trans.localRotation != toRotation && !m_exitAction){
			// MoveRotation
			rotateJourneyLength = Quaternion.Dot(fromRotation, toRotation);
			rotateFracJourney += (Time.deltaTime) * m_states.m_climb.m_interpolation.m_fallRotationSpeed * 2 / rotateJourneyLength;
			trans.localRotation = Quaternion.Slerp(fromRotation, toRotation, m_states.m_climb.m_interpolation.m_snapCurve.Evaluate(rotateFracJourney));

			yield return null;
		}
		// Debug.Log("Je m'exit");
	}
	IEnumerator TimeToExitAction(){
		yield return new WaitForSeconds(.5f);
		m_exitAction = true;
	}

	public RaycastHit topRightClimbHit;
	public RaycastHit topLeftClimbHit;
	public RaycastHit botRightClimbHit;
	public RaycastHit botftClimbHit;
	public bool RayCastForwardToStartClimbing(){
		//Debug.Log("I touch " + hit.collider.gameObject.name);
		
		if(Physics.Raycast(m_raycasts.m_topRightLeg.position, m_raycasts.m_topRightLeg.transform.forward, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)
		&&
		Physics.Raycast(m_raycasts.m_topLeftLeg.position, m_raycasts.m_topLeftLeg.transform.forward, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision)){
			m_climbArea = topRightClimbHit.collider.GetComponent<ClimbArea>();
			return true;
		}else{
			return false;
		}
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
	}

	public void RayCastDownToStopSideScrollingMovement(){
		// RIGHT check
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopRight = !Physics.Raycast(m_raycasts.m_topRightLeg.position, - m_raycasts.m_topRightLeg.transform.up, out topRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotRight = !Physics.Raycast(m_raycasts.m_botRightLeg.position, - m_raycasts.m_botRightLeg.transform.up, out botRightClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);

		// LEFT check
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopLeft = !Physics.Raycast(m_raycasts.m_topLeftLeg.position, - m_raycasts.m_topLeftLeg.transform.up, out topLeftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBotLeft = !Physics.Raycast(m_raycasts.m_botLeftLeg.position, - m_raycasts.m_botLeftLeg.transform.up, out botftClimbHit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);

		// Middle check
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInTopMiddle = !Physics.Raycast(m_raycasts.m_topMiddleLeg.position, - m_raycasts.m_topMiddleLeg.transform.up, /*out botftClimbHit,*/ m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);

		// BOT check
		RaycastHit hit;
		m_states.m_climb.m_checkCollision.m_outOfClibingAreaInBot = !Physics.Raycast(m_raycasts.m_middleAss.position, - m_raycasts.m_middleAss.transform.up, out hit, m_raycasts.m_maxCastDistance, m_states.m_climb.m_climbCollision);
	}

	public bool RayCastToCanPush(){

		RaycastHit objectToPush;

		Physics.Raycast(m_physics.castCenter.position, m_physics.castCenter.forward, out objectToPush, m_physics.m_maxCenterDistance, m_states.m_push.m_pushLayer);

		if(objectToPush.collider == null){
			return false;
		}

		PushableObject pushableObject = objectToPush.collider.gameObject.GetComponent<PushableObject>();

		pushableObject.On_PlayerSnapToObject();

		if(pushableObject.CanSnapToThisPoint(pushableObject.ClosedPosition)){
			m_states.m_push.m_hit = objectToPush;
			return true;
		}else{
			return false;
		}

	}

	public void WhenCameraIsCloseToTheFerret(float distance){
		if(distance < m_cameras.m_miniDistanceToSeeFurret && CanHideFerret){
			m_meshes.m_meshSkin.SetActive(false);
		}else{
			m_meshes.m_meshSkin.SetActive(true);
		}
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
		StartCoroutine(DelayToTakeObjectInMouse());
	}
	IEnumerator DelayToTakeObjectInMouse(){
		yield return new WaitForSeconds(m_states.m_takeObject.m_timeToTakeObject);
		StartCoroutine(DelayToTakeAnObject());

		if(m_states.m_takeObject.m_actualGrappedObject != null){
			m_states.m_takeObject.m_actualGrappedObject.On_ObjectIsTake(false);
			m_states.m_takeObject.m_actualGrappedObject = null;
			m_states.m_takeObject.m_iHaveAnObject = false;
		}
		if(m_states.m_takeObject.m_actualClosedObjectToBeGrapped != null){
			m_states.m_takeObject.m_actualGrappedObject = m_states.m_takeObject.m_actualClosedObjectToBeGrapped;
			m_states.m_takeObject.m_actualGrappedObject.On_ObjectIsTake(true);
			m_states.m_takeObject.m_actualClosedObjectToBeGrapped = null;
			m_states.m_takeObject.m_iHaveAnObject = true;
			Level.AddFX(m_states.m_takeObject.m_takeObjectFx, m_states.m_takeObject.m_objectPosition.position, Quaternion.identity);
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

	public bool RaycastFromFerretAss(){
		return Physics.Raycast(m_raycasts.m_middleAss.position, - m_raycasts.m_middleAss.transform.up, m_raycasts.m_maxCastDistance, m_physics.m_groundLayer);
	}

	public void StartOrientationAfterClimb(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		StartCoroutine(OrientationAfterClimb(transformPosition, fromPosition, toPosition, transformRotation, fromRotation, toRotation));
	}
	IEnumerator OrientationAfterClimb(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation){
		
   		Animator.SetBool("Climb", false);

		m_isInLerpRotation = true;

		m_canMoveOnClimb = false;

		// m_rigidbody.isKinematic = false;
		// m_rigidbody.useGravity = true;

		AnimationCurve animationCurve = m_states.m_climb.m_interpolation.m_snapCurve;

		float changePositionSpeed;
		float changeRotationSpeed;

		changePositionSpeed = m_states.m_climb.m_interpolation.m_fallPositionSpeed;
		changeRotationSpeed = m_states.m_climb.m_interpolation.m_fallRotationSpeed;

		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		Vector3 newToPosTarget = new Vector3(toPosition.x, transform.position.y - m_states.m_climb.m_interpolation.m_fallingInY, toPosition.z);
		Vector3 newPos = new Vector3(transform.position.x, 0, transform.position.z);
		Vector3 newToPos = new Vector3(toPosition.x, 0, toPosition.z);

		while(newPos != newToPos){
			newPos = new Vector3(transform.position.x, 0, transform.position.z);
			// Debug.Log("Nous travaillons activement !");
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, newToPosTarget);
			moveFracJourney += (Time.deltaTime) * changePositionSpeed / moveJourneyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, newToPosTarget, animationCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateJourneyLength = Vector3.Distance(fromPosition, newToPosTarget);
			rotateFracJourney += (Time.deltaTime) * changeRotationSpeed / rotateJourneyLength;
			transformRotation.rotation = Quaternion.Lerp(fromRotation, toRotation, animationCurve.Evaluate(rotateFracJourney));

			yield return null;
		}

		m_rigidbody.isKinematic = false;
		m_rigidbody.useGravity = true;

		m_isInLerpRotation = false;

		m_endOfOrientationAfterClimb = true;
		yield return new WaitForSeconds(0.5f);
		m_endOfOrientationAfterClimb = false;
	}

	public void On_PlayerDie(){
		if(!m_playerDebugs.m_playerCanDie){
			return;
		}
		ChangeState(8);
	}

	public int CheckClimbAreaType(){
		int i = new int();
		switch(ClimbArea.m_climbType){ 
			case ClimbTypes.forward:
				i = 0;
			break;
			case ClimbTypes.right:
				i = 1;
			break;
			case ClimbTypes.backward:
				i = 2;
			break;
			case ClimbTypes.left:
				i = 3;
			break;
		}
		return i;
	}

	[Space, SerializeField] Transform m_startPlayerParent;
	public void SetPlayerParent(Transform newParent, bool resetParent = false){
		if(!resetParent){
			transform.SetParent(newParent);
		}else{
			transform.SetParent(m_startPlayerParent);
		}
	}

	public void On_CinematicIsLaunch(bool b){
		if(b){
			ChangeState(9);
		}else{
			ChangeState(ReturnLastState());
		}
	}

	public void On_CheckPointIsTake(Transform checkPointTrans){
		m_savePosition = checkPointTrans.position;
		m_saveRotation = checkPointTrans.rotation;
	}

	public void ResetCheckPointPosition(){
		transform.position = m_savePosition;
		transform.rotation = m_saveRotation;
	}

	public int ReturnLastState(){
		for (int i = 0, l = m_sM.States.Count; i < l; ++i){
            if(m_sM.IsLastStateIndex(i)){
                return i;
            }
        }
		return 0;
	}

	InteractiveObject m_closedInteractiveObject;
	public void SetClosedInteractiveObject(InteractiveObject closedInteractiveObject, bool isEnter){
		if(isEnter){
			m_closedInteractiveObject = closedInteractiveObject;

		}else{
			m_closedInteractiveObject = null;
		}
	}

	public void StartRotateToPushableObjectInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation, Transform secondTransformRotation, Quaternion secondFromRotation, Quaternion secondToRotation){
		StartCoroutine(RotateToPushableObjectInterpolation(transformPosition, fromPosition, toPosition, transformRotation, fromRotation, toRotation, secondTransformRotation, secondFromRotation, secondToRotation));
	}
	IEnumerator RotateToPushableObjectInterpolation(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition, Transform transformRotation, Quaternion fromRotation, Quaternion toRotation, Transform secondTransformRotation, Quaternion secondFromRotation, Quaternion secondToRotation){
		
		Rigidbody.isKinematic = true;
		m_canMoveOnPush = false;

		float journeyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();
		float rotateSecondFracJourney = new float();

		while(transform.position != toPosition){
			// Debug.Log("je calcul comme un FPD");
			// MovePosition
			journeyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * m_states.m_push.m_snapSpeed / journeyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, m_states.m_push.m_snapCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateFracJourney += (Time.deltaTime) * m_states.m_push.m_snapSpeed / journeyLength;
			transformRotation.rotation = Quaternion.Slerp(fromRotation, toRotation, m_states.m_push.m_snapCurve.Evaluate(rotateFracJourney));

			// MoveSecondRotation
			rotateSecondFracJourney += (Time.deltaTime) * m_states.m_push.m_snapSpeed  / journeyLength;
			secondTransformRotation.rotation = Quaternion.Slerp(secondFromRotation, secondToRotation, m_states.m_push.m_snapCurve.Evaluate(rotateSecondFracJourney));

			yield return null;
		}
			// Debug.Log("J'ai fini");

		Rigidbody.isKinematic = false;
		m_canMoveOnPush = true;
		yield break;
	}

	public void On_EndClimbAnimIsFinished(){
		transform.position = m_states.m_climb.m_endClimbAnimPos.position;
		transform.rotation = m_states.m_climb.m_endClimbAnimPos.rotation;
		StartCoroutine(WaitClimbCameraMove());
	}
	IEnumerator WaitClimbCameraMove(){
		yield return new WaitForSeconds(m_states.m_climb.m_waitClimbCameraMove);
		m_updates.m_followPlayer.On_PlayerEndClimb(true);
		// m_updates.m_followPlayer.ReturnToPlayerAfterClimb();
	}

	[HideInInspector] public float m_targetRunSpeed;
	float m_actualSpeed = 0;
	public void SetRunSpeed(){
		m_actualSpeed = Mathf.Lerp(m_actualSpeed, m_targetRunSpeed, m_states.m_run.m_changeRunSpeed * Time.deltaTime);
		Animator.SetFloat("Run", m_actualSpeed);
	}

	float m_actualTimer = 0;
	float m_rangeTimer;
	public void StartIddleTimer(){
		m_rangeTimer = Random.Range(m_states.m_iddle.m_minTimeToSwitchIddle2, m_states.m_iddle.m_maxTimeToSwitchIddle2);
		m_actualTimer = 0;
	}
	void SetIddleTimer(){
		m_actualTimer += Time.deltaTime;
		if(m_actualTimer > m_rangeTimer){
			Animator.SetTrigger("Iddle2");
			m_actualTimer = 0;
			m_rangeTimer = Random.Range(m_states.m_iddle.m_minTimeToSwitchIddle2, m_states.m_iddle.m_maxTimeToSwitchIddle2);
		}
	}

	public void StartJumpAfterEndPauseCorout(){
		StartCoroutine(JumpAfterEndPause());
	}
	IEnumerator JumpAfterEndPause(){
		m_canJumpAfterEndPause = false;
		yield return new WaitForSeconds(m_playerDebugs.m_timeToJumpAfterEndPause);
		m_canJumpAfterEndPause = true;
	}

	public void FallingTimeToDoSound(float timeInFallState){

		if(timeInFallState < m_states.m_fall.m_timeToDoMinSound){
			return;
		}

		float fallTimeValue = Mathf.InverseLerp(m_states.m_fall.m_timeToDoMinSound, m_states.m_fall.m_timeToDoMaxSound, timeInFallState);
		// Debug.Log("fallTimeValue = " + fallTimeValue);

		float fallEvalCurve = m_states.m_fall.m_soundCurve.Evaluate(fallTimeValue);
		// Debug.Log("fallEvalCurve = " + fallEvalCurve);

		FX fallFX = Level.AddFX(m_states.m_fall.m_landingFx, m_states.m_fall.m_landingPos.position, m_states.m_fall.m_landingPos.rotation);
		// Debug.Log("fallFX = " + fallFX);

		float soundVolume = Mathf.Lerp(m_states.m_fall.m_minSoundVolume, m_states.m_fall.m_maxSoundVolume, fallEvalCurve);
		fallFX.GetComponent<AudioSource>().volume = soundVolume;
	}

#endregion Public functions

}

