using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlanningScript : MonoBehaviour
{
    [SerializeField] private bool trolled;
    [SerializeField] private bool seesGrenade;
    [SerializeField] private bool seesPlayer;
    [SerializeField] private bool awareOfPlayer;
    [SerializeField] private bool sawPlayer;

    enum State
    {
        Attacking,
        Dodging,
        Wandering,
        Retreating,
        Hunting
    }
    [SerializeField] State state = State.Wandering;

    private void GatherSenses()
    {
        // Detect the player. If true, seesPlayer and awareOfPlayer = true.
        // awareOfPlayer means that the enemy is aware of the player's existence (but not necessarily that they're in the line of sight)
        // If they can't see the player, 
    }
    private void GatherSight()
    {
        if (seesGrenade)
        {
            //Run random chance to dodge
            state = State.Dodging;
        }
        if (seesPlayer)
        {
            //Enter attacking state
            sawPlayer = true;
        }
        else if (sawPlayer)
        {
            state = State.Hunting;
        }
    }
    private void Dodge()
    {
        //Jump certain distance away
        //Animate
    }


}
