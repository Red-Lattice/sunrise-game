using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DebugScripts
{
    #if UNITY_EDITOR
        [MenuItem("Custom Debug/Force Cleanup NavMesh")]
        public static void ForceCleanupNavMesh()
        {
            if (Application.isPlaying)
                return;

            NavMesh.RemoveAllNavMeshData();
        }
    #endif
}
