using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowResult2DPhase : Phase
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
        stimulus.color = new Color(
            response.x[0],
            0.2f,
            response.x[1]
        );
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
