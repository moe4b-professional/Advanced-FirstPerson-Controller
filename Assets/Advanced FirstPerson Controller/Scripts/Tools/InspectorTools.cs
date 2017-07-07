#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class InspectorTools
{
    public static SerializedProperty[] GetChildern(this SerializedProperty property)
    {
        SerializedProperty iterator = property.Copy();

        SerializedProperty endProperty = iterator.GetEndProperty();

        iterator.NextVisible(true);

        List<SerializedProperty> properties = new List<SerializedProperty>();
        while (true)
        {
            if (SerializedProperty.EqualContents(iterator, endProperty))
                break;

            properties.Add(iterator.Copy());

            iterator.NextVisible(false);
        }

        return properties.ToArray();
    }
}

public static class GUILayoutArea
{
    public static Rect ProgressSingleLineHeight(ref Rect rect, float topAdd = 1.5f, float bottomAdd = 1.5f)
    {
        return Progress(ref rect, EditorGUIUtility.singleLineHeight, topAdd, bottomAdd);
    }

    public static Rect Progress(ref Rect rect, float height, float topAdd = 1.5f, float bottomAdd = 1.5f)
    {
        rect.y += topAdd;

        Rect rc = new Rect(rect.x, rect.y, rect.width, height);

        rect.y += height + bottomAdd;

        return rc;
    }
}

public class LayoutPropertyDrawer
{
    public SerializedProperty property;

    public string label;
    public bool drawHeader = true;

    public virtual void Draw()
    {
        if (drawHeader)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawElements();
                EditorGUI.indentLevel--;
            }
        }
        else
            DrawElements();
    }

    public virtual void DrawElements()
    {

    }

    protected LayoutPropertyDrawer(SerializedProperty property)
    {
        this.property = property;

        label = property.displayName;
    }
}

public class InspectorList : ReorderableList
{
    public string label;

    public InspectorList(SerializedProperty property) : base(property.serializedObject, property)
    {
        label = property.displayName;

        drawHeaderCallback = delegate (Rect rect)
        {
            if (EditorGUI.indentLevel == 0)
            {
                rect.x += 10;
                rect.width -= 10;
            }
            else
            {
                rect.x = 0f;
                rect = EditorGUI.IndentedRect(rect);
            }

            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
        };
        drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(GUILayoutArea.ProgressSingleLineHeight(ref rect), GetArrayElement(index), new GUIContent());
        };
    }

    public virtual void Draw()
    {
        if (serializedProperty.isExpanded)
            DoLayoutList();
        else
        {
            serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded, label, true);
        }
    }

    public SerializedProperty GetPropertyOfIndex(int index, string name)
    {
        return serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(name);
    }

    public SerializedProperty GetArrayElement(int index)
    {
        return serializedProperty.GetArrayElementAtIndex(index);
    }
}

public class DualInspectorList
{
    public InspectorList main;

    public InspectorList preview;

    public Func<SerializedProperty, SerializedProperty> GetPreviewProperty;
    public Action<InspectorList> InitPreview;

    public DualInspectorList(SerializedProperty main)
    {
        this.main = new InspectorList(main);
        
        this.main.onAddCallback += delegate
        {
            ReorderableList.defaultBehaviours.DoAddButton(this.main);
            RefreshSelectedList();
        };
        this.main.onRemoveCallback += delegate
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(this.main);
            RefreshSelectedList();
        };
        this.main.onSelectCallback += delegate
        {
            RefreshSelectedList();
        };
    }

    public void RefreshSelectedList()
    {
        if (main.index == -1)
        {
            preview = null;
        }
        else
        {
            preview = new InspectorList(GetPreviewProperty(main.GetArrayElement(main.index)));
            InitPreview(preview);

            preview.serializedProperty.isExpanded = true;
        }
    }

    public void Draw()
    {
        main.Draw();

        if (preview != null && main.serializedProperty.isExpanded)
        {
            preview.Draw();
        }
    }
}

public class InspectorBase<InspectedType> : Editor where InspectedType : Object
{
    new protected InspectedType target;
    new protected InspectedType[] targets;

    protected virtual void OnEnable()
    {
        target = (InspectedType)base.target;
    }
}

public class CustomGUIOverride
{
    public SerializedProperty property;
    public SerializedObject serializedObject;

    public Dictionary<string, Action<SerializedProperty>> Overrides;
    public List<string> Ignores;

    public Action<SerializedProperty> DefaultPropertyDrawOverride;

    public void AddOverride(string name, Action<SerializedProperty> action)
    {
        Overrides.Add(name, action);
    }
    public void AddIgnore(string name)
    {
        Ignores.Add(name);
    }

    public void Draw()
    {
        SerializedProperty current = property.Copy();

        if(serializedObject == null)
            current.isExpanded = EditorGUILayout.Foldout(current.isExpanded, current.displayName, true);

        bool show = current.isExpanded || serializedObject != null;

        current.NextVisible(true);

        if (show)
        {
            if (serializedObject == null)
                EditorGUI.indentLevel++;

            while (true)
            {
                if (Overrides.ContainsKey(current.name))
                    Overrides[current.name](current);
                else if(!Ignores.Contains(current.name))
                {
                    if (DefaultPropertyDrawOverride != null)
                        DefaultPropertyDrawOverride(current);
                    else
                        EditorGUILayout.PropertyField(current, true);
                }

                if (!current.NextVisible(false) || (SerializedProperty.EqualContents(current, property.GetEndProperty()) && serializedObject == null))
                    break;
            }

            if (serializedObject == null)
                EditorGUI.indentLevel--;
        }
    }

    public CustomGUIOverride(SerializedProperty property)
    {
        Overrides = new Dictionary<string, Action<SerializedProperty>>();
        Ignores = new List<string>();

        this.property = property;
    }
    public CustomGUIOverride(SerializedObject serializedObject, bool skipScriptField = false)
    {
        this.serializedObject = serializedObject;

        Overrides = new Dictionary<string, Action<SerializedProperty>>();
        Ignores = new List<string>();

        property = serializedObject.GetIterator();
        property.Next(true);

        if (skipScriptField)
            property.NextVisible(false);
    }
}

public class ListPopup<TData>
{
    public SerializedProperty property;

    public int selectedIndex;
    public string[] displayNames;

    public event Action<string> OnValueChanged;

    public virtual void Draw()
    {
        selectedIndex = EditorGUILayout.Popup(property.displayName, selectedIndex, displayNames);

        ProcessValue();
    }
    public virtual void Draw(ref Rect rect)
    {
        selectedIndex = EditorGUI.Popup(GUILayoutArea.ProgressSingleLineHeight(ref rect), property.displayName, selectedIndex, displayNames);

        ProcessValue();
    }

    protected virtual void ProcessValue()
    {
        if (displayNames.Length > 0 && property.stringValue != displayNames[selectedIndex])
        {
            property.stringValue = displayNames[selectedIndex];

            if (OnValueChanged != null)
                OnValueChanged(property.stringValue);
        }
    }

    public ListPopup(SerializedProperty property, IList<TData> list, Func<TData, string> nameGetter)
    {
        this.property = property;

        if (list != null && list.Count != 0)
        {
            displayNames = new string[list.Count];

            for (int i = 0; i < displayNames.Length; i++)
            {
                displayNames[i] = nameGetter(list[i]);

                if (property.stringValue == displayNames[i])
                    selectedIndex = i;
            }
        }
        else
            displayNames = new string[] { };
    }
}

public class EnumDrawer<T>
{
    public SerializedProperty property;
    public string label;

    public T Value { get; protected set; }

    string[] enumValues;

    public virtual void Draw()
    {
        property.enumValueIndex = EditorGUILayout.Popup(label, property.enumValueIndex, property.enumDisplayNames);

        Value = GeneralTools.ParseEnum<T>(enumValues[property.enumValueIndex]);
    }

    public EnumDrawer(SerializedProperty property)
    {
        this.property = property;
        this.label = property.displayName;

        enumValues = property.enumNames;
    }
}
public class PropertyDisplayEnumDrawer<T> : EnumDrawer<T>
{
    Dictionary<T, Action> Values;

    public void AddOverride(T value, SerializedProperty property)
    {
        AddOverride(value, ()=>DefaultValueDraw(property));
    }

    public void AddOverride(T value, Action action)
    {
        Values.Add(value, action);
    }

    public override void Draw()
    {
        base.Draw();

        foreach (var item in Values)
        {
            if(Value.Equals(item.Key))
                item.Value();
        }
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

public class CreateScriptableObjectButton<TScriptableObject> where TScriptableObject : ScriptableObject
{
    public string buttonName;

    public string fileName;
    public virtual string FullFileName { get { return fileName + ".asset"; } }

    public string folderPath;
    public virtual string FullFolderPath { get { return "Assets/" + folderPath; } }

    public CreateScriptableObjectButton(string fileName, string folderPath)
    {
        buttonName = "Create " + fileName;

        this.fileName = fileName;
        this.folderPath = folderPath;
    }

    public virtual void Draw()
    {
        if (GUILayout.Button(buttonName))
        {
            if (!AssetDatabase.IsValidFolder(FullFolderPath))
                AssetDatabase.CreateFolder("Assets/", folderPath);

            EditorTools.CreateAsset(typeof(TScriptableObject), FullFolderPath + '/' + FullFileName);
        }
    }
}
#endif