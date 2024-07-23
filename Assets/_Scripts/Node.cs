using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private List<Node> childNodes = new();
    [SerializeField] private Color drawLineColor = Color.white;

    //private GameManager _gameManager;

    //private void Awake()
    //{
    //    _gameManager = GameManager.Instance;
    //}

    private void OnDrawGizmosSelected()
    {
        //if (_gameManager.DrawAllNodeLines) return;

        //DrawLines();
    }

    private void OnDrawGizmos()
    {
        //if (!_gameManager.DrawAllNodeLines) return;

        DrawLines();
    }

    private void DrawLines()
    {
        if (childNodes.Count <= 0) return;

        foreach (var child in childNodes)
        {
            var p1 = transform.position;
            var p2 = child.transform.position;
            var thickness = 4;
            Handles.DrawBezier(p1, p2, p1, p2, drawLineColor, null, thickness);
        }
    }
}