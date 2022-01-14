using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using TrialConfig = GenericDictionary<string, System.Collections.Generic.List<float>>;

public class AEPsychAskPhase : Phase
{
    // Required override
    public override void Enter()
    {
        if (!AEPsychClient.Instance.AskForNextTrialConfig(UpdateNextTrialConfig))
        {
            Debug.LogError("[AEPsych] Invalid State");
        }
    }

    private void UpdateNextTrialConfig(TrialConfig config, bool isFinished)
    {
        ((AEPsychTrial)trial).config = config;
        ((AEPsychTrial)trial).isFinished = isFinished;
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
