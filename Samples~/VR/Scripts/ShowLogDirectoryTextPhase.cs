using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using TMPro;

public class ShowLogDirectoryTextPhase : Phase
{
    public TextMeshPro text;
    
    // Required override
    public override void Enter()
    {
        text.text = "Done!\nExperiment logs are available at: '" + DataLogger.Instance.currentFilePath + "'";
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
