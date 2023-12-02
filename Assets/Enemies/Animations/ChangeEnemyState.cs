using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEnemyState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyController enemy = animator.GetComponentInParent<EnemyController>();
        if (stateInfo.IsTag("Paralysed"))
        {
            enemy.changeToParalysed();
        }
        else if (stateInfo.IsTag("Attack"))
        {
            enemy.changeToAttack();
        }
        else if (stateInfo.IsTag("Death"))
        {
            enemy.die();
        }
        else
        {
            enemy.endAttack();
        }
    }
}
