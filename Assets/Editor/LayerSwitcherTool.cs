using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class LayerSwitcherTool : EditorWindow
{
    public enum Mode { ReparentOriginal, ReplaceWithPrefab }

    private Mode selectedMode = Mode.ReparentOriginal;
    private GameObject replacementPrefab;
    private float offsetY = 0.5f;

    [MenuItem("Tools/Layer Switcher Tool")]
    public static void ShowWindow()
    {
        GetWindow<LayerSwitcherTool>("Layer Switcher Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Layer Switcher Settings", EditorStyles.boldLabel);
        selectedMode = (Mode)EditorGUILayout.EnumPopup("Mode", selectedMode);
        offsetY = EditorGUILayout.FloatField("Y Offset", offsetY);

        if (selectedMode == Mode.ReplaceWithPrefab)
        {
            replacementPrefab = (GameObject)EditorGUILayout.ObjectField("Replacement Prefab", replacementPrefab, typeof(GameObject), false);
        }

        if (GUILayout.Button("Process Selected Objects"))
        {
            if (selectedMode == Mode.ReplaceWithPrefab && replacementPrefab == null)
            {
                EditorUtility.DisplayDialog("Missing Prefab", "Please assign a replacement prefab.", "OK");
                return;
            }

            ProcessSelectedObjects();
        }
    }

    private void ProcessSelectedObjects()
    {
        foreach (GameObject original in Selection.gameObjects)
        {
            Vector3 originalWorldPos = original.transform.position;
            Vector3 parentWorldPos = new Vector3(originalWorldPos.x, originalWorldPos.y - offsetY, originalWorldPos.z);

            // Create wrapper parent
            GameObject wrapper = new GameObject(original.name + "_Group");
            wrapper.transform.position = parentWorldPos;
            wrapper.AddComponent<SortingGroup>();
            Undo.RegisterCreatedObjectUndo(wrapper, "Create Wrapper");

            GameObject child;

            if (selectedMode == Mode.ReplaceWithPrefab)
            {
                // Instantiate replacement prefab
                child = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);
                child.name = original.name;
                Undo.RegisterCreatedObjectUndo(child, "Instantiate Replacement");

                // Set under wrapper and position
                child.transform.SetParent(wrapper.transform);
                child.transform.localPosition = new Vector3(0f, offsetY, 0f);

                // Destroy original
                Undo.DestroyObjectImmediate(original);
            }
            else // Reparent original
            {
                Undo.SetTransformParent(original.transform, wrapper.transform, "Reparent Object");
                original.transform.localPosition = new Vector3(0f, offsetY, 0f);
                child = original;
            }

            // Attach script and assign colliders
            var switcher = Undo.AddComponent<SpriteLayerSwitcher2>(wrapper);
            var colliders = child.GetComponents<Collider2D>();
            if (colliders.Length >= 2)
            {
                SerializedObject so = new SerializedObject(switcher);
                so.FindProperty("above").objectReferenceValue = colliders[0];
                so.FindProperty("below").objectReferenceValue = colliders[1];
                so.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"'{child.name}' has fewer than 2 Collider2D components.");
            }
        }
    }
}
