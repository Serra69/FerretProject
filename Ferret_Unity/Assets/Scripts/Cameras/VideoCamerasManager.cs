using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamerasManager : MonoBehaviour {

#region Singleton
	public static VideoCamerasManager Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of VideoCamerasManager");
		}
	}
#endregion

	[SerializeField] GameObject m_pathsCanvas;
	[Space]
	[SerializeField] PauseGame m_pauseGame;
	PlayerManager m_playerManager;

	bool m_pathsCanvasIsActive = false;

	void Start(){
		m_playerManager = PlayerManager.Instance;
		m_pathsCanvas.SetActive(m_pathsCanvasIsActive);
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.V)){
			SetPathsCanvasIsActive(!m_pathsCanvasIsActive);

			m_playerManager.m_videoCamerasManagerIsActive = m_pathsCanvasIsActive;
			m_pauseGame.m_videoCamerasManagerIsActive = m_pathsCanvasIsActive;
		}
	}

	public void SetPathsCanvasIsActive(bool isActive){
		m_pathsCanvasIsActive = isActive;
		m_pathsCanvas.SetActive(m_pathsCanvasIsActive);
	}

}
