using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using Moe.Tools;

namespace AFPC
{
    public class TerrainSoundSurface : MonoBehaviour
    {
        [SerializeField]
        Terrain terrain;
        public Terrain Terrain
        {
            get
            {
                return terrain;
            }
        }

        [SerializeField]
        List<SetData> sets;
        public List<SetData> Sets { get { return sets; } }

        void Reset()
        {
            terrain = GetComponent<Terrain>();

            UpdateSoundsSetsCount();
        }

        void UpdateSoundsSetsCount()
        {
            string[] texturesNames = terrain.terrainData.splatPrototypes.GetArrayOf(delegate (SplatPrototype splat) { return splat.texture.name; });

            List<SetData> newSet = new List<SetData>();

            SetData tempObj;

            for (int x = 0; x < texturesNames.Length; x++)
            {
                tempObj = new SetData(texturesNames[x]);

                for (int y = 0; y < sets.Count; y++)
                {
                    if (sets[y].TextureName == texturesNames[x])
                    {
                        tempObj.Set = sets[y].Set;
                        break;
                    }
                }

                newSet.Add(tempObj);
            }

            sets = newSet;
        }

        public string GetSplatMapTextureName(int index)
        {
            return terrain.terrainData.splatPrototypes[index].texture.name;
        }

        public MovementSoundSet GetSet(Vector3 position)
        {
            position.y = 0f;

            return sets[GetMainTexture(terrain, position)].Set;
        }

        //shameless copy-paste 
        //Thanks duck http://answers.unity3d.com/users/82/duck.html
        //http://answers.unity3d.com/questions/34328/terrain-with-multiple-splat-textures-how-can-i-det.html
        public static float[] GetTextureMix(Terrain terrain, Vector3 worldPos)
        {
            // returns an array containing the relative mix of textures
            // on the main terrain at this world position.
            // The number of values in the array will equal the number
            // of textures added to the terrain.

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;
            // calculate which splat map cell the worldPos falls within (ignoring y)
            int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
            // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            // extract the 3D array data to a 1D array:
            float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
            for (int n = 0; n < cellMix.Length; ++n)
            {
                cellMix[n] = splatmapData[0, 0, n];
            }
            return cellMix;
        }
        public static int GetMainTexture(Terrain terrain, Vector3 worldPos)
        {
            // returns the zero-based index of the most dominant texture
            // on the main terrain at this world position.
            float[] mix = GetTextureMix(terrain, worldPos);
            float maxMix = 0;
            int maxIndex = 0;
            // loop through each mix value and find the maximum
            for (int n = 0; n < mix.Length; ++n)
            {
                if (mix[n] > maxMix)
                {
                    maxIndex = n;
                    maxMix = mix[n];
                }
            }

            return maxIndex;
        }

        [Serializable]
        public class SetData
        {
            [SerializeField]
            public string TextureName;

            [SerializeField]
            MovementSoundSet set;
            public MovementSoundSet Set { get { return set; } set { set = value; } }

            public SetData(string texureName)
            {
                this.TextureName = texureName;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(TerrainSoundSurface))]
        public class Inspector : MoeInspector<TerrainSoundSurface>
        {
            SerializedProperty terrain;

            InspectorList sets;

            protected override void OnEnable()
            {
                base.OnEnable();

                terrain = serializedObject.FindProperty("terrain");

                sets = new InspectorList(serializedObject.FindProperty("sets"));

                sets.drawElementCallback = DrawSoundSetElement;

                sets.displayAdd = false;
                sets.displayRemove = false;
                sets.draggable = false;
            }

            private void DrawSoundSetElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                string textureName = sets.GetPropertyOfIndex(index, "TextureName").stringValue;

                Object soundsSet = sets.GetPropertyOfIndex(index, "set").objectReferenceValue;

                sets.GetPropertyOfIndex(index, "set").objectReferenceValue =
                    EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), textureName, soundsSet, typeof(MovementSoundSet), false);
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.PropertyField(terrain);

                if (terrain.objectReferenceValue)
                {
                    sets.Draw();

                    if (GUILayout.Button("Update"))
                    {
                        target.UpdateSoundsSetsCount();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}