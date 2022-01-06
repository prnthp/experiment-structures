using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExperimentStructures
{
    [DisallowMultipleComponent]
    public class ExperimentManager : MonoBehaviour
    {
        private static ExperimentManager _instance;

        public static ExperimentManager Instance
        {
            get
            {
                if (!FindObjectOfType<ExperimentManager>())
                {
                    var go = new GameObject("ExperimentManager");
                    _instance = go.AddComponent<ExperimentManager>();
                    return _instance;
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        public delegate void NextPhase();

        public event NextPhase nextPhase;

        public void RaiseNextPhase()
        {
            nextPhase?.Invoke();
        }

        public delegate void StartPhase();

        public event StartPhase startPhase;

        public void RaiseStartPhase()
        {
            startPhase?.Invoke();
        }

        public void ForceNextPhase()
        {
            RaiseNextPhase();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ExperimentManager), true)]
    public class ExperimentManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Force Next Phase")) ((ExperimentManager)target).ForceNextPhase();
        }
    }
#endif
}