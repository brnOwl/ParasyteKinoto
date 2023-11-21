using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject gameManager;
    public float longevityTime = 20f;
    Vector3 dir = Vector3.zero;
    Rigidbody2D rb;

    private void Start()
    {
        //gameManager = GameManager.Instance.gameObject;
        StartCoroutine(Disappear());
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, rb.velocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyBody" || collision.gameObject.tag == "PlayerBody")
        {
            Debug.Log("Projectile");
            //gameManager.GetComponent<GameManager>().TakeDamage(10f, collision.gameObject);
        }
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(longevityTime);
        Destroy(gameObject);
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
