using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool groundedStatus;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        groundedStatus = true;
    }
    void OnTriggerExit(Collider other)
    {
        groundedStatus = false;
    }

    public bool IsGrounded()
    {
        return groundedStatus;
    }
}
