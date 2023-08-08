using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace OkigaeSan
{
    public static class Utilty
    {
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

        public static void CreateAnimation(GameObject obj, bool state)
        {
            string AssetsPath = "Assets/KanKan/OkigaeSan/Animation/";

            float val = state ? 1f : 0f;
            string sState = state ? "_On" : "_Off";

            var curve = AnimationCurve.Linear(0, val, (float)1/60, val);
            var clip = new AnimationClip();

            clip.name = obj.name + sState;

            clip.SetCurve(GetFullPath(obj), typeof(GameObject), "isActive", curve);
            AssetDatabase.CreateAsset(clip, AssetsPath + clip.name + ".anim");
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

