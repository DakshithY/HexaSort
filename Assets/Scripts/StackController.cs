using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask HexazonLayermask;
    [SerializeField] private LayerMask GridHexazonLayermask;
    [SerializeField] private LayerMask GroundLayermask;

    private Hexstack currentStack;
    private Vector3 InitialHexPos;
    [Header("Data")]
    private GridCell TargetGridCell;
    [Header("Actions")]
    public static Action<GridCell> onStackPlaced;
    void Update()
    {
        ControlsManager();
    }

    private void ControlsManager()
    {
        if (Input.GetMouseButtonDown(0))
            MouseDown();
        else if (Input.GetMouseButton(0) && currentStack != null)
            MouseDrag();
        else if (Input.GetMouseButtonUp(0) && currentStack != null)
            MouseUp();
    }

    private void MouseDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(Rayclick(), out hit, 500, HexazonLayermask))
        {
            Hexagon hex = hit.collider.GetComponent<Hexagon>();

            if (hex != null && hex.HexStack != null)
            {
                currentStack = hex.HexStack;
                InitialHexPos = currentStack.transform.position;
            }
            else
            {
                currentStack = null;
            }
        }
        else
        {
            currentStack = null;
        }
    }

    private void MouseDrag()
    {
        if (currentStack == null) return;

        RaycastHit hit;
        if (Physics.Raycast(Rayclick(), out hit, 500, GridHexazonLayermask))
        {
            DraggingOnGrid(hit);
        }
        else
        {
            DraggingOnGround();
        }
    }

    private void DraggingOnGround()
    {
        if (currentStack == null) return;

        RaycastHit hit;
        if (Physics.Raycast(Rayclick(), out hit, 500, GroundLayermask))
        {
            Vector3 targetPos = new Vector3(hit.point.x, 2, hit.point.z);
            currentStack.transform.position = Vector3.MoveTowards(
                currentStack.transform.position,
                targetPos,
                Time.deltaTime * 30
            );
            TargetGridCell = null;
        }
    }

    private void DraggingOnGrid(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            DraggingOnGround();
            return;
        }

        GridCell gridCell = hit.collider.GetComponent<GridCell>();
        if (gridCell == null)
        {
            DraggingOnGround();
            return;
        }

        if (gridCell.Isoccupied)
            DraggingOnGround();
        else
            DraggingOnNonOccupiedCell(gridCell);
    }

    private void DraggingOnNonOccupiedCell(GridCell gridCell)
    {
        if (currentStack == null) return;

        Vector3 targetPos = new Vector3(gridCell.transform.position.x, 2, gridCell.transform.position.z);
        currentStack.transform.position = Vector3.MoveTowards(
            currentStack.transform.position,
            targetPos,
            Time.deltaTime * 30
        );

        TargetGridCell = gridCell;
    }

    private void MouseUp()
    {
        if (TargetGridCell == null) 
        {
            currentStack.transform.position = InitialHexPos;
            currentStack = null;
            return;
        }
        currentStack.transform.position = new Vector3(TargetGridCell.transform.position.x,0.2f,TargetGridCell.transform.position.z);
        currentStack.transform.SetParent(TargetGridCell.transform);
        currentStack.Place();
        TargetGridCell.AssignStack(currentStack);
        onStackPlaced?.Invoke(TargetGridCell);
        TargetGridCell = null;
        currentStack = null;
    }

    private Ray Rayclick() => Camera.main.ScreenPointToRay(Input.mousePosition);
}
