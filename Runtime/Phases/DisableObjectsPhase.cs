using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class DisableObjectsPhase : Phase
{
    public List<GameObject> objects;

    public override void Enter()
    {
        foreach (var go in objects)
            if (go)
                go.SetActive(false);
    }

    public override void Loop()
    {
    }

    public override void OnExit()
    {
    }
}