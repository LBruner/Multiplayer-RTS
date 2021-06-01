using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera maincamera;

    private List<Unit> selectedUnits = new List<Unit>();

    private void Start()
    {
        maincamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach(Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = maincamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)){ return; }

        if(!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

        if(!unit.hasAuthority) { return; }

        Debug.Log(selectedUnits.Count);
        selectedUnits.Add(unit);

        foreach(Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select(); 
        }
    }
}
