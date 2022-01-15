// MIT License

// Copyright (c) 2021 Jom Preechayasomboon

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


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
    [DefaultExecutionOrder(-800)]
    public abstract class Phase : Structure
    {
        [SerializeField] public bool onlyOnFirstRepetition;

        public float StartTime { get; private set; }
        public float EndTime { get; private set; }

        [Tooltip(
            "A negative duration will require the 'RaiseNextPhase' event to be triggered."
            + "\nZero is a single frame that goes through Enter, Loop, OnExit, but Unity events are not guaranteed."
            + "\nSet the GuaranteeUnityFrameCycle property to ensure Unity's Update is called.")]
        [SerializeField]
        protected float duration = float.NegativeInfinity;

        private bool _timeLimited = true;

        /// <summary>
        /// If you are using OnEnable(), Update(), OnDisable() or Unity events like OnCollisionEnter(), you should
        /// enable this. This will guarantee that at least 1 frame is completed.
        /// </summary>
        public bool GuaranteeUnityFrameCycle { get; set; } = false;

        private int _completedUnityCycle = 0;

        public bool Alive { get; private set; }
        
        [HideInInspector] public Trial trial;

        private void Awake()
        {
            if (trial == null)
            {
                Debug.LogWarning(
                    $"[Experiment Structures] Orphaned Phase {name}. Did you forget to put me under a Trial?");
                gameObject.SetActive(false);
            }
        }

        internal void _Enter()
        {
            _completedUnityCycle = 0;
            
            if (onlyOnFirstRepetition && trial.CurrentRepetition > 0)
            {
                _timeLimited = true;
                EndTime = StartTime;
                Exit();
                return;
            }
            
            Enter(); // Run user implemented Enter code
            
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
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Setup the phase.
        /// </summary>
        public abstract void Enter();

        internal void _Loop()
        {
            if (!Alive) return;
            
            if (_timeLimited & (Time.time >= EndTime))
            {
                if (GuaranteeUnityFrameCycle && _completedUnityCycle < 2)
                {
                    _completedUnityCycle++;
                }
                else
                {
                    Loop(); // Run user implemented Loop code
                    Exit();
                    return;
                }
            }
            
            Loop(); // Run user implemented Loop code
        }

        /// <summary>
        /// Anything that would go to the Update() method should go here
        /// </summary>
        public abstract void Loop();

        private void Exit()
        {
            ExperimentManager.Instance.nextPhase -= Exit;

            if (onlyOnFirstRepetition && trial.CurrentRepetition > 0)
            {
                Alive = false;
                trial.PhaseComplete(this);
                return; // Skipping any state-machine methods
            }
            
            Alive = false;
            gameObject.SetActive(false);
            trial.PhaseComplete(this);
            
            OnExit(); // Run user implemented Exit code
        }

        /// <summary>
        /// OnExit() is called once before completing the Phase
        /// </summary>
        public abstract void OnExit();

        protected static void ExitPhase()
        {
            ExperimentManager.Instance.RaiseNextPhase();
        }
        
        public float Duration => duration;

        public void SetDuration(float newDuration)
        {
            duration = newDuration;
        }
        
#if UNITY_EDITOR
#pragma warning disable CS0649
        // Disable null warnings for Unity < 2020
        [SerializeField] [HideInInspector] [ClassExtends(typeof(Phase))]
        internal ClassTypeReference replacePhaseWith;

        [SerializeField] [HideInInspector] [ClassExtends(typeof(Phase))]
        internal ClassTypeReference addPhase;
#pragma warning restore CS0649
#endif
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

                        var oldSo = new SerializedObject(phase);

                        Undo.DestroyObjectImmediate(phase.gameObject.GetComponent<Phase>());

                        var newPhase = (Phase)Undo.AddComponent(go, newPhaseReference.Type);
                        newPhase.SetDuration(oldDuration);
                        newPhase.onlyOnFirstRepetition = oldOnlyOnFirstRepetition;
                        newPhase.name = newPhaseReference.Type.Name;

                        var so = new SerializedObject(newPhase);
                        var sp = so.GetIterator();

                        if (sp.NextVisible(true)) // First element is always m_script, which we should not replace
                        {
                            while (sp.NextVisible(true))
                            {
                                var oldProperty = oldSo.FindProperty(sp.propertyPath);
                                if (oldProperty != null && (oldProperty.propertyType == sp.propertyType))
                                {
                                    so.CopyFromSerializedProperty(oldProperty);
                                }
                            }
                            so.ApplyModifiedPropertiesWithoutUndo();
                        }

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

            Debug.LogWarning("[Experiment Structures] Assigning icon to new Phase. Reimporting Asset. Please wait.");

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