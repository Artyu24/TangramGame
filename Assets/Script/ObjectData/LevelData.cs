using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    [SerializeField] private ShapeData levelShape;
    [SerializeField] private ShapeData[] pieceShapeArray;

    public ShapeData LevelShape => levelShape;
    public ShapeData[] PieceShapeArray => pieceShapeArray;
}
