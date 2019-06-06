using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

#region Singleton
	public static FollowPlayer Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of FollowPlayer");
		}
	}
#endregion //Singleton

	[SerializeField] Transform m_objectToFollow;
	[SerializeField] Transform m_objectToRotate;
	public float m_speed = 0.5f;
	[SerializeField] AnimationCurve m_curve;

	public PlayerClimbCamera m_playerClimbCamera = new PlayerClimbCamera();
	[System.Serializable] public class PlayerClimbCamera {
		public float m_additionalY = 2;
		public float m_additionalZ = 2;
		[Space]
		public float m_speed = 0.25f;
		public AnimationCurve m_moveCurve;
	}
	
	bool m_followLookAtPoint = true;
    public bool FollowLookAtPoint{
        get{
            return m_followLookAtPoint;
        }
		set{
            m_followLookAtPoint = value;
        }
    }

	Vector3 m_offset;
	Vector3 m_desiredPosition;
	Quaternion m_desiredRotation;
	PlayerManager m_playerManager;
	CameraFollowLookAt m_cameraFollowLookAt;

	void Start(){
		m_offset = transform.localPosition;
		m_playerManager = PlayerManager.Instance;
		m_cameraFollowLookAt = CameraFollowLookAt.Instance;
	}

	void Update(){
		m_desiredPosition = m_objectToFollow.position + m_offset;
		if(m_followLookAtPoint){
			transform.position = m_desiredPosition;
			UpdateFollowPlayerPosition();
		}

		m_desiredRotation = m_objectToRotate.rotation;
		if(m_followLookAtPoint){
			transform.rotation = m_desiredRotation;
			UpdateFollowPlayerRotation();
		}
	}

	public void UpdateFollowPlayerPosition(){
		transform.position = m_desiredPosition;
	}
	public void UpdateFollowPlayerRotation(){
		transform.rotation = m_desiredRotation;
	}

	public void ReturnToPlayer(){
		StartCoroutine(ReturnToPlayerCorout());
	}
	IEnumerator ReturnToPlayerCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();

		while(transform.position != m_desiredPosition){
			moveJourneyLength = Vector3.Distance(transform.position, m_desiredPosition);
			moveFracJourney += (Time.deltaTime) * m_speed / moveJourneyLength;
			transform.position = Vector3.Lerp(transform.position, m_desiredPosition, m_curve.Evaluate(moveFracJourney));
			yield return null;
		}
		m_followLookAtPoint = true;
	}

	public void On_PlayerEndClimb(bool endClimb){
		if(!endClimb){
			m_cameraFollowLookAt.m_mainObjectToFollow = transform;
			m_followLookAtPoint = false;
		}else{
			m_playerManager.m_updates.m_followPlayer.ReturnToPlayerAfterClimb();
		}
	}

	public void ReturnToPlayerAfterClimb(){
		StartCoroutine(ReturnToPlayerAfterClimbCorout());
	}
	IEnumerator ReturnToPlayerAfterClimbCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();

		while(transform.position != m_desiredPosition){
			moveJourneyLength = Vector3.Distance(transform.position, m_desiredPosition);
			moveFracJourney += (Time.deltaTime) * m_playerClimbCamera.m_speed / moveJourneyLength;
			transform.position = Vector3.Lerp(transform.position, m_desiredPosition, m_playerClimbCamera.m_moveCurve.Evaluate(moveFracJourney));
			yield return null;
		}
		m_followLookAtPoint = true;
		// m_cameraFollowLookAt.m_mainObjectToFollow = m_cameraFollowLookAt.MainObject;
	}

	/*public void EndClimbMoveCamera(){
		StartCoroutine(StartEndClimbMoveCamera());
	}
	IEnumerator StartEndClimbMoveCamera(){

		m_followLookAtPoint = false;

		float moveJourneyLength;
		float moveFracJourney = new float();

		/*Vector3 desiredPos = new Vector3(m_playerManager.transform.position.x, 
										m_playerManager.transform.position.y + m_playerManager.transform.up.y * m_playerClimbCamera.m_additionalY,
										m_playerManager.transform.position.z + m_playerManager.transform.up.z * m_playerClimbCamera.m_additionalZ);*

		Vector3 desiredPos = m_playerManager.transform.position + Vector3.down * m_playerClimbCamera.m_additionalY + Vector3.up * m_playerClimbCamera.m_additionalZ;

		// (m_playerManager.transform, m_playerManager.transform.position, m_playerManager.transform.position + Vector3.down * 2 + Vector3.up * 3, 

		GameObject go = Instantiate(new GameObject(), desiredPos, Quaternion.identity);
		go.name = "TestPosition";

		while(transform.position != desiredPos){
			moveJourneyLength = Vector3.Distance(transform.position, desiredPos);
			moveFracJourney += (Time.deltaTime) * m_playerClimbCamera.m_speed / moveJourneyLength;
			transform.position = Vector3.Lerp(transform.position, desiredPos, m_playerClimbCamera.m_moveCurve.Evaluate(moveFracJourney));
			yield return null;
		}
		// m_followLookAtPoint = true;
	}*/

}
