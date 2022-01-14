using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class PayRespectsPhase : Phase
{
    // Required override
    public override void Enter()
    {
        
    }

    // Required override
    public override void Loop()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            ExperimentManager.Instance.RaiseNextPhase();
        }
    }

    // Required override
    public override void OnExit()
    {
        
    }
}
