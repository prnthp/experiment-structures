using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class WaitForAnyButtonPhase : Phase
{

    // Required override
    public override void Enter()
    {
        BackPlaneButtonManager.onButtonPressed += OnButtonPressedHandler;
    }

    private void OnButtonPressedHandler(BackPlaneButtonManager.Button button)
    {
        ExperimentManager.Instance.RaiseNextPhase();
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        BackPlaneButtonManager.onButtonPressed -= OnButtonPressedHandler;
    }
}
