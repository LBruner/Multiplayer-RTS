using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target = null;

    public Targetable GetTarget()
    {
        return target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Command]
    public void CmdSetTarget(GameObject targetObject)
    {
        if (!targetObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }

        target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

}
