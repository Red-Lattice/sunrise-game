using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimationState;

public class PlayerGunHandler : MonoBehaviour
{
    /**
    * projectileGetter is a scriptable object that stores references to all the projectiles
    * activeWeaponScript is the script for the weapon currently in use.
    * weaponObject is the gameObject in the world that handles the gun
    * All fields that have [SerializeField] need to be set in the editor
    **/
    [Header("Scriptable Objects")]
    [SerializeField] private GunScriptableObject gunGetter;
    [SerializeField] private GrenadesScriptableObject grenadeGetter;

    [Header("Other Objects Needed To Function")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private CameraController camControl;
    [SerializeField] private GameObject uiCam;
    private Rigidbody playerRB;
    
    [Header("Parameters")]
    public int maxWeapons = 2;

    private int numberOfWeapons;
    private int weaponSelected;
    private Weapon[] availableWeapons;
    private Weapon activeWeaponScript;
    private float recoilMovement;
    private float recoilDecay = 2f;
    private Animator gun;
    [SerializeField] private Animator rightArm;
    [SerializeField] private Animator leftArm;
    private Animator[] guns;

    void Start()
    {
        rightArm.gameObject.SetActive(false);
        weaponSelected = 0;
        recoilDecay = 3f;
        availableWeapons = new Weapon[maxWeapons];
        guns = new Animator[maxWeapons];
        playerRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rightArm.gameObject.activeInHierarchy && numberOfWeapons > 0) {rightArm.gameObject.SetActive(true);}
        ProcessRecoil();

        if (Input.GetKeyDown(KeyCode.F))
        {
            ThrowGrenade();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q)) // Clean this shit up later
        {
            PunchSomething();
            AnimatePunch();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {switchWeapon();} // Switches weapon

        if (Input.GetMouseButtonDown(0) && activeWeaponScript != null)
        {
            if (!activeWeaponScript.triggerWeapon()) {return;}

            AnimateGunfire();

            if (recoilMovement >= 1f) {return;}
            camControl.Punch(new Vector2(0f, 1f));
            recoilMovement += 1f;
        }
    }

    private void ProcessRecoil() {
        if (recoilMovement > 0f)
        {
            camControl.Punch(new Vector2(0f, -Time.deltaTime * recoilDecay));
            recoilMovement -= Time.deltaTime * recoilDecay;
        }
    }

    private void ThrowGrenade() {
        GameObject newGrenade = Instantiate(grenadeGetter.getPrefab("Default"), transform.position + transform.forward, Quaternion.identity);
        Rigidbody grenadeRB;
        if (newGrenade.TryGetComponent(out grenadeRB)) 
        {
            grenadeRB.AddForce(camControl.transform.forward * 1000f, ForceMode.Acceleration);
            grenadeRB.angularVelocity += Vector3.forward * Random.Range(-10.0f, 10.0f);
            grenadeRB.angularVelocity += Vector3.left * Random.Range(-10.0f, 10.0f);
        }
    }

    private void PunchSomething() {
        if (Physics.Raycast(camControl.transform.position, camControl.transform.forward, out RaycastHit hit, 3f))
        {
            if (hit.transform.TryGetComponent(out StatManager statManager))
            {
                statManager.DealDamage(30f + (4f * playerRB.velocity.magnitude), "Melee", transform.gameObject, transform.position);
            }
        }
    }

    private void AnimatePunch() {
        GunAnimator.PlayAnimation(MakeAnimInfoStruct(Punch));
    }

    private void AnimateGunfire() {
        if (gun != null) {GunAnimator.PlayAnimation(MakeAnimInfoStruct(Fire));}
        GunAnimator.PlayAnimation(new AnimationInfo(Fire, rightArm, None));
    }

    private AnimationInfo MakeAnimInfoStruct(AnimationState toState) {
        return new AnimationInfo(Fire, gun, GunAnimator.FindAnimationState(gun));
    }

    
    /// <summary>
    /// This code manages adding a weapon. It also automatically switches to the new weapon.
    /// </summary>
    public void setWeapon(string weaponName)
    {
        GameObject gunInQuestion = gunGetter.getGunPrefab(weaponName);
        // This runs when you're adding a new weapon
        if (numberOfWeapons < maxWeapons) {
            if (gun != null) {
                gun.gameObject.SetActive(false);
            }
            activeWeaponScript = getAddedWeapon(weaponName);
            numberOfWeapons++;
            availableWeapons[numberOfWeapons - 1] = activeWeaponScript;
            guns[numberOfWeapons - 1] = gun = Instantiate(gunInQuestion, weaponObject.transform).GetComponent<Animator>();

            weaponSelected++;
            GunAnimator.PlayAnimation(new AnimationInfo(Enter, rightArm, GunAnimator.FindAnimationState(rightArm)));
            return;
        }

        gun.gameObject.SetActive(false);

        gun = Instantiate(gunInQuestion, weaponObject.transform).GetComponent<Animator>();
        activeWeaponScript = getAddedWeapon(weaponName);
        weaponSelected = weaponSelected % numberOfWeapons;
        availableWeapons[weaponSelected] = activeWeaponScript;
        guns[weaponSelected] = gun;
        weaponSelected++;
        GunAnimator.PlayAnimation(new AnimationInfo(Enter, rightArm, GunAnimator.FindAnimationState(rightArm)));
    }

    /**
    * Runs when you switch between weapons you have. 
    * Does nothing if you don't have weapons
    */
    void switchWeapon() {
        if (numberOfWeapons <= 0) {return;} // We don't want a divide by 0 error!

        if (gun != null) {
            activeWeaponScript.enabled = false;
            gun.gameObject.SetActive(false);
        }
        GunAnimator.PlayAnimation(new AnimationInfo(Enter, rightArm, GunAnimator.FindAnimationState(rightArm)));

        weaponSelected = weaponSelected % numberOfWeapons;
        weaponSelected++;

        (activeWeaponScript = availableWeapons[weaponSelected - 1]).enabled = true;
        (gun = guns[weaponSelected - 1]).gameObject.SetActive(true);
    }

    private Weapon getAddedWeapon(string weaponName)
    {
        // Every time you add a new weapon type, add it to this.
        switch (weaponName)
        {
            case "Pistol":
                return weaponObject.AddComponent<Weapon_Pistol>().SetShooter(transform.gameObject);
            case "Plasma Pulser":
                return weaponObject.AddComponent<Weapon_PlasmaPulser>().SetShooter(transform.gameObject);
            default:
                return weaponObject.AddComponent<Weapon>().SetShooter(transform.gameObject);
        }
    }

    public bool weaponsNeededCheck()
    {
        return numberOfWeapons < maxWeapons;
    }
}

