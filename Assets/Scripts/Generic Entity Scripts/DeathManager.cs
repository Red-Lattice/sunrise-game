using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager
{
    private GameObject deadEntityPrefab;
    private string entityName;
    private GameObject holder;

    public DeathManager(string entityName, DeadEntitiesScriptableObject deso, GameObject holder)
    {
        this.entityName = entityName;
        this.holder = holder;
        deadEntityPrefab = deso.getDeadEntityPrefab(entityName);
    }

    public void kill()
    {
        if (entityName == "Player")
        {
            //Debug.Log("Player died lol");
            return;
        }

        GameObject.Instantiate(deadEntityPrefab, holder.transform.position, Quaternion.identity);
        holder.SetActive(false);
    }
}
