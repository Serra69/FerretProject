using UnityEngine;

public class Path : MonoBehaviour {

	public PathType m_pathType = PathType.Simple;
	public enum PathType {
		Simple,	
		Wait
	}

	[Range(0.05f, 2)] [SerializeField] private float m_gizmosRadius = 0.25f;
	[SerializeField] private Color m_simplePathColor = Color.magenta;
	[SerializeField] private Color m_waitPathColor = Color.yellow;

	void OnDrawGizmos(){
		switch(m_pathType){ 
			case PathType.Simple:
				Gizmos.color = m_simplePathColor;
				Gizmos.DrawSphere(transform.position, m_gizmosRadius);
			break;
			case PathType.Wait:
				Gizmos.color = m_waitPathColor;
				Gizmos.DrawSphere(transform.position, m_gizmosRadius);
			break;
		}
	}

	void GetPathType(GameObject go){
		switch(m_pathType){
			case PathType.Simple:
				go.SendMessage("GetPathType", 1, SendMessageOptions.RequireReceiver);
			break;
			case PathType.Wait:
				go.SendMessage("GetPathType", 2, SendMessageOptions.RequireReceiver);
			break;
		}
	}

}
