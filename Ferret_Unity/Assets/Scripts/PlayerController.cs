using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	public MouvementSpeed m_mouvementSpeed = new MouvementSpeed();
	public Colliders m_colliders = new Colliders();
	public Cameras m_cameras = new Cameras();
	public Layers m_layersMask = new Layers();
	public Debugs m_debugs = new Debugs();

	[System.Serializable] public class MouvementSpeed {
		[Header("Speed of the walk")] 
		public float walkSpeed;

		[Header("Multipliers based on the walk speed")]
		public float runMultiplier;
		public float climbMultiplier;
		public float crawlMultiplier;
	}
	[System.Serializable] public class Colliders {
		public NormalColliders m_normalColliders = new NormalColliders();
		[System.Serializable] public class NormalColliders {
			[Header("Colliders of normal state")]
			public SphereCollider m_headColl;
			public CapsuleCollider m_bodyColl;
		}
		public CrawlColliders m_crawlColliders = new CrawlColliders();
		[System.Serializable] public class CrawlColliders {
			[Header("Colliders of crawl state")]
			public SphereCollider m_headColl;
			public CapsuleCollider m_bodyColl;
		}
	}
	[System.Serializable] public class Cameras {
		public Transform m_thirdPersonCameraPos; 
		public Transform m_firstPersonCameraPos;

		public Camera m_thirdPersonCamera;
		public Camera m_firstPersonCamera;
	}
	[System.Serializable] public class Layers {
		public LayerMask m_whatIsGround;
		public LayerMask m_whatIsTopCollider;
	}
	
	[System.Serializable] public class Debugs {
		[Header("Rays to check if grounded")]
		public bool m_showRayForGround;
		public Color m_colorToRay = Color.magenta;
	}

	[SerializeField] float m_jumpForce = 20;
	[SerializeField] bool m_useGravity = true;	
	[SerializeField] float m_gravity = 9.81f;
	[SerializeField] float m_rotateSpeed = 10;

	[SerializeField] Transform m_pivot;
	[SerializeField] GameObject m_ferret;
	[SerializeField] GameObject m_ferretMesh;
	[SerializeField] GameObject m_ferretCrawlMesh;
	
	#region I Do A Capacity ?
		bool m_isWalking;
		bool m_isRunning;
		bool m_isJumping;
		bool m_isGrabing;
		bool m_isClimbing;
		bool m_isCrawling;
		bool m_isCatching;
		bool m_isPushing;
	#endregion

	#region All buttons
		float m_hAxis_Button;
		float m_vAxis_Button;
		float m_runButton;
		bool m_jumpButton;
		bool m_crawlButton;
		bool m_takeButton;
	#endregion

	private Rigidbody m_rbody;
	private CapsuleCollider m_capsuleColl;
	private Animator m_animator;
	private Vector3 m_moveDirection = Vector3.zero;
	private float m_ySpeed;
	private float m_currentMoveSpeed; // The speed of the action : marche, course, à plat ventre, escalade, ...
	private bool m_isGrounded;
	private bool m_canUncrawl;

	private float m_ungroundedDelay = 0.2f;
	private bool m_iWouldKnowIfIsGrounded = true;

	void Start(){
		m_rbody = GetComponent<Rigidbody>();
		if(m_rbody == null){
			Debug.LogError("No rigidbody found");
		}

		m_capsuleColl = GetComponent<CapsuleCollider>();
		if(m_capsuleColl == null){
			m_capsuleColl = GetComponentInChildren<CapsuleCollider>();
			if(m_capsuleColl == null){
				Debug.LogError("No CapsuleCollider found");
			}
		}

		m_animator = GetComponentInChildren<Animator>();
		if(m_animator == null){
			Debug.LogError("No Animator found");
		}
	}

	void UpdateButtons(){
		m_hAxis_Button = Input.GetAxis("Horizontal");
		m_vAxis_Button = Input.GetAxis("Vertical");
		m_runButton = Input.GetAxis("Run");
		m_jumpButton = Input.GetButtonDown("Jump");
		m_crawlButton = Input.GetButtonDown("Crawl");
		m_takeButton = Input.GetButtonDown("Take");
	}

	void Update(){
		UpdateButtons();
		SettingPlayerSpeed();
		CheckGroundStatus();
		CheckTopCollider();
		PlayerCapacity();

		//m_animator.SetBool("isGrounded", m_isGrounded);
		//m_animator.SetFloat("Speed", (Mathf.Abs(m_vAxis_Button) + Mathf.Abs(m_hAxis_Button)));
	}

	void FixedUpdate(){
		MovePlayer();
		RotatePlayer();
	}

	void MovePlayer(){

		m_moveDirection = new Vector3(m_hAxis_Button, 0, m_vAxis_Button);
		m_moveDirection = transform.TransformDirection(m_moveDirection);
		m_moveDirection = m_moveDirection.normalized;
		m_moveDirection *= m_currentMoveSpeed;

		//if(m_isGrounded){
			if(m_jumpButton){
				Debug.Log("I jump");
				StartCoroutine(UngroundDelay());
				m_moveDirection.y = m_jumpForce;
			}else{
				m_moveDirection.y = 0;
			}
		//}
		if(m_useGravity)
		m_moveDirection.y -= m_gravity * Time.deltaTime;
		
		m_rbody.MovePosition(transform.position + m_moveDirection * Time.deltaTime); //c'est bien ça !
	}

	void RotatePlayer(){
		// Rotate the player in different directions based on camera look direction
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			transform.rotation = Quaternion.Euler(0f, m_pivot.rotation.eulerAngles.y, 0f);
			Quaternion newRotation = Quaternion.LookRotation(new Vector3(m_moveDirection.x, 0f, m_moveDirection.z));
			m_ferret.transform.rotation = Quaternion.Slerp(m_ferret.transform.rotation, newRotation, m_rotateSpeed * Time.deltaTime);
		}
	}

	void CheckGroundStatus(){
		if(m_iWouldKnowIfIsGrounded){
			Vector3 positionTop = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.forward * m_colliders.m_normalColliders.m_bodyColl.height/2;
			Vector3 positionTopRight = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.right * m_colliders.m_normalColliders.m_bodyColl.radius + Vector3.forward * m_colliders.m_normalColliders.m_bodyColl.height/2;
			Vector3 positionTopLeft = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.left * m_colliders.m_normalColliders.m_bodyColl.radius + Vector3.forward * m_colliders.m_normalColliders.m_bodyColl.height/2;
			Vector3 positionBotRight = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.right * m_colliders.m_normalColliders.m_bodyColl.radius + Vector3.back * m_colliders.m_normalColliders.m_bodyColl.height/2;
			Vector3 positionBotLeft = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.left * m_colliders.m_normalColliders.m_bodyColl.radius + Vector3.back * m_colliders.m_normalColliders.m_bodyColl.height/2;
			Vector3 positionBot = m_ferret.transform.position + m_colliders.m_normalColliders.m_bodyColl.center + Vector3.back * m_colliders.m_normalColliders.m_bodyColl.height/2;

			float maxDistance = m_colliders.m_normalColliders.m_bodyColl.radius + 0.1f;

			if( Physics.Raycast(positionTop, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) ||
				Physics.Raycast(positionTopRight, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) || 
				Physics.Raycast(positionTopLeft, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) ||
				Physics.Raycast(positionBotRight, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) ||
				Physics.Raycast(positionBotLeft, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) ||
				Physics.Raycast(positionBot, Vector3.down, maxDistance, m_layersMask.m_whatIsGround) ){
			//Debug.Log("I touch the ground");
				m_isGrounded = true;
				//Debug.Log(Physics.Raycast(positionTopRight, Vector3.down, maxDistance, m_layersMask.m_whatIsGround));
			}else{
				//Debug.Log("I don't touch the ground");
				m_isGrounded = false;
			}

			if(m_debugs.m_showRayForGround){
				Debug.DrawLine(positionTop, positionTop + Vector3.down * maxDistance, m_debugs.m_colorToRay);
				Debug.DrawLine(positionTopRight, positionTopRight + Vector3.down * maxDistance, m_debugs.m_colorToRay);
				Debug.DrawLine(positionTopLeft, positionTopLeft + Vector3.down * maxDistance, m_debugs.m_colorToRay);
				Debug.DrawLine(positionBotRight, positionBotRight + Vector3.down * maxDistance, m_debugs.m_colorToRay);
				Debug.DrawLine(positionBotLeft, positionBotLeft + Vector3.down * maxDistance, m_debugs.m_colorToRay);
				Debug.DrawLine(positionBot, positionBot + Vector3.down * maxDistance, m_debugs.m_colorToRay);
			}
		}

		/*RaycastHit hit;
		Vector3 p1 = transform.position + m_capsuleColl.center;
		Vector3 p2 = p1 + Vector3.right * m_capsuleColl.height;
		if(Physics.CapsuleCast(p1, p2, m_capsuleColl.radius, - Vector3.up, out hit, 0.05f, m_whatIsGround)){
			Debug.Log("return true");
			return true;
		}else{
			Debug.Log("return false");
			return false;
		}*/

		//Debug.DrawRay(transform.position + m_colliders.m_normalColliders.m_bodyColl.height, 

		//Debug.DrawRay(transform.position, - Vector3.up * m_capsuleColl.bounds.extents.y, Color.magenta);
		//Debug.Log("I touch the ground ? " + Physics.Raycast(transform.position, - Vector3.up, m_capsuleColl.bounds.extents.y, m_layersMask.m_whatIsGround));

		/*if(m_iWouldKnowIfIsGrounded){
			if(Physics.Raycast(transform.position, - Vector3.up, m_capsuleColl.bounds.extents.y, m_layersMask.m_whatIsGround)){
				m_isGrounded = true;
			}else{
				m_isGrounded = false;
			}
		}else{
			m_isGrounded = false;
		}*/

		/*if(m_iWouldKnowIfIsGrounded){
			Vector3 center = transform.position + new Vector3(0, 0, -0.06f);
			Vector3 halfExtends = new Vector3(0.3f, 0.5f, 1f) / 2;
			Vector3 direction = - Vector3.up;
			Quaternion orientation = m_ferret.transform.rotation;
			float maxDistance = - 0.5f;
			int layerMask = m_layersMask.m_whatIsGround;
			
			if(Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)){
				m_isGrounded = true;
				Debug.Log("CheckBotCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			}else{
				m_isGrounded = false;
				Debug.Log("CheckBotCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			}
		}*/
	}

	IEnumerator UngroundDelay(){
		m_iWouldKnowIfIsGrounded = false;
		yield return new WaitForSeconds(m_ungroundedDelay);
		m_iWouldKnowIfIsGrounded = true;
	}

	void CheckTopCollider(){	// Si un objet est au dessus du joueur quand il rampe
		if(m_isCrawling){
			Vector3 center = transform.position + new Vector3(0, 0, 0.075f);
			Vector3 halfExtends = new Vector3(0.3f, 0.5f, 1.25f) / 2;
			Vector3 direction = Vector3.up;
			Quaternion orientation = m_ferret.transform.rotation;
			float maxDistance = 0.75f;
			int layerMask = m_layersMask.m_whatIsTopCollider;
			
			if(Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)){
				m_canUncrawl = false;
				Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			}else{
				m_canUncrawl = true;
				Debug.Log("CheckTopCollider = " + (Physics.BoxCast(center, halfExtends, direction, orientation, maxDistance, layerMask)));
			}
		}
	}

	void SettingPlayerSpeed(){
		//if(IsGrounded()){
			// Si on bouge pas
			if( m_hAxis_Button == 0 && m_vAxis_Button == 0){
				m_isWalking = false;
				m_isRunning = false;
			}
			// Si on marche
			if( m_hAxis_Button != 0 && m_vAxis_Button != 0 && m_runButton == 0 && !m_isCrawling){
				m_isWalking = true;
				m_isRunning = false;
				m_currentMoveSpeed = m_mouvementSpeed.walkSpeed;
			}
			// Si on court
			if( m_hAxis_Button != 0 && m_vAxis_Button != 0 && m_runButton < - 0.05f && !m_isCrawling){
				m_isWalking = false;
				m_isRunning = true;
				m_currentMoveSpeed = m_mouvementSpeed.walkSpeed * m_mouvementSpeed.runMultiplier;
			}
			// Si on rampe
			if(m_hAxis_Button != 0 && m_vAxis_Button != 0 && m_isCrawling){
				m_isWalking = false;
				m_isRunning = false;
				m_currentMoveSpeed = m_mouvementSpeed.walkSpeed * m_mouvementSpeed.crawlMultiplier;
			}
		//}
	}

	void PlayerCapacity(){
		if(m_crawlButton){
			m_isCrawling =! m_isCrawling;
		}
		if(m_isCrawling){
			m_ferretMesh.SetActive(false);
			m_ferretCrawlMesh.SetActive(true);

			m_colliders.m_normalColliders.m_headColl.enabled = false;
			m_colliders.m_normalColliders.m_bodyColl.enabled = false;

			m_colliders.m_crawlColliders.m_headColl.enabled = true;
			m_colliders.m_crawlColliders.m_bodyColl.enabled = true;

		}else{
			if(m_canUncrawl){	// Le furet se relève que s'il n'y a pas d'obstacle au dessus de lui !
				m_ferretMesh.SetActive(true);
				m_ferretCrawlMesh.SetActive(false);

				m_colliders.m_normalColliders.m_headColl.enabled = true;
				m_colliders.m_normalColliders.m_bodyColl.enabled = true;

				m_colliders.m_crawlColliders.m_headColl.enabled = false;
				m_colliders.m_crawlColliders.m_bodyColl.enabled = false;
			}
		}
	}

	/*void OnDrawGizmos(){
		Gizmos.color = Color.magenta;
		if(m_isCrawling){
			Gizmos.DrawWireCube(m_ferret.transform.position + new Vector3(0, 0.75f, 0.075f), new Vector3(0.5f, 0.5f, 1.35f));
		}
	}*/

}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

	[SerializeField] private GameObject m_playerMesh;
	[SerializeField] private Animator m_animator;
	[SerializeField] private float m_moveSpeed = 10;
	[SerializeField] private float m_jumpForce = 20;	
	[SerializeField] private float m_gravityScale = 9.81f;

	[SerializeField] private Transform m_pivot;
	[SerializeField] private float m_rotateSpeed = 10;

	private CharacterController m_charController;
	private Vector3 m_moveDirection = Vector3.zero;
	private float m_ySpeed;

	void Start(){
		m_charController = GetComponent<CharacterController>();
	}

	void Update(){

		//m_moveDirection = new Vector3(Input.GetAxis("Horizontal") * m_moveSpeed, m_moveDirection.y, Input.GetAxis("Vertical") * m_moveSpeed);

		float yStore = m_moveDirection.y;
		m_moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
		m_moveDirection = m_moveDirection.normalized * m_moveSpeed;
		m_moveDirection.y = yStore;

		if(m_charController.isGrounded){
			m_moveDirection.y = 0;
			if(Input.GetButtonDown("Jump")){
				m_moveDirection.y = m_jumpForce;
			}
		}

		m_moveDirection.y = m_moveDirection.y + (Physics.gravity.y * m_gravityScale * Time.deltaTime);
		m_charController.Move(m_moveDirection * Time.deltaTime);

		// Move the player in different directions based on camera look direction
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			transform.rotation = Quaternion.Euler(0f, m_pivot.rotation.eulerAngles.y, 0f);
			Quaternion newRotation = Quaternion.LookRotation(new Vector3(m_moveDirection.x, 0f, m_moveDirection.z));
			m_playerMesh.transform.rotation = Quaternion.Slerp(m_playerMesh.transform.rotation, newRotation, m_rotateSpeed * Time.deltaTime);
		}
		m_animator.SetBool("isGrounded", m_charController.isGrounded);
		m_animator.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
	}
	
}*/
