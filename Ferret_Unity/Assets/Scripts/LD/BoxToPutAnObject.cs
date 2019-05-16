using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class BoxToPutAnObject : MonoBehaviour {

	[Header("Object")]
	[SerializeField] GameObject m_onlyObjectToBeSnap;
	[SerializeField] Transform m_transformToSnap;
    [Space]
    [SerializeField] Transform m_parentToPutObject;

	[Header("Interpolations")]
	[SerializeField] float m_interpolationSpeed = 5;
	[SerializeField] AnimationCurve m_interpolationCurve;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_colorGizmos = Color.magenta;

	[Header("Event")]
	[SerializeField] UnityEvent m_onBoxIsOpen;

	bool m_objectIsPut = false;
    ObjectToBeGrapped m_grappedObj;

    BoxCollider m_boxColl;
    BoxCollider BoxCollider{
        get
		{
            return GetComponent<BoxCollider>();
        }
    }

	void OnTriggerEnter(Collider col){
		if(col.gameObject == m_onlyObjectToBeSnap && !m_objectIsPut){
			m_objectIsPut = true;

            m_grappedObj = col.GetComponent<ObjectToBeGrapped>();
            m_grappedObj.CanBeGrapped = false;
            m_grappedObj.SetAnUnusableobject(true);

			StartCoroutine(ClimbInterpolation(col.transform, col.transform.position, m_transformToSnap.transform.position, col.transform.rotation, m_transformToSnap.transform.rotation));
		}
	}

	IEnumerator ClimbInterpolation(Transform trans, Vector3 fromPosition, Vector3 toPosition, Quaternion fromRotation, Quaternion toRotation){

		float moveJourneyLength;
		float moveFracJourney = new float();
		float rotateJourneyLength;
		float rotateFracJourney = new float();

		while(trans.position != toPosition){
			// MovePosition
			moveJourneyLength = Vector3.Distance(fromPosition, toPosition);
			moveFracJourney += (Time.deltaTime) * m_interpolationSpeed / moveJourneyLength;
			trans.position = Vector3.Lerp(fromPosition, toPosition, m_interpolationCurve.Evaluate(moveFracJourney));

			// MoveRotation
			rotateJourneyLength = Vector3.Distance(fromPosition, toPosition);
			rotateFracJourney += (Time.deltaTime) * m_interpolationSpeed / rotateJourneyLength;
			trans.rotation = Quaternion.Slerp(fromRotation, toRotation, m_interpolationCurve.Evaluate(rotateFracJourney));

			yield return null;
		}
        m_grappedObj.transform.SetParent(m_parentToPutObject);
        m_onBoxIsOpen.Invoke();
	}

	void OnDrawGizmos(){
		Gizmos.color = m_colorGizmos;
		if(!m_showGizmos){
			return;
		}
		if(BoxCollider != null){
			Gizmos.DrawWireCube(transform.position + BoxCollider.center, BoxCollider.size);
		}
	}
	
}
