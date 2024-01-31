using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    //Weapon-Specific Variable stats
    public int currentRealDmgModifier;
    public int currentImagDmgModifier;
    public int ModDurationLeft;
    public bool modded;

    //Weapon-Specific Constant Stats
    public bool permMod;
    public bool modable;
    public int baseRealDmgModifier;
    public int baseImagDmgModifier;
    public int MaxModDuration;
    public bool real;
    public bool damagingDown;

    public bool startOnEnemy;

    public string weaponDescription;

    //TO-DO Weapons should be able to affect enemy Unit HP in many ways more than addition and subtraction
    public virtual void Attack(Unit enemyUnit){}

    //Always allow the player to do one unit of health, real or not, in the direction the corresponding weapon works
    public virtual void MinAttack(Unit enemyUnit){
    }

}
