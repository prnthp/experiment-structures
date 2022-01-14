using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class PlayAnimationPhase : Phase
{
    public Animator animator;
    public string animationName;
    
    // Required override
    public override void Enter()
    {
        animator.Play(animationName);
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        
    }
}
