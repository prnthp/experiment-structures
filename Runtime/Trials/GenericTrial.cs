using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.Events;

public class GenericTrial : Trial
{
    [Space] public UnityEvent onTrialBegin;

    public UnityEvent onTrialComplete;

    public UnityEvent onNextRepetition;

    protected override void OnTrialBegin()
    {
        onTrialBegin.Invoke();
    }

    protected override void OnNextRepetition()
    {
        onNextRepetition.Invoke();
    }

    protected override void OnTrialComplete()
    {
        onTrialComplete.Invoke();
    }
}