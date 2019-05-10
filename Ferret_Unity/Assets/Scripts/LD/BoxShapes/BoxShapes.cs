using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxShapes : ShapeEnum {

	[Header("Objects positions")]
	[SerializeField] Transform m_rectangleTrans;
	[SerializeField] Transform m_circleTrans;
	[SerializeField] Transform m_triangleTrans;

	[Header("Interpolations")]
	[SerializeField] float m_interpolationSpeed = 5;
	[SerializeField] AnimationCurve m_interpolationCurve;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] BoxCollider m_boxColl;
	[SerializeField] Color m_colorGizmos = Color.magenta;

	Animator m_animator;
	int m_actualObjectsInBox = 0;

	void Start(){
		m_animator = GetComponent<Animator>();
	}

	void OnTriggerEnter(Collider col){
		if(col.GetComponent<Shapes>() != null){
			Shapes shapes = col.GetComponent<Shapes>();
			ObjectToBeGrapped grappedObj = col.GetComponent<ObjectToBeGrapped>();
			Transform shapeTrans = shapes.transform;

			grappedObj.CanBeGrapped = false;

			switch(shapes.m_shapesType){
				case ShapesType.Rectangle:
					StartCoroutine(ClimbInterpolation(shapeTrans, shapeTrans.position, m_rectangleTrans.position, shapeTrans.rotation, m_rectangleTrans.rotation));
				break;
				case ShapesType.Circle:
					StartCoroutine(ClimbInterpolation(shapeTrans, shapeTrans.position, m_circleTrans.position, shapeTrans.rotation, m_circleTrans.rotation));
				break;
				case ShapesType.Triangle:
					StartCoroutine(ClimbInterpolation(shapeTrans, shapeTrans.position, m_triangleTrans.position, shapeTrans.rotation, m_triangleTrans.rotation));
				break;
			}
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
		m_actualObjectsInBox ++;
		CheckObjectsNumber();
	}

	void CheckObjectsNumber(){
		if(m_actualObjectsInBox == 3){
			m_animator.SetTrigger("Open");
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = m_colorGizmos;
		if(!m_showGizmos){
			return;
		}
		if(m_boxColl != null){
			Gizmos.DrawWireCube(transform.position + m_boxColl.center, m_boxColl.size);
		}
	}

}
