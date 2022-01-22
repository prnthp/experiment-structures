using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.UI;

public class AEPsychSampleShowResultPhase : Phase
{
    public Image stimulus;

    public bool isDetectionThreshold = false;

    // Required override
    public override void Enter()
    {
        var query = new AEPsychClient.AEPsychQuery(AEPsychClient.AEPsychQuery.QueryType.max);
        
        if (isDetectionThreshold)
        {
            query.message.query_type = AEPsychClient.AEPsychQuery.QueryType.inverse;
            query.message.y = 0.75f; // Common detection threshold
            query.message.probability_space = true;
        }

        AEPsychClient.Instance.Query(query, QueryResponse);
    }

    private void QueryResponse(AEPsychClient.AEPsychQuery.Message response)
    {
        var color = Color.white;
        color.a = response.x["alpha"][0];
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
