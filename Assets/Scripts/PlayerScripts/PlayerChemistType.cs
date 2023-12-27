using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChemistType : PlayerTypeRange
{
    SpriteRenderer potionSprite;

    private new void Awake()
    {
        base.Awake();
        potionSprite = transform.Find("RotationPoint/FirePoint/Sprite").GetComponent<SpriteRenderer>();
    }

    private new void Update()
    {
        base.Update();

        potionSprite.flipY = (playerDirection.x < 0);
    }
}
