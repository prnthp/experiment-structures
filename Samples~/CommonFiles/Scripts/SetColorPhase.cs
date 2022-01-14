using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class SetColorPhase : Phase
{
    public Image LeftColorImage;
    public Image RightColorImage;

    public RandomColorPicker colorPicker;

    // Required override
    public override void Enter()
    {
        LeftColorImage.color = colorPicker.LeftColor;
        RightColorImage.color = colorPicker.RightColor;
        
        DataLogger.Instance.Datapoints.SetValue("left_color", ColorUtility.ToHtmlStringRGBA(LeftColorImage.color));
        DataLogger.Instance.Datapoints.SetValue("right_color", ColorUtility.ToHtmlStringRGBA(RightColorImage.color));
    }

    public override void Loop()
    {
        
    }

    public override void OnExit()
    {
        
    }
}
