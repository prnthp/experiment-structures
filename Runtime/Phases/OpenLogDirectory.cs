using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ExperimentStructures;

public class OpenLogDirectory : Phase
{
    public string path;

    public void SetPath(string fullPath, string _)
    {
        // Deprecated
        // path = fullPath;
    }

    // Required override
    public override void Enter()
    {
        
    }

    // Required override
    public override void Loop()
    {

    }

    // Required override
    public override void OnExit()
    {
        path = DataLogger.Instance.currentFilePath;
        OpenInFileBrowser.Open(path);
    }
}
