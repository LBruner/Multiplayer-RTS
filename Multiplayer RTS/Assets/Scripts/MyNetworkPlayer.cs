using Mirror;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] TMP_Text displayNameText;
    [SerializeField] Renderer displayRendererColor;

    [SyncVar (hook = nameof(HandleDisplayTextUpdated))]
    [SerializeField]
    string displayName;

    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    [SerializeField]
    Color displayColor;

    #region Server
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

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        RpcSetNewName(newDisplayName);

        if (newDisplayName.Length < 2 || newDisplayName.Length > 10)
            return;

        SetDisplayName(newDisplayName);
    }

    #endregion

    #region Client

    [ClientRpc] void RpcSetNewName(string name)
    {
        Debug.Log(name);
    }
    public void HandleDisplayTextUpdated(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    private void HandleDisplayColorUpdated(Color oldColor,Color newColor)
    {
        displayRendererColor.material.SetColor("_BaseColor", newColor);
    }

    [ContextMenu("Set My Name")]
    private void SetMyName()
    {
        CmdSetDisplayName("HelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHello");
    }
    #endregion
}
