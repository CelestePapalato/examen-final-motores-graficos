using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = animator.GetComponentInParent<PlayerController>();
        if (stateInfo.IsTag("Paralysed"))
        {
            player.changeToParalysed();
        }
        else if (stateInfo.IsTag("Attack"))
        {
            player.changeToAttack();
        }
        else if (stateInfo.IsTag("Dodge"))
        {
            player.changeToDodge();
        }
        else
        {
            player.changeToIdle();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.IsTag("Attack"))
        {
            PlayerController player = animator.GetComponent<PlayerController>();
            player.rotacionJugadorDuranteAtaque();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        if (stateInfo.IsTag("Dodge"))
        {
            PlayerController player = animator.GetComponent<PlayerController>();
            player.exitDodge();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
