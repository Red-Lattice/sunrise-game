using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Start()
    {
        animator = this.transform.GetComponent<Animator>();
    }
    public void fire()
    {
        animator.SetTrigger("Fire");
    }
}
