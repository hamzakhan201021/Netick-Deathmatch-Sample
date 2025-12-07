using System;
using UnityEditor;
using UnityEngine;

namespace FpsAnimationDev
{

    public class HandIKGeneratorWindow : EditorWindow
    {
        private FpsAnimationIK targetIK;

        [MenuItem("Tools/Misc/Hand IK Generator")]
        public static void Open()
        {
            GetWindow<HandIKGeneratorWindow>("Hand IK Generator");
        }

        void OnGUI()
        {
            targetIK = (FpsAnimationIK)EditorGUILayout.ObjectField("FPS IK Script", targetIK, typeof(FpsAnimationIK), true);

            if (targetIK == null) return;

            if (GUILayout.Button("Generate Hand IK Targets"))
            {
                GenerateHandTargets(targetIK._leftHandT, "LeftHandTargets");
                GenerateHandTargets(targetIK._rightHandT, "RightHandTargets");
            }
        }

        private void GenerateHandTargets(FpsAnimationIK.HandIKTargets hand, string rootName)
        {
            var root = new GameObject(rootName).transform;

            hand.HandIK = CreateChild(root, rootName + "_HandIK");

            hand.ThumbProximal = CreateChain(root, "ThumbProximal", "ThumbIntermediate", "ThumbDistal", out hand.ThumbIntermediate, out hand.ThumbDistal);
            hand.IndexProximal = CreateChain(root, "IndexProximal", "IndexIntermediate", "IndexDistal", out hand.IndexIntermediate, out hand.IndexDistal);
            hand.MiddleProximal = CreateChain(root, "MiddleProximal", "MiddleIntermediate", "MiddleDistal", out hand.MiddleIntermediate, out hand.MiddleDistal);
            hand.RingProximal = CreateChain(root, "RingProximal", "RingIntermediate", "RingDistal", out hand.RingIntermediate, out hand.RingDistal);
            hand.LittleProximal = CreateChain(root, "LittleProximal", "LittleIntermediate", "LittleDistal", out hand.LittleIntermediate, out hand.LittleDistal);
        }

        private Transform CreateChild(Transform parent, string name)
        {
            var obj = new GameObject(name).transform;
            obj.SetParent(parent);
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;
            obj.localScale = Vector3.one;
            return obj;
        }

        private Transform CreateChain(Transform parent, string proximalName, string interName, string distalName,
            out Transform intermediate, out Transform distal)
        {
            var proximal = CreateChild(parent, proximalName);
            intermediate = CreateChild(proximal, interName);
            distal = CreateChild(intermediate, distalName);
            return proximal;
        }
        

    }

}