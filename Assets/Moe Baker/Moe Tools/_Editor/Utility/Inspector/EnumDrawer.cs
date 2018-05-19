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
        public SerializedProperty Property { get; protected set; }
        public string Label { get; set; }

        public T Value { get; protected set; }

        public string[] Values { get; protected set; }

        public virtual void Draw()
        {
            Property.enumValueIndex = EditorGUILayout.Popup(Label, Property.enumValueIndex, Property.enumDisplayNames);

            Value = MoeTools.Enum.Parse<T>(Values[Property.enumValueIndex]);
        }

        public EnumDrawer(SerializedProperty property)
        {
            this.Property = property;
            this.Label = property.displayName;

            Values = property.enumNames;
        }
    }

    public class EnumVisibiltyControllerDrawer<T> : EnumDrawer<T>
        where T : IFormattable, IConvertible, IComparable
    {
        Dictionary<T, Action> Draws;

        public virtual void Assign(T value, SerializedProperty property)
        {
            Assign(value, () => DrawDefault(property));
        }
        public void Assign(T value, Action drawaAction)
        {
            Draws.Add(value, drawaAction);
        }

        public override void Draw()
        {
            base.Draw();

            if (Draws.ContainsKey(Value) && Draws[Value] != null)
                Draws[Value]();
        }
        protected virtual void DrawDefault(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, true);
        }

        public EnumVisibiltyControllerDrawer(SerializedProperty property) : base(property)
        {
            Draws = new Dictionary<T, Action>();
        }
    }
}
#endif