using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Rotorz.Games.Reflection;
using System.Reflection;
#endif

namespace ExperimentStructures
{
    /// <summary>
    ///  A "phase" is similar to a state in a state machine, you 'enter' a phase,
    /// stay in it, and then finally 'exit' it. A phase can either be time dependent
    /// or not.
    /// </summary>
    [DefaultExecutionOrder(-700)]
    public abstract class Phase : Structure
    {
        [SerializeField] public bool onlyOnFirstRepetition;

        public float StartTime { get; private set; }
        public float EndTime { get; private set; }

        [Tooltip(
            "A negative duration will require the 'RaiseNextPhase' event to be triggered."
            + "\nZero equates to a single Unity frame of execution.")]
        [SerializeField]
        protected float duration = float.NegativeInfinity;

        private bool _timeLimited = true;
        public bool Alive { get; private set; }
        [HideInInspector] public Trial trial;

        private void Start()
        {
            if (trial == null)
            {
                Debug.LogWarning(
                    $"[Experiment Structures] Orphaned Phase {name}. Did you forget to put me under a Trial?");
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Setup the phase.
        /// </summary>
        public void _Enter()
        {
            // Subscribe to nextPhase event
            ExperimentManager.Instance.nextPhase += Exit;
            // Publish start of Phase
            ExperimentManager.Instance.RaiseStartPhase();
            if (duration < 0)
            {
                _timeLimited = false;
            }
            else
            {
                StartTime = Time.time;
                EndTime = StartTime + duration;
            }

            Alive = true;

            if (onlyOnFirstRepetition && trial.CurrentRepetition > 0)
            {
                _timeLimited = true;
                EndTime = StartTime;
                return;
            }

            Enter(); // Run user implemented Enter code
        }

        public abstract void Enter();


        public void _Loop()
        {
            if (!Alive) return;

            if (_timeLimited & (Time.time >= EndTime)) Exit();

            if (onlyOnFirstRepetition && trial.CurrentRepetition > 0) return;
            Loop(); // Run user implemented Loop code
        }

        /// <summary>
        /// Anything that would go to the Update() method should go here
        /// </summary>
        public abstract void Loop();

        /// <summary>
        /// Exit the phase.
        /// </summary>
        protected void Exit()
        {
            ExperimentManager.Instance.nextPhase -= Exit;

            if (onlyOnFirstRepetition && trial.CurrentRepetition > 0)
            {
                Alive = false;
                trial.PhaseComplete(this);
                return;
            }

            OnExit(); // Run user implemented Exit code
            Alive = false;
            trial.PhaseComplete(this);
        }

        public abstract void OnExit();

        public float Duration => duration;

        public void SetDuration(float newDuration)
        {
            duration = newDuration;
        }

        [SerializeField] [HideInInspector] [ClassExtends(typeof(Phase))]
        internal ClassTypeReference replacePhaseWith;

        [SerializeField] [HideInInspector] [ClassExtends(typeof(Phase))]
        internal ClassTypeReference addPhase;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Phase), true)]
    public class PhaseEditor : Editor
    {
        private static bool _showUtilities;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var phase = (Phase)target;

            EditorGUILayout.Space(10);
            _showUtilities = EditorGUILayout.Foldout(_showUtilities, "Utilities", EditorStyles.foldoutHeader);
            if (_showUtilities)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Replace Phase", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("replacePhaseWith"));

                serializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Replace", GUILayout.Width(80f)))
                {
                    var newPhaseReference = phase.replacePhaseWith;

                    if (newPhaseReference.Type != null)
                    {
                        Undo.IncrementCurrentGroup();
                        Undo.SetCurrentGroupName("Replace Phase");
                        var undoGroupIndex = Undo.GetCurrentGroup();

                        var go = phase.gameObject;
                        var oldDuration = phase.Duration;
                        var oldOnlyOnFirstRepetition = phase.onlyOnFirstRepetition;

                        Undo.DestroyObjectImmediate(phase.gameObject.GetComponent<Phase>());

                        var newPhase = (Phase)Undo.AddComponent(go, newPhaseReference.Type);
                        newPhase.SetDuration(oldDuration);
                        newPhase.onlyOnFirstRepetition = oldOnlyOnFirstRepetition;
                        newPhase.name = newPhaseReference.Type.Name;
                        Undo.CollapseUndoOperations(undoGroupIndex);

                        return;
                    }
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Add Phase to End of Trial", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("addPhase"));

                serializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Add", GUILayout.Width(80f)))
                {
                    var newPhaseReference = phase.addPhase;

                    if (newPhaseReference.Type != null)
                    {
                        Undo.IncrementCurrentGroup();
                        Undo.SetCurrentGroupName("Add Phase");
                        var undoGroupIndex = Undo.GetCurrentGroup();

                        var go = new GameObject(newPhaseReference.Type.Name);
                        go.AddComponent(newPhaseReference.Type);
                        go.transform.SetParent(phase.transform.parent);

                        Undo.RegisterCreatedObjectUndo(go, "");

                        Undo.CollapseUndoOperations(undoGroupIndex);

                        Selection.activeTransform = go.transform;
                        phase.addPhase = null;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        #region Update Icon

        private static MethodInfo CopyMonoScriptIconToImporters =
            typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters",
                BindingFlags.Static | BindingFlags.NonPublic);

        private static MethodInfo SetIconForObject =
            typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);

        private static MethodInfo GetIconForObject =
            typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);

        private void Reset()
        {
            if (GetIconForObject.Invoke(null, new[] { target }) != null) return;

            Debug.LogWarning("Assigning icon to new Phase. Reimporting Asset.");

            var icon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    AssetDatabase.GUIDToAssetPath("c1366d142351f43f480a75df48ffd4ad"));
            SetIconForObject.Invoke(null, new[] { target, icon });
            var behaviour = target as MonoBehaviour;
            var script = MonoScript.FromMonoBehaviour(behaviour);
            CopyMonoScriptIconToImporters.Invoke(null, new[] { script });
        }

        #endregion
    }
#endif
}