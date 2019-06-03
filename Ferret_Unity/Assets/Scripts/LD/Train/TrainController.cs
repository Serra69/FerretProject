using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : TrainPathsTypes {

	[Header("SETUP")]
	[SerializeField] bool m_startTPAtFirstPoint = true;

	[Header("Objects to move")]
	[SerializeField] Transform m_myParent;
	[SerializeField] Transform m_train;

	[Header("Points")]
	[SerializeField] Transform m_startPoint;
	[SerializeField] TrainPoint[] m_points;

	[Header("Move")]
	[SerializeField] float[] m_moveSpeeds;
	[SerializeField] AnimationCurve m_moveCurve;

	[Header("FX")]
	[SerializeField] GameObject m_startMoveFX;

	public MofeFX m_moveFX = new MofeFX();
	[System.Serializable] public class MofeFX {

		[Header("Fade in/out")]
		[Range(0, 1)] public float m_startFadeIn = 0.01f;
		[Range(0, 1)] public float m_startFadeOut = 0.75f;
		public float m_fadeInTime = 0.5f;
		public float m_fadeOutTime = 0.5f;

		[Header("Pitch")]
		public float m_minPitch = 0.99f;
		public float m_maxPitch = 1.01f;
		public AnimationCurve m_pitchCurve;
	}

	[Header("Gizmos")]
	[SerializeField] Color m_gizmosColor = Color.magenta;

	TrainPoint m_trainPoint;
	int m_nextPoint = 0;

	Rigidbody m_rbody;

	AudioSource m_moveFxAudioSource;
	bool m_moveFxCanFadeIn = true;

	void Start(){

		if(m_moveSpeeds.Length != m_points.Length){
			Debug.LogError("You need to have the same length of array in Points & Move Speeds");
		}

		m_rbody = m_train.GetComponent<Rigidbody>();

		if(m_startTPAtFirstPoint){
			m_train.position = m_startPoint.position;
		}


		Vector3 targetPos = m_points[0].transform.position;
		targetPos.y = m_train.position.y;
		m_train.LookAt(targetPos);

		m_moveFxAudioSource = GetComponent<AudioSource>();

		// StartCoroutine(Move(m_train, m_train.position, m_points[m_nextPoint].transform.position));
	}

	void Update(){
		/*if(Input.GetKeyDown(KeyCode.P)){
			ChoseNextTarget();
		}*/
	}

	float GetTrainSpeed(){
		if(m_nextPoint - 1 >= 0){
			return m_moveSpeeds[m_nextPoint - 1];
		}else{
			return m_moveSpeeds[m_nextPoint];
		}
	}
	float GetNextPathNumber(){
		if(m_nextPoint - 1 >= 0){
			return m_nextPoint - 1;
		}else{
			return m_nextPoint;
		}
	}

	IEnumerator Move(Transform transformPosition, Vector3 fromPosition, Vector3 toPosition){

		bool startFadeIn = false;
		bool startFadeOut = false;

		float moveJourneyLength;
		float moveFracJourney = new float();

		while(transformPosition.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * GetTrainSpeed() / moveJourneyLength;
			transformPosition.position = Vector3.Lerp(fromPosition, toPosition, m_moveCurve.Evaluate(moveFracJourney));

			if(m_moveFxAudioSource != null){
				if(moveFracJourney >= m_moveFX.m_startFadeIn && !startFadeIn){
					startFadeIn = true;
					StartCoroutine(FadeIn());
				}
				if(moveFracJourney >= m_moveFX.m_startFadeOut && !startFadeOut){
					startFadeOut = true;
					StartCoroutine(FadeOut());
				}

				m_moveFxAudioSource.pitch = Mathf.Lerp(m_moveFX.m_minPitch, m_moveFX.m_maxPitch, m_moveFX.m_pitchCurve.Evaluate(moveFracJourney));
			}

			yield return null;
		}

		switch(m_trainPoint.m_pointTypes){ 
			case PointTypes.WaitSeconds:
				PistonController m_pistonController = m_trainPoint.GetComponent<PistonController>();

				transform.SetParent(m_pistonController.m_rotate.m_transform);

				m_pistonController.DoYourJob();
				m_pistonController.TrainController = this;

			break;
			case PointTypes.Wait:
				
			break;
		}
	}

	public void ChoseNextTarget(){

		StartCoroutine(Move(m_train, m_train.position, m_points[m_nextPoint].transform.position));

		m_trainPoint = m_points[m_nextPoint];
		m_nextPoint ++;
	}

	public void ResetTrainParent(){
		transform.SetParent(m_myParent.transform);
	}

	public void EnVoiture(){
		if(m_startMoveFX != null){
			Level.AddFX(m_startMoveFX, transform.position, transform.rotation);
		}
	}

	IEnumerator FadeIn(){
		if(m_moveFxCanFadeIn){
			m_moveFxAudioSource.Play();
			m_moveFxAudioSource.volume = 0;
			float moveFracJourney = 0;
			float fadeInTime = m_moveFX.m_fadeInTime - 0.30f;
			float v = 1 / ((fadeInTime + (fadeInTime/2)) / Time.fixedDeltaTime);
			while(m_moveFxAudioSource.volume < 1){
				moveFracJourney += v;
				m_moveFxAudioSource.volume = Mathf.Lerp(0, 1, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_moveFxCanFadeIn = false;
		}
		yield break;
	}
	IEnumerator FadeOut(){
		if(m_moveFxAudioSource.isPlaying){
			float moveFracJourney = 0;
			float fadeOutTime = m_moveFX.m_fadeOutTime - 0.30f;
			float v = 1 / ((fadeOutTime + (fadeOutTime/2)) / Time.fixedDeltaTime);
			while(m_moveFxAudioSource.volume > 0){
				moveFracJourney += v;
				m_moveFxAudioSource.volume = Mathf.Lerp(1, 0, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_moveFxAudioSource.Stop();
			m_moveFxCanFadeIn = true;
		}
		yield break;
	}
	
	void OnDrawGizmosSelected(){
		Gizmos.color = m_gizmosColor;

		if(m_points == null){
			return;
		}

		for(int i = 0; i < m_points.Length; i++){

			if(m_startPoint != null && m_points[0] != null){
				Gizmos.DrawLine(m_startPoint.transform.position, m_points[0].transform.position);
			}

			if(i+1 != m_points.Length){
				if(m_points[i] != null && m_points[i + 1] != null){
					Gizmos.DrawLine(m_points[i].transform.position, m_points[i + 1].transform.position);
				}
			}
		}
	}

}
 