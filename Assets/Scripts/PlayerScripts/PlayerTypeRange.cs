using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;
using ClassTyping;

public class PlayerTypeRange : PlayerType
{
    [Header("Player Projectiles")]
    public GameObject playerProjectile;
    public float projectileVelocity = 20f;
    public float bulletDespawnTime = 5f;

    public Transform firePoint;

    public new void Awake()
    {
        base.Awake();
        firePoint = transform.Find("RotationPoint/FirePoint").transform;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void ActivateAttack(InputAction.CallbackContext context)
    {
        //Debug.Log("ATTACK");
        FireProjectile();

    }

    private void FireProjectile()
    {
        GameObject newProjectile = Instantiate(playerProjectile.gameObject);

        newProjectile.transform.position = firePoint.transform.position;
        newProjectile.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        // Assuming: PlayerProjectile == 12 // tag and layer
        newProjectile.layer = 12;
        newProjectile.tag = "PlayerProjectile";

        newProjectile.GetComponent<Rigidbody2D>().velocity = firePoint.right * projectileVelocity;
        newProjectile.GetComponent<Projectile>().DespawnProjectile(bulletDespawnTime);
        //newProjectile.GetComponent<Projectile>().SetLauncher(EntityOwner.Player);
    }
}
