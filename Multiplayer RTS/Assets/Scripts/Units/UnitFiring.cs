using System;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectillePrefab = null;
    [SerializeField] private Transform projectilleSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target == null)
        {
            return;
        }

        if (!CanFireAtTarget()) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectilleRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectilleSpawnPoint.position);
            GameObject projectilleInstance = Instantiate(projectillePrefab, projectilleSpawnPoint.position, projectilleRotation);

            NetworkServer.Spawn(projectilleInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }
}