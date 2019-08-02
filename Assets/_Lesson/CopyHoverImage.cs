using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CopyHoverImage : MonoBehaviour
{
    [MenuItem("Tools/Copy Hover...")]
    public static void CopyHover()
    {
        List<Transform> hoverObjects = new List<Transform>();

        foreach(Transform child in Selection.activeGameObject.GetComponentsInChildren<Transform>())
        {
            if(child.gameObject.name == "Background")
            {
                child.gameObject.name = "BG";
                //child.transform.SetAsFirstSibling();
                //GameObject background = GameObject.Instantiate(child.gameObject, child.parent);
                //background.name = "Background";
                //hoverObjects.Add(child);
                //Debug.Log(child.gameObject.name);
            }
        }

        Debug.Log(hoverObjects.Count);
    }
}
