using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassTyping;

public class EnemyChemistType : EnemyTypeRange
{
    new void Start()
    {
        base.Start();

        fireCoolDown = true;
        enemyTypeEnum = ClassType.Chemist;
    }
}
