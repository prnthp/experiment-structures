using System.Collections;
using System.Collections.Generic;
using ExperimentStructures;
using UnityEngine;

public class BackPlaneButtonManager : MonoBehaviour
{
    public enum Button
    {
        Left, Right
    }
    
    public delegate void OnButtonPressed(Button button);
    public static event OnButtonPressed onButtonPressed;
    public void RaiseOnButtonPressed(Button button)
    {
        onButtonPressed?.Invoke(button);
    }

    public GameObject leftButtonBody;
    public GameObject rightButtonBody;

    public ConfigurableJoint leftButtonJoint;
    public ConfigurableJoint rightButtonJoint;

    public GameObject leftButton;
    public GameObject rightButton;

    public float leftStiffnessDebug;
    public float rightStiffnessDebug;

    private float _leftDebounceTimer;
    private float _rightDebounceTimer;

    public ButtonHaptics leftButtonHaptics;
    public ButtonHaptics rightButtonHaptics;

    private float _leftStiffness;
    private float _rightStiffness;
    private float _leftAmplitude;
    private float _rightAmplitude;
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == leftButtonBody && Time.time > _leftDebounceTimer)
        {
            _leftDebounceTimer = Time.time + 0.5f;
            RaiseOnButtonPressed(Button.Left);
        }

        if (other.gameObject == rightButtonBody && Time.time > _rightDebounceTimer)
        {
            _rightDebounceTimer = Time.time + 0.5f;
            RaiseOnButtonPressed(Button.Right);
        }
    }
    
    public void SetButtonStiffness(float leftStiffness, float rightStiffness)
    {
        _leftStiffness = leftStiffness;
        _rightStiffness = rightStiffness;
        
        JointDrive drive = leftButtonJoint.yDrive;
        drive.positionSpring = leftStiffness;
        leftButtonJoint.yDrive = drive;

        drive = rightButtonJoint.yDrive;
        drive.positionSpring = rightStiffness;
        rightButtonJoint.yDrive = drive;
        
    }

    public void SetButtonHaptics(float leftAmplitude, float rightAmplitude)
    {
        _leftAmplitude = leftAmplitude;
        _rightAmplitude = rightAmplitude;
        
        leftButtonHaptics.notchAmplitude = leftAmplitude;
        rightButtonHaptics.notchAmplitude = rightAmplitude;
        
    }

    public void LogStiffnesses()
    {
        DataLogger.Instance.Datapoints.SetValue("left_stiffness", _leftStiffness);
        DataLogger.Instance.Datapoints.SetValue("right_stiffness", _rightStiffness);
        DataLogger.Instance.Datapoints.SetValue("left_amplitude", _leftAmplitude);
        DataLogger.Instance.Datapoints.SetValue("right_amplitude", _rightAmplitude);
    }
    
    public void SetButtonStiffness()
    {
        SetButtonStiffness(leftStiffnessDebug, rightStiffnessDebug);
    }
    
    public void ShowButtons()
    {
        leftButton.SetActive(true);
        rightButton.SetActive(true);
    }
    
    public void HideButtons()
    {
        leftButton.SetActive(false);
        rightButton.SetActive(false);
    }

    public void HideLeftButton()
    {
        leftButton.SetActive(false);
    }

    public void HideRightButton()
    {
        rightButton.SetActive(false);
    }
}
