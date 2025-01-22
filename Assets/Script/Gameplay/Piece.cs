using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Cinemachine.DocumentationSortingAttribute;

public class Piece : MonoBehaviour
{
    #region Component / Utility

    [SerializeField] private BoxCollider2D pieceCollider;
    public BoxCollider2D PieceCollider => pieceCollider;
    [SerializeField] private LayerMask blockLayer;
    private ContactFilter2D filterPieceContact;
    #endregion

    #region Movement

    private Vector3 mousePos;
    private Vector3 initPos = Vector3.zero;
    private Vector3 initScale = Vector3.one;
    public Vector3 InitPos { get => initPos; set => initPos = value; }
    public Vector3 InitScale { get => initScale; set => initScale = value; }

    #endregion

    #region Data

    private bool canPutDown;

    private ShapeData pieceShape;
    public ShapeData PieceShape { get => pieceShape; set => pieceShape = value; }

    private List<BlockData> pieceBlocksUsed = new List<BlockData>();
    public List<BlockData> PieceBlocksUsed => pieceBlocksUsed;
    
    private List<BlockData> levelBlocksUsed = new List<BlockData>();
    public List<BlockData> LevelBlocksUsed => levelBlocksUsed;

    #endregion


    private void Awake()
    {
        filterPieceContact.SetLayerMask(blockLayer);
    }

    /// <summary>
    /// Setup all the renderer of the piece's blocks and his scale
    /// </summary>
    public void InitDrag()
    {
        transform.DOScale(new Vector3(1, 1), 0.2f);

        foreach (BlockData block in pieceBlocksUsed)
        {
            block.SpRenderer.sortingOrder = 4;

            block.ShadowSpRenderer.color = Color.red;
            block.ShadowSpRenderer.gameObject.SetActive(true);
        }

        if (levelBlocksUsed.Count != 0)
        {
            foreach (BlockData levelBlock in levelBlocksUsed)
            {
                GameManager.instance.ActualLevelArray[levelBlock.RowId].columns[levelBlock.ColumnId] = true;
            }
            levelBlocksUsed.Clear();
        }
    }

    /// <summary>
    /// Check if all the piece's blocks can be put down
    /// </summary>
    public List<BlockData> CanPiecePutDown()
    {
        canPutDown = true;
        List<BlockData> nearestLevelBlocksList = new List<BlockData>();

        foreach (BlockData block in PieceBlocksUsed)
        {
            //Check all the collider that the block touch
            Collider2D[] colArrayTemp = new Collider2D[4];
            Physics2D.OverlapCollider(block.Col, filterPieceContact, colArrayTemp);

            //Calculates which of the colliders found is the closest
            float distMin = Mathf.Infinity;
            Collider2D colSave = null;
            for (int i = 0; i < colArrayTemp.Length; i++)
            {
                if (colArrayTemp[i] != null)
                {
                    //If the block touch have the Level State, are the nearest and and is not the nearest for another block
                    if (PullingManager.instance.ColliderBlockDict[colArrayTemp[i]].ActualState == ShapeState.LEVEL 
                        && Vector3.Distance(block.transform.position, colArrayTemp[i].transform.position) < distMin 
                        && !nearestLevelBlocksList.Contains(PullingManager.instance.ColliderBlockDict[colArrayTemp[i]]))
                    {
                        distMin = Vector3.Distance(block.transform.position, colArrayTemp[i].transform.position);
                        colSave = colArrayTemp[i];
                    }
                }
            }

            if (colSave == null)
            {
                canPutDown = false;
                continue;
            }

            //Check if the block are available
            if (!PullingManager.instance.ColliderBlockDict[colSave].CanPutBlockOn())
            {
                canPutDown = false;
                continue;
            }

            nearestLevelBlocksList.Add(PullingManager.instance.ColliderBlockDict[colSave]);
        }

        if (nearestLevelBlocksList.Count != pieceBlocksUsed.Count)
            canPutDown = false;

        return nearestLevelBlocksList;
    }

    /// <summary>
    /// Reset the Piece position if he can't be put down
    /// </summary>
    public void ResetPos()
    {
        transform.DOMove(initPos, 0.2f);
        transform.DOScale(initScale, 0.2f);
        
        foreach (BlockData block in pieceBlocksUsed)
        {
            block.SpRenderer.sortingOrder = 1;

            block.ShadowSpRenderer.gameObject.SetActive(false);
        }
    }

    #region Movement

    private Vector3 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        if (!GameManager.instance.IsLevelFinished)
        {
            InitDrag();
            mousePos = gameObject.transform.position - GetMousePos();
            AudioManager.instance.PlayRandom(SoundState.StartDrag);
        }
    }

    private void OnMouseDrag()
    {
        if (!GameManager.instance.IsLevelFinished)
        {
            transform.position = GetMousePos() + mousePos;
            
            //Shadow Effect
            CanPiecePutDown();

            foreach (BlockData block in pieceBlocksUsed)
            {
                if (canPutDown)
                    block.ShadowSpRenderer.color = Color.green;
                else
                    block.ShadowSpRenderer.color = Color.red;
            }
        }
    }

    private void OnMouseUp()
    {
        if (!GameManager.instance.IsLevelFinished)
        {
            AudioManager.instance.PlayRandom(SoundState.DropPiece);

            CanPiecePutDown();

            if (canPutDown)
            {
                //Get all the block that we will use and set not available anymore
                levelBlocksUsed = CanPiecePutDown();
                foreach (BlockData levelBlock in levelBlocksUsed)
                {
                    GameManager.instance.ActualLevelArray[levelBlock.RowId].columns[levelBlock.ColumnId] = false;
                }

                transform.position = GetMiddlePositionOfBlocks(levelBlocksUsed);

                //Set the piece's blocks renderer
                foreach (BlockData block in pieceBlocksUsed)
                {
                    block.SpRenderer.sortingOrder = 2;
                    block.ShadowSpRenderer.gameObject.SetActive(false);
                }

                GameManager.instance.IsLevelCompleted();
            }
            else
                ResetPos();
        }
    }

    /// <summary>
    /// Calculate the min/max position for X and Y to get the middle of all the level blocks that we use
    /// </summary>
    private Vector3 GetMiddlePositionOfBlocks(List<BlockData> allBlocks)
    {
        float posX = Mathf.Infinity;
        float posY = -Mathf.Infinity;

        for (int i = 0; i < allBlocks.Count; i++)
        {
            if (allBlocks[i].transform.position.x < posX)
                posX = allBlocks[i].transform.position.x;

            if (allBlocks[i].transform.position.y > posY)
                posY = allBlocks[i].transform.position.y;
        }

        return new Vector3(posX + (float)(pieceShape.nbrColumn -1) / 2, posY - (float)(pieceShape.nbrRow - 1) / 2);
    }
    
    #endregion

}
