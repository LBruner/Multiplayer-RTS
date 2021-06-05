using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private UnitMovement unitMovement;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AutorityOnUnitSpawned;
    public static event Action<Unit> AutorityOnUnitDespawned;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += HandleUnitDie;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleUnitDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void HandleUnitDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
    #region Client

    public override void OnStartAuthority()
    {
        AutorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (hasAuthority) { return; }

        AutorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }
    #endregion
}
