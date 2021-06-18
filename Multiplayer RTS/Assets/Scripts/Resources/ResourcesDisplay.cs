using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (player != null)
        {
            ClientHandleResourcesUpdated(player.GetResources());
            
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
