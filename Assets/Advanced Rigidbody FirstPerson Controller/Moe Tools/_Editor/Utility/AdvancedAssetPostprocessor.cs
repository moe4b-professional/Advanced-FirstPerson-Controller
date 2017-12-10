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
    public class AdvancedAssetPostprocessor : AssetPostprocessor
    {
        public virtual string CSFile
        {
            get
            {
                return ToString();
            }
        }
        public string PostprocessorPath
        {
            get
            {
                string PpPath = MoeTools.Editor.GetScriptPath(CSFile);
                if (PpPath == string.Empty)
                    throw new Exception("Couldn't Find Path For " + ToString() + ", Cs File : " + CSFile);

                return PpPath;
            }
        }

        public string GlobalAssetPath
        {
            get
            {
                return MoeTools.Editor.ToSystemPath(assetPath);
            }
        }

        public bool FirstImport
        {
            get
            {
                return !File.Exists(GlobalAssetPath + ".meta");
            }
        }

        public bool WithinProcessorBounds
        {
            get
            {
                return GlobalAssetPath.Contains(PostprocessorPath);
            }
        }
    }
}
#endif