using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

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
    public static class PlayerPrefsX
    {
        public static Dictionary<string, object> Dictionary { get; private set; }
        public static string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "Player Prefs X.dat");
            }
        }

        public static BinaryFormatter Formatter { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Start()
        {
            Dictionary = new Dictionary<string, object>();
            Formatter = new BinaryFormatter();

            if (File.Exists(SavePath))
                Load();
            else
                Save();
        }

        public static void Save()
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate))
            {
                Formatter.Serialize(fs, Dictionary);
            }
        }
        public static void Load()
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.Open))
            {
                try
                {
                    Dictionary = Formatter.Deserialize(fs) as Dictionary<string, object>;
                }
                catch (Exception)
                {
                    Debug.LogError("Error While Loading Player Prefs X, Resetting");

                    Dictionary = new Dictionary<string, object>();

                    fs.Close();

                    Save();
                }
            }
        }

        public static void Add(string ID, object obj)
        {
            if (CheckSerialization(obj))
            {
                if (Dictionary.ContainsKey(ID))
                    Dictionary[ID] = obj;
                else
                    Dictionary.Add(ID, obj);

                Save();
            }
            else
            {
                var type = obj.GetType();

                Debug.LogError("Cant Serialize Type (" + type.FullName + "), Its Either Not Marked Serializable Or Has Memebers Not Marked Serializable");
            }
        }
        public static bool CheckSerialization(object obj)
        {
            if (!obj.GetType().IsSerializable) return false;

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    Formatter.Serialize(ms, obj);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool Remove(string ID)
        {
            if (IsDefined(ID))
            {
                Dictionary.Remove(ID);

                Save();

                return true;
            }
            else
            {
                Debug.LogError("Pref With ID : " + ID + " Not Defined");

                return true;
            }
        }

        public static bool IsDefined(string ID)
        {
            return Dictionary.ContainsKey(ID);
        }

        public static object Get(string ID)
        {
            return Get<object>(ID);
        }
        public static T Get<T>(string ID)
        {
            return Get(ID, default(T));
        }
        public static T Get<T>(string ID, T defaultValue)
        {
            if (Dictionary.ContainsKey(ID))
                return (T)Dictionary[ID];
            else
                return defaultValue;
        }

        public static void Set(string ID, object obj)
        {
            Add(ID, obj);
        }

        public static void Clear()
        {
            Dictionary.Clear();

            Save();
        }
    }
}