using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    // These are the base things a goal needs to be able to run

    bool CanRun();

    int CalculatePriority();

    void OnTickGoal();

    void OnGoalActivated();

    void OnGoalDeactivated();
}

public class Goal_Base : MonoBehaviour, IGoal
{
    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;

    void Awake()
    {
        Agent = GetComponent<CharacterAgent>();
        Sensors = GetComponent<AwarenessSystem>();
    }

    void Update()
    {
        OnTickGoal();
    }

    public virtual int CalculatePriority()
    {
        return -1;
    }

    public virtual bool CanRun()
    {
        return false;
    }

    public virtual void OnTickGoal()
    {

    }

    public virtual void OnGoalActivated()
    {

    }

    public virtual void OnGoalDeactivated()
    {

    }
}
