using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AnswerStiffnessQuestionPhase : Phase
{
    // Required override
    public override void Enter()
    {
        BackPlaneButtonManager.onButtonPressed += OnButtonPressedHandler;
    }

    private void OnButtonPressedHandler(BackPlaneButtonManager.Button button)
    {
        if (button == BackPlaneButtonManager.Button.Left)
        {
            DataLogger.Instance.SetDataPoint("answer, left");
            ExperimentManager.Instance.RaiseNextPhase();
        }

        if (button == BackPlaneButtonManager.Button.Right)
        {
            DataLogger.Instance.SetDataPoint("answer, right");
            ExperimentManager.Instance.RaiseNextPhase();
        }
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
