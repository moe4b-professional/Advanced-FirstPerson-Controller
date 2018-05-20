using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AFPC
{
	public abstract partial class ControllerLookTargetBase : FPController.Module
	{
        public ControllerLook Look { get { return Controller.Look; } }
        public ControllerCharacterLook CharacterLook { get { return Look.CharacterLook; } }

        public CameraRig CameraRig { get { return Controller.CameraRig; } }
        new public CameraRigCamera camera { get { return CameraRig.camera; } }

        public Vector3? Position { get; set; }
        public virtual void SetPosition(Vector3 value)
        {
            Position = value;
        }
        public virtual void ClearPosition()
        {
            Position = null;
        }

        [SerializeField]
        protected float speed = 360f;
        public float Speed { get { return speed; } }

        [SerializeField]
        protected float range = 40f;
        public float Range { get { return range; } }
    }

    public partial class ControllerLookTarget : ControllerLookTargetBase
    {

    }
}