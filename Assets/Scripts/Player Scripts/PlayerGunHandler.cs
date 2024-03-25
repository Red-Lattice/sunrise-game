using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AnimationState;
using static BulletType;

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
    [Header("Other Objects Needed To Function")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private CameraController camControl;
    [SerializeField] private Animator rightArm;
    [SerializeField] private Animator leftArm;
    private Rigidbody playerRB;
    
    [Header("Parameters")]
    public int maxWeapons = 2;
    //****************************
    private int numberOfWeapons;
    private int weaponSelectedIndex;
    private WeaponStruct[] availableWeapons;
    private float recoilMovement;
    private Animator gun;
    private Animator[] guns;

    void Start()
    {
        rightArm.gameObject.SetActive(false);
        leftArm.gameObject.SetActive(false);
        weaponSelectedIndex = 0;
        availableWeapons = new WeaponStruct[maxWeapons];
        guns = new Animator[maxWeapons];
        playerRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GunAnimator.PlayAnimation(new AnimationInfo(Wall, leftArm, None));
        }
        if (Input.GetMouseButtonUp(1))
        {
            GunAnimator.PlayAnimation(new AnimationInfo(Idle, leftArm, None));
        }

        //DebugStructs();

        UpdateCooldowns();

        if (!rightArm.gameObject.activeInHierarchy && numberOfWeapons > 0) {rightArm.gameObject.SetActive(true);}
        if (!leftArm.gameObject.activeInHierarchy && numberOfWeapons > 0) {leftArm.gameObject.SetActive(true);}

        ProcessRecoil();

        if (Input.GetKeyDown(KeyCode.Q)) // Clean this shit up later
        {
            PunchSomething();
            AnimatePunch();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {SwitchWeapon();} // Switches weapon

        if (WeaponsEmpty()) {return;}

        if (Input.GetMouseButtonDown(0) && Weapon.NotNull(ref weapon))
        {
            if (weapon.cooldown > 0f) {return;}
            Weapon.Fire(transform.gameObject, ref weapon, camControl.transform);

            AnimateGunfire();

            if (recoilMovement >= 1f) {return;}
            camControl.Punch(new Vector2(0f, 1f));
            recoilMovement += 1f;
        }
    }

    ref WeaponStruct weapon => ref availableWeapons[weaponSelectedIndex - 1];

    private void UpdateCooldowns() {
        for (int i = 0; i < availableWeapons.Length; i++) {
            if (availableWeapons[i].cooldown > 0f) {availableWeapons[i].cooldown -= Time.deltaTime;}
        }
    }

    /*public List<string> debug_ActiveWeapons;
    private void DebugStructs() {
        debug_ActiveWeapons.Clear();
        foreach (WeaponStruct wep in availableWeapons) {
            debug_ActiveWeapons.Add(wep.gunType.ToString());
        }
    }*/

    private bool WeaponsEmpty() {
        foreach (WeaponStruct wep in availableWeapons) {
            if (Weapon.NotNull(wep)) {return false;}
        }
        return true;
    }

    private void ProcessRecoil() {
        if (recoilMovement > 0f)
        {
            camControl.Punch(new Vector2(0f, -Time.deltaTime * 3f));
            recoilMovement -= Time.deltaTime * 3f;
        }
    }

    private void PunchSomething() {
        if (Physics.Raycast(camControl.transform.position, camControl.transform.forward, out RaycastHit hit, 3f))
        {
            if (hit.transform.TryGetComponent(out StatManager statManager))
            {
                float dmg = 30f + (4f * playerRB.velocity.magnitude);
                statManager.DealDamage(dmg, Melee, transform.gameObject, transform.position);
            }
        }
    }

    private void AnimatePunch() {
        GunAnimator.PlayAnimation(MakeAnimInfoStruct(Punch));
    }

    private void AnimateGunfire() {
        if (gun != null) {GunAnimator.PlayAnimation(MakeAnimInfoStruct(Fire));}
        GunAnimator.PlayAnimation(new AnimationInfo(Fire, rightArm, None));
        GunAnimator.PlayAnimation(new AnimationInfo(Fire, leftArm, None));
    }

    private AnimationInfo MakeAnimInfoStruct(AnimationState toState) {
        return new AnimationInfo(toState, gun, GunAnimator.FindAnimationState(gun));
    }

    
    /// <summary>
    /// This code manages adding a weapon. It also automatically switches to the new weapon.
    /// </summary>
    public void SetWeapon(string weaponName)
    {
        GameObject gunInQuestion = gunGetter.getGunPrefab(weaponName);
        if (gun != null) {
            gun.gameObject.SetActive(false);
        }
        numberOfWeapons++;
        availableWeapons[numberOfWeapons - 1] = Weapon.WeaponStructFromName(weaponName);
        guns[numberOfWeapons - 1] = gun = Instantiate(gunInQuestion, weaponObject.transform).GetComponent<Animator>();
        SwitchWeapon();
    }

    /**
    * Runs when you switch between weapons you have. 
    * Does nothing if you don't have weapons
    */
    void SwitchWeapon() {
        if (numberOfWeapons <= 0) {return;} // We don't want a divide by 0 error!

        if (gun != null) {
            gun.gameObject.SetActive(false);
        }
        GunAnimator.PlayAnimation(new AnimationInfo(Enter, rightArm, None));
        GunAnimator.PlayAnimation(new AnimationInfo(Enter, leftArm, None));

        weaponSelectedIndex = weaponSelectedIndex % numberOfWeapons;
        weaponSelectedIndex++;

        (gun = guns[weaponSelectedIndex - 1]).gameObject.SetActive(true);
    }

    public bool weaponsNeededCheck() {return numberOfWeapons < maxWeapons;}
}

