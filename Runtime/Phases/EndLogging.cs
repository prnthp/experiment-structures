using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class EndLogging : Phase
{
    public override void Enter()
    {
        DataLogger.Instance.EndLogging();
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}