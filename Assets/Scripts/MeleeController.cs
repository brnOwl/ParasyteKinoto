using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public float meleeDamage = 100f;
    protected bool isMelee;
    // Start is called before the first frame update
    void Start()
    {
        isMelee = false;
    }

    public void SetMelee(bool setOption)
    {
        gameObject.SetActive(setOption);
    }

    public void SwitchMelee()
    {
        isMelee = !isMelee;
        gameObject.SetActive(isMelee);
    }

}
