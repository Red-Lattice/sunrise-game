using UnityEngine;

public static class DecisionMaker {
    public static void GenerateActions(this CleanAI ai) {
        if (ai.hasTarget) {ai.GenerateTargetActions(); return;}
        ai.GenerateNonTargetActions();
    }
    private static void GenerateNonTargetActions(this CleanAI ai) {

    }

    private static void GenerateTargetActions(this CleanAI ai) {
        Vector3 newPosition = ai.aiData.moveToLocation;
        Quaternion newRotation = ai.aiData.lookAtRot;
        if (ai.HasLineOfSightToTarget()) {
            newRotation = Quaternion.LookRotation(ai.targetPosition - ai.transform.position);
            if (ai.DistanceCheck(newPosition)) {
                Vector3 randomPos = ai.RandomPosition();
                if (Vector3.Distance(newPosition, ai.targetPosition) < 10f) {newPosition = randomPos;}
            }
        } else {
            newPosition = ai.targetPosition;
            newRotation = Quaternion.identity;
        }
        ai.aiData = new BrainData(newPosition, newRotation);
    }

    public static Vector3 RandomPosition(this CleanAI ai) {
        float randomMagnitude = Random.Range(0.75f, 2f);
        return ai.transform.position + new Vector3(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f)).normalized * randomMagnitude;
    }
}
