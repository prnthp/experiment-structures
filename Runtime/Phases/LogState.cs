using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

/// <summary>
/// To "stamp" a state in Data Logger, LogState() must be called. This Phase provides a simple way to do that.
/// </summary>
public class LogState : Phase
{
    public override void Enter()
    {
        DataLogger.Instance.LogState();
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}