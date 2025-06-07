using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class Hexstack : MonoBehaviour
{
    public List<Hexagon> Hexagons { get; private set; }

    public void Add(Hexagon Hexagon)
    {
        if (Hexagons == null)
        {
            Hexagons = new List<Hexagon>();
        }

        Hexagons.Add(Hexagon);
        Hexagon.SetParent(transform);
    }

    public Color GetTopHexaColor() => Hexagons[^1].Color;

    public void Place()
    {
        foreach (Hexagon hexagon in Hexagons)
        {
            hexagon.DisableColliders();
        }
    }

    public bool Contains(Hexagon hexagon) => Hexagons.Contains(hexagon);

    public void Remove(Hexagon hexagon)
    {
        Hexagons.Remove(hexagon);

        if (Hexagons.Count <= 0)
            DestroyImmediate(gameObject);
    }
    public void ClearStack()
    {
        if (Hexagons == null || Hexagons.Count == 0)
            return;

        foreach (Hexagon hex in Hexagons.ToList())
        {
            hex.SetParent(null);
        }

        Hexagons.Clear();
        Destroy(gameObject);
    }
}