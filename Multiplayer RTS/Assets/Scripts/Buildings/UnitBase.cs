using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += HandleBaseDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleBaseDie;

        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void HandleBaseDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion

}
