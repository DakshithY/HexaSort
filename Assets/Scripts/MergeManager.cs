using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MergeManager : MonoBehaviour
{
    [Header("Elements")]
    private List<GridCell> UpdatedCells = new List<GridCell>();
    public int TargetScore;
    public AudioSource Stackcomplete;
    public AudioSource MoveSound;
    public AudioSource StackPlaced;
    public int Moves;
    public GameObject Win;
    public GameObject Lose;
    public TextMeshProUGUI moves;
    public TextMeshProUGUI TargetS;
    private void Awake()
    {
        StackController.onStackPlaced += StackPlacedCallback;
        Win.SetActive(false);
        Lose.SetActive(false);
    }
    private void Start()
    {
        TargetScore = TargetScore;
        Moves = Moves;
    }
    private void Update()
    {
        TargetS.text = "" + TargetScore;
        moves.text = "" + Moves;
        if (Moves == 0) // Check if it's the last move
        {
            StartCoroutine(CheckGameResult());
        }

        if (TargetScore <= 0)
        {
            Win.SetActive(true); // Show win screen
        }
    }
    private IEnumerator CheckGameResult()
    {
        yield return new WaitForSeconds(1f); // Delay of 1 second before showing the result

        if (TargetScore <= 0 && Moves >= 0)
        {
            yield return new WaitForSeconds(1f);
            Win.SetActive(true); // Show win screen
        }
        else
        {
            Lose.SetActive(true); // Show lose screen
        }
    }

    private void OnDestroy()
    {
        StackController.onStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        StartCoroutine(StackPlacedCoroutine(gridCell));
    }
    private List<GridCell> GetNeighbourGridCells(GridCell gridCell)
    {
        LayerMask gridCellMask = 1 << gridCell.gameObject.layer;
        List<GridCell> neighbourGridcells = new List<GridCell>();

        Collider[] NeighbourGridCell = Physics.OverlapSphere(gridCell.transform.position, 2, gridCellMask);

        foreach (Collider gridcellcollider in NeighbourGridCell)
        {
            GridCell neighbourGridCell = gridcellcollider.GetComponent<GridCell>();

            if (!neighbourGridCell.Isoccupied)
                continue;
            if (neighbourGridCell == gridCell)
                continue;

            neighbourGridcells.Add(neighbourGridCell);
        }
        return neighbourGridcells;
    }
    private List<GridCell> GetSimilarNeighbourGridCells(Color TopHexaFirstColor, GridCell[] neighbourGridcells)
    {
        List<GridCell> SimilarneighbourGridcells = new List<GridCell>(0);

        foreach (GridCell neighbourgridcell in neighbourGridcells)
        {
            Color neighbourgridtopcolor = neighbourgridcell.Stack.GetTopHexaColor();

            if (TopHexaFirstColor == neighbourgridtopcolor)

                SimilarneighbourGridcells.Add(neighbourgridcell);

        }
        return SimilarneighbourGridcells;
    }
    private List<Hexagon> GetHexagonsToAdd(Color TopHexaFirstColor, GridCell[] SimilarneighbourGridcells)
    {
        List<Hexagon> Hexagonstoadd = new List<Hexagon>();

        foreach (GridCell neighbourCell in SimilarneighbourGridcells)
        {
            Hexstack neighbourCellHexStack = neighbourCell.Stack;

            for (int i = neighbourCellHexStack.Hexagons.Count - 1; i >= 0; i--)
            {
                Hexagon hexagon = neighbourCellHexStack.Hexagons[i];

                if (hexagon.Color != TopHexaFirstColor)
                    break;

                Hexagonstoadd.Add(hexagon);
                hexagon.SetParent(null);
            }

        }
        return Hexagonstoadd;
    }

    IEnumerator StackPlacedCoroutine(GridCell gridCell)
    {
        Moves = Moves - 1;
        StackPlaced.Play();
        UpdatedCells.Add(gridCell);
        while (UpdatedCells.Count > 0)
            yield return CheckForMerge(UpdatedCells[0]);
        CheckForLoseCondition();
    }

    IEnumerator CheckForMerge(GridCell gridCell)
    {
        UpdatedCells.Remove(gridCell);

        if (!gridCell.Isoccupied)
            yield break;

        List<GridCell> neighbourGridcells = GetNeighbourGridCells(gridCell);

        if (neighbourGridcells.Count <= 0)
        {
            Debug.Log("No Neighbours");
            yield break;
        }
        Color TopHexaFirstColor = gridCell.Stack.GetTopHexaColor();
        List<GridCell> SimilarneighbourGridcells = GetSimilarNeighbourGridCells(TopHexaFirstColor, neighbourGridcells.ToArray());

        if (SimilarneighbourGridcells.Count <= 0)
        {
            yield break;
        }
        UpdatedCells.AddRange(SimilarneighbourGridcells);
        List<Hexagon> Hexagonstoadd = GetHexagonsToAdd(TopHexaFirstColor, SimilarneighbourGridcells.ToArray());
        RemoveHexagonsFromStack(Hexagonstoadd, SimilarneighbourGridcells.ToArray());
        MoveHexagons(gridCell, Hexagonstoadd);
        yield return new WaitForSeconds(.2f + (Hexagonstoadd.Count + 2) * .01f);
        yield return CompleteStack(gridCell, TopHexaFirstColor);
    }
    private void RemoveHexagonsFromStack(List<Hexagon> Hexagonstoadd, GridCell[] SimilarneighbourGridcells)
    {

        foreach (GridCell neighbourCell in SimilarneighbourGridcells)
        {
            Hexstack stack = neighbourCell.Stack;
            foreach (Hexagon hexagon in Hexagonstoadd)
            {
                if (stack.Contains(hexagon))
                    stack.Remove(hexagon);
            }
        }
    }
    private void MoveHexagons(GridCell gridCell, List<Hexagon> Hexagonstoadd)
    {
        float initialY = gridCell.Stack.Hexagons.Count * 0.2f;
        for (int i = 0; i < Hexagonstoadd.Count; i++)
        {
            Hexagon hexagon = Hexagonstoadd[i];

            float targetY = initialY + i * 0.2f;
            Vector3 TargetlocalPos = Vector3.up * targetY;

            gridCell.Stack.Add(hexagon);
            MoveSound.Play();
            hexagon.MoveToLocal(TargetlocalPos);

        }
    }
    private IEnumerator CompleteStack(GridCell gridCell, Color TopHexaColor)
    {
        if (gridCell.Stack.Hexagons.Count < 10)
        {
            yield break;
        }

        List<Hexagon> similarHexagons = new List<Hexagon>();
        for (int i = gridCell.Stack.Hexagons.Count - 1; i >= 0; i--)
        {
            Hexagon hexagon = gridCell.Stack.Hexagons[i];

            if (hexagon.Color != TopHexaColor)
                break;

            similarHexagons.Add(hexagon);
        }
        int SimilarHexagonCounts = similarHexagons.Count;

        if (similarHexagons.Count < 10)
            yield break;

        float delay = 0;
        while (similarHexagons.Count > 0)
        {
            similarHexagons[0].SetParent(null);
            Stackcomplete.Play();
            similarHexagons[0].Vanish(delay);
            TargetScore--;
            delay += 0.01f;
            gridCell.Stack.Remove(similarHexagons[0]);
            similarHexagons.RemoveAt(0);
        }
        UpdatedCells.Add(gridCell);
        yield return new WaitForSeconds(.2f + (SimilarHexagonCounts + 2) * .01f);
    }
    private void CheckForLoseCondition()
    {
        GridCell[] allGridCells = FindObjectsOfType<GridCell>(); // Get all grid cells in the scene

        // Check if all cells are occupied
        foreach (GridCell cell in allGridCells)
        {
            if (!cell.Isoccupied)
            {
                return; // If any cell is empty, return (game is still playable)
            }
        }

        // Check if any possible merges exist
        foreach (GridCell cell in allGridCells)
        {
            List<GridCell> neighbours = GetNeighbourGridCells(cell);
            if (neighbours.Count > 0)
            {
                Color topColor = cell.Stack.GetTopHexaColor();
                List<GridCell> similarNeighbours = GetSimilarNeighbourGridCells(topColor, neighbours.ToArray());

                if (similarNeighbours.Count > 0)
                {
                    return;
                }
            }
        }

       
        Lose.SetActive(true);
    }

} 

