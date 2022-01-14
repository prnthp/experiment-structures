using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleTrialSelectionPhase : Phase
{
    public GenericDictionary<Button, Trial> buttonTrialPairs;

    public SkipToTrialPhase skipToTrialPhase;
    
    // Required override
    public override void Enter()
    {
        foreach (var pair in buttonTrialPairs)
        {
            pair.Key.onClick.AddListener(() => skipToTrialPhase.target = pair.Value);
            pair.Key.onClick.AddListener(ExitPhase);
        }
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        foreach (var pair in buttonTrialPairs)
        {
            pair.Key.onClick.RemoveAllListeners();
        }
    }
}
