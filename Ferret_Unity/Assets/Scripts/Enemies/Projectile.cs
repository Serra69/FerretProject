using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	[SerializeField] float m_speed = 25f;

	Rigidbody m_rigidbody;
	bool m_checkCollisoin = false;
	bool m_isStick = false;
	CapsuleCollider m_collider;

	void Start(){
		m_rigidbody = GetComponent<Rigidbody>();
		m_collider = GetComponent<CapsuleCollider>();
	}

	void FixedUpdate(){
		m_rigidbody.velocity = transform.forward * m_speed;
	}

	void OnCollisionEnter(Collision col){
		// print("colission with : " + col.gameObject.name);

		if(m_isStick){
			return;
		}

		if(col.gameObject.CompareTag("Player")){
			PlayerManager playerManager = col.gameObject.GetComponentInParent<PlayerManager>();
			if(!playerManager.PlayerIsDead){
				// transform.parent = col.gameObject.transform;
				playerManager.On_PlayerDie();
			}
		}

		if(col.gameObject.CompareTag("Untagged")){
			transform.parent = col.gameObject.transform;
		}

		On_ProjectileIsStuck();

	}

	void On_ProjectileIsStuck(){
		m_isStick = true;
		m_rigidbody.isKinematic = true;
		m_rigidbody.Sleep();
		m_collider.enabled = false;
	}
	
}

/*using System.Collections;
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
	
}*/