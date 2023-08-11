using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace OkigaeSan
{
    public partial class AnimationManager
    {
        private AnimationClip CreateAnimation(GameObject obj,string uniqeName ,bool state)
        {
            string AssetsPath = "Assets/KanKan/OkigaeSan/Animation/";

            float val = state ? 1f : 0f;
            string sState = state ? "_On" : "_Off";

            var curve = AnimationCurve.Linear(0, val, (float)1 / 60, val);
            var clip = new AnimationClip();

            clip.name = uniqeName + sState;

            clip.SetCurve(Utilty.GetFullPath(obj), typeof(GameObject), "isActive", curve);
            AssetDatabase.CreateAsset(clip, AssetsPath + clip.name + ".anim");

            return clip;
        }
        private void CreateParamaterManagerLayer()
        {

        }

        private ParamaterManager GetParamaterManager(string pName)
        {
            return new ParamaterManager();
        }
        private void SetPramaterManager(ParamaterManager pManager)
        {

        }

        private void RemovePramaterManager(string pName)
        {

        }

        private void UpdateParamaterManager(string pName, ParamaterManager pManager)
        {

        }

        private int GetAvailableOutFitNum()
        {
            return 0;
        }

        private void CreateOnOffLayer(AnimationClip firstAnim, AnimationClip secondAnim, string uniqeName, bool defaultBool)
        {
            var rootStateMachine = new AnimatorStateMachine
            {
                anyStatePosition = new Vector3(0, -100, 0),
                entryPosition = Vector3.zero,
                exitPosition = new Vector3(500, 0, 0)
            };

            var firstState = new AnimatorState
            {
                name = firstAnim.name,
                motion = firstAnim,
                writeDefaultValues = _WriteDefault
            };
            var secondState = new AnimatorState
            {
                name = secondAnim.name,
                motion = secondAnim,
                writeDefaultValues = _WriteDefault
            };

            var paramator = new AnimatorControllerParameter
            {
                name = uniqeName,
                defaultBool = defaultBool,
                type = AnimatorControllerParameterType.Bool
            };

            var firstTransiton = new AnimatorStateTransition
            {
                hasExitTime = false,
                conditions = new[] { new AnimatorCondition { mode = defaultBool ? AnimatorConditionMode.IfNot : AnimatorConditionMode.If, parameter = uniqeName } },
                destinationState = secondState
            };

            var secondTransiton = new AnimatorStateTransition
            {
                hasExitTime = false,
                conditions = new[] {new AnimatorCondition { mode = defaultBool ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, parameter = uniqeName } },
                destinationState = firstState
            };

            var layer = new AnimatorControllerLayer
            {
                name = uniqeName,
                avatarMask = null,
                defaultWeight = 1,
                blendingMode = AnimatorLayerBlendingMode.Override,
                syncedLayerAffectsTiming = false,
                iKPass = false,
                stateMachine = rootStateMachine
            };

            _controller.AddParameter(paramator);

            firstState.AddTransition(firstTransiton);
            secondState.AddTransition(secondTransiton);

            rootStateMachine.AddState(firstState, new Vector3(200,0,0));
            rootStateMachine.AddState(secondState, new Vector3(200,100,0));

            _controller.AddLayer(layer);
        }

        private void DeleteOnOffLayer(string layerName, string localParamater)
        {

        }
    }
}