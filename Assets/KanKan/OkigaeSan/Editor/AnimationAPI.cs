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
        private static readonly string ToolResoruceFolderPath = "Assets/KanKan/OkigaeSan/Tool_resource/";
        private static readonly string AnimationFolderPath = "Assets/KanKan/OkigaeSan/Animation/";
        private static readonly string AnimationManagerLayerName = "OS_GroupAnimations";
        private static readonly string AnimationManagerIsLocal = "IsLocal";
        private static readonly string AnimationManagerIsFirstLoad = "OS_IsFirstLoad";

        private static AnimationClip DumyClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AnimationManager.ToolResoruceFolderPath + "Dumy.animation");
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

        private void CreateDefaultParametars()
        {
            if (!this.ExistParametar(AnimationManager.AnimationManagerLayerName))
            {
                var paramater = new AnimatorControllerParameter
                {
                    name = AnimationManagerLayerName,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0
                };
                _controller.AddParameter(paramater);
            }

            if (!this.ExistParametar(AnimationManager.AnimationManagerIsFirstLoad))
            {
                var paramater = new AnimatorControllerParameter
                {
                    name = AnimationManager.AnimationManagerIsFirstLoad,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false
                };
                _controller.AddParameter(paramater);
            }

            if (!this.ExistParametar(AnimationManager.AnimationManagerIsLocal))
            {
                var paramater = new AnimatorControllerParameter
                {
                    name = AnimationManager.AnimationManagerIsLocal,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false
                };
                _controller.AddParameter(paramater);
            }
        }
        private void CreateParamaterManagerLayer()
        {
            var rootStateMachine = new AnimatorStateMachine
            {
                anyStatePosition = new Vector3(20, -100, 0),
                entryPosition = new Vector3(20, 0, 0),
                exitPosition = new Vector3(1500, 300, 0)
            };

            var IsLocalState = new AnimatorState
            {
                name = AnimationManager.AnimationManagerIsLocal,
                writeDefaultValues = _WriteDefault,
                motion = AnimationManager.DumyClip
            };

            var IsFirstLoadState = new AnimatorState
            {
                name = AnimationManager.AnimationManagerIsFirstLoad,
                writeDefaultValues = _WriteDefault,
                motion = AnimationManager.DumyClip
            };

            var SelectState = new AnimatorState
            {
                name = "Select",
                writeDefaultValues = _WriteDefault,
                motion = AnimationManager.DumyClip
            };

            var Select2State = new AnimatorState
            {
                name = "Select2",
                writeDefaultValues = _WriteDefault,
                motion = AnimationManager.DumyClip
            };

            var IsLocalCondtion = new AnimatorCondition
            {
                parameter = AnimationManager.AnimationManagerIsLocal,
                mode = AnimatorConditionMode.If
            };

            var IsFirstLoadCondtion = new AnimatorCondition
            {
                parameter = AnimationManager.AnimationManagerIsFirstLoad,
                mode = AnimatorConditionMode.If
            };

            var IsLocalTransition = new AnimatorStateTransition
            {
                hasExitTime = false,
                hasFixedDuration = false,
                exitTime = 0,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { IsLocalCondtion },
                destinationState = IsFirstLoadState
            };

            var IsFirstLoadTransition = new AnimatorStateTransition
            {
                hasExitTime = false,
                hasFixedDuration = false,
                exitTime = 0,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { IsFirstLoadCondtion },
                destinationState = SelectState
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

            IsLocalState.AddTransition(IsLocalTransition);
            IsFirstLoadState.AddTransition(IsFirstLoadTransition);

            rootStateMachine.AddState(IsLocalState, new Vector3(0, 100, 0));
            rootStateMachine.AddState(IsFirstLoadState, new Vector3(0, 200, 0));
            rootStateMachine.AddState(SelectState, new Vector3(0, 300, 0));
            rootStateMachine.AddState(Select2State, new Vector3(900, 300, 0));

            rootStateMachine.AddState(emptyState, new Vector3(-20, 100, 0));
            rootStateMachine.AddState(resetState, new Vector3(1200, 100, 0));

            _controller.AddLayer(layer);
        }

        private ParamaterManager GetParamaterManager(string pName)
        {
            return new ParamaterManager();
        }
        public void SetPramaterManager(string pName, ParamaterManager pManager)
        {
            int outfitNum = this.GetAvailableOutFitNum();
            float yPos = this.GetAvailableParamaterManagerYPos();
            var stateMachine = this.GetAnimatorLayer(AnimationManagerLayerName).stateMachine;
            var stateEmpty = this.GetAnimatorState(AnimationManagerLayerName, "Empty");
            var selectState = this.GetAnimatorState(AnimationManager.AnimationManagerLayerName, "Select");
            var select2State = this.GetAnimatorState(AnimationManager.AnimationManagerLayerName, "Select2");

            var waitState = new AnimatorState
            {
                name = "Wait" + outfitNum,
                writeDefaultValues = _WriteDefault,
                motion = AnimationManager.DumyClip
            };

            var stateOn = new AnimatorState
            {
                name = pName + "_On",
                writeDefaultValues = _WriteDefault,
                behaviours = new StateMachineBehaviour[] {pManager.GetDriver(true)}
            };
            var stateOff = new AnimatorState
            {
                name = pName + "_Off",
                writeDefaultValues = _WriteDefault,
                behaviours = new StateMachineBehaviour[] { pManager.GetDriver(false) }
            };

            var EqualOutfitNumCondition = new AnimatorCondition
            {
                parameter = AnimationManager.AnimationManagerLayerName,
                mode = AnimatorConditionMode.Equals,
                threshold = outfitNum
            };

            var NotEqualOutfitCondition = new AnimatorCondition
            {
                parameter = AnimationManager.AnimationManagerLayerName,
                mode = AnimatorConditionMode.NotEqual,
                threshold = outfitNum
            };

            var select2WaitTransition = new AnimatorStateTransition
            {
                hasExitTime = false,
                exitTime = 0,
                hasFixedDuration = false,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { EqualOutfitNumCondition },
                destinationState = waitState
            };

            var wait2OffTransition = new AnimatorStateTransition
            {
                hasExitTime = false,
                exitTime = 0,
                hasFixedDuration = false,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { NotEqualOutfitCondition },
                destinationState = stateOff
            };

            var off2SelectTransition = new AnimatorStateTransition
            {
                hasExitTime = true,
                exitTime = 0,
                hasFixedDuration = false,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { },
                destinationState = select2State
            };

            var select22OnTransition = new AnimatorStateTransition
            {
                hasExitTime = false,
                exitTime = 0,
                hasFixedDuration = false,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                conditions = new AnimatorCondition[] { EqualOutfitNumCondition },
                destinationState = stateOn
            };

            var on2ExitTransition = new AnimatorStateTransition
            {
                hasExitTime = true,
                exitTime = 0,
                hasFixedDuration = false,
                duration = 0,
                offset = 0,
                interruptionSource = TransitionInterruptionSource.None,
                isExit = true,
                conditions = new AnimatorCondition[] { }
            };

            selectState.AddTransition(select2WaitTransition);
            waitState.AddTransition(wait2OffTransition);
            stateOff.AddTransition(off2SelectTransition);
            select2State.AddTransition(select22OnTransition);
            stateOn.AddTransition(on2ExitTransition);

            stateMachine.AddState(waitState, new Vector3(300, yPos, 0));
            stateMachine.AddState(stateOff, new Vector3(600, yPos, 0));
            stateMachine.AddState(stateOn, new Vector3(1200, yPos, 0));
        }

        private void RemovePramaterManager(string pName)
        {

        }

        private void UpdateParamaterManager(string pName, ParamaterManager pManager)
        {

        }

        private int GetAvailableOutFitNum()
        {
            float yPos = this.GetAvailableParamaterManagerYPos();
            return (int)((yPos - 300f) / 100f);
        }

        private float GetAvailableParamaterManagerYPos()
        {
            float yPos = 300f;
            var layer = GetAnimatorLayer(AnimationManagerLayerName);

            foreach (var state in layer.stateMachine.states)
            {
                string stateName = state.state.name;
                
                if (stateName != AnimationManager.AnimationManagerIsLocal
                    && stateName != AnimationManager.AnimationManagerIsFirstLoad
                    && stateName != "Select"
                    && stateName != "Select2")
                {
                    if (yPos == state.position.y)
                    {
                        yPos += 100f;
                    }
                    }
                }
            return yPos;
        }
        private AnimatorControllerLayer GetAnimatorLayer(string layerName)
        {
            AnimatorControllerLayer layer = null;

            for (int i = 0; i < _controller.layers.Length; i++)
            {
                if (_controller.layers[i].name == layerName)
                {
                    layer = _controller.layers[i];
                    break;
                }
            }
            return layer;
        }

        private AnimatorState GetAnimatorState(string targetLayer, string stateName)
        {
            AnimatorControllerLayer layer;
            AnimatorState state = null;

            layer = GetAnimatorLayer(targetLayer);
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

        private bool ExistParametar(string uniqeName)
        {
            bool exist = false;
            foreach (var param in _controller.parameters)
            {
                if (param.name == uniqeName)
                {
                    exist = true;
                    break;
                }
            }
            return exist;
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
        private List<string> parameterNames;

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
            parameterNames = new List<string>();
        }

        public void AddParameter(string uniqeName)
        {
            parameterNames.Add(uniqeName);
        }

        public void RemoveParameter(string uniqeName)
        {
            var newParameters = new List<string>();
            foreach (var parameter in parameterNames)
            {
                if (parameter != uniqeName)
                {
                    newParameters.Add(parameter);
                }
            }
            parameterNames = newParameters;
        }

        public VRCAvatarParameterDriver GetDriver(bool value)
        {
            var driver = new VRCAvatarParameterDriver();
            driver.isEnabled = true;
            driver.isLocalPlayer = false;
            var parameters = new List<VRC_AvatarParameterDriver.Parameter>();

            float VRC_value = value ? 1f : 0f;
            foreach (var parameterName in parameterNames)
            {
                var parameter = new VRC_AvatarParameterDriver.Parameter();
                parameter.type = VRC_AvatarParameterDriver.ChangeType.Set;
                parameter.name = parameterName;
                parameter.value = VRC_value;
                parameters.Add(parameter);
            }
            driver.parameters = parameters;
            
            return driver;
        }
    }
}