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

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
	public partial class SoundSet
	{
		[CustomEditor(typeof(SoundSet))]
        public class Inspector : InspectorBaseCustomDrawer<SoundSet>
        {
            InspectorList clips;

            protected override void OnEnable()
            {
                base.OnEnable();

                clips = new InspectorList(serializedObject.FindProperty("clips"));

                gui.Overrides.Add(clips.serializedProperty.name, DrawClips);
            }

            protected virtual void DrawClips()
            {
                clips.Draw();
            }
        }
	}
}
#endif