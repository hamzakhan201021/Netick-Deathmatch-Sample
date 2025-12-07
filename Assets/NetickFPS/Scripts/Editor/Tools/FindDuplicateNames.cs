using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindDuplicateNames : EditorWindow
{
    [MenuItem("Tools/Misc/Find Duplicate Names")]
    public static void ShowWindow()
    {
        GetWindow<FindDuplicateNames>("Find Duplicates");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scan Scene for Duplicate Names"))
        {
            FindDuplicates();
        }
    }

    private static void FindDuplicates()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        Dictionary<string, List<GameObject>> nameDict = new Dictionary<string, List<GameObject>>();

        foreach (var go in allObjects)
        {
            if (!nameDict.ContainsKey(go.name))
                nameDict[go.name] = new List<GameObject>();

            nameDict[go.name].Add(go);
        }

        bool foundAny = false;

        foreach (var kvp in nameDict)
        {
            if (kvp.Value.Count > 1)
            {
                foundAny = true;
                Debug.Log($"Duplicate name: \"{kvp.Key}\" - Count: {kvp.Value.Count}");
                foreach (var obj in kvp.Value)
                    Debug.Log($"    {obj.name} in hierarchy path: {GetHierarchyPath(obj)}");
            }
        }

        if (!foundAny)
            Debug.Log("No duplicate names found in the scene.");
    }

    private static string GetHierarchyPath(GameObject go)
    {
        string path = go.name;
        Transform current = go.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }
}

public class RotationComparer : EditorWindow
{
    private Vector3? firstRotation = null;
    private Vector3? secondRotation = null;
    private Vector3 result = Vector3.zero;

    [MenuItem("Tools/Misc/Rotation Comparer")]
    public static void ShowWindow()
    {
        GetWindow<RotationComparer>("Rotation Comparer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a GameObject in the Hierarchy", EditorStyles.boldLabel);

        if (GUILayout.Button("Store First Rotation"))
        {
            StoreRotation(ref firstRotation);
        }

        if (GUILayout.Button("Store Second Rotation"))
        {
            StoreRotation(ref secondRotation);
        }

        GUILayout.Space(10);

        GUILayout.Label("Stored Rotations:", EditorStyles.boldLabel);
        GUILayout.Label("First: " + (firstRotation.HasValue ? firstRotation.Value.ToString("F3") : "None"));
        GUILayout.Label("Second: " + (secondRotation.HasValue ? secondRotation.Value.ToString("F3") : "None"));

        GUILayout.Space(10);

        if (firstRotation.HasValue && secondRotation.HasValue)
        {
            if (GUILayout.Button("Subtract (Second - First)"))
            {
                result = secondRotation.Value - firstRotation.Value;
            }

            GUILayout.Label("Result: " + result.ToString("F3"));
        }
    }

    private void StoreRotation(ref Vector3? storage)
    {
        if (Selection.activeGameObject != null)
        {
            // storage = Selection.activeGameObject.transform.localEulerAngles;
            storage = GetInspectorLocalEulerAngles(Selection.activeGameObject);
            Debug.Log("Stored rotation: " + storage.Value.ToString("F3"));
        }
        else
        {
            Debug.LogWarning("No GameObject selected!");
        }
    }

    public static Vector3 GetInspectorLocalEulerAngles(GameObject go)
    {
        Vector3 euler = go.transform.localEulerAngles;

        euler.x = GetNormalizedAngle(euler.x);
        euler.y = GetNormalizedAngle(euler.y);
        euler.z = GetNormalizedAngle(euler.z);

        return euler;
    }

    // public static float GetNormalizedAngle(float angle)
    // {
    //     if (angle > 180)
    //     {
    //         Debug.Log("Removed 360");
    //         angle -= 360;
    //         GetNormalizedAngle(angle);
    //     }
    //     else if (angle < 180) 
    //     {
    //         Debug.Log("Added 360");
    //         angle += 360;
    //         GetNormalizedAngle(angle);
    //     }

    //     return angle;
    // }
    public static float GetNormalizedAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}


public class FingerCopyWindow : EditorWindow
{
    public Transform thumbRoot;
    public Transform indexRoot;
    public Transform middleRoot;
    public Transform ringRoot;
    public Transform littleRoot;

    public FpsAnimationIK targetScript;
    Vector3[] buffer = new Vector3[15];

    [MenuItem("Tools/Misc/Finger Copy Window")]
    static void Open() => GetWindow<FingerCopyWindow>("Finger Copier");

    void OnGUI()
    {
        thumbRoot = (Transform)EditorGUILayout.ObjectField("Thumb Root", thumbRoot, typeof(Transform), true);
        indexRoot = (Transform)EditorGUILayout.ObjectField("Index Root", indexRoot, typeof(Transform), true);
        middleRoot = (Transform)EditorGUILayout.ObjectField("Middle Root", middleRoot, typeof(Transform), true);
        ringRoot = (Transform)EditorGUILayout.ObjectField("Ring Root", ringRoot, typeof(Transform), true);
        littleRoot = (Transform)EditorGUILayout.ObjectField("Little Root", littleRoot, typeof(Transform), true);

        targetScript = (FpsAnimationIK)EditorGUILayout.ObjectField("Target Script", targetScript, typeof(FpsAnimationIK), true);

        if (GUILayout.Button("Copy From Assigned Fingers")) Copy();
        if (GUILayout.Button("Paste To Left Offsets")) Paste(targetScript._leftOffsets);
        if (GUILayout.Button("Paste To Right Offsets")) Paste(targetScript._rightOffsets);
    }

    void Copy()
    {
        Transform[] roots =
        {
            thumbRoot,
            indexRoot,
            middleRoot,
            ringRoot,
            littleRoot
        };

        int b = 0;
        for (int i = 0; i < 5; i++)
        {
            var p = roots[i];
            var m = p.GetChild(0);
            var d = m.GetChild(0);

            buffer[b++] = p.localEulerAngles;
            buffer[b++] = m.localEulerAngles;
            buffer[b++] = d.localEulerAngles;
        }
    }

    void Paste(FpsAnimationIK.FingerOffset o)
    {
        Undo.RecordObject(targetScript, "Paste Finger Offsets");

        o.ThumbProximal = buffer[0];
        o.ThumbIntermediate = buffer[1];
        o.ThumbDistal = buffer[2];

        o.IndexProximal = buffer[3];
        o.IndexIntermediate = buffer[4];
        o.IndexDistal = buffer[5];

        o.MiddleProximal = buffer[6];
        o.MiddleIntermediate = buffer[7];
        o.MiddleDistal = buffer[8];

        o.RingProximal = buffer[9];
        o.RingIntermediate = buffer[10];
        o.RingDistal = buffer[11];

        o.LittleProximal = buffer[12];
        o.LittleIntermediate = buffer[13];
        o.LittleDistal = buffer[14];

        EditorUtility.SetDirty(targetScript);
    }
}