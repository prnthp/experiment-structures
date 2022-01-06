using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ExperimentStructures
{
    [CustomEditor(typeof(DataLogger), true)]
    public class DataLoggerEditor : Editor
    {
        private ReorderableList listValues;

        private void OnEnable()
        {
            listValues = new ReorderableList(serializedObject, serializedObject.FindProperty("entries"), true, true,
                true, true);

            AddValuesCallback(listValues, "Datapoints (Key-Value Pairs)");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            EditorGUILayout.Separator();
            listValues.DoLayoutList();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        public static void AddValuesCallback(ReorderableList list, string header)
        {
            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, header); };

            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("key"), GUIContent.none
                    );
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 128, rect.y, rect.width - 128, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("value"), GUIContent.none
                    );
                };

            list.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
            };
        }
    }
}
