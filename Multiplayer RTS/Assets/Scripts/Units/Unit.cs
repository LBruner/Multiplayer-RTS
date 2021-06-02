using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private UnitMovement unitMovement;
    [SerializeField] private Targeter targeter = null;

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
    }
    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion
    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly || hasAuthority) { return; }

        AutorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || hasAuthority) { return; }

        AutorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if(!hasAuthority) { return; }
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
