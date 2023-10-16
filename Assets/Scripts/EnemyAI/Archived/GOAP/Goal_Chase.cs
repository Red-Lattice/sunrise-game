using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Chase : Goal_Base
{
    [SerializeField] float MinAwarenessToChase = 1.5f;
    [SerializeField] float AwarenessToStopChase = 1f;

    public override int CalculatePriority()
    {
        // no targets
        // if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
        //    return 0;

        return -1;
    }

    public override void OnGoalActivated()
    {

    }

    public override void OnGoalDeactivated()
    {

    }

    public override bool CanRun()
    {
        // if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
        //    return false;

        // Check if we have anything we are aware of
        //foreach(var candidate in Sensors.ActiveTargets.Values)
        //{
        //    if (candidate.Awareness >= MinAwarenessToChase)
        //        return true;
        //}

        return true;
    }

    public override void OnTickGoal()
    {
        
    }
}
