using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class ShuffleTrialsPhase : Phase
{
    public Block block; 
    
    // Required override
    public override void Enter()
    {
        block.ShuffleTrials(false);
    }

    // Required override
    public override void Loop()
    {

    }

    // Required override
    public override void OnExit()
    {

    }
}
