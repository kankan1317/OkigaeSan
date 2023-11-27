﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OkigaeSan
{
    public class Utilty
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
    }

}

