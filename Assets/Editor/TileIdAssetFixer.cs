using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TileIdAssetFixer
{
    [MenuItem("Tools/Fix ShapeData tileId Values")]
    public static void Run()
    {
        // adjust this mapping to your new enum order:
        // oldInt → newInt
        var map = new Dictionary<int,int>
        {
            { 5, 4 },
            { 6, 5 },
            { 7, 6 },
            { 8, 7 },
            // …etc for every shifted value…
        };

        string[] files = Directory.GetFiles("Assets/ShapeData", "*.asset", SearchOption.AllDirectories);
        int fixedCount = 0;

        foreach (var path in files)
        {
            string text = File.ReadAllText(path);
            bool changed = false;

            foreach (var kv in map)
            {
                string pattern = $"tileId: {kv.Key}\\b";
                string replace = $"tileId: {kv.Value}";
                if (Regex.IsMatch(text, pattern))
                {
                    text = Regex.Replace(text, pattern, replace);
                    changed = true;
                }
            }

            if (changed)
            {
                File.WriteAllText(path, text);
                fixedCount++;
                Debug.Log($"Patched {Path.GetFileName(path)}");
            }
        }

        if (fixedCount > 0)
            AssetDatabase.Refresh();

        Debug.Log($"ShapeData Fixer: {fixedCount} files updated.");
    }
}