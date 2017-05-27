using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void MoveOnlyParent( this Transform parent, Vector3 whereToMove )
    {
        List<Vector3> originalPositions = new List<Vector3>();
        var transforms = parent.gameObject.GetComponentsInChildren<Transform>();
        foreach (var trans in transforms)
        {
            originalPositions.Add(trans.position);
        }
        parent.position = whereToMove;
        int i = 0;
        foreach (var trans in transforms)
        {
            if (trans != parent)
            {
                trans.position = originalPositions[i];
            }
            ++i;
        }
    }
}
