#if UNITY_EDITOR
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditorInternal;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Moe.Tools
{
    [RequireComponent(typeof(LightProbeGroup))]
    public class LightProbeRegion : MonoBehaviour
    {
        [SerializeField]
        public Vector3 size = Vector3.one;

        [SerializeField]
        PlacementData placement = new PlacementData(1);
        [Serializable]
        public struct PlacementData
        {
            [SerializeField]
            public int x;
            [SerializeField]
            public int y;
            [SerializeField]
            public int z;

            public PlacementData(int size)
            {
                x = size;
                y = size;
                z = size;
            }
        }

        [HideInInspector]
        LightProbeGroup probes;
        public LightProbeGroup Probes
        {
            get
            {
                if (probes == null)
                    probes = GetComponent<LightProbeGroup>();

                return probes;
            }
        }

        void Reset()
        {
            probes = GetComponent<LightProbeGroup>();
            probes.probePositions = new Vector3[0];
        }

        void Build()
        {
            Vector3 index;
            List<Vector3> positions = new List<Vector3>();

            index = new Vector3(size.x / 2, size.y / 2, size.z / 2);

            for (int x = 0; x < placement.x; x++)
            {
                for (int y = 0; y < placement.y; y++)
                {
                    for (int z = 0; z < placement.z; z++)
                    {
                        index.x = Mathf.Lerp(size.x / 2, -size.x / 2, x / 1f / (placement.x - 1));
                        index.y = Mathf.Lerp(size.y / 2, -size.y / 2, y / 1f / (placement.y - 1));
                        index.z = Mathf.Lerp(size.z / 2, -size.z / 2, z / 1f / (placement.z - 1));

                        positions.Add(index);
                    }
                }
            }

            Probes.probePositions = positions.ToArray();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Vector3.zero, size);
        }

        [CanEditMultipleObjects]
        [CustomEditor(typeof(LightProbeRegion))]
        public class Inspector : InspectorBase<LightProbeRegion>
        {
            protected override void OnEnable()
            {
                base.OnEnable();
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Build"))
                {
                    ForAllTargets((LightProbeRegion lpr) => lpr.Build());
                }
            }
        }
    }
}
#endif