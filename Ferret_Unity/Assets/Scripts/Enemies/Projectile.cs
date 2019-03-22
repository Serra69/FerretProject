using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	[SerializeField] float m_speed = 25f;

	Rigidbody m_rigidbody;
	bool m_checkCollisoin = false;
	bool m_isStick = false;

	void Start(){
		m_rigidbody = GetComponent<Rigidbody>();

		StartCoroutine(ActivateCollisionDetection());
	}

	void FixedUpdate(){
		m_rigidbody.velocity = transform.forward * m_speed * Time.deltaTime ;
	}

	void OnCollisionEnter(Collision col){
		if(!m_checkCollisoin)
			return;

		if(!m_isStick){
			m_isStick = true;
			m_rigidbody.isKinematic = true;
			transform.parent = col.gameObject.transform;
			print("colission with : " + col.gameObject.name);
			Destroy(GetComponent<Collider>());

			if(col.gameObject.GetComponentInParent<PlayerManager>() != null){
				print("I shoot the player !");
			}
		}
	}

	IEnumerator ActivateCollisionDetection(){
		yield return new WaitForSeconds(0.1f);
		m_checkCollisoin = true;
	}
	
}
