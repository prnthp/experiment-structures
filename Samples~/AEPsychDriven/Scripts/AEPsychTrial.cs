using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEditor;
using TrialConfig = GenericDictionary<string, System.Collections.Generic.List<float>>;

public class AEPsychTrial : Trial
{
    [Tooltip("The trial will run endlessly until the AEPsych server notifies us that the trial has finished")]
    public bool useAEPsychIsFinishedFlag = true;
    public int currentStrategyId;
    public string configFilePath;
    public TrialConfig config;
    public int outcome;
    public bool isFinished;

    // // Optional override
    protected override void OnTrialBegin()
    {
        Endless = useAEPsychIsFinishedFlag;
    }

    // // Optional override
    protected override void OnNextRepetition()
    {
        if (isFinished)
        {
            EndTrial();
        }
    }

    // // Optional override
    // protected override void OnTrialComplete()
    // {
    //     
    // }
}