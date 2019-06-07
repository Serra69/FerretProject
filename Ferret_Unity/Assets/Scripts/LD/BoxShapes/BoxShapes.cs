using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxShapes : ShapeEnum {

	[Header("Objects positions")]
	[SerializeField] Transform m_rectangleTrans;
	[SerializeField] Transform m_circleTrans;
	[SerializeField] Transform m_triangleTrans;

	[Header("Interpolations")]
	[SerializeField] float m_interpolationSpeed = 5;
	[SerializeField] AnimationCurve m_interpolationCurve;

	[Header("Event")]
	[SerializeField] UnityEvent m_onBoxIsOpen;

	[Header("FX")]
	[SerializeField] GameObject m_puttingPieceFx;
	[Range(0, 1), SerializeField] float m_timeToStartSound = 0.75f;
	[Space]
	[SerializeField] GameObject m_openBoxFx;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_colorGizmos = Color.magenta;

	bool m_rectangleIsInBox = false;
	bool m_triangleIsInBox = false;
	bool m_circleIsInBox = false;

	Animator m_animator;
	// int m_actualObjectsInBox = 0;

	BoxCollider m_boxColl;
    BoxCollider BoxCollider{
        get
		{
            return GetComponent<BoxCollider>();
        }
    }

	void Start(){
		m_animator = GetComponent<Animator>();
	}

	void OnTriggerEnter(Collider col){
		if(col.GetComponent<Shapes>() != null){
			Shapes shapes = col.GetComponent<Shapes>();
			ObjectToBeGrapped grappedObj = col.GetComponent<ObjectToBeGrapped>();
			Transform shapeTrans = shapes.transform;

			grappedObj.CanBeGrapped = false;
			grappedObj.SetAnUnusableobject(true);

			switch(shapes.m_shapesType){
				case ShapesType.Rectangle:
					StartCoroutine(ShapeInterpolation(shapeTrans, shapeTrans.position, m_rectangleTrans.position, shapeTrans.rotation, m_rectangleTrans.rotation, ShapesType.Rectangle));
				break;
				case ShapesType.Circle:
					StartCoroutine(ShapeInterpolation(shapeTrans, shapeTrans.position, m_circleTrans.position, shapeTrans.rotation, m_circleTrans.rotation, ShapesType.Circle));
				break;
				case ShapesType.Triangle:
					StartCoroutine(ShapeInterpolation(shapeTrans, shapeTrans.position, m_triangleTrans.position, shapeTrans.rotation, m_triangleTrans.rotation, ShapesType.Triangle));
				break;
			}
		}
	}

	IEnumerator ShapeInterpolation(Transform trans, Vector3 fromPosition, Vector3 toPosition, Quaternion fromRotation, Quaternion toRotation, ShapesType shap){

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

			// if()

		}
		// print("End of coroutine");

		switch(shap){
			case ShapesType.Rectangle:
				m_rectangleIsInBox = true;
			break;
			case ShapesType.Circle:
				m_circleIsInBox = true;
			break;
			case ShapesType.Triangle:
				m_triangleIsInBox = true;
			break;
		}
		CheckObjectsNumber();

		// m_actualObjectsInBox ++;
	}

	void CheckObjectsNumber(){
		/*if(m_actualObjectsInBox == 3){
			m_animator.SetTrigger("Open");
		}*/
		if(m_rectangleIsInBox && m_circleIsInBox && m_triangleIsInBox){
			m_animator.SetTrigger("Open");
		}
	}

	public void OnBoxOpenAnimationIsFinish(){
		m_onBoxIsOpen.Invoke();
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
