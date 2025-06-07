using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gameobjectextenetions
{
    public static void Clear (this GameObject go)
    {
        while (go.transform.childCount > 0) 
        {
            Transform child = go.transform.GetChild(0);
            child.SetParent(null);
            GameObject.Destroy(child.gameObject);
        }
    }
}
