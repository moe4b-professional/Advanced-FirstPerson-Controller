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
    [CreateAssetMenu(menuName = MoeTools.Constants.Paths.Tools + "Player Prefs X")]
    public class PlayerPrefsX : ScriptableObjectResourceSingleton<PlayerPrefsX>
    {
        [SerializeField]
        protected string fileName = "Player Prefs X.dat";
        public string FileName { get { return fileName; } }
        public string DirectoryPath
        {
            get
            {
                if (Application.isEditor)
                    return Application.dataPath;

                return Application.persistentDataPath;
            }
        }
        public string SavePath
        {
            get
            {
                return Path.Combine(DirectoryPath, fileName);
            }
        }

        public Dictionary<string, object> Dictionary { get; private set; }

        public BinaryFormatter Formatter { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnGameLoad()
        {
            if (InstanceAvailable)
                Instance.Configure();
        }

        protected virtual void Configure()
        {
            Dictionary = new Dictionary<string, object>();
            Formatter = new BinaryFormatter();

            if (File.Exists(SavePath))
                Load();
            else
                Save();
        }

        public virtual void Save()
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate))
            {
                Formatter.Serialize(fs, Dictionary);
            }
        }
        public virtual void Load()
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

        public virtual object Get(string ID)
        {
            return Get<object>(ID);
        }
        public virtual T Get<T>(string ID)
        {
            if (!Dictionary.ContainsKey(ID))
                throw new ArgumentException("ID " + ID.Enclose() + " Not Found Within Player Prefs X");

            var value = Dictionary[ID];

            if (value == null || value is T)
            {
                if (value == null && typeof(T).IsValueType)
                    throw new InvalidCastException("Cannot Cast ID " + ID.Enclose() + " To Type " + 
                        typeof(T).Name.Enclose() + " Because Value Types Cannot Be Assigned Null");

                return (T)value;
            }

            throw new InvalidCastException("Tried To Retrieve " + ID.Enclose() +
                    " As A Type " + typeof(T).Name + " But The Current Value Is Of Type " + value.GetType().Name.Enclose());
        }
        public virtual T Get<T>(string ID, T defaultValue)
        {
            if (Dictionary.ContainsKey(ID))
                return Get<T>(ID);
            else
                return defaultValue;
        }

        public virtual void Set(string ID, object obj)
        {
            if (!CheckSerialization(obj))
                throw new ArgumentException("Cant Serialize Type " + obj.GetType().Name.Enclose() + ", Its Either Not Marked Serializable Or Has Memebers Not Marked Serializable");

            if (IsDefined(ID))
                Dictionary[ID] = obj;
            else
                Dictionary.Add(ID, obj);

            Save();
        }

        public virtual object GetOrSet(string ID, object value)
        {
            return GetOrSet<object>(ID, value);
        }
        public virtual T GetOrSet<T>(string ID, T value)
        {
            if (IsDefined(ID))
                return Get<T>(ID);
            else
            {
                Set(ID, value);

                Save();

                return value;
            }
        }

        public virtual bool Remove(string ID)
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

        public virtual bool CheckSerialization(object obj)
        {
            if (obj == null)
                return true;

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

        public virtual bool IsDefined(string ID)
        {
            return Dictionary.ContainsKey(ID);
        }

        public virtual void Clear()
        {
            Dictionary.Clear();

            Save();
        }
    }
}