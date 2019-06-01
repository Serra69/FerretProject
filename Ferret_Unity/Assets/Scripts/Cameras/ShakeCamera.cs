using UnityEngine;
using Cinemachine;

public class ShakeCamera : MonoBehaviour {

	public Vector3 m_ImpulseVelocity = Vector3.one;

	[Header("Test input")]
	public KeyCode m_activateImpulseKey = KeyCode.B;

	CinemachineImpulseSource m_impulseSource;

	void Start(){
		m_impulseSource = GetComponent<CinemachineImpulseSource>();
	}

	void Update(){
		if(Input.GetKeyDown(m_activateImpulseKey)){
			StartShake();
		}
	}

	public void StartShake(){
		m_impulseSource.GenerateImpulseAt(transform.position, m_ImpulseVelocity);
	}

}
