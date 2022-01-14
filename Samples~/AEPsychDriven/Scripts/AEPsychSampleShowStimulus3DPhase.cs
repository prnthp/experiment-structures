using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowStimulus3DPhase : Phase
{
    public Text stimulusText;
    public Image stimulus;

    public string configKey1 = "R";
    public string configKey2 = "G";
    public string configKey3 = "B";
    
    // Required override
    public override void Enter()
    {
        var aePsychTrial = (AEPsychTrial)trial;
        
        var r = aePsychTrial.config[configKey1][0];
        var g = aePsychTrial.config[configKey2][0];
        var b = aePsychTrial.config[configKey3][0];

        var color = new Color(r, g, b);

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
