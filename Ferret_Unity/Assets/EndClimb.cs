using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndClimb : StateMachineBehaviour {

	// [SerializeField] float m_timeToTpPlayer = 1;

	PlayerManager m_playerManager;
	// float m_timer = 0;
	// bool m_isDone = false;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(m_playerManager == null)
			m_playerManager = PlayerManager.Instance;
		// m_timer = 0;
		// m_isDone = false;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		/*m_timer += Time.deltaTime;
		if(m_timer > m_timeToTpPlayer && !m_isDone){
			m_isDone = true;
			// m_playerManager.On_EndClimbAnimIsFinished();
		}*/
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		m_playerManager.On_EndClimbAnimIsFinished();
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
