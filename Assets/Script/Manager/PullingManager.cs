using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullingManager : MonoBehaviour
{
    public static PullingManager instance;

    private GameObject blockPrefab;
    private Stack<BlockData> blockPullingStack = new Stack<BlockData>();
    
    private GameObject piecePrefab;
    private Stack<Piece> piecePullingStack = new Stack<Piece>();

    private Dictionary<Collider2D, BlockData> colliderBlockDict = new Dictionary<Collider2D, BlockData>();
    public Dictionary<Collider2D, BlockData> ColliderBlockDict => colliderBlockDict;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        blockPrefab = Resources.Load<GameObject>("Block");
        piecePrefab = Resources.Load<GameObject>("Piece");

        //Init block Stack
        for (int i = 0; i < 50; i++)
        {
            GameObject blockObjectTemp = Instantiate(blockPrefab, transform);
            blockObjectTemp.SetActive(false);

            BlockData blockDataTemp = blockObjectTemp.GetComponent<BlockData>();
            blockPullingStack.Push(blockDataTemp);
            colliderBlockDict.Add(blockDataTemp.Col, blockDataTemp);
        }
    }

    /// <summary>
    /// Pick a Block from the Stack
    /// </summary>
    public BlockData PickABlock()
    {
        BlockData blockDataTemp = null;

        if (blockPullingStack.Count != 0)
            blockDataTemp = blockPullingStack.Pop();
        else
        {
            blockDataTemp = Instantiate(blockPrefab, transform).GetComponent<BlockData>();
            colliderBlockDict.Add(blockDataTemp.Col, blockDataTemp);
        }

        return blockDataTemp;
    }

    /// <summary>
    /// Returns the block to the Stack with a Reset
    /// </summary>
    public void PullBlock(BlockData blockDataTemp)
    {
        //reset
        blockDataTemp.ActualState = ShapeState.NOTHING;
        blockDataTemp.SpRenderer.color = Color.white;
        blockDataTemp.RowId = 0;
        blockDataTemp.ColumnId = 0;

        blockDataTemp.transform.parent = transform;
        blockDataTemp.gameObject.SetActive(false);

        blockPullingStack.Push(blockDataTemp);
    }

    /// <summary>
    /// Pick a Parent Piece from the Stack
    /// </summary>
    public Piece PickParentPiece()
    {
        Piece pieceTemp = null;

        if (piecePullingStack.Count != 0)
            pieceTemp = piecePullingStack.Pop();
        else
            pieceTemp = Instantiate(piecePrefab, transform).GetComponent<Piece>();

        return pieceTemp;
    }

    /// <summary>
    /// Returns the piece to the Stack with a Reset
    /// </summary>
    public void PullParentPiece(Piece pieceTemp)
    {
        //reset
        pieceTemp.transform.localScale = pieceTemp.InitScale;
        pieceTemp.PieceShape = null;
        pieceTemp.PieceBlocksUsed.Clear();
        pieceTemp.LevelBlocksUsed.Clear();

        pieceTemp.transform.parent = transform;
        pieceTemp.gameObject.SetActive(false);

        piecePullingStack.Push(pieceTemp);
    }
}
