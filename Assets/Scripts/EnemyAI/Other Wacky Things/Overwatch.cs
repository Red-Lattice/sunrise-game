using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Overwatch
{
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() {
        UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
        playerLoop.subSystemList[5].updateDelegate += UpdateAI;
        UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
    }

    public static void UpdateAI() {
        
    }
}