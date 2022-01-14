using System.Collections;
using System.Collections.Generic;
using ExperimentStructures;
using UnityEngine;
using UnityEngine.UI;

public class UpdateText : MonoBehaviour
{
    public Block block;
    public Text trialText;
    public Text repetitionText;
    public Text phaseText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (block.CurrentTrial)
        {
            trialText.text = block.CurrentTrial.name;
            phaseText.text = block.CurrentTrial.CurrentPhase.name;
            repetitionText.text = "Repetition: " + (block.CurrentTrial.CurrentRepetition + 1) + " of " +
                                  block.CurrentTrial.Repetitions;
        }
    }
}
