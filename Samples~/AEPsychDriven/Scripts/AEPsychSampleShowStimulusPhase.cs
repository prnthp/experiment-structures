using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowStimulusPhase : Phase
{
    public Text stimulusText;
    public Image stimulus;

    public string configKey = "alpha";
    
    // Required override
    public override void Enter()
    {
        var aePsychTrial = (AEPsychTrial)trial;
        var color = Color.white;
        color.a = aePsychTrial.config[configKey][0];
        stimulus.color = color;
        
        stimulusText?.gameObject.SetActive(true);
        stimulus.gameObject.SetActive(true);
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        stimulusText?.gameObject.SetActive(false);
        stimulus.gameObject.SetActive(false);
    }
}
