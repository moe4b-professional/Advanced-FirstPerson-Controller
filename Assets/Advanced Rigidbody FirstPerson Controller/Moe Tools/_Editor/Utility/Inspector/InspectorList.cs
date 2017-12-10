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

namespace Moe.Tools
{
    public class InspectorList : ReorderableList
    {
        public string label;

        bool listExpaned = true;
        public virtual bool Expanded
        {
            get
            {
                if (serializedProperty == null)
                    return listExpaned;

                return serializedProperty.isExpanded;
            }
            set
            {
                if (serializedProperty == null)
                    listExpaned = value;
                else
                    serializedProperty.isExpanded = value;
            }
        }

        int previousIndex;

        public virtual void Draw()
        {
            if (Expanded)
                DoLayoutList();
            else
                Expanded = EditorGUILayout.Foldout(Expanded, label, true);

            CheckIndexChange();
        }
        public virtual void Draw(Rect rect)
        {
            if (Expanded)
                DoList(rect);
            else
                Expanded = EditorGUI.Foldout(rect, Expanded, label, true);

            CheckIndexChange();
        }

        public event Action OnIndexChanged;
        protected virtual void CheckIndexChange()
        {
            if (previousIndex != index)
            {
                previousIndex = index;

                if (OnIndexChanged != null)
                    OnIndexChanged();
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

        public InspectorList(SerializedProperty property) : base(property.serializedObject, property)
        {
            label = property.displayName;
            Init();
        }
        public InspectorList(string label, IList elements, Type elementType) : base(elements, elementType)
        {
            this.label = label;
            Init();
        }

        protected virtual void Init()
        {
            previousIndex = index;

            drawHeaderCallback = DrawDefaultHeader;
            drawElementCallback = DrawDefaultElement;

            elementHeight = 18;
        }

        protected virtual void DrawDefaultHeader(Rect rect)
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

            Expanded = EditorGUI.Foldout(rect, Expanded, label, true);
        }

        protected virtual void DrawDefaultElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            GUIArea.Progress(ref rect, GUIArea.LayoutOffset);

            elementHeight = GetDefaultElementsSize();

            if (GetArrayElement(index).hasVisibleChildren)
                DrawDefaultMultiElements(rect, index, isActive, isFocused);
            else
                DrawDefaultSingleElements(rect, index, isActive, isFocused, GetArrayElement(index));
        }
        protected virtual void DrawDefaultMultiElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            List<SerializedProperty> childern = GetArrayElement(index).GetChildern();

            if (childern.Count == 1)
            {
                DrawDefaultSingleElements(rect, index, isActive, isFocused, childern[0]);

                return;
            }

            Rect childRect;
            for (int i = 0; i < childern.Count; i++)
            {
                childRect = GUIArea.ProgressLayout(ref rect, EditorGUI.GetPropertyHeight(childern[i], true));
                EditorGUI.PropertyField(childRect, childern[i]);
            }
        }
        protected virtual void DrawDefaultSingleElements(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), property);
        }

        protected virtual float GetDefaultElementsSize()
        {
            SerializedProperty property = GetArrayElement(0);

            float height = GUIArea.LayoutOffset * 2;

            if(property.hasVisibleChildren)
            {
                List<SerializedProperty> childern = property.GetChildern();

                for (int i = 0; i < childern.Count; i++)
                    height += EditorGUI.GetPropertyHeight(childern[i], false) + GUIArea.LayoutOffset;
            }
            else
                height += EditorGUI.GetPropertyHeight(property, false) + GUIArea.LayoutOffset;

            return height;
        }
    }

    public class SelectionInspectorList : InspectorList
    {
        Func<SerializedProperty, string> ElementNameGetter;

        public PropertyCustomGUI SelectionGUI;
        public void UpdateUI()
        {
            if (index == -1)
                SelectionGUI = null;
            else
            {
                SelectionGUI = new PropertyCustomGUI(GetArrayElement(index));

                SelectionGUI.DrawFoldout = false;
            }
        }

        public override void Draw()
        {
            MoeTools.Inspector.StartVertical(serializedProperty.displayName);

            base.Draw();

            if (SelectionGUI != null && Expanded)
                DrawSelected();

            MoeTools.Inspector.EndVertical();
        }
        protected virtual void DrawSelected()
        {
            SelectionGUI.Draw();
        }

        public static Func<SerializedProperty, string> DefaultNameGetter
        {
            get
            {
                return GetElmentNameGetter("name");
            }
        }
        public static Func<SerializedProperty, string> GetElmentNameGetter(string propertyPath)
        {
            return delegate (SerializedProperty property) { return property.FindPropertyRelative(propertyPath).stringValue; };
        }
        public SelectionInspectorList(SerializedProperty property, Func<SerializedProperty, string> elementNameGetter) : base(property)
        {
            this.ElementNameGetter = elementNameGetter;
        }

        protected override void Init()
        {
            base.Init();

            OnIndexChanged += UpdateUI;
            UpdateUI();

            label = "List";
        }

        protected override void DrawDefaultElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            string label = ElementNameGetter(GetArrayElement(index));

            if (label == string.Empty)
                label = "--------";

            EditorGUI.LabelField(GUIArea.ProgressLine(ref rect), label);
        }
    }

    public class DualInspectorList
    {
        public InspectorList main;

        public InspectorList preview;

        public Func<SerializedProperty, SerializedProperty> GetPreviewProperty;
        public Func<PreviewListData> GetPreviewListData;
        public Action<InspectorList> InitPreview;

        public DualInspectorList(SerializedProperty mainProperty)
        {
            this.main = new InspectorList(mainProperty);

            Init();
        }
        public DualInspectorList(string label, IList mainList, Type elementsType)
        {
            this.main = new InspectorList(label, mainList, elementsType);

            Init();
        }

        protected virtual void Init()
        {
            this.main.OnIndexChanged += RefreshSelectedList;
        }

        public void RefreshSelectedList()
        {
            if (main.index == -1)
            {
                preview = null;
            }
            else
            {
                if (GetPreviewProperty != null)
                    preview = new InspectorList(GetPreviewProperty(main.GetArrayElement(main.index)));
                else if (GetPreviewListData != null)
                {
                    PreviewListData data = GetPreviewListData();

                    preview = new InspectorList(data.label, data.list, data.type);
                }

                InitPreview(preview);

                preview.Expanded = true;
            }
        }

        public void Draw()
        {
            main.Draw();

            if (preview != null && main.Expanded)
                preview.Draw();
        }

        [Serializable]
        public struct PreviewListData
        {
            public string label;
            public IList list;
            public Type type;

            public PreviewListData(string label, IList list, Type type)
            {
                this.label = label;
                this.list = list;
                this.type = type;
            }
        }
    }
}
#endif