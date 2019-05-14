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

	[SerializeField] private float m_timeToShowReward = 5.0f;
	[SerializeField] private Animator m_rewardAnimator;

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

	void Update(){
		if(Input.GetButton("Fire2")){
			On_NarrativeRewardIsDiscovered(0);
		}
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
		m_narrativeUI.m_narrativeRewardCanvas.SetActive(true);
		yield return new WaitForSeconds(m_timeToShowReward);
		m_rewardAnimator.SetTrigger("TakeReward");
		yield return new WaitForSeconds(2);
		m_narrativeUI.m_narrativeRewardCanvas.SetActive(false);
	}

}