using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using TMPro;
using UnityEngine.UI;

public class WaitForExplorationPhase : Phase
{
    public TextMeshPro text;
    private float _endTime;

    public BackPlaneButtonManager buttonManager;

    private int _leftPressCount;
    private int _rightPresCount;

    public MeshRenderer leftLED1Renderer;
    public MeshRenderer leftLED2Renderer;
    public MeshRenderer rightLED1Renderer;
    public MeshRenderer rightLED2Renderer;

    public Material ledOff;
    public Material ledOn;

    private bool _pressed;

    public float actualDuration;
    
    // Required override
    public override void Enter()
    {
        text.text = "Press both buttons twice.";
        
        BackPlaneButtonManager.onButtonPressed += OnButtonPressedHandler;
        
        _leftPressCount = 0;
        _rightPresCount = 0;

        leftLED1Renderer.material = ledOff;
        leftLED2Renderer.material = ledOff;
        rightLED1Renderer.material = ledOff;
        rightLED2Renderer.material = ledOff;

        _pressed = false;
    }

    private void OnButtonPressedHandler(BackPlaneButtonManager.Button button)
    {
        if (!_pressed)
        {
            _endTime = Time.time + actualDuration;
            _pressed = true;
        }

        if (button == BackPlaneButtonManager.Button.Left)
        {
            _leftPressCount++;

            if (_leftPressCount == 1)
                leftLED1Renderer.material = ledOn;
            if (_leftPressCount == 2)
                leftLED2Renderer.material = ledOn;
        }

        if (button == BackPlaneButtonManager.Button.Right)
        {
            _rightPresCount++;
            
            if (_rightPresCount == 1)
                rightLED1Renderer.material = ledOn;
            if (_rightPresCount == 2)
                rightLED2Renderer.material = ledOn;
        }
    }
    
    // Required override
    public override void Loop()
    {
        if (_pressed)
            text.text = "Time remaining: " + (_endTime - Time.time).ToString("F1") + " sec.";

        if (_leftPressCount >= 2 && _rightPresCount >= 2)
        {
            ExperimentManager.Instance.RaiseNextPhase();
        }

        if (_pressed && Time.time > _endTime)
        {
            ExperimentManager.Instance.RaiseNextPhase();
        }
    }

    // Required override
    public override void OnExit()
    {
        BackPlaneButtonManager.onButtonPressed -= OnButtonPressedHandler;
    }
}
