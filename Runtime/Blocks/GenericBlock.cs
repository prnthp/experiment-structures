using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentStructures;
using UnityEngine.Events;

public class GenericBlock : Block
{
    public UnityEvent onBlockStart;

    public UnityEvent onBlockEnd;

    // Optional override
    protected override void OnBlockStart()
    {
        onBlockStart.Invoke();
    }

    // Optional override
    protected override void OnBlockEnd()
    {
        onBlockEnd.Invoke();
    }
}