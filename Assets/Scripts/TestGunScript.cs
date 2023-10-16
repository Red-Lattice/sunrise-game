using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunScript : MonoBehaviour
{
    [SerializeField] private Animator gunPivotAnimator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gunPivotAnimator.SetTrigger("FireGun");
        }
    }
}
