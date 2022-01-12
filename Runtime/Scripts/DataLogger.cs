using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Events;

namespace ExperimentStructures
{
    [DisallowMultipleComponent]
    public class DataLogger : MonoBehaviour
    {
        public DatapointPairs Datapoints { get; private set; }
        
        public List<string> keys;

        private static DataLogger _instance;

        public static DataLogger Instance
        {
            get
            {
                if (!FindObjectOfType<DataLogger>())
                {
                    var go = new GameObject("DataLogger");
                    _instance = go.AddComponent<DataLogger>();
                    return _instance;
                }

                return _instance;
            }
        }

        private string _header;
        private FileStream _fileStream;
        private TextWriter _textWriter;

        private string _currentFilePath = "test.csv";
        private string _currentFileName = "";

        private bool _loggingActive;

        [Space] [Help("Logs will always be appended with yyyy-MM-dd-HH-mm-ss", MessageType.None)]
        public string defaultFileName = "";

        [Space]
        [Help(
            "Leave this blank to use Application.persistentDataPath\nThe application will attempt to create a new directory if not found.\nMobile builds will require skipping path validation.",
            MessageType.None)]
        public string customPath = "";

        [Space]
        [Help(
            "Skipping path validation is useful for mobile builds where you already know you have been granted write access.",
            MessageType.None)]
        public bool skipMobilePathValidation = false;

        [Space] public UnityEvent onStartLogging;

        [Tooltip("Passes (full path + filename, filename) of the latest log. Useful for queuing up uploads to AWS S3 etc.")]
        public UnityEvent<string, string> onEndLogging;

        [Space] [Help("This is simply what is fed into the ToString(floatFormat) method.", MessageType.None)]
        public string floatFormat = "E5";

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _loggingActive = false;

            Debug.Log("[Experiment Structures] Logs are saved to: " + Path.GetFullPath(ValidatedPathPrepend()));
        }

        private void OnDestroy()
        {
            if (_loggingActive) EndLogging();
        }

        public void StartLogging(string fileName)
        {
            if (_loggingActive)
            {
                Debug.Log("[Experiment Structures] Started logging already, closing old file and opening new one.");
                EndLogging();
            }
            
            _currentFileName = (fileName == "" ? "" : fileName + "-") +
                               DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";

            _currentFilePath = ValidatedPathPrepend() + "/" + _currentFileName;

            try
            {
                CreateNewLogFile(_currentFilePath);
            }
            catch (Exception)
            {
                Debug.LogError($"[Experiment Structures] Could not write to '{Path.GetFullPath(_currentFilePath)}'.");
                _currentFilePath = Application.persistentDataPath + "/" + _currentFileName;
                CreateNewLogFile(_currentFilePath);
            }

            Debug.Log($"[Experiment Structures] Logging experiment to '{Path.GetFullPath(_currentFilePath)}'");

            Datapoints = new DatapointPairs();

            if (keys != null)
            {
                foreach (var key in keys)
                {
                    Datapoints.AddValue(key, "");
                }
            }

            _header = "";
            _header += "time,";

            foreach (var key in Datapoints.Headers) _header += key + ",";

            _header = _header.Remove(_header.Length - 1);

            _loggingActive = true;
            
            onStartLogging?.Invoke();
        }

        private void CreateNewLogFile(string path)
        {
            _currentFilePath = path;
            _fileStream = new FileStream(_currentFilePath, FileMode.OpenOrCreate);
            _textWriter = new StreamWriter(_fileStream, System.Text.Encoding.UTF8, 1024, true);
            _textWriter.WriteLine(_header);
            _textWriter.Flush();
        }

        private string ValidatedPathPrepend()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!skipPathValidation && !Application.isEditor)
            {
                if (customPath != "")
                {
                    Debug.LogError(
                        $"[Experiment Structures] Mobile platforms unlikely to support system paths, using using Application.persistentDataPath instead.");
                }

                return Application.persistentDataPath;
            }
#endif

            var pathPrepend = customPath == "" ? Application.persistentDataPath : customPath;

            if (!Directory.Exists(pathPrepend))
                try
                {
                    Directory.CreateDirectory(pathPrepend);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.LogError(
                        $"[Experiment Structures] Could not find or create directory '{Path.GetFullPath(pathPrepend)}', using Application.persistentDataPath instead.");
                    return Application.persistentDataPath;
                }
                catch (Exception)
                {
                    Debug.LogError(
                        $"[Experiment Structures] Could not find or create directory '{Path.GetFullPath(pathPrepend)}', using Application.persistentDataPath instead.");
                    return Application.persistentDataPath;
                }

            return pathPrepend;
        }

        public void EndLogging()
        {
            if (!_loggingActive)
            {
                Debug.LogWarning("[Experiment Structures] No logging active.");
                return;
            }

            _textWriter.Close();
            _fileStream.Close();

            onEndLogging?.Invoke(_currentFilePath, _currentFileName);

            _loggingActive = false;
        }

        public void LogState()
        {
            if (!_loggingActive)
            {
                Debug.Log("[Experiment Structures] No logging active. Creating new file.");
                StartLogging(defaultFileName);
            }

            var output = "";
            output += Time.time.ToString(floatFormat) + ",";
            foreach (var value in Datapoints.Data) output += value + ",";

            output = output.Remove(output.Length - 1);
            _textWriter.WriteLine(output);
            _textWriter.Flush();
        }
        
        public void SetDataPoint(string input)
        {
            var data = input.Split(',');
            if (data.Length != 2)
            {
                Debug.LogError("[Experiment Structures] Invalid datapoint format, must be key,value");
                return;
            }
            
            Datapoints.SetValue(data[0].Trim(), data[1].Trim());
        }

        private float _startTime;

        public void StartTimer()
        {
            _startTime = Time.time;
        }

        public float StopTimer()
        {
            return Time.time - _startTime;
        }

        public void LogTimer(string key)
        {
            Datapoints.SetValue(key, StopTimer());
        }

        private void OnValidate()
        {
            try
            {
                _ = 69.420f.ToString(floatFormat);
            }
            catch (FormatException)
            {
                Debug.LogError("[DataLogger] Invalid format specified. Examples: E5, F3, ##.###");
            }
        }

        /// <summary>
        /// Thin wrapper of a Dictionary
        /// </summary>
        [Serializable]
        public class DatapointPairs
        {
            private Dictionary<string, string> _dict;

            public DatapointPairs()
            {
                _dict = new Dictionary<string, string>();
            }

            /// <summary>
            /// Adds the value with <paramref name="name" /> and setting the value to <paramref name="defaultValue"/>.
            /// </summary>
            /// <param name="name">Name of value.</param>
            /// <param name="defaultValue">Default value.</param>
            public void AddValue(string name, string defaultValue)
            {
                AddValueToDict(name, defaultValue);
            }

            /// <summary>
            /// Adds the value with <paramref name="name"/> and sets the value to zero (0.0f)
            /// </summary>
            /// <param name="name">Name of value.</param>
            public void AddValue(string name)
            {
                AddValueToDict(name, "");
            }

            public string GetValue(string name)
            {
                return _dict[name];
            }

            public bool SetValue(string name, float value)
            {
                return SetValue(name, value.ToString("E5"));
            }

            public bool SetValue(string name, int value)
            {
                return SetValue(name, value.ToString());
            }

            public bool SetValue(string name, bool value)
            {
                return SetValue(name, value ? "TRUE" : "FALSE");
            }

            /// <summary>
            /// Sets the value with given <paramref name="name"/> and sets the value to <paramref name="value"/>
            /// </summary>
            /// <param name="name">Name of value</param>
            /// <param name="value">Value of value</param>
            public bool SetValue(string name, string value)
            {
                if (_dict.ContainsKey(name))
                {
                    _dict[name] = value;
                    return true;
                }
                else
                {
                    Debug.LogError(
                        $"[Experiment Structures][DatapointPairs] Datapoint with Key:{name} does not exist.");
                    return false;
                }
            }

            private bool AddValueToDict(string check, string value)
            {
                if (!_dict.ContainsKey(check))
                {
                    _dict.Add(check, value);
                    return true;
                }

                Debug.LogWarning($"[Experiment Structures][DatapointPairs] Datapoint with Key {check} already exists.");
                return false;
            }

            private List<string> headers = new List<string>();

            public List<string> Headers
            {
                get
                {
                    headers.Clear();

                    foreach (var pair in _dict) headers.Add(pair.Key);

                    return headers;
                }
            }

            private List<string> data = new List<string>();

            public List<string> Data
            {
                get
                {
                    data.Clear();

                    foreach (var pair in _dict) data.Add(pair.Value);

                    return data;
                }
            }
        }
    }
}