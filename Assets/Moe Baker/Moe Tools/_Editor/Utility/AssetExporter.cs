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
    [CreateAssetMenu(menuName = MoeTools.Constants.Paths.Tools + "Moe Tools Exporter")]
	public class AssetExporter : ScriptableObject
	{
		[SerializeField]
        protected string source;
        public string Source { get { return source; } }

        [SerializeField]
        protected string destination;
        public string Destination { get { return destination; } }

        [SerializeField]
        protected string fileName = "Package";
        public virtual string FileName { get { return fileName; } }

        public virtual string ExtendedFileName { get { return fileName + Extension; } }
        public const string Extension = ".unitypackage";

        public string DestinationFilePath { get { return Path.Combine(Destination, ExtendedFileName); } }

        public virtual ExportPackageOptions ExportFlags { get { return ExportPackageOptions.Recurse; } }

        public virtual void Process()
        {
            if (File.Exists(DestinationFilePath))
                File.Delete(DestinationFilePath);

            AssetDatabase.ExportPackage(MoeTools.Editor.ToUnityPath(source), ExtendedFileName, ExportFlags);

            File.Move(ExtendedFileName, DestinationFilePath);

            AssetDatabase.Refresh();
        }

        [CustomEditor(typeof(AssetExporter))]
        public class Inspector : InspectorBaseCustomDrawer<AssetExporter>
        {
            public PathDrawer Source { get; protected set; }
            public PathDrawer Destination { get; protected set; }

            protected override void OnEnable()
            {
                base.OnEnable();

                Source = new PathDrawer(serializedObject.FindProperty("source"), PathDrawer.TargetType.Folder);
                Destination = new PathDrawer(serializedObject.FindProperty("destination"), PathDrawer.TargetType.Folder);

                gui.Overrides.Add(Source.Property, Source.Draw);
                gui.Overrides.Add(Destination.Property, Destination.Draw);
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Process"))
                    Process();
            }

            protected virtual void Process()
            {
                target.Process();
            }
        }
    }
}
#endif