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

public class ControllerStatesScritableObjectBase<TData> : ScriptableObject
{
    [SerializeField]
    TData walking;
    public TData Walking { get { return walking; } }

    [SerializeField]
    TData sprint;
    public TData Sprint { get { return sprint; } }

    [SerializeField]
    TData crouch;
    public TData Crouch { get { return crouch; } }

    [SerializeField]
    TData prone;
    public TData Prone { get { return prone; } }

    public TData GetData(ControllerState state)
    {
        switch (state)
        {
            case ControllerState.Walking:
                return walking;
            case ControllerState.Sprinting:
                return sprint;
            case ControllerState.Crouching:
                return crouch;
            case ControllerState.Proning:
                return prone;
        }

        throw new ArgumentException("Controller State Data " + state.ToString() + " Not Defined");
    }
}