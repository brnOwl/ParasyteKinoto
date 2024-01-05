using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSplash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSplash());
    }

    IEnumerator StartSplash()
    {
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
