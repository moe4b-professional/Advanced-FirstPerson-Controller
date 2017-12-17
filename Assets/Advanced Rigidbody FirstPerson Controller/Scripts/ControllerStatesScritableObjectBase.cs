using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace ARFC
{
    public class ControllerStatesScritableObjectBase<TData> : ScriptableObject
    {
        [SerializeField]
        TData standing;
        public TData Standing { get { return standing; } }

        [SerializeField]
        TData sprint;
        public TData Sprint { get { return sprint; } }

        [SerializeField]
        TData crouch;
        public TData Crouch { get { return crouch; } }

        [SerializeField]
        TData prone;
        public TData Prone { get { return prone; } }

        [SerializeField]
        TData slide;
        public TData Slide { get { return crouch; } }

        public TData GetData(ControllerState state)
        {
            switch (state)
            {
                case ControllerState.Walking:
                    return standing;
                case ControllerState.Sprinting:
                    return sprint;
                case ControllerState.Crouching:
                    return crouch;
                case ControllerState.Proning:
                    return prone;
                case ControllerState.Sliding:
                    return slide;
            }

            throw new ArgumentException("Controller State Data " + state.ToString() + " Not Defined");
        }
    }
}