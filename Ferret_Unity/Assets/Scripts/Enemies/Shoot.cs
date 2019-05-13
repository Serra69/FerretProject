using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

	[SerializeField] GameObject m_projectile;
	[SerializeField] float m_fireCooldown = 2f;
	[SerializeField] Transform m_projectileSpawnPoint;

	FieldOfView m_fov;
	bool m_canShoot = true;
	PlayerManager m_playerManager;

	void Start(){
		m_fov = GetComponent<FieldOfView>();
		m_playerManager = PlayerManager.Instance;
	}

	void Update(){
		if(m_fov.m_playerTarget != null){
			if(m_canShoot && !m_playerManager.PlayerIsDead){
				Fire();
			}
		}
	}

	void Fire(){
		m_canShoot = false;
		StartCoroutine(FireCooldown());

		Instantiate(m_projectile, m_projectileSpawnPoint.position, m_projectileSpawnPoint.rotation);
	}

	IEnumerator FireCooldown(){
		yield return new WaitForSeconds(m_fireCooldown);
		m_canShoot = true;
	}

}
