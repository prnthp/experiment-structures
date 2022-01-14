using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class RotateObjectPhase : Phase
{
    public Transform objectToRotate;
    public float speed;
    
    // Required override
    public override void Enter()
    {
        
    }

    // Required override
    public override void Loop()
    {
        objectToRotate.Rotate(Vector3.up + Vector3.forward, Time.deltaTime * speed);
    }

    // Required override
    public override void OnExit()
    {
        
    }
}
