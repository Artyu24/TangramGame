using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Piece Parameters

    [Header("Piece"),SerializeField] private float xPieceOffSet = 1.5f; 
    [SerializeField] private float yPieceOffSet = 2f;
    [SerializeField] private Vector3 pieceScale;
    [SerializeField] private List<Color> colorList;

    #endregion

    #region Level Parameters

    [Header("Levels"),SerializeField] private LevelData[] levelDataArray;
    private bool isLevelFinished = true;
    public bool IsLevelFinished { get => isLevelFinished; set => isLevelFinished = value; }

    #endregion

    #region Current level variable

    private int actualLevelID = 0;
    private LevelData actualLevelData;
    private Row[] actualLevelArray;
    public Row[] ActualLevelArray => actualLevelArray;

    private List<BlockData> allBlockUsed = new List<BlockData>();
    private List<Piece> allPieceUsed = new List<Piece>();
    
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        //Save the actual level
        if (PlayerPrefs.HasKey("levelID"))
            actualLevelID = PlayerPrefs.GetInt("levelID");
        else
            PlayerPrefs.SetInt("levelID", 0);
    }

    private void Start()
    {
        StartNextLevel();
    }

    /// <summary>
    /// Start the next Level and Setup it
    /// </summary>
    public void StartNextLevel()
    {
        InterfaceManager.instance.SetTextLevel(actualLevelID);
        InterfaceManager.instance.TriggerStartScreen();

        actualLevelData = levelDataArray[actualLevelID];
        PrintLevel(actualLevelData);
    }

    /// <summary>
    /// Print all the levels with the piece & the camera management
    /// </summary>
    private void PrintLevel(LevelData levelData)
    {

        #region Print Level

        Vector2 posLvTemp = Vector2.zero;

        for (int i = 0; i < levelData.LevelShape.nbrRow; i++)
        {
            posLvTemp.x = 0;
            
            for (int j = 0; j < levelData.LevelShape.nbrColumn; j++)
            {
                BlockData blockDataTakenTemp = PullingManager.instance.PickABlock();

                #region Setup Block

                blockDataTakenTemp.RowId = i;
                blockDataTakenTemp.ColumnId = j;

                if (levelData.LevelShape.rows[i].columns[j])
                    blockDataTakenTemp.ActualState = ShapeState.LEVEL;
                else
                    blockDataTakenTemp.ActualState = ShapeState.OBSTRUCT;

                blockDataTakenTemp.transform.parent = transform;
                blockDataTakenTemp.transform.position = posLvTemp;
                SetupBlock(blockDataTakenTemp);
                
                allBlockUsed.Add(blockDataTakenTemp);
                #endregion

                posLvTemp.x ++;
            }

            posLvTemp.y -= 1;
        }
        
        posLvTemp.x --;

        #endregion

        //Clone the scriptable object for get all the multidimensional boolean array
        ShapeData levelShapeClone = Instantiate(levelData.LevelShape);
        actualLevelArray = levelShapeClone.rows;

        #region Print & Manage Piece

        #region Print Piece

        List<Color> allColor = new List<Color>(colorList);

        for (int i = 0; i < actualLevelData.PieceShapeArray.Length; i++)
        {
            Color colorPicked = allColor[Random.Range(0, allColor.Count)];
            PrintPiece(actualLevelData.PieceShapeArray[i], colorPicked); 
            allColor.Remove(colorPicked);
        }
        
        #endregion

        #region Placement of piece

        PiecePosState actualPiecePosState = PiecePosState.MIDDLE;

        for (int i = 0; i < allPieceUsed.Count; i++)
        {
            Vector3 posTemp = GetNextPiecePos(ref actualPiecePosState, new Vector3(posLvTemp.x / 2, posLvTemp.y - 1));
            allPieceUsed[i].transform.position = posTemp;
            allPieceUsed[i].InitPos = posTemp;

            if(actualPiecePosState == PiecePosState.MIDDLE)
                posLvTemp.y -= yPieceOffSet;
        }

        if (actualPiecePosState == PiecePosState.MIDDLE)
            posLvTemp.y += yPieceOffSet;

        #endregion

        #endregion

        #region Camera Management

        //Setup Camera
        float xMin = posLvTemp.x / 2 - xPieceOffSet;
        float xMax = levelData.LevelShape.nbrColumn - 1 < posLvTemp.x / 2 + xPieceOffSet ? posLvTemp.x / 2 + xPieceOffSet : levelData.LevelShape.nbrColumn - 1;

        //Si il n'existe qu'une seule piece
        if (allPieceUsed.Count == 1)
        {
            xMin = -0.5f;
            xMax = levelData.LevelShape.nbrColumn - 0.5f;
        }

        CameraManager.instance.SetupCamera(xMin, xMax, posLvTemp.y - yPieceOffSet, 0.5f);
        
        #endregion
    }

    /// <summary>
    /// Print Parent Piece
    /// </summary>
    private void PrintPiece(ShapeData shapeDataTemp, Color colorPicked)
    {
        #region Setup Parent Piece from pulling

        Piece pieceTemp = PullingManager.instance.PickParentPiece();
        pieceTemp.PieceShape = shapeDataTemp;
        pieceTemp.PieceCollider.size = new Vector2(shapeDataTemp.nbrColumn, shapeDataTemp.nbrRow);

        pieceTemp.transform.parent = null;
        pieceTemp.gameObject.SetActive(true);
        pieceTemp.transform.localScale = pieceScale;
        pieceTemp.InitScale = pieceScale;

        allPieceUsed.Add(pieceTemp);
        
        #endregion

        Vector2 posTemp = new Vector2((float)-(shapeDataTemp.nbrColumn - 1) / 2, (float)(shapeDataTemp.nbrRow - 1) / 2);

        for (int i = 0; i < shapeDataTemp.nbrRow; i++)
        {
            posTemp.x = (float)-(shapeDataTemp.nbrColumn - 1) / 2;

            for (int j = 0; j < shapeDataTemp.nbrColumn; j++)
            {
                if (shapeDataTemp.rows[i].columns[j])
                {
                    #region Setup Block

                    BlockData blockDataTakenTemp = PullingManager.instance.PickABlock();
                    blockDataTakenTemp.ActualState = ShapeState.PIECE;

                    blockDataTakenTemp.transform.parent = pieceTemp.transform;
                    blockDataTakenTemp.transform.localPosition = posTemp;
                    blockDataTakenTemp.SpRenderer.color = colorPicked;

                    SetupBlock(blockDataTakenTemp);

                    pieceTemp.PieceBlocksUsed.Add(blockDataTakenTemp);
                    allBlockUsed.Add(blockDataTakenTemp);
                    
                    #endregion
                }

                posTemp.x++;
            }
            posTemp.y -= 1;
        }
    }

    /// <summary>
    /// Setup the block according to its state
    /// </summary>
    private void SetupBlock(BlockData blockDataTemp)
    {
        blockDataTemp.gameObject.SetActive(true);
        blockDataTemp.transform.localScale = Vector3.one;

        switch (blockDataTemp.ActualState)
        {
            case ShapeState.LEVEL:
                blockDataTemp.SpRenderer.color = Color.white;
                blockDataTemp.SpRenderer.sortingOrder = 0;
                break;
            case ShapeState.OBSTRUCT:
                blockDataTemp.SpRenderer.color = new Color((float)48 /255, (float)48 /255,(float)48/255,255);
                blockDataTemp.SpRenderer.sortingOrder = 0;
                break;
            case ShapeState.PIECE:
                blockDataTemp.SpRenderer.sortingOrder = 1;
                break;
        }
    }

    /// <summary>
    /// Return the position of the piece according to the state
    /// </summary>
    private Vector3 GetNextPiecePos(ref PiecePosState posState, Vector3 pos)
    {
        switch (posState)
        {
            case PiecePosState.MIDDLE:
                posState = PiecePosState.LEFT;
                break;
            case PiecePosState.LEFT:
                pos.x -= xPieceOffSet;
                posState = PiecePosState.RIGHT;
                break;
            case PiecePosState.RIGHT:
                pos.x += xPieceOffSet;
                posState = PiecePosState.MIDDLE;
                break;
        }

        return pos;
    }


    public void IsLevelCompleted()
    {
        //Check is all the blocks are used
        for (int i = 0; i < actualLevelData.LevelShape.nbrRow; i++)
        {
            for (int j = 0; j < actualLevelData.LevelShape.nbrColumn; j++)
            {
                if (actualLevelArray[i].columns[j])
                    return;
            }
        }

        //Level Change
        isLevelFinished = true;
        actualLevelID++;
        
        AudioManager.instance.PlayRandom(SoundState.Victory);

        //If there is no new level
        if (actualLevelID >= levelDataArray.Length)
        {
            InterfaceManager.instance.TriggerEndWindow();
            return;
        }

        InterfaceManager.instance.TriggerVictoryScreen();

        PlayerPrefs.SetInt("levelID", actualLevelID);
    }

    /// <summary>
    /// Reset all the blocks & parent piece used AND reset all actual Game Data
    /// </summary>
    private void ResetAllGame()
    {
        actualLevelData = null;
        actualLevelArray = null;

        foreach (BlockData block in allBlockUsed)
        {
            PullingManager.instance.PullBlock(block);
        }
        allBlockUsed.Clear();

        foreach (Piece piece in allPieceUsed)
        {
            PullingManager.instance.PullParentPiece(piece);
        }
        allPieceUsed.Clear();
    }

    public void RestartGame()
    {
        ResetAllGame();
        StartNextLevel();
    }
}