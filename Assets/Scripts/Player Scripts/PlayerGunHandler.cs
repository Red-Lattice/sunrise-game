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

    [SerializeField] private int numberOfWeapons;
    [SerializeField] private int weaponSelected;
    [SerializeField] private Weapon[] availableWeapons;
    private Weapon activeWeaponScript;
    private GunAnimator gunAnimator;
    private LepPlayerMovement playerMovement;
    private string currentMode;
    private float recoilMovement;
    private float recoilDecay = 2f;

    void Start()
    {
        weaponSelected = 0;
        recoilDecay = 3f;
        availableWeapons = new Weapon[maxWeapons];
        playerMovement = this.transform.GetComponent<LepPlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (recoilMovement > 0f)
        {
            camControl.Punch(new Vector2(0f, -Time.deltaTime * recoilDecay));
            recoilMovement -= Time.deltaTime * recoilDecay;
        }
        // Switches weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switchWeapon();
        }
        if (Input.GetMouseButtonDown(0) && activeWeaponScript != null)
        {
            activeWeaponScript.triggerWeapon();
            gunAnimator.fire();
            camControl.Punch(new Vector2(0f, 1f));
            recoilMovement += 1f;
        }
        if (gunAnimator != null)
        {
            updateContinuousAnimations();
        }
    }

    // Updating the running vs idling animation
    void updateContinuousAnimations()
    {
        if (gunAnimator == null) {return;}

        currentMode = playerMovement.getMode();       
        if (playerMovement.getMode() == "Walking" && playerMovement.getPlayerSpeed() > 5)
        {
            gunAnimator.setContinuousState("Running");
            return;
        }
        gunAnimator.setContinuousState("Idle");
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
            if (gunAnimator != null)
            {
                Destroy(gunAnimator.gameObject);
                Destroy(gunAnimator);
            }
            activeWeaponScript = getAddedWeapon(weaponName);
            numberOfWeapons++;
            availableWeapons[numberOfWeapons - 1] = activeWeaponScript;
            gunAnimator = Instantiate(gunInQuestion, uiCam.transform).GetComponent<GunAnimator>();
            weaponSelected++;
            return;
        }

        Destroy(gunAnimator.transform.parent);

        gunAnimator = Instantiate(gunInQuestion, uiCam.transform).GetComponent<GunAnimator>();
        activeWeaponScript = getAddedWeapon(weaponName);
        availableWeapons[weaponSelected] = activeWeaponScript;
        weaponSelected++;
    }

    /**
    * Runs when you switch between weapons you have. 
    * Does nothing if you don't have weapons
    */
    void switchWeapon()
    {
        if (numberOfWeapons > 0) // We don't want a divide by 0 error!
        {
            if (gunAnimator != null)
            {
                Destroy(gunAnimator.gameObject);
                Destroy(gunAnimator);
            }
            weaponSelected = weaponSelected % numberOfWeapons;
            weaponSelected++;
            activeWeaponScript = availableWeapons[weaponSelected - 1];
            string weaponName = activeWeaponScript.weaponName;
            GameObject gunInQuestion = gunGetter.getGunPrefab(weaponName);
            gunAnimator = Instantiate(gunInQuestion, uiCam.transform).GetComponent<GunAnimator>();
        }
    }

    private Weapon getAddedWeapon(string weaponName)
    {
        // Every time you add a new weapon type, add it to this.
        switch (weaponName)
        {
            case "Pistol":
                return weaponObject.AddComponent<Weapon_Pistol>();
            case "Plasma Pulser":
                return weaponObject.AddComponent<Weapon_PlasmaPulser>();
            default:
                return weaponObject.AddComponent<Weapon>();
        }
    }

    public bool weaponsNeededCheck()
    {
        return (numberOfWeapons < maxWeapons);
    }

    #region Getters

    public ProjectileScriptableObjects getProjectileGetter()
    {
        return projectileGetter;
    }
    #endregion
}

