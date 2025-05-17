using UnityEngine;
using UnityEditor; // Required for Editor scripts
using System.Collections.Generic;

public class DuplicatePositionRemover : EditorWindow
{
    [MenuItem("Tools/Remove Duplicate Positions")] // Adds a menu item
    public static void ShowWindow()
    {
        GetWindow<DuplicatePositionRemover>("Duplicate Remover");
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove GameObjects with Duplicate Positions", EditorStyles.boldLabel);

        if (GUILayout.Button("Remove Duplicates"))
        {
            RemoveDuplicates();
        }
    }

    private void RemoveDuplicates()
    {
        // Get all selected GameObjects
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected!");
            return;
        }

        Dictionary<Vector3, GameObject> uniquePositions = new Dictionary<Vector3, GameObject>();
        List<GameObject> duplicatesToDelete = new List<GameObject>();

        foreach (GameObject obj in selectedObjects)
        {
            Vector3 pos = obj.transform.position;

            if (uniquePositions.ContainsKey(pos))
            {
                duplicatesToDelete.Add(obj);
            }
            else
            {
                uniquePositions[pos] = obj;
            }
        }

        if (duplicatesToDelete.Count > 0)
        {
            // Delete duplicates (register undo operation)
            Undo.RegisterCompleteObjectUndo(duplicatesToDelete.ToArray(), "Remove duplicate positions");
            foreach (GameObject duplicate in duplicatesToDelete)
            {
                Undo.DestroyObjectImmediate(duplicate);
            }

            Debug.Log($"Removed {duplicatesToDelete.Count} duplicate objects.");
        }
        else
        {
            Debug.Log("No duplicates found.");
        }
    }
}