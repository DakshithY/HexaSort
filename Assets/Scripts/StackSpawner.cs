using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;

public class StackSpawner : MonoBehaviour
{
    [Header("Elements")]
    public Transform StackPositionParent;
    public Transform[] stackChilds;
    [SerializeField] private Hexagon Hexagon;
    [SerializeField] private Hexstack Hexastack;
    private int StackCounter;
    [Header("Settings")]
    [NaughtyAttributes.MinMaxSlider(0, 8)]
    [SerializeField] private Vector2Int minmaxhexacount;    
    [SerializeField] private Color[] Colors;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        StackController.onStackPlaced += StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell) 
    {
        StackCounter++;
        if (StackCounter >= 3) 
        {
            StackCounter = 0;
            StacksGenerator();
        }
    }
    void Start()
    {
        StacksGenerator();
    }

    private void StacksGenerator()
    {
        if (StackPositionParent == null)
        {
            Debug.LogError("StackPositionParent is null!");
            return;
        }

        for (int i = 0; i < StackPositionParent.childCount; i++)
        {
            Transform child = StackPositionParent.GetChild(i);
            if (child == null)
            {
                Debug.LogWarning($"StackPositionParent child at index {i} is missing!");
                continue;
            }
            StackGenerator(child);
        }
    }

    private void StackGenerator(Transform parent)
    {
        Hexstack hexastack = Instantiate(Hexastack, parent.position, Quaternion.identity, parent);
        hexastack.name = $"Stack{parent.GetSiblingIndex()}";
        int amount = Random.Range(minmaxhexacount.x,minmaxhexacount.y);
        int firstcolorhexagon = Random.Range(0, amount);

        Color[] colorarray = Getrandomcolors();
        
        for (int i = 0; i < amount; i++)
        {
            Vector3 hexalocalpos = Vector3.up * i * 0.2f;
            Vector3 spawnpos = hexastack.transform.TransformPoint(hexalocalpos);
            Hexagon Hexainstance = Instantiate(Hexagon, spawnpos, Quaternion.identity, hexastack.transform);
            Hexainstance.Color = i<firstcolorhexagon ? colorarray[0] : colorarray[1];

            Hexainstance.Configure(hexastack);
            hexastack.Add(Hexainstance);
        }
    }
    private Color[] Getrandomcolors()
    {
        List<Color> colorlist = new List<Color>();
        colorlist.AddRange(Colors);

        if (colorlist.Count <= 0)
        {
            Debug.Log("Color not found");
            return null;
        }
        Color firstcolor = colorlist.OrderBy(x => Random.value).First();
        colorlist.Remove(firstcolor);

        if (colorlist.Count <= 0)
        {
            Debug.Log("Only one color found");
            return null;
        }
        Color secondcolor = colorlist.OrderBy(x => Random.value).First();

        return new Color[] { firstcolor, secondcolor };
    }
}

