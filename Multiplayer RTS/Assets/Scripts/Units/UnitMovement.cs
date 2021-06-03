﻿using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;
    [ServerCallback]
    private void Update()
    {
        
        Targetable target = targeter.GetTarget();
        if(target != null)
        {
            if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                Debug.Log("5");
                agent.SetDestination(target.transform.position);
            }
            else if(agent.hasPath)
            {
                Debug.Log("foajs");
                agent.ResetPath();
            }

            return;
        }

        if(!agent.hasPath) { return; }

        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client
    
    #endregion
}
