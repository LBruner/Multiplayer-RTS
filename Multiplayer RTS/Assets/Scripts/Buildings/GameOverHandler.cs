using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{   
    private List<UnitBase> bases = new List<UnitBase>();
    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawn;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawn;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawn;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawn;
    }

    [Server]
    private void ServerHandleBaseSpawn(UnitBase unitBase)
    {
        bases.Add(unitBase);
        Debug.Log(unitBase);
        Debug.Log(bases.Count);
    }
    
    [Server]
    private void ServerHandleBaseDespawn(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1) { return; }

        Debug.Log("Game Over!");
    }

    #endregion

    #region  Client



    #endregion
}