using UnityEngine;
using UnityEditor;

namespace ExperimentStructures
{
    public class ExperimentControllerPanel : EditorWindow
    {
        [MenuItem("Window/Experiment Controller")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ExperimentControllerPanel window = (ExperimentControllerPanel)EditorWindow.GetWindow(typeof(ExperimentControllerPanel));
            Vector2 minSize = window.minSize;
            minSize.y = 20;
            window.minSize = minSize;
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Controller", EditorStyles.boldLabel);

            if (GUILayout.Button("Force Next Phase", GUILayout.Height(50)))
            {
                ExperimentManager.Instance.RaiseNextPhase();
            }
        }
    }

}