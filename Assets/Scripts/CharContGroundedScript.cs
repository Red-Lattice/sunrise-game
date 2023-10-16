using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharContGroundedScript : MonoBehaviour
{
    [SerializeField] private RBPlayerScript pc;
    [SerializeField] private CharacterController charCon;
    
    //void Update()
    //{
    //    pc.updateGrounding(charCon.isGrounded);
    //}

    public void updateGrounded()
    {
        pc.updateGrounding(charCon.isGrounded);
    }
}
