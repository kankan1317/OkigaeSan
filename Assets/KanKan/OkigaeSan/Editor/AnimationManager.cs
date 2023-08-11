using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using VRC.SDK3;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;

namespace OkigaeSan
{
    public partial class AnimationManager
    {
        private AnimatorController _controller;

        private bool _WriteDefault;
        AnimationManager()
        {
            _controller = new AnimatorController();
            _WriteDefault = false;
        }
        
        public static AnimationManager Init(AnimatorController controller, bool writeDefault)
        {
            var manager = new AnimationManager
            {
                _controller = controller,
                _WriteDefault = writeDefault
            };
            manager.CreateParamaterManagerLayer();

            return manager;
        }
        public static AnimationManager Load(AnimatorController controller)
        {
            if (controller == null) throw new ArgumentNullException();
            var manager = new AnimationManager
            {
                _controller = controller
            };

            return manager;
        }

        public void Save(GameObject avater, string path)
        {
            AssetDatabase.CreateAsset(_controller, path);
        }

        public void AddObjects(GameObject obj, string uniqeName, bool defaultBool)
        {
            var defaultAnim = CreateAnimation(obj,uniqeName ,defaultBool);
            var secondAnim = CreateAnimation(obj, uniqeName, !defaultBool);

            CreateOnOffLayer(defaultAnim, secondAnim, uniqeName, defaultBool);
        }

        public void RemoveObjects(string uniqeName)
        {

        }
    }

    public class ParamaterManager
    {

    }
}
