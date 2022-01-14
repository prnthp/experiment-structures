using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowStimulus2DPhase : Phase
{
    public Text stimulusText;
    public Image stimulus;

    public string configKey1 = "R";
    public string configKey2 = "B";
    
    // Required override
    public override void Enter()
    {
        var aePsychTrial = (AEPsychTrial)trial;
        
        stimulus.color = new Color(
            aePsychTrial.config[configKey1][0], 
            0.2f, 
            aePsychTrial.config[configKey2][0]);
        
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