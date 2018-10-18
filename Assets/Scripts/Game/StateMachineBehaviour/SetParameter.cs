using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParameter : StateMachineBehaviour
{
    public enum ParameterType
    {
        kInt,
        kFloat,
        kBool
    }

    public enum Timing
    {
        kOnStateEnter,
        kOnStateExit,
        kOnStateUpdate
    }

    [System.Serializable]
    public struct ParameterInfo
    {
        public ParameterType type;
        public string name;
        public float value;
        public Timing timing;
    }

    [SerializeField] ParameterInfo[] kParameters;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetParameters(animator, Timing.kOnStateEnter);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetParameters(animator, Timing.kOnStateUpdate);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetParameters(animator, Timing.kOnStateExit);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

    private void SetParameters(Animator animator, Timing timing)
    {
        foreach (var parameter in kParameters)
        {
            if(parameter.timing == timing)
            {
                switch (parameter.type)
                {
                    case ParameterType.kBool:
                        animator.SetBool(parameter.name, parameter.value == 0f ? false : true);
                        break;
                    case ParameterType.kFloat:
                        animator.SetFloat(parameter.name, parameter.value);
                        break;
                    case ParameterType.kInt:
                        animator.SetInteger(parameter.name, (int)parameter.value);
                        break;
                }
            }
        }
    }
}
