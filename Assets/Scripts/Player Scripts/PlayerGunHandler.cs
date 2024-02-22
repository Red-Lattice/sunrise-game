using System.Collections.Generic;
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
    [SerializeField] private GrenadesScriptableObject grenadeGetter;

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
        weaponSelectedIndex = 0;
        availableWeapons = new WeaponStruct[maxWeapons];
        guns = new Animator[maxWeapons];
        playerRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //DebugStructs();

        UpdateCooldowns();

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
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {SwitchWeapon();} // Switches weapon

        if (WeaponsEmpty()) {return;}

        var weapon = availableWeapons[weaponSelectedIndex - 1];
        if (Input.GetMouseButtonDown(0) && Weapon.NotNull(weapon))
        {
            if (weapon.cooldown > 0f) {return;}
            Weapon.Fire(transform.gameObject, weapon, camControl.transform);
            availableWeapons[weaponSelectedIndex - 1].cooldown = Weapon.cooldowns[(int)weapon.gunType]; // Structs are weird

            AnimateGunfire();

            if (recoilMovement >= 1f) {return;}
            camControl.Punch(new Vector2(0f, 1f));
            recoilMovement += 1f;
        }
    }

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

    private void ThrowGrenade() {
        GameObject newGrenade = Instantiate(grenadeGetter.defaultGrenade, transform.position + transform.forward, Quaternion.identity);
        if (newGrenade.TryGetComponent(out Rigidbody grenadeRB))
        {
            grenadeRB.AddForce(camControl.transform.forward * 1000f, ForceMode.Acceleration);
            
            grenadeRB.angularVelocity += 
                Vector3.forward * Random.Range(-10.0f, 10.0f)
                + Vector3.left * Random.Range(-10.0f, 10.0f);
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

        weaponSelectedIndex = weaponSelectedIndex % numberOfWeapons;
        weaponSelectedIndex++;

        (gun = guns[weaponSelectedIndex - 1]).gameObject.SetActive(true);
    }

    public bool weaponsNeededCheck() {return numberOfWeapons < maxWeapons;}
}

