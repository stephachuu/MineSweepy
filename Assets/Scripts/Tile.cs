using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    Empty,
    Mine,
    Numbered
}

public class Tile : MonoBehaviour
{
    public TextMeshProUGUI mineCountText;
    public Image tileImage;
    public Image flaggedImage;
    public Image mineImage;
    public Image xImage;
    public ClickableObject clickableObject;

    public Sprite hiddenSprite;
    public Sprite revealedSprite;

    public List<Color> numberColors;

    public int RowNumber  { get; set; }
    public int ColumnNumber { get; set; }
    public TileType Type { get; private set; }
    public bool Revealed { get; private set; }
    public bool Flagged { get; private set; }

    private int _mineCheckCount = 0;
    private MineBoard _parentBoard = null;

    public void Init(MineBoard parentBoard, int row, int column)
    {
        _parentBoard = parentBoard;

        RowNumber = row;
        ColumnNumber = column;
        Revealed = false;
        Flagged = false;
        Type = TileType.Empty;

        clickableObject.Clicked += OnClick;
        clickableObject.RightClicked += OnRightClick;

        mineCountText.gameObject.SetActive(false);
        flaggedImage.gameObject.SetActive(false);
        mineImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);

        tileImage.sprite = hiddenSprite;
    }

    public void SetMineTile()
    {
        Type = TileType.Mine;
    }

    public void IncreaseMineCount()
    {
        if (Type != TileType.Numbered)
        {
            Type = TileType.Numbered;
        }

        _mineCheckCount++;
    }

    public void RevealTile(bool gameOver = false)
    {
        Revealed = true;
        tileImage.sprite = revealedSprite;
        _parentBoard.UpdateRevealedCount();

        clickableObject.Clicked -= OnClick;
        clickableObject.RightClicked -= OnRightClick;

        if (Flagged)
        {
            if (gameOver)
            {
                if (Type != TileType.Mine)
                {
                    xImage.gameObject.SetActive(true);
                }
            }
            else
            {
                Flagged = false;
                flaggedImage.gameObject.SetActive(false);
                _parentBoard.UpdateFlaggedCount(false);
            }
        }

        if (Type == TileType.Numbered && !Flagged)
        { 
            mineCountText.gameObject.SetActive(true);
            mineCountText.text = _mineCheckCount.ToString();
            mineCountText.color = GetNumberColor(_mineCheckCount);
        }
        else if (Type == TileType.Mine && gameOver)
        {
            mineImage.gameObject.SetActive(true);
        }
    }

    public Color GetNumberColor(int value)
    {
        if (numberColors != null && value > 0 && numberColors.Count >= value)
        {
            return numberColors[value - 1];
        }

        return Color.black;
    }

    
    public void OnClick()
    {
        if (Flagged || Revealed)
        {
            return;
        }

        tileImage.sprite = revealedSprite;

        switch (Type)
        {
            case TileType.Mine:
                mineImage.gameObject.SetActive(true);
                _parentBoard.MineTriggerGameOver();
                break;
            case TileType.Numbered:
                _parentBoard.CheckRevealTile(RowNumber, ColumnNumber);
                break;
            case TileType.Empty:
                _parentBoard.CheckRevealTile(RowNumber, ColumnNumber);
                break;
        }

        _parentBoard.CheckIfWin();
    }

    public void OnRightClick()
    {
        if (Revealed)
        {
            return;
        }

        if (Flagged)
        {
            Flagged = false;
            flaggedImage.gameObject.SetActive(false);
            _parentBoard.UpdateFlaggedCount(false);            
        }
        else
        {
            if (_parentBoard.CanAddFlag())
            {
                Flagged = true;
                flaggedImage.gameObject.SetActive(true);
                _parentBoard.UpdateFlaggedCount(true);
                _parentBoard.CheckIfWin();
            }
        }
    }
}
