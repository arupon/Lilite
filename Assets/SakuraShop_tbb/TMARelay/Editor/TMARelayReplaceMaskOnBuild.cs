/*
MIT License

Copyright (c) 2023 Sakura (tbbsakura)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// アバタービルド時の処理

using System;
using UnityEngine;
using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;

using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using TMARelay;

[assembly: ExportsPlugin(typeof(TMARelayReplaceMaskOnBuild))]

namespace TMARelay
{
    public class TMARelayReplaceMaskOnBuild : Plugin<TMARelayReplaceMaskOnBuild>
    {
        private const string PATH1 = "Packages/com.vrchat.avatars/Samples/AV3 Demo Assets/Animation/Controllers/vrc_AvatarV3HandsLayer.controller";
        private const string PATH2 = "Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3HandsLayer.controller";
        
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating) // Generating だと GestureLayerController は既に Clone されている (Resolving だとまだダメ)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Replacing Gesture Layer Mask (BluildPhase.Generating)", ctx =>
                {
                    // ctx.AvatarRootTransform TMARelayReplaceMask コンポーネントを持っているものを列挙して処理
                    Component[] _components;
                    _components = ctx.AvatarRootTransform.GetComponentsInChildren(typeof(TMARelayReplaceMask),true);
					VRCAvatarDescriptor _avatar = ctx.AvatarRootTransform.GetComponent(typeof(VRCAvatarDescriptor)) as VRCAvatarDescriptor;
                    foreach (TMARelayReplaceMask x in _components )
                    {
                        DoReplace( _avatar, x.m_newAvatarMask );
                        GameObject.DestroyImmediate(x);
                    }
                });
        }

        private void DoReplace( VRCAvatarDescriptor _avatar, AvatarMask _newMask )
        {
			if ( _newMask == null ) return;
			if ( _avatar == null ) return;

            AnimatorController animatorController = null;
            int nGesture = -1;
            for (int i = 0; i < _avatar.baseAnimationLayers.Length; i++)
            {
                var layer = _avatar.baseAnimationLayers[i];
                if (layer.type == VRCAvatarDescriptor.AnimLayerType.Gesture )
                {
                    if (!layer.isDefault && layer.animatorController != null && layer.animatorController is AnimatorController c)
                    {
                        animatorController = c;
                    }
                    else {
                        animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(PATH1);
                        if (animatorController == null)
                        {
                            animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(PATH2);
                            if ( animatorController == null ) {
                                animatorController = new AnimatorController();
                            }
                        }
                    }
                    if ( animatorController != null ) {
                        Debug.Log( "Sakura : Instantiate: ControllerName: " + animatorController.name );
                        animatorController = UnityEngine.Object.Instantiate(animatorController);
                        Debug.Log( "Sakura : Clone: ControllerName: " + animatorController.name );
                    }
                    nGesture = i;
                    break;
                }
            }

            if ( animatorController != null && nGesture >= 0 ) {
                Debug.Log( "Sakura : ControllerName: " + animatorController.name + "(Mask: " + animatorController.layers[0].avatarMask.name + ") New Mask: " + _newMask.name);
                UnityEditor.Animations.AnimatorControllerLayer[] layers = animatorController.layers; // this is a copy
                layers[0].avatarMask = _newMask;
                animatorController.layers = layers; // overwrite 
                _avatar.baseAnimationLayers[nGesture].animatorController =  animatorController;
                _avatar.baseAnimationLayers[nGesture].isDefault = false;
            }
        }
   }
}