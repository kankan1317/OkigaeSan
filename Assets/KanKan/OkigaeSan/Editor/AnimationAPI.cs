using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;

namespace OkigaeSan
{
    public partial class AnimationManager
    {
        private readonly string AnimationFolderPath = "Assets/KanKan/OkigaeSan/Animation/";
        private readonly string AnimationManagerLayerName = "OS_GroupAnimations";
        private AnimationClip CreateAnimation(GameObject obj,string uniqeName ,bool state)
        {
            return CreateAnimation(new GameObject[] {obj}, uniqeName, state);
        }

        private AnimationClip CreateAnimation(GameObject[] objs,string uniqeName ,bool state)
        {
            float val = state ? 1f : 0f;
            string sState = state ? "_On" : "_Off";

            var curve = AnimationCurve.Linear(0, val, (float)1 / 60, val);
            var clip = new AnimationClip();

            clip.name = uniqeName + sState;

            foreach (var obj in objs)
            {
                clip.SetCurve(Utilty.GetFullPath(obj), typeof(GameObject), "isActive", curve);
            }
            AssetDatabase.CreateAsset(clip, AnimationFolderPath + clip.name + ".anim");

            return clip;
        }
        private void CreateParamaterManagerLayer()
        {
            var rootStateMachine = new AnimatorStateMachine
            {
                anyStatePosition = new Vector3(0, -100, 0),
                entryPosition = Vector3.zero,
                exitPosition = new Vector3(0, 200, 0)
            };

            var emptyState = new AnimatorState
            {
                name = "Empty",
                writeDefaultValues = _WriteDefault
            };

            var resetState = new AnimatorState
            {
                name = "DefaultReset",
                writeDefaultValues = _WriteDefault,
                behaviours = new StateMachineBehaviour[] { ParamaterManager.GetResetDriver(AnimationManagerLayerName) }
            };

            var paramater = new AnimatorControllerParameter
            {
                name = AnimationManagerLayerName,
                type = AnimatorControllerParameterType.Int,
                defaultInt = 0
            };

            var transition = new AnimatorStateTransition
            {
                hasExitTime = true,
                conditions = new AnimatorCondition[] { },
                destinationState = emptyState
            };

            var layer = new AnimatorControllerLayer
            {
                name = AnimationManagerLayerName,
                avatarMask = null,
                defaultWeight = 1,
                blendingMode = AnimatorLayerBlendingMode.Override,
                syncedLayerAffectsTiming = false,
                iKPass = false,
                stateMachine = rootStateMachine
            };

            _controller.AddParameter(paramater);

            resetState.AddTransition(transition);

            rootStateMachine.AddState(emptyState, new Vector3(-20, 100, 0));
            rootStateMachine.AddState(resetState, new Vector3(1200, 100, 0));

            _controller.AddLayer(layer);
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

        private AnimatorState GetAnimatorState(string targetLayer, string stateName)
        {
            AnimatorControllerLayer layer = null;
            AnimatorState state = null;

            for (int i = 0; i < _controller.layers.Length; i++)
            {
                if (_controller.layers[i].name == targetLayer)
                {
                    layer = _controller.layers[i];
                    break;
                }
            }
            if (layer == null) return null;

            foreach (var _state in layer.stateMachine.states)
            {
                if (_state.state.name == stateName)
                {
                    state = _state.state;
                    break;
                }
            }
            return state;
        }

        private void CreateOnOffLayer(AnimationClip firstAnim, AnimationClip secondAnim, string uniqeName, bool defaultBool)
        {
            var rootStateMachine = new AnimatorStateMachine
            {
                anyStatePosition = new Vector3(0, -100, 0),
                entryPosition = Vector3.zero,
                exitPosition = new Vector3(0, 100, 0)
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

        private void DeleteOnOffLayer(string uniqeName)
        {
            // 対象のLayerがあれば削除
            for (int i = 0; i < _controller.layers.Length; i++)
            {
                if (_controller.layers[i].name == uniqeName)
                {
                    _controller.RemoveLayer(i);
                }
            }
            // 対象のparameterがあれば削除
            for (int i = 0; i < _controller.parameters.Length; i++)
            {
                if (_controller.parameters[i].name == uniqeName)
                {
                    _controller.RemoveParameter(_controller.parameters[i]);
                }
            }
            // 対象のanimファイルがあれば削除
            string animOnPath = AnimationFolderPath + uniqeName + "_On.anim";
            string animOffPath = AnimationFolderPath + uniqeName + "_Off.anim";
            if (File.Exists(animOnPath))
            {
                AssetDatabase.DeleteAsset(animOnPath);
            }
            if (File.Exists(animOffPath))
            {
                AssetDatabase.DeleteAsset(animOffPath);
            }
        }

        private GameObject[] GetAnimatedObjects(AnimationClip clip)
        {
            var curveBindongs = AnimationUtility.GetCurveBindings(clip);

            var objects = new GameObject[curveBindongs.Length];

            for (int i = 0; i < curveBindongs.Length; i++)
            {
                var targetObj = curveBindongs[i].path.Length > 0
                    ? GameObject.Find(curveBindongs[i].path)
                    : null;
                objects[i] = targetObj;
            }

            return objects;
        }
    }
    public class ParamaterManager
    {
        private List<VRC_AvatarParameterDriver.Parameter> parameters;

        public static VRCAvatarParameterDriver GetResetDriver(string pName)
        {
            var driver = new VRCAvatarParameterDriver();
            var parameter = new VRC_AvatarParameterDriver.Parameter();
            parameter.type = VRC_AvatarParameterDriver.ChangeType.Set;
            parameter.name = pName;
            parameter.value = 0f;

            driver.isEnabled = true;
            driver.isLocalPlayer = false;
            driver.parameters = new List<VRC_AvatarParameterDriver.Parameter> { parameter };

            return driver;
        }
        public ParamaterManager()
        {
            parameters = new List<VRC_AvatarParameterDriver.Parameter>();
        }

        public void AddParameter(string uniqeName, bool value)
        {
            var parameter = new VRC_AvatarParameterDriver.Parameter();
            parameter.type = VRC_AvatarParameterDriver.ChangeType.Set;
            parameter.name = uniqeName;
            parameter.value = value ? 1f : 0f;
            parameters.Add(parameter);
        }

        public void RemoveParameter(string uniqeName)
        {
            var newParameters = new List<VRC_AvatarParameterDriver.Parameter>();
            foreach (var parameter in parameters)
            {
                if (parameter.name != uniqeName)
                {
                    newParameters.Add(parameter);
                }
            }
            parameters = newParameters;
        }

        public VRCAvatarParameterDriver GetDriver()
        {
            var driver = new VRCAvatarParameterDriver();
            driver.isEnabled = true;
            driver.isLocalPlayer = false;
            driver.parameters = parameters;
            
            return driver;
        }
    }
}