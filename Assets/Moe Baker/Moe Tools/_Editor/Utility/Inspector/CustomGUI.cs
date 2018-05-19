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

            public void Remove(SerializedProperty property)
            {
                Remove(property);
            }
            public void Remove(string name)
            {
                if (Contains(name))
                    Dictionary.Remove(name);
            }

            public bool Contains(SerializedProperty property)
            {
                return Contains(property.name);
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

        public ElementController Ignores { get; protected set; }
        public ElementController Disabled { get; protected set; }

        [Serializable]
        public class ElementController
        {
            public HashSet<string> HashSet { get; protected set; }

            public void Add(SerializedProperty property)
            {
                Add(property.name);
            }
            public void Add(string name)
            {
                HashSet.Add(name);
            }

            public void Remove(SerializedProperty property)
            {
                Remove(property.name);
            }
            public void Remove(string name)
            {
                if (Contains(name))
                    HashSet.Remove(name);
            }

            public bool Contains(SerializedProperty property)
            {
                return Contains(property.name);
            }
            public bool Contains(string name)
            {
                return HashSet.Contains(name);
            }

            public ElementController()
            {
                HashSet = new HashSet<string>();
            }
        }

        public virtual void Draw()
        {
            for (int i = 0; i < Childern.Count; i++)
                ProcessProperty(Childern[i]);
        }

        protected virtual void ProcessProperty(SerializedProperty property)
        {
            if (Ignores.Contains(property))
                return;

            if (Overrides.Contains(property))
                Overrides.Draw(property);
            else if(Disabled.Contains(property))
                DrawDisabled(property);
            else
                DrawDefault(property);
        }

        protected virtual void DrawDefault(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, true);
        }
        protected virtual void DrawDisabled(SerializedProperty property)
        {
            GUI.enabled = false;

            DrawDefault(property);

            GUI.enabled = true;
        }

        public BaseCustomGUI()
        {
            Overrides = new OverrideController();
            Ignores = new ElementController();
            Disabled = new ElementController();
        }
    }

    public class PropertyCustomGUI : BaseCustomGUI
    {
        public SerializedProperty Property { get; protected set; }
        public bool DrawFoldout { get; set; }
        public bool Indent { get; set; }

        public override void Draw()
        {
            if (DrawFoldout)
                Property.isExpanded = EditorGUILayout.Foldout(Property.isExpanded, Property.displayName, true);

            if (Property.isExpanded || !DrawFoldout)
            {
                if (Indent)
                    EditorGUI.indentLevel++;

                DrawChildern();

                if (Indent)
                    EditorGUI.indentLevel--;
            }
        }
        public virtual void DrawChildern()
        {
            base.Draw();
        }

        public PropertyCustomGUI(SerializedProperty property) : base()
        {
            this.Property = property;
            this.Childern = MoeTools.Inspector.GetChildern(property).ToList();

            DrawFoldout = true;
            Indent = true;
        }
    }

    public class InspectorCustomGUI : BaseCustomGUI
    {
        public SerializedObject SerializedObject { get; protected set; }

        ScriptUIType scriptUI;
        public ScriptUIType ScriptUI
        {
            get
            {
                return scriptUI;
            }
            set
            {
                scriptUI = value;
                InitChildern();
            }
        }
        public const ScriptUIType DefaultScriptUI = ScriptUIType.DisabledField;

        public const string ScriptPropertyName = "m_Script";
        public virtual bool IsScriptProperty(SerializedProperty property)
        {
            return property.name == ScriptPropertyName;
        }

        protected virtual void InitChildern()
        {
            Childern = MoeTools.Inspector.GetChildern(SerializedObject);
        }

        protected override void ProcessProperty(SerializedProperty property)
        {
            if (IsScriptProperty(property))
                DrawScript(property);
            else
                base.ProcessProperty(property);
        }

        protected virtual void DrawScript(SerializedProperty property)
        {
            switch (scriptUI)
            {
                case ScriptUIType.None:
                    break;

                case ScriptUIType.EditableField:
                    DrawDefault(property);
                    break;

                case ScriptUIType.DisabledField:
                    DrawDisabled(property);
                    break;
            }
        }

        public InspectorCustomGUI(SerializedObject serializedObject) : this(serializedObject, DefaultScriptUI)
        {

        }
        public InspectorCustomGUI(SerializedObject serializedObject, ScriptUIType scriptUI) : base()
        {
            this.SerializedObject = serializedObject;

            this.scriptUI = scriptUI;

            InitChildern();
        }

        public enum ScriptUIType
        {
            None, EditableField, DisabledField
        }
    }
}
#endif