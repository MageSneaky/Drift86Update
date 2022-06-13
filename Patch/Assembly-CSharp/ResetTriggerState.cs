using System;
using UnityEngine;

public class ResetTriggerState : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.ResetOnStateEnter)
		{
			animator.ResetTrigger(this.TriggerName);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.ResetOnStateExit)
		{
			animator.ResetTrigger(this.TriggerName);
		}
	}

	[SerializeField]
	private bool ResetOnStateEnter;

	[SerializeField]
	private bool ResetOnStateExit;

	[SerializeField]
	private string TriggerName;
}
