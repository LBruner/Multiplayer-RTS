using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[SelectionBase]
public class Bulding : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    public static event Action<Bulding> ServerOnBuldingSpawned;
    public static event Action<Bulding> ServerOnBuldingDespawned;

    public static event Action<Bulding> AuthorityOnBuldingSpawned;
    public static event Action<Bulding> AuthorityOnBuldingDespawned;


    public Sprite GetICon()
    {
        return icon;
    }

    public int GetID()
    {
        return id;
    }
    public int GetPrice()
    {
        return price;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnBuldingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuldingDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnBuldingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnBuldingDespawned?.Invoke(this);
    }
    #endregion

}
