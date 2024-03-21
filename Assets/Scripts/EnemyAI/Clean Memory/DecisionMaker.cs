using UnityEngine;

public static class DecisionMaker {
    public static void GenerateActions(this CleanAI ai) {
        if (ai.hasTarget) {ai.GenerateTargetActions(); return;}
        ai.GenerateNonTargetActions();
    }
    private static void GenerateNonTargetActions(this CleanAI ai) {
        if (ai.boredom >= Random.Range(25,255)) {
            ai.boredom = 0;
            ai.aiData = new BrainData(ai.RandomPosition(), ai.aiData.lookAtRot);
        }
        ++ai.boredom;
    }

    private static void GenerateTargetActions(this CleanAI ai) {
        Vector3 newPosition = ai.aiData.moveToLocation;
        Quaternion newRotation = ai.aiData.lookAtRot;
        if (ai.HasLineOfSightToTarget()) {
            newRotation = Quaternion.LookRotation(ai.targetPosition - ai.transform.position);
            
            if (!ai.DistanceCheck(newPosition)) {goto FINISH;}

            Vector3 randomPos = ai.RandomBackUp();
            if (Vector3.Distance(newPosition, ai.targetPosition) < 10f) {newPosition = randomPos;}
            goto FINISH;
        }
        newPosition = ai.targetPosition;
        newRotation = Quaternion.identity;

        FINISH:
        ai.aiData = new BrainData(newPosition, newRotation);
    }

    public static Vector3 RandomPosition(this CleanAI ai) {
        float randomMagnitude = Random.Range(-2f, 2f);
        return ai.transform.position + new Vector3(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f)).normalized * randomMagnitude;
    }
    public static Vector3 RandomBackUp(this CleanAI ai) {
        float randomMagnitude = Random.Range(-2f, -0.75f);
        bool chargeIn = ai.ChargeIn();
        return chargeIn ? ai.targetPosition : ai.transform.position + Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * ai.transform.forward * randomMagnitude;
    }

    public const float chargeInDistance = 1.25f;
    public static float chargeInRangeFromDistance => 10f / chargeInDistance;
    public static bool ChargeIn(this CleanAI ai) {
        return (10f / ai.DistanceToTarget()) > Random.Range(0f, chargeInRangeFromDistance);
    }
}
