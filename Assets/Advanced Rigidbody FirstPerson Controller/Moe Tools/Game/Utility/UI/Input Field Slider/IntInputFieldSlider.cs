using System;
using System.IO;
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
    public class IntInputFieldSlider : InputFieldSlider<int>
    {
        protected override InputField.ContentType ContentType { get { return InputField.ContentType.IntegerNumber; } }

        public override int ClampValue(int data)
        {
            return GetData(Mathf.Clamp(data, MinValue, MaxValue));
        }

        public override int GetData(float value)
        {
            return Mathf.RoundToInt(value);
        }

        public override float GetFloat(int data)
        {
            return GetData(data);
        }

        protected override int GetData(string text)
        {
            int value;

            if (int.TryParse(text, out value))
                return value;
            else
                return Mathf.RoundToInt(MinValue);
        }
    }
}