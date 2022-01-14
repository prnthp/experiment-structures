using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.Events;

public class AEPsychSetupPhase : Phase
{
    [Multiline(20)] public string config;
    
    // Required override
    public override void Enter()
    {
        var aePsychTrial = (AEPsychTrial)trial;
        var fullPath = Path.Combine(Application.streamingAssetsPath, aePsychTrial.configFilePath);

        using (var reader = new StreamReader(fullPath))
        {
            config = reader.ReadToEnd();
        }
            
        if (!AEPsychClient.Instance.SetupTrials(config, SetStrategy))
        {
            Debug.LogError("[AEPsych] Invalid State");
        }
    }
        

    private void SetStrategy(int id)
    {
        ((AEPsychTrial)trial).currentStrategyId = id;
        ExitPhase();
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
