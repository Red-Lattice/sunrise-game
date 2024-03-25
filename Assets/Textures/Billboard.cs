using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraTransform;

    void Update() {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
