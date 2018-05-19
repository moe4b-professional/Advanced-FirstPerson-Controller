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
    [CreateAssetMenu(menuName = MoeTools.Constants.Paths.Tools + "Canvas Scale Profile")]
	public class CanvasScaleProfile : ScriptableObject
	{
        [SerializeField]
        protected ScaleModeOverrideValue scaleMode;
        public ScaleModeOverrideValue ScaleMode { get { return scaleMode; } }
        [Serializable]
        public class ScaleModeOverrideValue : OverrideValue<CanvasScaler.ScaleMode>
        {

        }

        [SerializeField]
        protected FloatOverrideValue scale;
        public FloatOverrideValue Scale { get { return scale; } }

        [SerializeField]
        protected MatchModeOverrideValue matchMode;
        public MatchModeOverrideValue MatchMode { get { return matchMode; } }
        [Serializable]
        public class MatchModeOverrideValue : OverrideValue<MatchModeData>
        {
            public virtual void Apply(CanvasScaler scaler)
            {
                scaler.screenMatchMode = value.Mode;
                scaler.referenceResolution = value.ReferenceResolution;
                scaler.matchWidthOrHeight = value.Match;
            }
        }
        [Serializable]
        public struct MatchModeData
        {
            [SerializeField]
            Vector2 referenceResolution;
            public Vector2 ReferenceResolution { get { return referenceResolution; } }

            [SerializeField]
            CanvasScaler.ScreenMatchMode mode;
            public CanvasScaler.ScreenMatchMode Mode { get { return mode; } }

            [SerializeField]
            [Range(0f, 1f)]
            float match;
            public float Match { get { return match; } }
        }

        public virtual void Apply(CanvasScaler scaler)
        {
            if (scaleMode.Enabled)
                scaler.uiScaleMode = scaleMode.Value;

            if (scale.Enabled)
                scaler.scaleFactor = scale.Value;

            if (matchMode.Enabled)
                matchMode.Apply(scaler);
        }
	}
}