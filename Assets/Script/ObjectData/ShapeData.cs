using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class ShapeData : ScriptableObject
{
    public int nbrRow;
    public int nbrColumn;

    public Row[] rows;

    public void CreateNewShape()
    {
        rows = new Row[nbrRow];

        for (int i = 0; i < nbrRow; i++)
        {
            rows[i] = new Row(nbrColumn);
        }
    }
}

[System.Serializable]
public class Row
{
    public bool[] columns;

    public Row(int size)
    {
        columns = new bool[size];
    }
}