using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;using UnityEngine.EventSystems;

public class BlockData : MonoBehaviour
{
    #region Component & Data

    [Header("General")]
    [SerializeField] private SpriteRenderer spRenderer;
    [SerializeField] private Collider2D col;

    public Collider2D Col => col;
    public SpriteRenderer SpRenderer => spRenderer;

    private ShapeState actualState = ShapeState.NOTHING;
    public ShapeState ActualState { get => actualState; set => actualState = value; }
    
    #endregion

    #region Piece
    [Header("Piece")]

    [SerializeField] private SpriteRenderer shadowSpRenderer;
    public SpriteRenderer ShadowSpRenderer => shadowSpRenderer;

    #endregion

    #region Level

    private int rowID, columnID;
    public int RowId { get => rowID; set => rowID = value; }
    public int ColumnId { get => columnID; set => columnID = value; }

    #endregion

    public bool CanPutBlockOn()
    {
        return GameManager.instance.ActualLevelArray[rowID].columns[columnID];
    }
}
