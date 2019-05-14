using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeRewardManager : MonoBehaviour {

#region Singleton
	public static NarrativeRewardManager Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of NarrativeRewardManager");
		}
	}
#endregion //Singleton

	[Header("Animations")]
	public Animations m_animations = new Animations();
	[System.Serializable] public class Animations {
		public float m_timeToShowReward = 5.0f;
		[Space]
		public float m_moveSpeed = 5.0f;
		public AnimationCurve m_movementCurve;
		[Space]
		public Transform m_objectToAnimate;

		[Header("Positions")]
		public Vector3 m_startPos;
		public Vector3 m_endPos;
	}


	[Header("Rewards")]
	public NarrativeRewards m_narrativeRewards = new NarrativeRewards();
	[System.Serializable] public class NarrativeRewards {
		public Reward[] m_rewards;
		[System.Serializable] public class Reward {
			private bool m_isFound;
			public string m_name = "Title";
			public Image m_image;
			[TextArea(2,6)] public string m_descriptionText = "Description text of the narrative reward!";
		}
	}

	[Header("UI")]
	public NarrativeUI m_narrativeUI = new NarrativeUI();
	[System.Serializable] public class NarrativeUI {
		public GameObject m_narrativeRewardCanvas;
		public Text m_titleTextReward;
		public Image m_imageReward;
		public Text m_descriptionTextReward;
	}

	void Start(){
		m_narrativeUI.m_narrativeRewardCanvas.SetActive(false);
	}

	public void On_NarrativeRewardIsDiscovered(int rewardNumber){
		ChangeNarrativeRewardImages(m_narrativeRewards.m_rewards[rewardNumber].m_name, m_narrativeRewards.m_rewards[rewardNumber].m_image, m_narrativeRewards.m_rewards[rewardNumber].m_descriptionText);
		StartCoroutine(ShowReward());
	}

	void ChangeNarrativeRewardImages(string title, Image image, string text){
		m_narrativeUI.m_titleTextReward.text = title;
		m_narrativeUI.m_imageReward = image;
		m_narrativeUI.m_descriptionTextReward.text = text;
	}

	IEnumerator ShowReward(){
		m_animations.m_objectToAnimate.localPosition = m_animations.m_startPos;
		m_narrativeUI.m_narrativeRewardCanvas.SetActive(true);
		yield return new WaitForSeconds(m_animations.m_timeToShowReward);
		StartCoroutine(MoveRewardUI());
	}

	IEnumerator MoveRewardUI(){
		float moveJourneyLength;
		float moveFracJourney = new float();
		while(m_animations.m_objectToAnimate.localPosition != m_animations.m_endPos){
			moveJourneyLength = Vector3.Distance(m_animations.m_objectToAnimate.localPosition, m_animations.m_endPos);
			moveFracJourney += (Time.deltaTime) * m_animations.m_moveSpeed / moveJourneyLength;
			
			m_animations.m_objectToAnimate.localPosition = Vector3.Lerp(m_animations.m_objectToAnimate.localPosition, m_animations.m_endPos, m_animations.m_movementCurve.Evaluate(moveFracJourney));
			yield return null;
		}
		m_narrativeUI.m_narrativeRewardCanvas.SetActive(false);
	}

}