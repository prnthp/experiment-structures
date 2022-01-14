using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleAnswerQuestionPhase : Phase
{
    public Text questionText;
    public string question;
    public Button leftButton;
    public Button rightButton;

    private AEPsychTrial _aePsychTrial;
    
    // Required override
    public override void Enter()
    {
        _aePsychTrial = (AEPsychTrial)trial;
        
        questionText.gameObject.SetActive(true);
        questionText.text = question;
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void OnLeftButtonClick()
    {
        // DataLogger.Instance.Datapoints.SetValue("outcome", 0);

        SetOutcome(1);
    }

    private void OnRightButtonClick()
    {
        // DataLogger.Instance.Datapoints.SetValue("outcome", 1);
        
        SetOutcome(0);
    }
    
    // Required override
    public override void Loop()
    {
        if (Input.GetKeyUp(KeyCode.Y))
            SetOutcome(1);
        else if (Input.GetKeyUp(KeyCode.N))
            SetOutcome(0);
    }

    private void SetOutcome(int value)
    {
        _aePsychTrial.outcome = value;
        ExitPhase();
    }

    // Required override
    public override void OnExit()
    {
        questionText.gameObject.SetActive(false);
        
        leftButton.onClick.RemoveListener(OnLeftButtonClick);
        rightButton.onClick.RemoveListener(OnRightButtonClick);
    }
}
