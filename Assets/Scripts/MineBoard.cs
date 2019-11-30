using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineBoard : MonoBehaviour
{
    public int rows;
    public int columns;
    public int mineCount;

    public VerticalLayoutGroup board;
    public HorizontalLayoutGroup panelRow;
    public Tile tilePrefab;

    public Text messageText;
    public Text flagCountText;
    public Button gameStartButton;

    // key = rows, value index = columns
    private Dictionary<int, List<Tile>> _tileDictionary = new Dictionary<int, List<Tile>>();

    private int _revealedTileCount = 0;
    private int _usedFlagCount = 0;
    private bool _lostGame = false;

    private const float SPACING = 5f;

    public void InitBoard()
    {
        if (rows <= 0 || columns <= 0 || mineCount > rows * columns)
        {
            Debug.LogError("This is WRONG!");
            return;
        }

        ClearBoard();
        GenerateBoard();
        AddMines();

        messageText.text = String.Empty;
        flagCountText.text = mineCount.ToString();
        gameStartButton.gameObject.SetActive(false);

        _revealedTileCount = 0;
        _usedFlagCount = 0;
        _lostGame = false;
    }

    public void GenerateBoard()
    {
        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            HorizontalLayoutGroup rowParent = (HorizontalLayoutGroup)Instantiate(panelRow);
            rowParent.transform.SetParent(board.transform);

            _tileDictionary.Add(rowIndex, new List<Tile>());

            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                Tile tile = (Tile)Instantiate(tilePrefab);
                tile.transform.SetParent(rowParent.transform);
                tile.Init(this, rowIndex, columnIndex);

                _tileDictionary[rowIndex].Add(tile);
            }
        }
    }

    public void ClearBoard()
    {
        for (int count = 0; count < board.transform.childCount; count++)
        {
            Destroy(board.transform.GetChild(count).gameObject);
        }

        _tileDictionary.Clear();
    }

    public void AdjustSpacing()
    {
        Tile tile = _tileDictionary[0][0];
        //float height = tile.rec
    }

    private void AddMines()
    {
        int minesAdded = 0;

        while (minesAdded < mineCount)
        {
            int mineRow = UnityEngine.Random.Range(0, rows);
            int mineColumn = UnityEngine.Random.Range(0, columns);

            Tile currentTile = _tileDictionary[mineRow][mineColumn];

            if (currentTile.Type != TileType.Mine)
            {
                currentTile.SetMineTile();
                MarkMineNeighbors(mineRow, mineColumn);
                minesAdded++;
            }
        }
    }

    private void MarkMineNeighbors(int tileRow, int tileColumn)
    {
        int topPos = Mathf.Max(0, tileRow - 1);
        int bottomPos = Mathf.Min(rows - 1, tileRow + 1);
        int leftPos = Mathf.Max(0, tileColumn - 1);
        int rightPos = Mathf.Min(columns - 1, tileColumn + 1);

        for (int row = topPos; row <= bottomPos; row++)
        {
            for (int col = leftPos; col <= rightPos; col++)
            {
                Tile currentTile = _tileDictionary[row][col];
                if (currentTile.Type != TileType.Mine)
                {
                    currentTile.IncreaseMineCount();
                }
            }
        }
    }

    private void WinGameEnd()
    {
        messageText.text = "YAAAAY!!! YOU WIN!";
        gameStartButton.gameObject.SetActive(true);
    }

    public void CheckIfWin()
    {
        if (_revealedTileCount + _usedFlagCount >= rows * columns && !_lostGame)
        {
            WinGameEnd();
        }
    }

    public void MineTriggerGameOver()
    {
        _lostGame = true;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Tile currentTile = _tileDictionary[row][col];
                if (!currentTile.Revealed)
                {
                    currentTile.RevealTile();
                }
            }
        }

        messageText.text = "womp womp you looooooose...";
        gameStartButton.gameObject.SetActive(true);
    }

    public void CheckRevealTile(int tileRow, int tileColumn)
    {
        Tile currentTile = _tileDictionary[tileRow][tileColumn];

        if (currentTile.Revealed || currentTile.Type == TileType.Mine)
        {
            return;
        }

        StartCoroutine(WaitToReveal());

        currentTile.RevealTile();

        if (currentTile.Type == TileType.Empty)
        {
            int topPos = Mathf.Max(0, tileRow - 1);
            int bottomPos = Mathf.Min(rows - 1, tileRow + 1);
            int leftPos = Mathf.Max(0, tileColumn - 1);
            int rightPos = Mathf.Min(columns - 1, tileColumn + 1);

            for (int row = topPos; row <= bottomPos; row++)
            {
                for (int col = leftPos; col <= rightPos; col++)
                {
                    CheckRevealTile(row, col);
                }
            }
        }
    }

    IEnumerator WaitToReveal()
    {
        yield return new WaitForSeconds(0.7f);
    }

    public void UpdateRevealedCount()
    {
        _revealedTileCount++;
    }

    public void UpdateFlaggedCount(bool used)
    {
        if (used)
        {
            _usedFlagCount++;
        }
        else
        {
            _usedFlagCount--;  
        }

        flagCountText.text = (mineCount - _usedFlagCount).ToString();
    }

    public bool CanAddFlag()
    {
        return _usedFlagCount < mineCount;
    }
}
