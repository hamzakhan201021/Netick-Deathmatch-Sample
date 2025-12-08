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

public class HandTargetBinderWindow : EditorWindow
{
    FpsAnimationIK targetComponent;

    Transform[] leftProximals = new Transform[5];
    Transform[] rightProximals = new Transform[5];

    [MenuItem("Tools/Misc/Hand Target Binder")]
    public static void ShowWindow()
    {
        GetWindow<HandTargetBinderWindow>("Hand Target Binder");
    }

    void OnGUI()
    {
        targetComponent = (FpsAnimationIK)EditorGUILayout.ObjectField("Target Component", targetComponent, typeof(FpsAnimationIK), true);

        EditorGUILayout.LabelField("Left Hand Proximal Fingers");
        leftProximals[0] = (Transform)EditorGUILayout.ObjectField("Thumb Proximal", leftProximals[0], typeof(Transform), true);
        leftProximals[1] = (Transform)EditorGUILayout.ObjectField("Index Proximal", leftProximals[1], typeof(Transform), true);
        leftProximals[2] = (Transform)EditorGUILayout.ObjectField("Middle Proximal", leftProximals[2], typeof(Transform), true);
        leftProximals[3] = (Transform)EditorGUILayout.ObjectField("Ring Proximal", leftProximals[3], typeof(Transform), true);
        leftProximals[4] = (Transform)EditorGUILayout.ObjectField("Little Proximal", leftProximals[4], typeof(Transform), true);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Right Hand Proximal Fingers");
        rightProximals[0] = (Transform)EditorGUILayout.ObjectField("Thumb Proximal", rightProximals[0], typeof(Transform), true);
        rightProximals[1] = (Transform)EditorGUILayout.ObjectField("Index Proximal", rightProximals[1], typeof(Transform), true);
        rightProximals[2] = (Transform)EditorGUILayout.ObjectField("Middle Proximal", rightProximals[2], typeof(Transform), true);
        rightProximals[3] = (Transform)EditorGUILayout.ObjectField("Ring Proximal", rightProximals[3], typeof(Transform), true);
        rightProximals[4] = (Transform)EditorGUILayout.ObjectField("Little Proximal", rightProximals[4], typeof(Transform), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Auto Bind Hand Targets"))
        {
            BindHandTargets();
        }
    }

    void BindHandTargets()
    {
        if (targetComponent == null) return;

        FpsAnimationIK.HandIKTargets leftT = targetComponent._leftHandT;
        FpsAnimationIK.HandIKTargets rightT = targetComponent._rightHandT;

        AssignFingers(leftT, leftProximals);
        AssignFingers(rightT, rightProximals);

        EditorUtility.SetDirty(targetComponent);
    }

    void AssignFingers(FpsAnimationIK.HandIKTargets t, Transform[] proximals)
    {
        t.ThumbProximal = proximals[0];
        t.IndexProximal = proximals[1];
        t.MiddleProximal = proximals[2];
        t.RingProximal = proximals[3];
        t.LittleProximal = proximals[4];

        t.ThumbIntermediate = GetChild(proximals[0], 0);
        t.ThumbDistal = GetChild(t.ThumbIntermediate, 0);

        t.IndexIntermediate = GetChild(proximals[1], 0);
        t.IndexDistal = GetChild(t.IndexIntermediate, 0);

        t.MiddleIntermediate = GetChild(proximals[2], 0);
        t.MiddleDistal = GetChild(t.MiddleIntermediate, 0);

        t.RingIntermediate = GetChild(proximals[3], 0);
        t.RingDistal = GetChild(t.RingIntermediate, 0);

        t.LittleIntermediate = GetChild(proximals[4], 0);
        t.LittleDistal = GetChild(t.LittleIntermediate, 0);

        t.HandIK = proximals[1].parent; // assume HandIK is the parent of IndexProximal
    }

    Transform GetChild(Transform parent, int index)
    {
        if (parent == null || parent.childCount <= index) return null;
        return parent.GetChild(index);
    }
}

[CustomEditor(typeof(FpsAnimationIK))]
public class HandAutoBinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FpsAnimationIK t = (FpsAnimationIK)target;

        if (GUILayout.Button("Auto Bind Left Hand"))
        {
            AutoBind(t, true);
        }

        if (GUILayout.Button("Auto Bind Right Hand"))
        {
            AutoBind(t, false);
        }

        if (GUILayout.Button("Auto Bind Both"))
        {
            AutoBind(t, true);
            AutoBind(t, false);
        }
    }

    void AutoBind(FpsAnimationIK c, bool left)
    {
        Animator a = c.GetComponent<Animator>();
        if (a == null) return;

        FpsAnimationIK.HandIKTargets h = left ? c._leftHand : c._rightHand;

        h.HandIK = a.GetBoneTransform(left ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);

        h.ThumbProximal = a.GetBoneTransform(left ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal);
        h.ThumbIntermediate = a.GetBoneTransform(left ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate);
        h.ThumbDistal = a.GetBoneTransform(left ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal);

        h.IndexProximal = a.GetBoneTransform(left ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal);
        h.IndexIntermediate = a.GetBoneTransform(left ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate);
        h.IndexDistal = a.GetBoneTransform(left ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal);

        h.MiddleProximal = a.GetBoneTransform(left ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal);
        h.MiddleIntermediate = a.GetBoneTransform(left ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate);
        h.MiddleDistal = a.GetBoneTransform(left ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal);

        h.RingProximal = a.GetBoneTransform(left ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal);
        h.RingIntermediate = a.GetBoneTransform(left ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate);
        h.RingDistal = a.GetBoneTransform(left ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal);

        h.LittleProximal = a.GetBoneTransform(left ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal);
        h.LittleIntermediate = a.GetBoneTransform(left ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate);
        h.LittleDistal = a.GetBoneTransform(left ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal);

        EditorUtility.SetDirty(c);
    }
}

public class UppercaseChildNamesWindow : EditorWindow
{
    private GameObject rootObject;

    [MenuItem("Tools/Misc/Uppercase Child Names")]
    public static void ShowWindow()
    {
        GetWindow<UppercaseChildNamesWindow>("Uppercase Child Names");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Root Object", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);

        if (rootObject != null)
        {
            if (GUILayout.Button("Uppercase All Children Names"))
            {
                Undo.RegisterFullObjectHierarchyUndo(rootObject, "Uppercase Child Names");
                UppercaseNamesRecursive(rootObject.transform);
                EditorUtility.SetDirty(rootObject);
                Debug.Log("All child names uppercased!");
            }
        }
        else
        {
            GUILayout.Label("Please assign a root GameObject.");
        }
    }

    private void UppercaseNamesRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.name = child.name.ToUpper();
            UppercaseNamesRecursive(child);
        }
    }
}