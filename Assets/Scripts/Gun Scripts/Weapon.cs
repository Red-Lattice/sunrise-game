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

    public static bool StaticTriggerWeapon()
    {
        Debug.LogError("Default static weapon fire method triggered (This should not happen)");
        return false;
    }

    public virtual bool triggerWeapon(Transform firerTransform)
    {
        return false;
    }

    public virtual void weaponHeld()
    {
        //Empty
    }

    public virtual bool punch()
    {
        return false;
    }
}
