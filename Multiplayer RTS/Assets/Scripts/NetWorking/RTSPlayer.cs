using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private LayerMask buldingBlockLayer = new LayerMask();
    [SerializeField] private Bulding[] buldings = new Bulding[0];
    [SerializeField] private float buldingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourceUpdated))]
    
    private int resources = 100;

    [SyncVar(hook =nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    private List<Bulding> myBuildings = new List<Bulding>();
    
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

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
   
    public bool CanPlaceBulding(BoxCollider buldingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buldingCollider.center, buldingCollider.size / 2, Quaternion.identity, buldingBlockLayer))
        {
            return false;
        }

        foreach (Bulding bulding in myBuildings)
        {
            if ((point - bulding.transform.position).sqrMagnitude <= buldingRangeLimit * buldingRangeLimit)
            {
                return true;
            }
        }

        return false;
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

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    private void ClientHandleResourceUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

       ((RTSNetworkManager)NetworkManager.singleton).StartGame();

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

        if(resources < buldingToPlace.GetPrice()) { return;}

        BoxCollider buldingCollider = buldingToPlace.GetComponent<BoxCollider>();
            

        if (!CanPlaceBulding(buldingCollider, point)) { return; }

        GameObject buldingInstance = 
        Instantiate(buldingToPlace.gameObject, point, Quaternion.identity);

        NetworkServer.Spawn(buldingInstance, connectionToClient);

        SetResources(resources - buldingToPlace.GetPrice());
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

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }

        ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }

        Unit.AutorityOnUnitDespawned -= AuthorityHandleUnitSpawned;
        Unit.AutorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Bulding.AuthorityOnBuldingSpawned -= AuthorityHandleBuldingSpawned;
        Bulding.AuthorityOnBuldingDespawned -= AuthorityHandleBuldingDespawned;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldstate, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
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
