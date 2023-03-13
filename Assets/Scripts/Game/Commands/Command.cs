using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
public abstract class Command
{
    protected Func<bool> conditionFunction;
    public abstract void Execute();

    // Insures that conditionFunction will always be set
    protected Command(Func<bool> conditionFunc)
    {
        conditionFunction = conditionFunc;
    }

    public bool ShouldExecute()
    {
        if (conditionFunction == null)
            throw new NullReferenceException();

        return conditionFunction();
    }

    public void ExecuteIfCondition()
    {
        if (ShouldExecute())
            Execute();
    }
}