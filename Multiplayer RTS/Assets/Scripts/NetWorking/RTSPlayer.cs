﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Bulding[] buldings = new Bulding[0];

    [SyncVar(hook = nameof(ClientHandleResourceUpdated))]
    private int resources = 100;

    private List<Unit> myUnits = new List<Unit>();
    private List<Bulding> myBuildings = new List<Bulding>();
    
    public event Action<int> ClientOnResourcesUpdated; 

    public int GetResources()
    {
        return resources;
    }

    public List<Unit> GetMyUnits()
    {
        return myUnits; 
    }

    public List<Bulding> GetMyBuildings()
    {
        return myBuildings;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Bulding.ServerOnBuldingSpawned += ServerHandleBuldingSpawned;
        Bulding.ServerOnBuldingDespawned += ServerHandleBuldingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Bulding.ServerOnBuldingSpawned -= ServerHandleBuldingSpawned;
        Bulding.ServerOnBuldingDespawned -= ServerHandleBuldingDespawned;
    }

    private void ClientHandleResourceUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    [Command]
    public void CmdTryPlaceBulding(int buldingID, Vector3 point)
    {
        Bulding buldingToPlace = null;
       
        foreach (Bulding bulding in buldings)
        {
            if(bulding.GetID() == buldingID)
            {
                buldingToPlace = bulding;
                break;
            }
        }

        if (buldingToPlace == null) { return; }

        GameObject buldingInstance = 
        Instantiate(buldingToPlace.gameObject, point, Quaternion.identity);

        NetworkServer.Spawn(buldingInstance, connectionToClient);
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    private void ServerHandleBuldingSpawned(Bulding bulding)
    {
        if (bulding.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(bulding);
    }
    private void ServerHandleBuldingDespawned(Bulding bulding)
    {
        if (bulding.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(bulding);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }

        Unit.AutorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AutorityOnUnitSpawned += AuthorityHandleUnitDespawned;
        Bulding.AuthorityOnBuldingSpawned += AuthorityHandleBuldingSpawned;
        Bulding.AuthorityOnBuldingDespawned += AuthorityHandleBuldingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }

        Unit.AutorityOnUnitDespawned -= AuthorityHandleUnitSpawned;
        Unit.AutorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Bulding.AuthorityOnBuldingSpawned -= AuthorityHandleBuldingSpawned;
        Bulding.AuthorityOnBuldingDespawned -= AuthorityHandleBuldingDespawned;

    }
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuldingSpawned(Bulding bulding)
    {
        myBuildings.Add(bulding);
    }

    private void AuthorityHandleBuldingDespawned(Bulding bulding)
    {
        myBuildings.Remove(bulding);
    }

    #endregion
}
