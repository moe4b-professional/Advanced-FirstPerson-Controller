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

namespace Moe.Tools
{
    public static partial class MoeTools
    {
        public static class ExceptionTools
        {
            public static class Templates
            {
                public static Exception MissingDependacny<TDependency, TDependant>(string objectName)
                {
                    return new Exception(
                        "A type of " +
                        typeof(TDependency).Name.Enclose() +
                        " is required by " +
                        typeof(TDependant).Name.Enclose() +
                        " on object " +
                        objectName.Enclose()
                        );
                }

                public static ArgumentOutOfRangeException NegativeValue<TClass>(string valueName, float value)
                {
                    return new ArgumentOutOfRangeException(valueName, typeof(TClass).Name + "'s " + valueName + " cannot be set to the negative value of " + value.ToString());
                }
            }
        }
    }
}