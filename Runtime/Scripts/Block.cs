using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor.Callbacks;
#endif

namespace ExperimentStructures
{
    /// <summary>
    /// A block can have multiple Trials. Similar to a trial, it switches through
    /// its child components, however, a block will completely disable the GameObject
    /// instead of using its own implementation.
    /// </summary>
    [DefaultExecutionOrder(-802)]
    public abstract class Block : Structure
    {
        [SerializeField] [HideInInspector] public List<Trial> trials;

        [SerializeField] [HideInInspector] private int currentTrialNum;

        [SerializeField] [HideInInspector] private GameObject currentTrialGameObject;

        [SerializeField] [HideInInspector] private GameObject currentPhaseGameObject;

        [SerializeField] public List<TrialCollection> trialsToShuffle;

        [System.Serializable]
        public class TrialCollection
        {
            public List<Trial> trials;
        }

        public int GetCurrentTrialNum => currentTrialNum;

        public Trial CurrentTrial => trials[currentTrialNum];

        private bool startedOnce;

        public void DebugShuffleTrials()
        {
            ShuffleTrials(true, 1);
        }

        /// <summary>
        /// Shuffle the trials as listed in the TrialCollections.
        /// Compared to shuffling cards, each "collection" is a single card.
        /// Hence the trials in each collection will maintain their order within the collection.
        /// To get repeatable results, set Unity's Random seed.
        /// </summary>
        /// <param name="clocked">Rotate the trials in a circle, otherwise randomly shuffle.</param>
        /// <param name="times">Amount of times to clock</param>
        public void ShuffleTrials(bool clocked, int times = 1)
        {
            var targetIdx = new List<int>();
            var shuffledCollection = new List<TrialCollection>(trialsToShuffle);

            foreach (var trialCollection in trialsToShuffle) targetIdx.Add(trials.IndexOf(trialCollection.trials[0]));

            if (clocked)
                shuffledCollection.Clock(times);
            else
                shuffledCollection.Shuffle();

            for (var i = 0; i < shuffledCollection.Count; i++) trials[targetIdx[i]] = shuffledCollection[i].trials[0];

            foreach (var trialCollection in shuffledCollection)
            {
                for (var i = 1; i < trialCollection.trials.Count; i++) trials.Remove(trialCollection.trials[i]);

                var targetTrialIdx = trials.IndexOf(trialCollection.trials[0]);

                for (var i = trialCollection.trials.Count - 1; i > 0; i--)
                    trials.Insert(targetTrialIdx + 1, trialCollection.trials[i]);
            }
        }

        private void Awake()
        {
            startedOnce = false;
        }

        private void Start()
        {
            if (!startedOnce)
            {
                trials = new List<Trial>(GetComponentsInChildren<Trial>(true));
                trials.RemoveAll(CheckTrialDisabled);
            }

            startedOnce = true;

            foreach (var trial in trials)
            {
                trial.gameObject.SetActive(false);
                trial.block = this;

                if (trial.GetComponentsInChildren<Trial>().Length > 1)
                    Debug.LogError(
                        "[Experiment Structures] Nesting trials is not supported and will result in undefined behavior");
            }

            currentTrialNum = 0;
            OnBlockStart();
            StartTrial(CurrentTrial);
        }

        protected virtual void OnBlockStart()
        {
        }

        protected virtual void OnBlockEnd()
        {
        }

        /// <summary>
        /// TODO: Add warning that OnTrialComplete will not be called
        /// </summary>
        /// <param name="trial"></param>
        public void SkipToTrial(Trial trial)
        {
            TrialComplete(CurrentTrial, false);
            
            currentTrialNum = trials.IndexOf(trial);
            
            StartTrial(trial);
        }

        public void TrialComplete(Trial trial, bool startNextTrial = true)
        {
            if (trial == CurrentTrial)
                trial.gameObject.SetActive(false);
            else
                Debug.LogError("[Experiment Structures] Invalid trial completion invocation from " + trial.name);

            if (!startNextTrial)
            {
                return;
            }
            
            currentTrialNum++;
            if (currentTrialNum >= trials.Count)
            {
                OnBlockEnd();
                Debug.Log($"[Experiment Structures] Block {name} Completed");
                return;
            }

            StartTrial(CurrentTrial);
        }

        public void PhaseComplete(Phase phase)
        {
        }

        private void StartTrial(Trial trial)
        {
            trial.gameObject.SetActive(true);
            trial.StartTrial();
            currentTrialGameObject = trial.gameObject;
            currentPhaseGameObject = trial.CurrentPhase.gameObject;
        }

        public void Restart()
        {
            Start();
        }

        private static bool CheckTrialDisabled(Trial trial)
        {
            return !trial.gameObject.activeSelf;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Block), true)]
    public class BlockEditor : Editor
    {
        private static bool _showUtilities;
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Monitor", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTrialGameObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentPhaseGameObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trials"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();
            DrawDefaultInspector();

            var block = (Block)target;

            EditorGUILayout.Separator();
            _showUtilities = EditorGUILayout.Foldout(_showUtilities, "Utilities", EditorStyles.foldoutHeader);
            if (_showUtilities)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Block Overview", EditorStyles.boldLabel);

                var trials = block.GetComponentsInChildren<Trial>(true);

                foreach (var trial in trials)
                {
                    EditorGUILayout.Separator();
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.BeginHorizontal();
                    var repetitions = EditorGUILayout.IntField(trial.name + " (reps)", trial.Repetitions, EditorStyles.miniTextField);
                    if (repetitions != trial.Repetitions)
                    {
                        Undo.RegisterCompleteObjectUndo(trial, "Change Trial Repetitions");
                        trial.Repetitions = repetitions;
                    }
                    EditorGUILayout.EndHorizontal();
                    var phases = trial.GetComponentsInChildren<Phase>(true);
                    EditorGUI.indentLevel = 1;
                    foreach (var phase in phases)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var duration = EditorGUILayout.FloatField(phase.name, phase.Duration);
                        if (GUILayout.Button("∞", GUILayout.Width(32f))) duration = Mathf.NegativeInfinity;
                        if (GUILayout.Button("0", GUILayout.Width(32f))) duration = 0f;
                        if (duration != phase.Duration)
                        {
                            Undo.RegisterCompleteObjectUndo(phase, "Change Phase Duration");
                            phase.SetDuration(duration);
                        }

                        var onlyOnFirstRepetition =
                            GUILayout.Toggle(phase.onlyOnFirstRepetition, "1ˢᵗ", GUILayout.Width(32f));
                        if (onlyOnFirstRepetition != phase.onlyOnFirstRepetition)
                        {
                            Undo.RegisterCompleteObjectUndo(phase, "Change Phase Properties");
                            phase.onlyOnFirstRepetition = onlyOnFirstRepetition;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel = 0;
                }


                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Overrides", EditorStyles.boldLabel);
                if (GUILayout.Button("(Debug) Shuffle Trials")) block.DebugShuffleTrials();

                if (GUILayout.Button("Restart")) block.Restart();

                if (GUILayout.Button("Force Next Phase")) ExperimentManager.Instance.RaiseNextPhase();
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

            Debug.LogWarning("[Experiment Structures] Assigning icon to new Block. Reimporting Asset. Please wait.");

            var icon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    AssetDatabase.GUIDToAssetPath("b9d2e55155a5f4c8c88c5a051e1ffd51"));
            SetIconForObject.Invoke(null, new[] { target, icon });
            var behaviour = target as MonoBehaviour;
            var script = MonoScript.FromMonoBehaviour(behaviour);
            CopyMonoScriptIconToImporters.Invoke(null, new[] { script });
        }

        #endregion
    }
#endif
}