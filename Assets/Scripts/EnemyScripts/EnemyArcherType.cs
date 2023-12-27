using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ClassTyping;

public class EnemyArcherType : EnemyTypeRange
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        fireCoolDown = true;
        enemyTypeEnum = ClassType.Archer;
    }
}
