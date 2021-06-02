using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent = null;

    [ServerCallback]
    private void Update()
    {
        if(!agent.hasPath) { return; }

        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client
    
    #endregion
}
