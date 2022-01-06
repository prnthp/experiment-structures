using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class StartLogging : Phase
{
    public string prepend;

    public override void Enter()
    {
        DataLogger.Instance.StartLogging(prepend);
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}