using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace OkigaeSan
{
    public static class Utilty
    {
        public static bool IsOkigaeSanParameter(string parameterName)
        {
            return "OS_" == parameterName.Substring(0, 3);
        }
        public static List<GameObject> GetChildren(GameObject obj)
        {
            List<GameObject> childrens = new List<GameObject>();

            Transform children = obj.GetComponentInChildren<Transform>();
            foreach (Transform child in children)
            {
                childrens.Add(child.gameObject);
            }
            return childrens;
        }

        public static string GetFullPath(GameObject obj)
        {
            return GetFullPath(obj.transform);
        }

        public static string GetFullPath(Transform obj)
        {
            string path = obj.name;
            var parent = obj.parent;

            while (parent)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;

            }
            return path ;
        }
    }
}

