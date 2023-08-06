using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace OkigaeSan
{
    public class OkigaeSanEditor : EditorWindow
    {
        private GameObject _avatarObj;

        [MenuItem("KanKan/OkigaeSan")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(OkigaeSanEditor));
        }

        public void OnGUI()
        {
            GUILayout.Label("アバターを設定して下さい");

            _avatarObj = EditorGUILayout.ObjectField("VRC Avatar", _avatarObj, typeof(GameObject), true) as GameObject;
            if (_avatarObj != null)
            {
                if (Check_VRC_Avatar(_avatarObj))
                {

                }
                else
                {
                    return;
                }
            }
        }

        private bool Check_VRC_Avatar(GameObject avatar)
        {
            List<ErrorCode> errorCode = new List<ErrorCode>();

            //VRC Avatar Descriptorが設定されているか確認する
            VRCAvatarDescriptor descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null)
            {
                errorCode.Add(ErrorCode.NotFoundVRCScript);
            }
            else
            {
                // AnimationContorollerの種類の定義元がわからないためLocalで定義
                // TODO: SDK側の定義情報があれば利用する
                ErrorCode[] AnimErrors = new ErrorCode[] {ErrorCode.NotFoundBaseAnimationController,
                                                          ErrorCode.NotFoundAddtiveAnimationController,
                                                          ErrorCode.NotFoundGestureAnimationController,
                                                          ErrorCode.NotFoundActionAnimationController,
                                                          ErrorCode.NotFoundFXAnimationController};
                // 定義されたAnimationContorollerの数が一致するか確認する
                if (AnimErrors.Length != descriptor.baseAnimationLayers.Length)
                {
                    errorCode.Add(ErrorCode.DetecedSDKChange);
                }
                else
                {
                    // AnimationContorollerがあるか確認する
                    for (int i = 0; i < descriptor.baseAnimationLayers.Length; i++)
                    {
                        var anim = descriptor.baseAnimationLayers[i];
                        if (anim.animatorController == null)
                        {
                            errorCode.Add(AnimErrors[i]);
                        }
                    }
                }
                

                // ExpressionMenuがあるか確認する
                if (descriptor.expressionsMenu == null)
                {
                    errorCode.Add(ErrorCode.NotFoundExpressionMenu);
                }
                // ExpressionParameterがあるか確認する
                if (descriptor.expressionParameters == null)
                {
                    errorCode.Add(ErrorCode.NotFoundExpressionParameter);
                }
            }

            if(errorCode.Count == 0) return true;

            foreach(ErrorCode error in errorCode)
            {
                if (error == ErrorCode.NotFoundVRCScript)
                {
                    EditorGUILayout.HelpBox("VRC用に設定されたObjectを指定して下さい", MessageType.Error, true);
                }
                else if (error == ErrorCode.DetecedSDKChange)
                {
                    EditorGUILayout.HelpBox("SDK側の変更が確認されました。Tool作成者に連絡をお願いします。", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundBaseAnimationController)
                {
                    EditorGUILayout.HelpBox("Avatar用BaseAnimationContollerが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundAddtiveAnimationController)
                {
                    EditorGUILayout.HelpBox("Avatar用AddtveAnimationContollerが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundGestureAnimationController)
                {
                    EditorGUILayout.HelpBox("Avatar用GestureAnimationContollerが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundActionAnimationController)
                {
                    EditorGUILayout.HelpBox("Avatar用ActionAnimationContollerが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundFXAnimationController)
                {
                    EditorGUILayout.HelpBox("Avatar用FXAnimationContollerが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundExpressionMenu)
                {
                    EditorGUILayout.HelpBox("Avatar用ExpressionMenuが見つかりませんでした", MessageType.Error, true);
                }
                else if (error == ErrorCode.NotFoundExpressionParameter)
                {
                    EditorGUILayout.HelpBox("Avatar用ExpressionParameterが見つかりませんでした", MessageType.Error, true);
                }
            }
            
            return false;
        }
    }
}

