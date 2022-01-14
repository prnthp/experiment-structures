using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AnswerSidePhase : Phase
{
    public Button LeftButton;
    public Button RightButton;

    // Required override
    public override void Enter()
    {
        GuaranteeUnityFrameCycle = true;
        
        LeftButton.onClick.AddListener(OnLeftButtonClick);
        RightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void OnLeftButtonClick()
    {
        DataLogger.Instance.Datapoints.SetValue("answer", 0);
    }

    private void OnRightButtonClick()
    {
        DataLogger.Instance.Datapoints.SetValue("answer", 1);
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        LeftButton.onClick.RemoveListener(OnLeftButtonClick);
        RightButton.onClick.RemoveListener(OnRightButtonClick);
    }
}
