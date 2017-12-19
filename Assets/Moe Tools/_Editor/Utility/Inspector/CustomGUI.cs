#if UNITY_EDITOR
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
using System.Linq;

namespace Moe.Tools
{
    public class BaseCustomGUI
    {
        public List<SerializedProperty> Childern { get; protected set; }

        public OverrideController Overrides { get; protected set; }
        [Serializable]
        public class OverrideController
        {
            public Dictionary<string, Action<SerializedProperty>> Dictionary { get; protected set; }

            public void Add(string name, Action action)
            {
                Add(name, (SerializedProperty property) => action());
            }
            public void Add(string name, Action<SerializedProperty> action)
            {
                Dictionary.Add(name, action);
            }

            public void Add(SerializedProperty property, Action action)
            {
                Add(property.name, (SerializedProperty argument) => action());
            }
            public void Add(SerializedProperty property, Action<SerializedProperty> action)
            {
                Dictionary.Add(property.name, action);
            }

            public void Remove(string name)
            {
                if (Contains(name))
                    Dictionary.Remove(name);
            }

            public bool Contains(string name)
            {
                return Dictionary.ContainsKey(name);
            }

            public void Draw(SerializedProperty property)
            {
                Dictionary[property.name](property);
            }

            public OverrideController()
            {
                Dictionary = new Dictionary<string, Action<SerializedProperty>>();
            }
        }

        public IgnoreController Ignores { get; protected set; }
        [Serializable]
        public class IgnoreController
        {
            public HashSet<string> HashSet { get; protected set; }

            public void Add(string name)
            {
                HashSet.Add(name);
            }
            public void Remove(string name)
            {
                if (Contains(name))
                    HashSet.Remove(name);
            }

            public bool Contains(string name)
            {
                return HashSet.Contains(name);
            }

            public IgnoreController()
            {
                HashSet = new HashSet<string>();
            }
        }

        public virtual void Draw()
        {
            for (int i = 0; i < Childern.Count; i++)
                DrawProperty(Childern[i]);
        }

        protected virtual void DrawProperty(SerializedProperty property)
        {
            if (Ignores.Contains(property.name))
                return;

            if (Overrides.Contains(property.name))
                Overrides.Draw(property);
            else
                DefaultDraw(property);
        }

        protected virtual void DefaultDraw(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, true);
        }

        public BaseCustomGUI()
        {
            Overrides = new OverrideController();
            Ignores = new IgnoreController();
        }
    }
    public class PropertyCustomGUI : BaseCustomGUI
    {
        public SerializedProperty Property { get; protected set; }
        public bool DrawFoldout = true;
        public bool Indent = true;

        public override void Draw()
        {
            if (DrawFoldout)
                Property.isExpanded = EditorGUILayout.Foldout(Property.isExpanded, Property.displayName, true);

            if (Property.isExpanded || !DrawFoldout)
            {
                if (Indent)
                    EditorGUI.indentLevel++;

                base.Draw();

                if (Indent)
                    EditorGUI.indentLevel--;
            }
        }

        public PropertyCustomGUI(SerializedProperty property) : base()
        {
            this.Property = property.Copy();
            this.Childern = MoeTools.Inspector.GetChildern(property).ToList();
        }
    }
    public class InspectorCustomGUI : BaseCustomGUI
    {
        public SerializedObject SerializedObject { get; protected set; }

        bool drawScriptField;
        public bool DrawScriptField
        {
            get
            {
                return drawScriptField;
            }
            set
            {
                drawScriptField = value;
                InitChildern();
            }
        }

        public InspectorCustomGUI(SerializedObject serializedObject) : this(serializedObject, true)
        {

        }
        public InspectorCustomGUI(SerializedObject serializedObject, bool drawScriptField) : base()
        {
            this.SerializedObject = serializedObject;

            this.drawScriptField = drawScriptField;

            InitChildern();
        }
        protected virtual void InitChildern()
        {
            Childern = MoeTools.Inspector.GetChildern(SerializedObject);

            if (!drawScriptField)
                Childern.RemoveAt(0);
        }
    }
}
#endif