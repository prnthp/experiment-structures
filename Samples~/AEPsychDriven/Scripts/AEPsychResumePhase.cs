using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class AEPsychResumePhase : Phase
{
    public int strategyId;
    
    // Required override
    public override void Enter()
    {
        if (!AEPsychClient.Instance.ResumeStrategy(strategyId, GetStrategy))
        {
            Debug.LogError("[AEPsych] Invalid State");
        }
    }

    private void GetStrategy(int id)
    {
        ExitPhase();
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
