using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShapeData))]
public class ShapeDataDrawer : Editor
{
    private ShapeData myObject => target as ShapeData;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("-------Shape Creation Tool-------");
        EditorGUILayout.HelpBox("Step to follow:\nDefine the number of rows required\nDefine the number of columns per row\nCheck the boxes where you wish to have a cube", MessageType.Info);
        EditorGUILayout.EndVertical();

        serializedObject.Update();

        int tempNbrRow = myObject.nbrRow;
        int tempNbrColumn = myObject.nbrColumn;

        myObject.nbrRow = EditorGUILayout.IntField("Row", myObject.nbrRow);
        myObject.nbrColumn = EditorGUILayout.IntField("Column", myObject.nbrColumn);

        if (myObject.nbrRow > 0 && myObject.nbrColumn > 0)
        {
            if (myObject.nbrColumn != tempNbrColumn || myObject.nbrRow != tempNbrRow)
            {
                myObject.CreateNewShape();
            }

            #region GUI Style
            var columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 250;
            columnStyle.alignment = TextAnchor.MiddleCenter;

            var rowStyle = new GUIStyle();
            rowStyle.fixedHeight = 25;
            rowStyle.alignment = TextAnchor.MiddleCenter;

            var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
            dataFieldStyle.normal.background = Texture2D.grayTexture;
            dataFieldStyle.onNormal.background = Texture2D.whiteTexture;
            #endregion

            for (int i = 0; i < myObject.nbrRow; i++)
            {
                EditorGUILayout.BeginHorizontal(columnStyle);

                for (int j = 0; j < myObject.nbrColumn; j++)
                {
                    EditorGUILayout.BeginHorizontal(rowStyle);

                    try
                    {
                        myObject.rows[i].columns[j] = EditorGUILayout.Toggle(myObject.rows[i].columns[j], dataFieldStyle);
                    }
                    catch (Exception)
                    {
                        myObject.CreateNewShape();
                    }

                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear"))
                UseAllBoard(false);

            if (GUILayout.Button("All"))
                UseAllBoard(true);

            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(myObject);
        serializedObject.ApplyModifiedProperties();
    }

    private void UseAllBoard(bool enable)
    {
        for (int i = 0; i < myObject.nbrRow; i++)
        {
            for (int j = 0; j < myObject.nbrColumn; j++)
            {
                myObject.rows[i].columns[j] = enable;
            }
        }
    }
}
