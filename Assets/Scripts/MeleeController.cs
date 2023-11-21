using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{

    bool isMelee;
    // Start is called before the first frame update
    void Start()
    {
        isMelee = false;
        gameObject.SetActive(isMelee);
    }

    public void SwitchMelee()
    {
        isMelee = !isMelee;
        gameObject.SetActive(isMelee);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}