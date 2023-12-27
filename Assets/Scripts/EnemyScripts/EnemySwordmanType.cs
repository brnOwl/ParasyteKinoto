using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassTyping;

public class EnemySwordmanType : EnemyType
{
    // Start is called before the first frame update
    public float meleeXOffset = 1.5f;

    // Get Scripts in children
    private MeleeController meleeController;

    new void Start()
    {
        base.Start();

        enemyTypeEnum = ClassType.Swordman;
        meleeController = GetComponentInChildren<MeleeController>();
    }

    new void Update()
    {
        base.Update();
        meleeController.transform.position = new Vector3(transform.position.x + meleeXOffset * facingRight, transform.position.y, transform.position.z);
    }




}
