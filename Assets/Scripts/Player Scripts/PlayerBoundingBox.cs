using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundingBox : MonoBehaviour
{
    private PlayerGunHandler pgh;
    private string highlightedWeapon;

    void Start()
    {
        highlightedWeapon = "";
        pgh = transform.parent.gameObject.GetComponent<PlayerGunHandler>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;
        if (otherGO.layer == LayerMask.NameToLayer("objects"))
        {
            switch (otherGO.tag)
            {
                case "Weapon":
                    addWeapon(otherGO);
                    break;
                case "Grenade":
                    break;
                case "Health Pack":
                    break;
            }
        }
    }

    void addWeapon(GameObject go)
    {
        highlightedWeapon = go.name;
        if (pgh.weaponsNeededCheck())
        {
            pgh.SetWeapon(highlightedWeapon);
            Destroy(go);
        }
    }

    public string getHighlightedWeapon()
    {
        return highlightedWeapon;
    }
}
