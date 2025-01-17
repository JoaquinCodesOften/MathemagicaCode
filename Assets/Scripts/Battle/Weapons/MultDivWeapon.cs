using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultDivWeapon : Weapon
{
    public override void Attack (Unit enemyUnit){
        if (thisWeapon.real) {
            if (!thisWeapon.damagingDown) {
                enemyUnit.currentHPReal *= thisWeapon.currentRealDmgModifier;
            } else {
                enemyUnit.currentHPReal /= thisWeapon.currentRealDmgModifier;
            }
        } else {
            if (!thisWeapon.damagingDown) {
                enemyUnit.currentHPImag *= thisWeapon.currentRealDmgModifier;
            } else {
                enemyUnit.currentHPImag /= thisWeapon.currentRealDmgModifier;
            }
        }

        if (!thisWeapon.permMod) {
            if (thisWeapon.modded) {
                thisWeapon.ModDurationLeft -= 1; 
                if (thisWeapon.ModDurationLeft == 0) {
                    thisWeapon.currentRealDmgModifier = thisWeapon.baseRealDmgModifier;
                    thisWeapon.ModDurationLeft = thisWeapon.MaxModDuration;
                    thisWeapon.modded = false;
                }
            }
        }
    }
}
