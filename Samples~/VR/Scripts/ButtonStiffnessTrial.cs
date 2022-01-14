using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;

public class ButtonStiffnessTrial : Trial
{
    public float referenceStiffness;
    public float referenceHapticAmplitude;
    public List<float> stiffnesses;
    public List<float> hapticAmplitudes;
    public int repetitionsPerStiffness;

    private List<float> _stiffnessLookup;
    private List<float> _hapticAmplitudeLookup;
    [SerializeField] private List<int> _sideLookup;
    private List<int> _repetitionIdx;

    public float currentStiffness => _stiffnessLookup[_repetitionIdx[CurrentRepetition]];
    public float currentHapticAmplitude => _hapticAmplitudeLookup[_repetitionIdx[CurrentRepetition]];
    public int currentSide => _sideLookup[_repetitionIdx[CurrentRepetition]];
    
    public bool haptics;
    public bool visual;
    public bool warmUp;

    // // Optional override
    protected override void OnTrialBegin()
    {
        if (hapticAmplitudes.Count != stiffnesses.Count)
        {
            Debug.LogError("Amount of amplitudes and stiffness must be equal.");
        }
        
        _stiffnessLookup = new List<float>();
        _sideLookup = new List<int>();
        _hapticAmplitudeLookup = new List<float>();
        _repetitionIdx = new List<int>();
        
        foreach (var stiffness in stiffnesses)
        {
            for (var i = 0; i < repetitionsPerStiffness; i++)
            {
                _stiffnessLookup.Add(stiffness);
                _sideLookup.Add(i % 2);
            }
        }
        
        foreach (var amplitude in hapticAmplitudes)
        {
            for (var i = 0; i < repetitionsPerStiffness; i++)
            {
                _hapticAmplitudeLookup.Add(amplitude);
            }
        }

        repetitions = stiffnesses.Count * repetitionsPerStiffness;
        
        for (var i = 0; i < repetitions; i++)
        {
            _repetitionIdx.Add(i);
        }
        
        _repetitionIdx.Shuffle();
    }

    // // Optional override
    protected override void OnNextRepetition()
    {
        if (warmUp)
        {
            DataLogger.Instance.Datapoints.SetValue("trial", "warmup");
        }
        else
        {
            if (visual && !haptics)
            {
                DataLogger.Instance.Datapoints.SetValue("trial", "visualOnly");
            }

            if (haptics && !visual)
            {
                DataLogger.Instance.Datapoints.SetValue("trial", "hapticsOnly");
            }

            if (haptics && visual)
            {
                DataLogger.Instance.Datapoints.SetValue("trial", "visualHaptics");
            }
        }

        DataLogger.Instance.Datapoints.SetValue("repetition", CurrentRepetition);
    }

    // // Optional override
    // protected override void OnTrialComplete()
    // {
    //     
    // }
}