using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using TrialConfig = GenericDictionary<string, System.Collections.Generic.List<float>>;

public class AEPsychTellPhase : Phase
{
    // Required override
    public override void Enter()
    {
        var aePsychTrial = (AEPsychTrial)trial;
        
        if (!AEPsychClient.Instance.TellOutcome(new AEPsychClient.AePsychTellRequest(aePsychTrial.config, aePsychTrial.outcome), TellCallbackHandler))
        {
            Debug.LogError("[AEPsych] Invalid State");
        }
    }

    private void TellCallbackHandler(string response)
    {
        // AEPsych just responds with "acq". ¿qué? Did you mean "ack"?
        if (response != "acq")
        {
            Debug.LogWarning($"[AEPsych] Response from AEPsych server was \"{ response }\", expected \"acq\"");
        }
        ExitPhase();
    }

    // Required override
    public override void Loop()
    {
        
    }

    // Required override
    public override void OnExit()
    {
        
    }
}
