using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class HexaGrid : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Grid grid;
    [Header("Settings")]
    [OnValueChanged("Updategridpos")]
    [SerializeField] private Vector3Int gridposition;

    private void Updategridpos() => gridposition = grid.WorldToCell(transform.position);
    

}
