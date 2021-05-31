using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    string displayName;

    [SyncVar]
    [SerializeField]
    Color displayColor;

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Server]
    public void SetRandomColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }
}
