using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour {

	[Header("Skeleton")]
	[SerializeField] GameObject m_skeleton;

	[Header("Rope")]
	[SerializeField] InteractiveObject[] m_ropeToCut = new InteractiveObject[3];	

	[Header("Show camera")]
	[SerializeField] CameraTrigger m_camTrigger;
	[SerializeField] float m_timeToShowLastTarget = 3;

	[Header("Shake camera")]
	[SerializeField] ShakeCamera m_shakeCamera;
	
	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_color = Color.white;

	Animator m_skeletonAnimator;
	int m_disablePikeNb = 0;

	void Start(){

		m_skeletonAnimator = m_skeleton.GetComponent<Animator>();

		if(m_ropeToCut != null){
			for (int i = 0, l = m_ropeToCut.Length; i < l; ++i){
				if(m_ropeToCut[i] != null){
					m_ropeToCut[i].SetSkeletonController(this);
				}
			}
		}
	}

	public void On_PikeIsDisable(){
		m_disablePikeNb ++;

		if(m_disablePikeNb == m_ropeToCut.Length){
			MovingSkeleton();
			m_camTrigger.m_lookPosition.m_timeToShowTarget = m_timeToShowLastTarget;
			m_camTrigger.On_SkeletonMoved();

			if(m_shakeCamera != null){
				m_shakeCamera.StartShake();
			}
		}else{
			m_camTrigger.On_SkeletonMoved();

			if(m_shakeCamera != null){
				m_shakeCamera.StartShake();
			}
		}
	}
	
	void MovingSkeleton(){
		// Debug.Log("SKELETON");
		m_skeletonAnimator.SetTrigger("ObjectifReached");
	}


	void OnDrawGizmos(){
		if(m_showGizmos){
			Gizmos.color = m_color;
			if(m_ropeToCut != null && m_skeleton != null){
				for (int i = 0, l = m_ropeToCut.Length; i < l; ++i){
					if(m_ropeToCut[i] != null){
						Gizmos.DrawLine(m_ropeToCut[i].transform.position, m_skeleton.transform.position);
					}
				}
			}
		}
	}

}
