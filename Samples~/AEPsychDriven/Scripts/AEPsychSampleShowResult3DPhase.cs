using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowResult3DPhase : Phase
{
    public Image stimulus;
    
    // Required override
    public override void Enter()
    {
        var query = new AEPsychClient.AEPsychQuery(AEPsychClient.AEPsychQuery.QueryType.max);
        
        AEPsychClient.Instance.Query(query, QueryResponse);
    }

    private void QueryResponse(AEPsychClient.AEPsychQuery.Message response)
    {
        stimulus.color = new Color(
            response.x["R"][0],
            response.x["G"][0],
            response.x["B"][0]
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
