using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class WrapWithSortingGroupEditor : EditorWindow
{
    private float yOffset = -0.18f;

    [MenuItem("Tools/Wrap With SortingGroup (GUI)")]
    public static void ShowWindow()
    {
        GetWindow<WrapWithSortingGroupEditor>("Wrap + SortingGroup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Wrap Objects and Add SortingGroup", EditorStyles.boldLabel);
        yOffset = EditorGUILayout.FloatField("Y Offset", yOffset);

        if (GUILayout.Button("Apply to Selected Objects"))
        {
            ApplyWrapping();
        }
    }

    private void ApplyWrapping()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == null) continue;

            Vector3 originalPos = obj.transform.position;
            Vector3 parentPos = new Vector3(originalPos.x, originalPos.y - yOffset, originalPos.z);
            Vector3 childLocalPos = new Vector3(0f, yOffset, 0f);

            // Create wrapper
            GameObject wrapper = new GameObject(obj.name + "_Group");
            wrapper.transform.position = parentPos;

            // Add SortingGroup and assign layer/order
            SortingGroup group = wrapper.AddComponent<SortingGroup>();
            group.sortingLayerName = "grass";
            group.sortingOrder = 10;

            Undo.RegisterCreatedObjectUndo(wrapper, "Create Wrapper");

            // Reparent and adjust local position
            Undo.SetTransformParent(obj.transform, wrapper.transform, "Reparent Object");
            obj.transform.localPosition = childLocalPos;
        }

        Debug.Log("Wrap and SortingGroup applied.");
    }
}
