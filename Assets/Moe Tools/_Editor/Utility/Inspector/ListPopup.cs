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
    public abstract class BaseListPopup<TData>
    {
        public SerializedProperty Property { get; protected set; }

        public bool DrawLabel = true;
        public virtual string Label
        {
            get
            {
                if (DrawLabel)
                    return Property.displayName;

                return "";
            }
        }

        public int Index { get; set; }
        public string[] Names { get; protected set; }

        public virtual void Draw(Rect rect)
        {
            Index = EditorGUI.Popup(rect, Label, Index, Names);

            ProcessValueChange();
        }
        public virtual void Draw()
        {
            Index = EditorGUILayout.Popup(Label, Index, Names);

            ProcessValueChange();
        }

        protected virtual void ProcessValueChange()
        {

        }

        public event Action OnValueChanged;
        protected virtual void ValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged();
        }

        public BaseListPopup(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter)
        {
            Constructor(property, list, nameGetter);
        }
        public BaseListPopup(SerializedProperty property, IList<string> list)
        {
            Constructor(property, list.ToArray());
        }

        protected virtual void Constructor(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter)
        {
            Constructor(property, list.IsNullOrEmpty() ? new string[] { } : list.GetArrayOf(nameGetter));
        }
        protected virtual void Constructor(SerializedProperty property, string[] names)
        {
            this.Property = property;

            this.Names = names;

            Index = GetValueIndex();
        }

        protected abstract int GetValueIndex();
    }

    public class ListPopup<TData> : BaseListPopup<TData>
    {
        public bool IsInt { get { return Property.propertyType == SerializedPropertyType.Integer; } }
        public int IntValue
        {
            get
            {
                return Property.intValue;
            }
            set
            {
                Property.intValue = value;
            }
        }

        public bool IsString { get { return Property.propertyType == SerializedPropertyType.String; } }
        public string StringValue
        {
            get
            {
                return Property.stringValue;
            }
            set
            {
                Property.stringValue = value;
            }
        }

        protected override void ProcessValueChange()
        {
            base.ProcessValueChange();

            if (Names.Length == 0)
                return;

            if (IsInt)
            {
                if (Index != IntValue)
                {
                    IntValue = Index;

                    ValueChanged();
                }
            }
            else if (IsString)
            {
                if (StringValue != Names[Index])
                {
                    StringValue = Names[Index];

                    ValueChanged();
                }
            }
        }

        public ListPopup(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter) : base(property, list, nameGetter)
        {
            
        }
        public ListPopup(SerializedProperty property, IList<string> list) : base(property, list)
        {
            
        }

        protected override int GetValueIndex()
        {
            if(IsInt)
            {
                return Mathf.Clamp(IntValue, 0, Names.Length - 1);
            }
            else if(IsString)
            {
                for (int i = 0; i < Names.Length; i++)
                {
                    if (Names[i] == StringValue)
                        return i;
                }

                return 0;
            }

            return 0;
        }
    }

    public class ObjectListPopup<TData> : BaseListPopup<TData>
        where TData : Object
    {
        public Object ObjectValue
        {
            get
            {
                return Property.objectReferenceValue;
            }
            set
            {
                Property.objectReferenceValue = value;
            }
        }

        public IList<TData> List { get; protected set; }

        public ObjectListPopup(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter) : base(property, list, nameGetter)
        {

        }

        protected override void Constructor(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter)
        {
            this.List = list;

            base.Constructor(property, list, nameGetter);
        }

        protected override int GetValueIndex()
        {
            for (int i = 0; i < List.Count; i++)
            {
                if(List[i] == ObjectValue)
                    return i;
            }

            return 0;
        }

        protected override void ProcessValueChange()
        {
            base.ProcessValueChange();

            if (List.IsNullOrEmpty())
            {
                if (ObjectValue != null)
                    ObjectValue = null;

                return;
            }

            if(List[Index] != ObjectValue)
            {
                ObjectValue = List[Index];

                ValueChanged();
            }
        }
    }
}
#endif