using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowResultPhase : Phase
{
    public Image stimulus;

    public AEPsychClient.AEPsychQuery.QueryType queryType;
    
    // Required override
    public override void Enter()
    {
        AEPsychClient.Instance.Query(queryType, QueryResponse);
    }

    private void QueryResponse(AEPsychClient.AEPsychQuery.Message response)
    {
        var color = Color.white;
        color.a = response.x[0];
        stimulus.color = color;
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
