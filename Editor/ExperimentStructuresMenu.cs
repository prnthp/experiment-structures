using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ExperimentStructures
{
    public static class ExperimentStructureMenu
    {
        [MenuItem("GameObject/Experiment Structures/Experiment Manager", false, 49)]
        public static void AddExperimentManager()
        {
            if (GameObject.FindObjectOfType<ExperimentManager>(true))
            {
                Debug.LogError("[Experiment Structures] Experiment Manager already exists in the Scene. There can only be one Experiment Manager.");
                return;
            }
            
            var go = new GameObject("ExperimentManager");
            go.AddComponent<ExperimentManager>();
            Selection.activeTransform = go.transform;
            Undo.RegisterCreatedObjectUndo(go, "Create Experiment Manager");
        }
        
        [MenuItem("GameObject/Experiment Structures/Data Logger", false, 49)]
        public static void AddDataLogger()
        {
            if (GameObject.FindObjectOfType<DataLogger>(true))
            {
                Debug.LogError("[Experiment Structures] Data Logger already exists in the Scene. There can only be one Data Logger.");
                return;
            }
            
            var go = new GameObject("DataLogger");
            go.AddComponent<DataLogger>();
            Selection.activeTransform = go.transform;
            Undo.RegisterCreatedObjectUndo(go, "Create Data Logger");
        }
        
        [MenuItem("GameObject/Experiment Structures/Blank Block", false, 48)]
        public static void AddBlankBlock()
        {
            var go = new GameObject("BlankBlock");
            go.AddComponent<BlankBlock>();
            
            if (Selection.activeTransform != null)
            {
                go.transform.SetParent(Selection.activeTransform);
            }
            
            Selection.activeTransform = go.transform;
            Undo.RegisterCreatedObjectUndo(go, "Create Block");
        }

        [MenuItem("GameObject/Experiment Structures/Blank Block", true)]
        public static bool ValidateAddBlankBlock()
        {
            return Selection.activeTransform == null 
                   || (!Selection.activeGameObject.TryGetComponent<Block>(out _) 
                       && !Selection.activeGameObject.TryGetComponent<Trial>(out _)
                       && !Selection.activeGameObject.TryGetComponent<Phase>(out _));
        }
        
        [MenuItem("GameObject/Experiment Structures/Blank Trial", false, 47)]
        public static void AddBlankTrial()
        {
            var go = new GameObject("BlankTrial");
            go.AddComponent<BlankTrial>();
            
            if (Selection.activeTransform != null)
            {
                go.transform.SetParent(Selection.activeTransform);
            }
            
            Selection.activeTransform = go.transform;
            Undo.RegisterCreatedObjectUndo(go, "Create Trial");
        }

        [MenuItem("GameObject/Experiment Structures/Blank Trial", true)]
        public static bool ValidateAddBlankTrial()
        {
            return Selection.activeTransform == null 
                   || (!Selection.activeGameObject.TryGetComponent<Trial>(out _)
                       && !Selection.activeGameObject.TryGetComponent<Phase>(out _));
        }
        
        [MenuItem("GameObject/Experiment Structures/Blank Phase", false, 46)]
        public static void AddBlankPhase()
        {
            var go = new GameObject("BlankPhase");
            go.AddComponent<BlankPhase>();
            
            if (Selection.activeTransform != null)
            {
                go.transform.SetParent(Selection.activeTransform);
            }
            
            Selection.activeTransform = go.transform;
            Undo.RegisterCreatedObjectUndo(go, "Create Phase");
        }

        [MenuItem("GameObject/Experiment Structures/Blank Phase", true)]
        public static bool ValidateAddBlankPhase()
        {
            return Selection.activeTransform == null 
                   || (!Selection.activeGameObject.TryGetComponent<Block>(out _)
                       && !Selection.activeGameObject.TryGetComponent<Phase>(out _));
        }

        private static string _directory = "Assets/ScriptTemplates";
        private static string _phaseTemplateFilename = "150-Experiment Structures__Phase-NewPhase.cs.txt";
        private static string _trialTemplateFilename = "151-Experiment Structures__Trial-NewTrial.cs.txt";
        private static string _blockTemplateFilename = "152-Experiment Structures__Block-NewBlock.cs.txt";
        
        [MenuItem("Assets/Create Experiment Structures Templates", priority = 500)]
        public static void CreateScriptTemplates()
        {
            if (!EditorUtility.DisplayDialog("Script Template Creation",
                    "This will create script templates for Blocks, Trials and Phases in your Assets/ScriptTemplates folder.\n\n" +
                    "After this, you can create new Blocks, Trials and Phases in the Project tab.",
                    "Create",
                    "Cancel"
                ))
            {
                return;
            }

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            using (var phaseTemplateFile = new StreamWriter(Path.Combine(_directory, _phaseTemplateFilename), false))
            {
                phaseTemplateFile.Write(_phaseTemplateContents);
            }
            using (var phaseTemplateFile = new StreamWriter(Path.Combine(_directory, _trialTemplateFilename), false))
            {
                phaseTemplateFile.Write(_trialTemplateContents);
            }
            using (var phaseTemplateFile = new StreamWriter(Path.Combine(_directory, _blockTemplateFilename), false))
            {
                phaseTemplateFile.Write(_blockTemplateContents);
            }
            
            AssetDatabase.Refresh();
            Debug.LogWarning("[Experiment Structures] Template Creation Complete. Unity Editor must be restarted before the templates are usable.");
        }

        #region Template Contents
        private static string _phaseTemplateContents =
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class #SCRIPTNAME# : Phase
{
    // Required override
    public override void Enter()
    {
        #NOTRIM#
    }

    // Required override
    public override void Loop()
    {
        #NOTRIM#
    }

    // Required override
    public override void OnExit()
    {
        #NOTRIM#
    }
}
";

        private static string _trialTemplateContents =
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class #SCRIPTNAME# : Trial
{
    // // Optional override
    // protected override void OnTrialBegin()
    // {
    //     
    // }

    // // Optional override
    // protected override void OnNextRepetition()
    // {
    //     
    // }

    // // Optional override
    // protected override void OnTrialComplete()
    // {
    //     
    // }
    }
}";

        private static string _blockTemplateContents =
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class #SCRIPTNAME# : Block
{
    // // Optional override
    // protected override void OnBlockStart()
    // {
    //     
    // }

    // // Optional override
    // protected override void OnBlockEnd()
    // {
    //     
    // }
}";

        #endregion
    }
}
