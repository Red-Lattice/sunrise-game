/**
* ALL WEAPON SCRIPTS MUST INHERIT FROM THIS OR WE WILL ALL DIE
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public virtual bool triggerWeapon()
    {
        return false;
    }

    public virtual void weaponHeld()
    {
        //Empty
    }
}
