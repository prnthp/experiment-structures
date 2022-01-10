using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class EnableObjectsPhase : Phase
{
    public List<GameObject> objects;

    public override void Enter()
    {
        foreach (var go in objects)
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