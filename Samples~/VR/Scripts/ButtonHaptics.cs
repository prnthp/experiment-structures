using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHaptics : MonoBehaviour
{
    public Transform buttonBase;
    public float notchDistance;
    private float _nextNotchDistance;

    public float notchFrequency;
    public float notchAmplitude;

    public OVRInput.Controller controller;

    private bool _pressing;

    public bool hapticsOn;

    private void FixedUpdate()
    {
        var distance = Vector3.Distance(buttonBase.position, transform.position);

        if (distance > _nextNotchDistance)
        {
            StartCoroutine(PlayHaptics(notchFrequency, notchAmplitude, 0.050f));
            _nextNotchDistance = distance + notchDistance;
        }

        if (distance < notchDistance)
        {
            _nextNotchDistance = notchDistance;
        }
    }

    private WaitForSeconds _waitForSeconds;
    
    IEnumerator PlayHaptics(float frequency, float amplitude, float duration)
    {
        _waitForSeconds = new WaitForSeconds(duration);
        // OVRInput.SetControllerVibration(0f, 0f, controller);
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        hapticsOn = true;
        yield return _waitForSeconds;
        OVRInput.SetControllerVibration(0f, 0f, controller);
        hapticsOn = false;
    }
}
