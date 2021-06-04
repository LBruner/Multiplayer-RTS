using Mirror;
using UnityEngine;

public class UnitProjectille : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        if(other.TryGetComponent<Health>(out Health health)){
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}