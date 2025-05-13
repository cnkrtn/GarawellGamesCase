using System.Collections.Generic;
using Core.GridService.Data;
using UnityEditor;
using UnityEngine;

public class LevelDesignerWindow : EditorWindow
{
    private const int CellGridSize = 4;
    private bool[,] _selected = new bool[CellGridSize, CellGridSize];
    private Vector2 _scroll;
    private LevelData _levelData;

    [MenuItem("Tools/Level Designer")]
    public static void ShowWindow() => GetWindow<LevelDesignerWindow>("Level Designer");

    void OnGUI()
    {
        GUILayout.Label("Level Data Asset", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _levelData = (LevelData)EditorGUILayout.ObjectField(_levelData, typeof(LevelData), false);
        if (GUILayout.Button("Create New", GUILayout.MaxWidth(100)))
            CreateNewLevelData();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Select Cells to Pre-Fill", EditorStyles.boldLabel);
        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(CellGridSize * 34));
        for (int y = CellGridSize - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < CellGridSize; x++)
                _selected[x, y] = GUILayout.Toggle(_selected[x, y], GUIContent.none, "Button", GUILayout.Width(30), GUILayout.Height(30));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        using (new EditorGUI.DisabledScope(_levelData == null))
            if (GUILayout.Button("Apply to Level Data"))
            {
                ApplyToLevelData();
                EditorUtility.SetDirty(_levelData);
                AssetDatabase.SaveAssets();
            }
    }

    void CreateNewLevelData()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "New Level Data", "LevelData", "asset", "Enter a file name");
        if (string.IsNullOrEmpty(path)) return;

        var asset = ScriptableObject.CreateInstance<LevelData>();
        asset.closedCells = new List<Vector2Int>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        _levelData = asset;
    }

    void ApplyToLevelData()
    {
        _levelData.closedCells = new List<Vector2Int>();
        for (int x = 0; x < CellGridSize; x++)
            for (int y = 0; y < CellGridSize; y++)
                if (_selected[x, y])
                    _levelData.closedCells.Add(new Vector2Int(x, y));
    }
}
