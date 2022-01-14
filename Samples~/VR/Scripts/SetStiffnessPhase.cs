using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class SetStiffnessPhase : Phase
{
    public BackPlaneButtonManager buttonManager;

    public bool referenceOnBoth;

    // Required override
    public override void Enter()
    {
        var stiffnessTrial = (ButtonStiffnessTrial)trial;

        if (referenceOnBoth)
        {
            buttonManager.SetButtonStiffness(stiffnessTrial.referenceStiffness, stiffnessTrial.referenceStiffness);
            buttonManager.SetButtonHaptics(0f, 0f);
            return;
        }
        
        buttonManager.SetButtonStiffness(stiffnessTrial.referenceStiffness, stiffnessTrial.referenceStiffness);
        buttonManager.SetButtonHaptics(0f, 0f);
        
        if (stiffnessTrial.currentSide == 0)
        {
            if (stiffnessTrial.haptics)
                buttonManager.SetButtonHaptics(stiffnessTrial.referenceHapticAmplitude, stiffnessTrial.currentHapticAmplitude);
            if (stiffnessTrial.visual)
                buttonManager.SetButtonStiffness(stiffnessTrial.referenceStiffness, stiffnessTrial.currentStiffness);
        }
        else
        {
            if (stiffnessTrial.haptics)
                buttonManager.SetButtonHaptics(stiffnessTrial.currentHapticAmplitude, stiffnessTrial.referenceHapticAmplitude);
            if (stiffnessTrial.visual)
                buttonManager.SetButtonStiffness(stiffnessTrial.currentStiffness, stiffnessTrial.referenceStiffness);
        }
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
