using System;
using UnityEngine;

public class SwitchBoolState : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.SwitchOnStateEnter)
		{
			animator.SetBool(this.BoolStateName, this.NewValue);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.SwitchOnStateExit)
		{
			animator.SetBool(this.BoolStateName, this.NewValue);
		}
	}

	[SerializeField]
	private bool SwitchOnStateEnter;

	[SerializeField]
	private bool SwitchOnStateExit;

	[SerializeField]
	private string BoolStateName;

	[SerializeField]
	private bool NewValue;
}
