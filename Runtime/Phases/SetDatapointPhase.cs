using System;
using System.Collections;
using System.Collections.Generic;
using ExperimentStructures;
using UnityEngine;

public class SetDatapointPhase : Phase
{
    public GenericDictionary<string, string> datapoints;
    
    public override void Enter()
    {
        foreach (var datapoint in datapoints)
        {
            DataLogger.Instance.Datapoints.SetValue(datapoint.Key, datapoint.Value);
        }
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}
