using System.Collections;
using System.Collections.Generic;
using ExperimentStructures;
using UnityEngine;
using UnityEngine.Events;

public class GenericPhase : Phase
{
    [Space] public UnityEvent onEnter;

    public UnityEvent onExit;

    public override void Enter()
    {
        onEnter.Invoke();
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
        onExit.Invoke();
    }
}