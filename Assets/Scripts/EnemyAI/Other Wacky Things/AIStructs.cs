using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTypes {
    ExternObjectInteraction,
    InternObjectInteration,
    MoveTo,
    Animate,
}

public struct AIGoal {

}

public struct AIAction {
    public Position targetPosition;

}

public struct Position {
    public float x;
    public float y;
    public float z;
}