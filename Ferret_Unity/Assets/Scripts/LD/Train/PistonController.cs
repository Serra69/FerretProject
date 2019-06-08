using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonController : TrainPathsTypes {

	[Header("Type of piston")]
	public PistonTypes m_pistonType = PistonTypes.MoveAndRotate;
	public bool m_rotateBeforeMove = true;
	[Space]
	public bool m_resetTransformAfterMoving = false;
	[SerializeField] float m_timeToWaitBeforeReset = 2;

	[Header("Rotate")]
	public Rotate m_rotate = new Rotate();
	[System.Serializable] public class Rotate {
		public Transform m_transform;
		public float m_speed = 2;
		public Vector3 m_toRotation;
		public AnimationCurve m_curve;
	}

	[Header("Move")]
	public Move m_move = new Move();
	[System.Serializable] public class Move {
		public Transform m_transform;
		public float m_speed = 2;
		public Vector3 m_toLocalePosition;
		public AnimationCurve m_curve;
	}

	[Header("FX")]
	[SerializeField] GameObject m_startFx;
	public MovementFX m_movementFX = new MovementFX();
	[System.Serializable] public class MovementFX {

		public AudioSource m_movementFxAudioSource;

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

	TrainController m_trainController;
    public TrainController TrainController
    {
        get
        {
            return m_trainController;
        }

        set
        {
            m_trainController = value;
        }
    }

	bool m_rotateIsFinished = false;
	bool m_moveIsFinished = false;

	Vector3 m_startPosition;
	Vector3 m_startRotation;

	bool m_movementFxCanFadeIn = true;

	void Start(){
		if(m_move.m_transform != null){
			m_startPosition = m_move.m_transform.localPosition;
		}
		if(m_rotate.m_transform != null){
			m_startRotation = m_rotate.m_transform.rotation.eulerAngles;
		}
	}

    public void DoYourJob(){


		switch(m_pistonType){ 
			case PistonTypes.Rotate:
				Level.AddFX(m_startFx, m_rotate.m_transform.position, Quaternion.identity);
				StartCoroutine(RotateCorout());
			break;
			case PistonTypes.Move:
				Level.AddFX(m_startFx, m_move.m_transform.position, Quaternion.identity);
				StartCoroutine(MoveCorout());
			break;
			case PistonTypes.MoveAndRotate:
				if(m_rotateBeforeMove){
					Level.AddFX(m_startFx, m_rotate.m_transform.position, Quaternion.identity);
					StartCoroutine(RotateCorout());
				}else{
					Level.AddFX(m_startFx, m_move.m_transform.position, Quaternion.identity);
					StartCoroutine(MoveAndRotateCorout());
				}
			break;
		}
	}

	IEnumerator RotateCorout(){
		float rotateJourneyLength;
		float rotateFracJourney = new float();
		bool startFadeIn = false;
		bool startFadeOut = false;
		while(m_rotate.m_transform.eulerAngles != m_rotate.m_toRotation){
			rotateJourneyLength = Vector3.Distance(m_rotate.m_transform.rotation.eulerAngles, m_rotate.m_toRotation);
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / rotateJourneyLength;

			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);

			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));

			if(m_movementFX.m_movementFxAudioSource != null){
				if(rotateFracJourney >= m_movementFX.m_startFadeIn && !startFadeIn){
					startFadeIn = true;
					StartCoroutine(FadeIn());
				}
				if(rotateFracJourney >= m_movementFX.m_startFadeOut && !startFadeOut){
					startFadeOut = true;
					StartCoroutine(FadeOut());
				}

				m_movementFX.m_movementFxAudioSource.pitch = Mathf.Lerp(m_movementFX.m_minPitch, m_movementFX.m_maxPitch, m_movementFX.m_pitchCurve.Evaluate(rotateFracJourney));
			}

			yield return null;
		}

		switch(m_pistonType){ 
			case PistonTypes.Rotate:
				m_trainController.ChoseNextTarget();
				m_trainController.ResetTrainParent();

				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
			case PistonTypes.MoveAndRotate:
				if(m_rotateBeforeMove){
					StartCoroutine(MoveCorout());
				}
			break;
		}
	}
	IEnumerator MoveCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();

		bool startFadeIn = false;
		bool startFadeOut = false;

		while(m_move.m_transform.localPosition != m_move.m_toLocalePosition){
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_move.m_toLocalePosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_move.m_toLocalePosition, m_move.m_curve.Evaluate(moveFracJourney));

			if(m_movementFX.m_movementFxAudioSource != null){
				if(moveFracJourney >= m_movementFX.m_startFadeIn && !startFadeIn){
					startFadeIn = true;
					StartCoroutine(FadeIn());
				}
				if(moveFracJourney >= m_movementFX.m_startFadeOut && !startFadeOut){
					startFadeOut = true;
					StartCoroutine(FadeOut());
				}

				m_movementFX.m_movementFxAudioSource.pitch = Mathf.Lerp(m_movementFX.m_minPitch, m_movementFX.m_maxPitch, m_movementFX.m_pitchCurve.Evaluate(moveFracJourney));
			}

			yield return null;
		}
		
		if(m_trainController != null){
			m_trainController.ChoseNextTarget();
			m_trainController.ResetTrainParent();
		}

		switch(m_pistonType){ 
			case PistonTypes.Move:
				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
			case PistonTypes.MoveAndRotate:
				if(m_resetTransformAfterMoving){
					StartCoroutine(ResetTransform());
				}
			break;
		}
	}
	IEnumerator MoveAndRotateCorout(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();

		bool startFadeIn = false;
		bool startFadeOut = false;

		while(m_move.m_transform.localPosition != m_move.m_toLocalePosition){
			// Move
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_move.m_toLocalePosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_move.m_toLocalePosition, m_move.m_curve.Evaluate(moveFracJourney));

			// Rotate
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / moveJourneyLength;
			Quaternion toRotation = Quaternion.Euler(m_rotate.m_toRotation);
			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));

			if(m_movementFX.m_movementFxAudioSource != null){
				if(rotateFracJourney >= m_movementFX.m_startFadeIn && !startFadeIn){
					startFadeIn = true;
					StartCoroutine(FadeIn());
				}
				if(rotateFracJourney >= m_movementFX.m_startFadeOut && !startFadeOut){
					startFadeOut = true;
					StartCoroutine(FadeOut());
				}

				m_movementFX.m_movementFxAudioSource.pitch = Mathf.Lerp(m_movementFX.m_minPitch, m_movementFX.m_maxPitch, m_movementFX.m_pitchCurve.Evaluate(rotateFracJourney));
			}

			yield return null;
		}
		m_trainController.ChoseNextTarget();
		m_trainController.ResetTrainParent();

		if(m_resetTransformAfterMoving){
			StartCoroutine(ResetTransform());
		}
	}
	IEnumerator ResetTransform(){
		yield return new WaitForSeconds(m_timeToWaitBeforeReset);
		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateFracJourney = new float();

		while(m_move.m_transform.localPosition != m_startPosition){
			// Move
			moveJourneyLength = Vector3.Distance(m_move.m_transform.localPosition, m_startPosition);
			moveFracJourney += (Time.deltaTime) * m_move.m_speed / moveJourneyLength;
			m_move.m_transform.localPosition = Vector3.Lerp(m_move.m_transform.localPosition, m_startPosition, m_move.m_curve.Evaluate(moveFracJourney));

			// Rotate
			rotateFracJourney += (Time.deltaTime) * m_rotate.m_speed / moveJourneyLength;
			Quaternion toRotation = Quaternion.Euler(m_startRotation);
			m_rotate.m_transform.rotation = Quaternion.Lerp(m_rotate.m_transform.rotation, toRotation, m_rotate.m_curve.Evaluate(rotateFracJourney));

			yield return null;
		}
	}

	void CheckPositionAndRotation(){
		if(m_rotateIsFinished && m_moveIsFinished){
			m_trainController.ChoseNextTarget();
			m_rotateIsFinished = false;
			m_moveIsFinished = false;
		}
	}

	IEnumerator FadeIn(){
		if(m_movementFxCanFadeIn){
			m_movementFX.m_movementFxAudioSource.Play();
			m_movementFX.m_movementFxAudioSource.volume = 0;
			float moveFracJourney = 0;
			float fadeInTime = m_movementFX.m_fadeInTime;
			float v = 1 / ((fadeInTime + (fadeInTime/2)) / Time.fixedDeltaTime);
			while(m_movementFX.m_movementFxAudioSource.volume < 1){
				moveFracJourney += v;
				m_movementFX.m_movementFxAudioSource.volume = Mathf.Lerp(0, 1, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_movementFxCanFadeIn = false;
		}
		yield break;
	}
	IEnumerator FadeOut(){
		if(m_movementFX.m_movementFxAudioSource.isPlaying){
			float moveFracJourney = 0;
			float fadeOutTime = m_movementFX.m_fadeOutTime;
			float v = 1 / ((fadeOutTime + (fadeOutTime/2)) / Time.fixedDeltaTime);
			while(m_movementFX.m_movementFxAudioSource.volume > 0){
				moveFracJourney += v;
				m_movementFX.m_movementFxAudioSource.volume = Mathf.Lerp(1, 0, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_movementFX.m_movementFxAudioSource.Stop();
			m_movementFxCanFadeIn = true;
		}
		yield break;
	}

}

