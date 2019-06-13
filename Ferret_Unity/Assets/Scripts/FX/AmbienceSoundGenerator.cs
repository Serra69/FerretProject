using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceSoundGenerator : MonoBehaviour {

	public AmbienceSounds m_ambienceSounds = new AmbienceSounds();
	[System.Serializable] public class AmbienceSounds {
	
		[Header("Sounds")]
		public AudioSource[] m_loopFX;

		[Header("Fade in / Fade out")]
		public float m_fadeInTime = 5.0f;
		public float m_fadeOutTime = 5.0f;

		[Header("Number of same loop")]
		public int m_numberOfLoopMini = 1;
		public int m_numberOfLoopMax = 1;
	}

	float[] m_loopTimes;
	bool[] m_canFadeIn;

	void Start(){
		m_loopTimes = new float[m_ambienceSounds.m_loopFX.Length];
		for (int i = 0; i < m_ambienceSounds.m_loopFX.Length; i++){
			m_loopTimes[i] = m_ambienceSounds.m_loopFX[i].clip.length;
		}
		m_canFadeIn = new bool[m_ambienceSounds.m_loopFX.Length];
		for(int i = 0; i < m_canFadeIn.Length; i ++){
			m_canFadeIn[i] = true;
		}
		StartCoroutine(AddSound(0));
	}

	IEnumerator AddSound(float waitToAddSound){
		// Debug.Log("Add sound in: " + waitToAddSound);
		yield return new WaitForSeconds(waitToAddSound);

		int num;
		num = Random.Range(0, m_ambienceSounds.m_loopFX.Length);
		if(m_ambienceSounds.m_loopFX[num].volume == 1 || m_ambienceSounds.m_loopFX[num].isPlaying){
			// Debug.Log("RETRY to Add sound in: " + 0);
			StartCoroutine(AddSound(0));
		}else{
			// Debug.Log("ADD " + num + " IS SUCCESS" );
			StartSound(num);
		}
	}

	void StartSound(int i){
		CalculTimeToEndSoundLoop(i);
		m_ambienceSounds.m_loopFX[i].volume = 0;
		m_ambienceSounds.m_loopFX[i].Stop();
		m_ambienceSounds.m_loopFX[i].Play();

		m_canFadeIn[i] = true;
		StartCoroutine(FadeIn(i));
	}

	IEnumerator FadeIn(int i){
		if((m_ambienceSounds.m_loopFX[i].isPlaying) && (m_canFadeIn[i])){
			float moveFracJourney = 0;
			float fadeInTime = m_ambienceSounds.m_fadeInTime - 0.30f;
			float v = 1 / ((fadeInTime + (fadeInTime/2)) / Time.fixedDeltaTime);
			while(m_ambienceSounds.m_loopFX[i].volume < 1){
				moveFracJourney += v;
				m_ambienceSounds.m_loopFX[i].volume = Mathf.Lerp(0, 1, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_canFadeIn[i] = false;
		}
		yield break;
	}
	IEnumerator FadeOut(int i){
		if(m_ambienceSounds.m_loopFX[i].isPlaying){
			float moveFracJourney = 0;
			float fadeOutTime = m_ambienceSounds.m_fadeOutTime - 0.30f;
			float v = 1 / ((fadeOutTime + (fadeOutTime/2)) / Time.fixedDeltaTime);
			while(m_ambienceSounds.m_loopFX[i].volume > 0){
				moveFracJourney += v;
				m_ambienceSounds.m_loopFX[i].volume = Mathf.Lerp(1, 0, moveFracJourney);
				yield return new WaitForSeconds(0.0008f);
			}
			m_ambienceSounds.m_loopFX[i].Stop();
			m_canFadeIn[i] = true;
		}
		yield break;
	}

	void CalculTimeToEndSoundLoop(int i){
		int alea = Random.Range(m_ambienceSounds.m_numberOfLoopMini, m_ambienceSounds.m_numberOfLoopMax);
		float f = m_loopTimes[i] * alea - m_ambienceSounds.m_fadeOutTime / 2;
		StartCoroutine(TimerToFadeOut(i, f));
		StartCoroutine(AddSound(f));
	}

	IEnumerator TimerToFadeOut(int currentLoop, float timeToStopSound){
		yield return new WaitForSeconds(timeToStopSound);
		StartCoroutine(FadeOut(currentLoop));
	}

}
