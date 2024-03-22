using System;
using UnityEngine;

public static class DecisionMaker {

    public static void GenerateActions(this CleanAI ai) {
        if (ai.hasTarget) {ai.GenerateTargetActions(); return;}
        ai.GenerateNonTargetActions();
    }

#region entryPoints

    /// <summary>
    /// Generates the actions an entity should take if it is not actively seeking a target
    /// </summary>
    private static void GenerateNonTargetActions(this CleanAI ai) {
        if (ai.boredom >= UnityEngine.Random.Range(25,255)) {
            ai.boredom = 0;
            ai.aiData = new(ai.RandomPosition(), ai.aiData.lookAtRot);
        }
        ++ai.boredom;
    }

    
    /// <summary>
    /// Generates actions for an entity if it has a current target
    /// </summary>
    private static void GenerateTargetActions(this CleanAI ai) {
        // Variable Initialization ----------------------------------------------------------
        Vector3 newPosition = ai.aiData.moveToLocation;
        Quaternion newRotation = ai.aiData.lookAtRot;

        // Execution ------------------------------------------------------------------------
        if (!ai.HasLineOfSightToTarget()) {
            newPosition = ai.targetPosition;
            newRotation = Quaternion.identity;
            ai.fireTrigger = false;
            goto FINISH;
        }

        newRotation = ai.RotateToTarget();

        FiringLogic(ai);

        if (!ai.DistanceCheck(newPosition)) {FiringLogic(ai); goto FINISH;}

        if (!ai.CanHitTarget()) {newPosition = ai.RandomStrafe(); goto FINISH;}

        if (ai.DistanceToTarget() < 10f) {newPosition = ai.RandomBackUp(); goto FINISH;}

        Tuple<bool,Vector3> tup = ai.TargetBoredom(); if (tup.Item1) {newPosition = tup.Item2;}

        // Result ---------------------------------------------------------------------------
        FINISH:
        ai.aiData = new(newPosition, newRotation);
    }
#endregion

    /// <summary>
    /// This method assumes that the ai having a valid line of sight was already passed.
    /// </summary>
    private static void FiringLogic(CleanAI ai) {
        ai.fireTrigger = ai.AcceptableAngleToTarget() && ai.CanHitTarget();;
    }
    private static Tuple<bool,Vector3> TargetBoredom(this CleanAI ai) {
        if (ai.boredom >= UnityEngine.Random.Range(25,255)) {
            ai.boredom = 0;
            return new(true, ai.RandomStrafe());
        }
        ++ai.boredom;
        return new(false, Vector3.zero);
    }

#region randomHelperFunctions
    public static Vector3 RandomPosition(this CleanAI ai) {
        float randomMagnitude = UnityEngine.Random.Range(-2f, 2f);
        return ai.transform.position 
            + UnityEngine.Random.insideUnitSphere * randomMagnitude;
    }
    public static Vector3 RandomBackUp(this CleanAI ai) {
        float randomMagnitude = UnityEngine.Random.Range(-2f, -0.75f);
        return ai.ChargeIn() 
            ? ai.targetPosition 
            : ai.transform.position + RandomSmallAngle() 
                * ai.transform.forward * randomMagnitude;
    }
    public static Vector3 RandomStrafe(this CleanAI ai) {
        float randomMagnitude = ((UnityEngine.Random.Range(-1f, 1f) > 0) ? 1f : -1f) * UnityEngine.Random.Range(1f, 2f);
        return ai.transform.position + ai.transform.right * randomMagnitude;
    }
    public static Quaternion RandomSmallAngle() {
        return Quaternion.Euler(0, UnityEngine.Random.Range(-15f, 15f), 0);
    }
#endregion
#region helpers
    public const float chargeInDistance = 1.25f;
    public static float chargeInRange => 10f / chargeInDistance;
    public static bool ChargeIn(this CleanAI ai) {
        return (10f / ai.DistanceToTarget()) > UnityEngine.Random.Range(0f, chargeInRange);
    }
    private static Quaternion RotateToTarget(this CleanAI ai) {
        return Quaternion.LookRotation(ai.targetPosition - ai.transform.position);
    }
#endregion
}
