using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server
    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("h1");

        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        Debug.Log("h2");

        if (!hasAuthority) { return; }

        Debug.Log("h3");
        CmdSpawnUnit();
    }

    #endregion
}
