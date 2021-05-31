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
    public void HandleDisplayTextUpdated(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    private void HandleDisplayColorUpdated(Color oldColor,Color newColor)
    {
        displayRendererColor.material.SetColor("_BaseColor", newColor);
    }  
}
