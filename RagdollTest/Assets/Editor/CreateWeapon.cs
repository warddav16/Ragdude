
using UnityEngine;
using UnityEditor;

public class CreateWeapon
{
    [MenuItem("Assets/Create/Weapon")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Weapon>();
    }
}