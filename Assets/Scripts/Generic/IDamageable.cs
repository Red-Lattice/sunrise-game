using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void DealDamage(float damage, BulletType bulletType, GameObject dealer, Vector3 hitPos);
    public Team GetTeam();
}
