using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICam : MonoBehaviour
{
    private Camera thisCam;
    public Canvas can;

    void Awake() {
        thisCam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H)) {
            thisCam.enabled = !thisCam.enabled;
            can.enabled = !can.enabled;
        }
    }
}
