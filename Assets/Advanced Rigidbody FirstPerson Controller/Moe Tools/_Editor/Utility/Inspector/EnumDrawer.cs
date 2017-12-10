#if UNITY_EDITOR
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
    public class EnumDrawer<T>
        where T : IFormattable, IConvertible, IComparable
    {
        public SerializedProperty property;
        public string label;

        public T Value { get; protected set; }

        string[] values;

        public virtual void Draw()
        {
            property.enumValueIndex = EditorGUILayout.Popup(label, property.enumValueIndex, property.enumDisplayNames);

            Value = MoeTools.Enum.Parse<T>(values[property.enumValueIndex]);
        }

        public EnumDrawer(SerializedProperty property)
        {
            this.property = property;
            this.label = property.displayName;

            values = property.enumNames;
        }
    }

    public class PropertyDisplayEnumDrawer<T> : EnumDrawer<T>
        where T : IFormattable, IConvertible, IComparable
    {
        Dictionary<T, Action> Values;

        public virtual void Assign(T value, SerializedProperty property)
        {
            Assign(value, () => DefaultValueDraw(property));
        }
        public void Assign(T value, Action action)
        {
            Values.Add(value, action);
        }

        public override void Draw()
        {
            base.Draw();

            if (Values.ContainsKey(Value) && Values[Value] != null)
                Values[Value]();
        }

        protected virtual void DefaultValueDraw(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, true);
        }

        public PropertyDisplayEnumDrawer(SerializedProperty property) : base(property)
        {
            Values = new Dictionary<T, Action>();
        }
    }
}
#endif