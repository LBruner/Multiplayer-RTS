using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private UnitMovement unitMovement;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    #region Client
    [Client]
    public void Select()
    {
        Debug.Log("He");
        if(!hasAuthority) { return; }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }
    #endregion
}
