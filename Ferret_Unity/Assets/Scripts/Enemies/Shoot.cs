using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

	[SerializeField] GameObject m_projectile;
	[SerializeField] float m_fireCooldown = 2f;
	[SerializeField] float m_timeToFire = 0.25f;
	[SerializeField] Transform m_projectileSpawnPoint;

	FieldOfView m_fov;
	bool m_canShoot = true;
	PlayerManager m_playerManager;

	void Start(){
		m_fov = GetComponent<FieldOfView>();
		m_playerManager = PlayerManager.Instance;
	}

	void Update(){
		if(m_fov.PlayerTarget != null){
			if(m_canShoot && !m_playerManager.PlayerIsDead){
				m_canShoot = false;
				StartCoroutine(TimeToFire());
			}
		}
	}

	void Fire(){
		if(m_fov.PlayerTarget != null){
			StartCoroutine(FireCooldown());
			Instantiate(m_projectile, m_projectileSpawnPoint.position, m_projectileSpawnPoint.rotation);
		}else{
			m_canShoot = true;
		}
	}

	IEnumerator FireCooldown(){
		yield return new WaitForSeconds(m_fireCooldown);
		m_canShoot = true;
	}

	IEnumerator TimeToFire(){
		yield return new WaitForSeconds(m_timeToFire);
		Fire();
	}

}
