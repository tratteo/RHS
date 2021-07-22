using System;
using UnityEditor;
using UnityEngine;

public static class PrefabsFactoryEditor
{
    #region StatusEffects

    [MenuItem("Assets/Create/Prefabs Factory/Status Effects/DamageOverTime", false, 0)]
    private static void DamageOverTime()
    {
        CreatePrefabAndAddComponents("damageovertime_sf", typeof(DamageOverTimeStatusEffect));
    }

    [MenuItem("Assets/Create/Prefabs Factory/Status Effects/Speed", false, 0)]
    private static void Speed()
    {
        CreatePrefabAndAddComponents("speed_sf", typeof(SpeedStatusEffect));
    }

    [MenuItem("Assets/Create/Prefabs Factory/Status Effects/Stun", false, 0)]
    private static void Stun()
    {
        CreatePrefabAndAddComponents("stun_sf", typeof(StunStatusEffect));
    }

    #endregion StatusEffects

    private static void CreatePrefabAndAddComponents(string name, params Type[] types)
    {
        string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (folderPath.Contains("."))
        {
            folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
        }
        GameObject obj = new GameObject();
        obj.name = name + ".prefab";
        foreach (Type type in types)
        {
            obj.AddComponent(type);
        }
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, folderPath + "/" + obj.name);
        UnityEngine.Object.DestroyImmediate(obj);
    }
}