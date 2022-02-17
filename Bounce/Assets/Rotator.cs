using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Rotate(Vector3.up * Time.deltaTime * animator.GetFloat("Speed"));
    }
}
