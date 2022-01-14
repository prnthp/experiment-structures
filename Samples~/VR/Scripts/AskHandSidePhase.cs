using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class AskHandSidePhase : Phase
{
    public GameObject LeftMarker;
    public GameObject RightMarker;

    public ButtonHaptics leftButtonHaptics;
    public ButtonHaptics rightButtonHaptics;

    // Required override
    public override void Enter()
    {
        BackPlaneButtonManager.onButtonPressed += OnButtonPressedHandler;
    }

    private void OnButtonPressedHandler(BackPlaneButtonManager.Button button)
    {
        if (button == BackPlaneButtonManager.Button.Left)
        {
            RightMarker.SetActive(false);
            leftButtonHaptics.controller = OVRInput.Controller.LTouch;
            rightButtonHaptics.controller = OVRInput.Controller.LTouch;
            ExperimentManager.Instance.RaiseNextPhase();
        }

        if (button == BackPlaneButtonManager.Button.Right)
        {
            LeftMarker.SetActive(false);
            leftButtonHaptics.controller = OVRInput.Controller.RTouch;
            rightButtonHaptics.controller = OVRInput.Controller.RTouch;
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
