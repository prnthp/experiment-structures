using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class SkipToTrialPhase : Phase
{
    public Trial target;
    
    // Required override
    public override void Enter()
    {
        
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        trial.block.SkipToTrial(target);
    }
}
