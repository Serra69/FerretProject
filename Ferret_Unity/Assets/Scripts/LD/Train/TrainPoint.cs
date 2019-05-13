using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPoint : TrainPathsTypes {

	public PointTypes m_pointTypes = PointTypes.WaitSeconds;
	[Space]
	// public float m_timeToWait = 4;

	public PointGizmos m_pointGizmos = new PointGizmos();
	[System.Serializable] public class PointGizmos {
		[Range(0.05f, 2)] public float m_radius = 0.25f;
		public Color m_startPointColor = Color.black;
		public Color m_waitSecondsPointColor = Color.magenta;
		public Color m_waitPointColor = Color.yellow;
	}

	void OnDrawGizmos(){
		switch(m_pointTypes){ 
			case PointTypes.StartPoint:
				Gizmos.color = m_pointGizmos.m_startPointColor;
				Gizmos.DrawSphere(transform.position, m_pointGizmos.m_radius);
			break;
			case PointTypes.WaitSeconds:
				Gizmos.color = m_pointGizmos.m_waitSecondsPointColor;
				Gizmos.DrawSphere(transform.position, m_pointGizmos.m_radius);
			break;
			case PointTypes.Wait:
				Gizmos.color = m_pointGizmos.m_waitPointColor;
				Gizmos.DrawSphere(transform.position, m_pointGizmos.m_radius);
			break;
		}
	}

}
