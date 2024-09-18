using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeFSM))]
public class NodeFSMEditor : Editor
{
    private SerializedProperty _neighboursProperty;
    private SerializedProperty _animalTypeProperty;

    private SerializedProperty _kittyHouseTransformProperty;
    private SerializedProperty _kittyHouseTmpTextProperty;
    private SerializedProperty _kittyHouseMeshRenderertProperty;

    private SerializedProperty _moleParentTransformProperty;
    private SerializedProperty _moleTransformProperty;
    private SerializedProperty _moleBlockProperty;

    private SerializedProperty _cryingKittyTransformProperty;
    private SerializedProperty _cryingKittyTmpTextProperty;
    private SerializedProperty _cryingRoundAmountProperty;


    private void OnEnable()
    {
        _neighboursProperty = serializedObject.FindProperty("Neighbours");

        _animalTypeProperty = serializedObject.FindProperty("AnimalType");

        _kittyHouseTransformProperty = serializedObject.FindProperty("KittyHouseTransform");
        _kittyHouseTmpTextProperty = serializedObject.FindProperty("KittyHouseTmpText");
        _kittyHouseMeshRenderertProperty = serializedObject.FindProperty("KittyHouseMeshRenderer");

        _moleParentTransformProperty = serializedObject.FindProperty("MoleParentTransform");
        _moleTransformProperty = serializedObject.FindProperty("MoleTransform");
        _moleBlockProperty = serializedObject.FindProperty("MoleBlock");

        _cryingKittyTransformProperty = serializedObject.FindProperty("CryingKittyTransform");
        _cryingKittyTmpTextProperty = serializedObject.FindProperty("CryingKittyTmpText");
        _cryingRoundAmountProperty = serializedObject.FindProperty("CryingRoundAmount");

    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;

        NodeFSM node = (NodeFSM)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(_neighboursProperty);
        EditorGUILayout.PropertyField(_kittyHouseTransformProperty);
        EditorGUILayout.PropertyField(_kittyHouseTmpTextProperty);
        EditorGUILayout.PropertyField(_kittyHouseMeshRenderertProperty);
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_moleParentTransformProperty);
        EditorGUILayout.PropertyField(_moleTransformProperty);
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_cryingKittyTransformProperty);
        EditorGUILayout.PropertyField(_cryingKittyTmpTextProperty);
        EditorGUILayout.Space(5);


        node.KittyHouseTransform?.gameObject.SetActive(false);
        node.MoleParentTransform?.gameObject.SetActive(false);
        node.CryingKittyTransform?.gameObject.SetActive(false);

        var newObstacleType = (ObstacleType)EditorGUILayout.EnumPopup("Obstacle Type", node.ObstacleType);
        if (newObstacleType != node.ObstacleType)
        {
            node.SetObstacleType(newObstacleType);
        }

        if (node.ObstacleType == ObstacleType.KittyHouse)
        {
            EditorGUILayout.LabelField("Kitty House Queue");
            if (node.KittyHouseOptions == null)
            {
                node.KittyHouseOptions = new List<AnimalType>();
            }

            int listSize = EditorGUILayout.IntField("Size", node.KittyHouseOptions.Count);
            if (listSize != node.KittyHouseOptions.Count)
            {
                while (listSize > node.KittyHouseOptions.Count)
                {
                    node.KittyHouseOptions.Add(AnimalType.NONE);
                }
                while (listSize < node.KittyHouseOptions.Count)
                {
                    node.KittyHouseOptions.RemoveAt(node.KittyHouseOptions.Count - 1);
                }
            }

            for (int i = 0; i < node.KittyHouseOptions.Count; i++)
            {
                node.KittyHouseOptions[i] = (AnimalType)EditorGUILayout.EnumPopup($"Kitty {i + 1} Type", node.KittyHouseOptions[i]);
            }

            node.KittyHouseTransform.gameObject.SetActive(true);
            node.KittyHouseTmpText.text = node.KittyHouseOptions.Count.ToString();

        }
        else if (node.ObstacleType == ObstacleType.DirtyKitty)
        {
            EditorGUILayout.PropertyField(_animalTypeProperty);
        }
        else if (node.ObstacleType == ObstacleType.BondedKities)
        {
            EditorGUILayout.PropertyField(_animalTypeProperty);
        }
        else if (node.ObstacleType == ObstacleType.Mole)
        {
            EditorGUILayout.PropertyField(_animalTypeProperty);
            EditorGUILayout.PropertyField(_moleBlockProperty);
            node.MoleParentTransform.gameObject.SetActive(true);

        }
        else if (node.ObstacleType == ObstacleType.CryingKitty)
        {
            if (node.CryingRoundAmount < 0) node.CryingRoundAmount = 0;

            EditorGUILayout.PropertyField(_cryingRoundAmountProperty);
            node.CryingKittyTransform.gameObject.SetActive(true);
            node.CryingKittyTmpText.text = node.CryingRoundAmount.ToString();
            EditorGUILayout.PropertyField(_animalTypeProperty);
        }
        else if (node.ObstacleType == ObstacleType.DirtyCryingKitty)
        {
            if (node.CryingRoundAmount < 0) node.CryingRoundAmount = 0;

            EditorGUILayout.PropertyField(_cryingRoundAmountProperty);
            node.CryingKittyTransform.gameObject.SetActive(true);
            node.CryingKittyTmpText.text = node.CryingRoundAmount.ToString();
            EditorGUILayout.PropertyField(_animalTypeProperty);

        }
        else if (node.ObstacleType == ObstacleType.NONE)
        {
            node.KittyHouseOptions = null;
            EditorGUILayout.PropertyField(_animalTypeProperty);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(node);
        }
    }
}
