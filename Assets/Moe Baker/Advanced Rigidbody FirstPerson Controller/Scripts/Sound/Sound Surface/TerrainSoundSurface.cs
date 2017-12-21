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

namespace ARFC
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
        List<TerrainControllerSoundsSet> soundsSets;
        public List<TerrainControllerSoundsSet> SoundsSets { get { return soundsSets; } }

        void Reset()
        {
            terrain = GetComponent<Terrain>();

            UpdateSoundsSetsCount();
        }

        void UpdateSoundsSetsCount()
        {
            string[] texturesNames = terrain.terrainData.splatPrototypes.GetArrayOf(delegate (SplatPrototype splat) { return splat.texture.name; });

            List<TerrainControllerSoundsSet> newSet = new List<TerrainControllerSoundsSet>();

            TerrainControllerSoundsSet tempObj;

            for (int x = 0; x < texturesNames.Length; x++)
            {
                tempObj = new TerrainControllerSoundsSet(texturesNames[x]);

                for (int y = 0; y < soundsSets.Count; y++)
                {
                    if (soundsSets[y].TextureName == texturesNames[x])
                    {
                        tempObj.SoundsSet = soundsSets[y].SoundsSet;
                        break;
                    }
                }

                newSet.Add(tempObj);
            }

            soundsSets = newSet;
        }

        public string GetSplatMapTextureName(int index)
        {
            return terrain.terrainData.splatPrototypes[index].texture.name;
        }

        public ControllerSoundStates GetSoundsSet(float x, float z)
        {
            return soundsSets[GetMainTexture(terrain, new Vector3(x, 0, z))].SoundsSet;
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
        public class TerrainControllerSoundsSet
        {
            [SerializeField]
            public string TextureName;

            [SerializeField]
            ControllerSoundStates soundsSet;
            public ControllerSoundStates SoundsSet { get { return soundsSet; } set { soundsSet = value; } }

            public TerrainControllerSoundsSet(string texureName)
            {
                this.TextureName = texureName;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(TerrainSoundSurface))]
        public class Inspector : InspectorBase<TerrainSoundSurface>
        {
            SerializedProperty terrain;

            InspectorList soundsSets;

            protected override void OnEnable()
            {
                base.OnEnable();

                terrain = serializedObject.FindProperty("terrain");

                soundsSets = new InspectorList(serializedObject.FindProperty("soundsSets"));

                soundsSets.drawElementCallback = DrawSoundSetElement;

                soundsSets.displayAdd = false;
                soundsSets.displayRemove = false;
                soundsSets.draggable = false;
            }

            private void DrawSoundSetElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                string textureName = soundsSets.GetPropertyOfIndex(index, "TextureName").stringValue;

                Object soundsSet = soundsSets.GetPropertyOfIndex(index, "soundsSet").objectReferenceValue;

                soundsSets.GetPropertyOfIndex(index, "soundsSet").objectReferenceValue =
                    EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), textureName, soundsSet, typeof(ControllerSoundStates), false);
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.PropertyField(terrain);

                if (terrain.objectReferenceValue)
                {
                    soundsSets.Draw();

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