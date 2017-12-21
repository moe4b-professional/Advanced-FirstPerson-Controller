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
    public interface IControllerHeadBobStates : IControllerStatesDataTemplate<HeadBobData>
    {

    }

    [CreateAssetMenu(menuName = HeadBobData.MenuPath + "States")]
    public class ControllerHeadBobStates : ControllerStatesScriptableObjectTemplate<HeadBobData>, IControllerHeadBobStates
    {

    }
}