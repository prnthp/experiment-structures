using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class EnableObjectsPhase : Phase
{
    public List<GameObject> objectsToEnable;

    public override void Enter()
    {
        foreach (var go in objectsToEnable)
            if (go)
                go.SetActive(true);
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}