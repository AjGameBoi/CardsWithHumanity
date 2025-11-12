using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public RectTransform container; // the UI holding the GridLayoutGroup
    public GridLayoutGroup grid;
    public int columns;
    public int rows;
    public Vector2 padding = new Vector2(10, 10);
    public Vector2 spacing = new Vector2(10, 10);
    public bool matchAspect = true; // if true, preserve square cards

    private void OnValidate()
    {
        if (grid != null)
        {
            UpdateGrid();
        }
    }

    public void SetupGrid(int cols, int rows)
    {
        this.columns = Mathf.Max(1, cols);
        this.rows = Mathf.Max(1, rows);
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        if (container == null || grid == null) return;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.padding = new RectOffset((int)padding.x, (int)padding.x, (int)padding.y, (int)padding.y);
        grid.spacing = spacing;

        float totalWidth = container.rect.width - (padding.x * 2) - spacing.x * (columns - 1);
        float totalHeight = container.rect.height - (padding.y * 2) - spacing.y * (rows - 1);

        float cellWidth = totalWidth / columns;
        float cellHeight = totalHeight / rows;

        if (matchAspect)
        {
            float s = Mathf.Min(cellWidth, cellHeight);
            grid.cellSize = new Vector2(s, s);
        }
        else
        {
            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }

    void Start()
    {
        UpdateGrid();
    }

    void Update()
    {
        
    }
}
