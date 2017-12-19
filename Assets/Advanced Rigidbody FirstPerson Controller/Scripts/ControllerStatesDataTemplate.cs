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
    public interface IControllerStatesDataTemplate<TData>
    {
        TData Walk { get; }

        TData Sprint { get; }

        TData Crouch { get; }

        TData Prone { get; }

        TData Slide { get; }

        TData GetData(ControllerState state);
    }

    public class ControllerStatesDataTemplate<TData> : IControllerStatesDataTemplate<TData>
    {
        [SerializeField]
        protected TData walk;
        public TData Walk { get { return walk; } }

        [SerializeField]
        protected TData sprint;
        public TData Sprint { get { return sprint; } }

        [SerializeField]
        protected TData crouch;
        public TData Crouch { get { return crouch; } }

        [SerializeField]
        protected TData prone;
        public TData Prone { get { return prone; } }

        [SerializeField]
        protected TData slide;
        public TData Slide { get { return crouch; } }

        public TData GetData(ControllerState state)
        {
            return GetData(this, state);
        }

        public static TData GetData(IControllerStatesDataTemplate<TData> template, ControllerState state)
        {
            switch (state)
            {
                case ControllerState.Walking:
                    return template.Walk;
                case ControllerState.Sprinting:
                    return template.Sprint;
                case ControllerState.Crouching:
                    return template.Crouch;
                case ControllerState.Proning:
                    return template.Prone;
                case ControllerState.Sliding:
                    return template.Slide;
            }

            throw new ArgumentException("Controller State Data " + state.ToString() + " Not Defined");
        }
    }

    public class ControllerStatesScriptableObjectTemplate<TData> : ScriptableObject, IControllerStatesDataTemplate<TData>
    {
        [SerializeField]
        protected TData walk;
        public TData Walk { get { return walk; } }

        [SerializeField]
        protected TData sprint;
        public TData Sprint { get { return sprint; } }

        [SerializeField]
        protected TData crouch;
        public TData Crouch { get { return crouch; } }

        [SerializeField]
        protected TData prone;
        public TData Prone { get { return prone; } }

        [SerializeField]
        protected TData slide;
        public TData Slide { get { return crouch; } }

        public TData GetData(ControllerState state)
        {
            return ControllerStatesDataTemplate<TData>.GetData(this, state);
        }
    }
}