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
	[SerializeField] float m_speed = 0.5f;
	[SerializeField] AnimationCurve m_curve;
	
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

	void Start(){
		m_offset = transform.localPosition;
	}

	void Update(){
		m_desiredPosition = m_objectToFollow.transform.position + m_offset;
		if(m_followLookAtPoint){
			transform.position = m_desiredPosition;
		}
	}

	public void ReturnToPlayer(){
		StartCoroutine(ReturnToPlayerCorout());
	}
	IEnumerator ReturnToPlayerCorout(){
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(transform.position != m_desiredPosition){
			// MoveRotation
			rotateJourneyLength = Vector3.Distance(transform.position, m_desiredPosition);
			rotateFracJourney += (Time.deltaTime) * m_speed / rotateJourneyLength;
			transform.position = Vector3.Lerp(transform.position, m_desiredPosition, m_curve.Evaluate(rotateFracJourney));
			yield return null;
		}
		m_followLookAtPoint = true;
	}

}
