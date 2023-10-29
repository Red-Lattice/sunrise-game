using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
* This script gets attached to a shield gameObject with 
* a shieldAnimator Animator component.
*/
public class ShieldAnimator : MonoBehaviour
{
    [SerializeField] private Animator shieldAnimator;

    // Start is called before the first frame update
    void Start()
    {
        shieldAnimator = transform.gameObject.GetComponent<Animator>();
    }

    public void ShieldFlash()
    {
        shieldAnimator.Play("Flash", -1, 0f);
    }
}
