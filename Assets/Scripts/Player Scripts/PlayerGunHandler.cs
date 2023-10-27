using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunHandler : MonoBehaviour
{
    /**
    * projectileGetter is a scriptable object that stores references to all the projectiles
    * activeWeaponScript is the script for the weapon currently in use.
    * weaponObject is the gameObject in the world that handles the gun
    * All fields that have [SerializeField] need to be set in the editor
    **/
    [Header("Scriptable Objects")]
    [SerializeField] private ProjectileScriptableObjects projectileGetter;
    [SerializeField] private GunScriptableObject gunGetter;

    [Header("Other Objects Needed To Function")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private CameraController camControl;
    [SerializeField] private GameObject uiCam;
    
    [Header("Parameters")]
    [SerializeField] private int maxWeapons = 2;

    private int numberOfWeapons;
    private int weaponSelected;
    private Weapon[] availableWeapons;
    private Weapon activeWeaponScript;
    private GunAnimator gunAnimator;

    void Start()
    {
        weaponSelected = 0;
        availableWeapons = new Weapon[maxWeapons];
        setWeapon("Pistol");
    }

    // Update is called once per frame
    void Update()
    {
        // Switches weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switchWeapon();
        }
        if (Input.GetMouseButtonDown(0))
        {
            activeWeaponScript.triggerWeapon();
            gunAnimator.fire();
        }
    }

    /**
    * This code manages adding a weapon.
    * It also automatically switches to the new weapon.
    */
    public void setWeapon(string weaponName)
    {
        GameObject gunInQuestion = gunGetter.getGunPrefab(weaponName);

        // This runs when you're adding a new weapon
        if (numberOfWeapons < maxWeapons)
        {
            activeWeaponScript = getAddedWeapon(weaponName);
            numberOfWeapons++;
            availableWeapons[numberOfWeapons - 1] = activeWeaponScript;
            gunAnimator = Instantiate(gunInQuestion, uiCam.transform).GetComponent<GunAnimator>();
            return;
        }

        Destroy(activeWeaponScript);
        Destroy(gunAnimator.transform.parent);

        gunAnimator = Instantiate(gunInQuestion, uiCam.transform).GetComponent<GunAnimator>();
        activeWeaponScript = getAddedWeapon(weaponName);
        availableWeapons[weaponSelected] = activeWeaponScript;
    }

    /**
    * Runs when you switch between weapons you have. 
    * Does nothing if you don't have weapons
    */
    void switchWeapon()
    {
        if (numberOfWeapons > 0) // We don't want a divide by 0 error!
        {
            weaponSelected = weaponSelected % numberOfWeapons;
            activeWeaponScript = availableWeapons[weaponSelected];
        }
    }

    private Weapon getAddedWeapon(string weaponName)
    {
        // Every time you add a new weapon type, add it to this.
        switch (weaponName)
        {
            case "Pistol":
                return weaponObject.AddComponent<Weapon_Pistol>();
            default:
                return weaponObject.AddComponent<Weapon>();
        }
    }

}

