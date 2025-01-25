using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace TMARelay {
    public class TMARelayReplaceMask : MonoBehaviour, VRC.SDKBase.IEditorOnly 
    {
        [Tooltip("Avatar Mask to be set on the 1st layer of GestureLayerController")]	
        public AvatarMask m_newAvatarMask;        

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

