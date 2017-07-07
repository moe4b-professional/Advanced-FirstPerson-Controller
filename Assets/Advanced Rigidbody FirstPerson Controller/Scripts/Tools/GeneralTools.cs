using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GeneralTools
{
	public static bool IntToBool(int value)
    {
        return value == 0 ? false : true;
    }
    public static int BoolToInt(bool value)
    {
        return value ? 1 : 0;
    }

    #region List
    public static void ForEach<T>(this IList<T> list, Action<T> command)
    {
        for (int i = 0; i < list.Count; i++)
            command(list[i]);
    }
    public static void ForEach<T>(this IList<T> list, Action<T, int> command)
    {
        for (int i = 0; i < list.Count; i++)
            command(list[i], i);
    }

    public static bool ContainsMemeber<T>(this IList<T> list, Func<T, bool> checkCondition)
    {
        try
        {
            list.FindMember(checkCondition);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    public static T FindMember<T>(this IList<T> list, Func<T, bool> checkCondition)
    {
        return list[FindMemberIndex(list, checkCondition)];
    }
    public static int FindMemberIndex<T>(this IList<T> list, Func<T, bool> checkCondition)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (checkCondition(list[i]))
                return i;
        }

        throw new ArgumentException("No Field That Corresponds To The Check Condition Was Found");
    }

    public static bool IsInRange(this IList list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    public static T GetRandom<T>(this IList<T> list)
    {
        return list[GetRandomIndex(list)];
    }
    public static int GetRandomIndex<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0f)
            throw new ArgumentNullException();

        if (list.Count == 1)
            return 0;

        return Random.Range(0, list.Count);
    }

    public static bool ContainstArray<T>(this IList<T> list, IList<T> parent)
    {
        for (int x = 0; x < parent.Count; x++)
        {
            bool found = false;
            for (int y = 0; y < list.Count; y++)
            {
                if (list[y].Equals(parent[x]))
                {
                    found = true;
                    break;
                }
            }

            if (found)
                continue;
            else
                return false;
        }

        return true;
    }

    public static TResault[] GetArrayOf<TSource, TResault>(this IList<TSource> list, Func<TSource, TResault> getFunction)
    {
        TResault[] array = new TResault[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            array[i] = getFunction(list[i]);
        }

        return array;
    }

    public static bool ContainsNulls(this IList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
                return true;
        }

        return false;
    }
    #endregion

    public static void CheckComponent<TRef>(Component[] components, string checkString, ref TRef resault) where TRef : Component
    {
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].name.ToLower().Contains(checkString))
                resault = components[i].gameObject.GetOrAddComponent<TRef>();
        }
    }

    public static void ValidateListFromSource<T>(IList<T> original, IList<T> invalid, Func<T, T, bool> checkCondition)
    {
        ValidateListFromSource(original, invalid, checkCondition, null);
    }
    public static void ValidateListFromSource<T>(IList<T> original, IList<T> invalid, Func<T, T, bool> checkCondition, Action<T, T> assignAction)
    {
        for (int x = 0; x < invalid.Count; x++)
        {
            for (int y = 0; y < original.Count; y++)
            {
                if (checkCondition(invalid[x], original[y]))
                {
                    if(assignAction == null)
                        original[y] = invalid[x];
                    else
                        assignAction(original[y], invalid[x]);

                    break;
                }
            }
        }
    }

    public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
    {
        return dictionary.Values.Contains(value);
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value);
    }

    public static T GetResource<T>(string name) where T : Object
    {
        return Resources.Load<T>(name);
    }

    public static int GetRandom(this DualInt dualInt)
    {
        return Random.Range(dualInt.x, dualInt.y);
    }

    public static float GetRandom(this IRange range)
    {
        return Random.Range(range.Min, range.Max);
    }
    public static float Clamp(this IRange range, float value)
    {
        return Mathf.Clamp(value, range.Min, range.Max);
    }

    #region GameObject
    public static void SetLayer(this GameObject gameobject, string layerName)
    {
        SetLayer(gameobject, LayerMask.NameToLayer(layerName));
    }
    public static void SetLayer(this GameObject gameobject, int layerIndex)
    {
        gameobject.layer = layerIndex;

        for (int i = 0; i < gameobject.transform.childCount; i++)
        {
            SetLayer(gameobject.transform.GetChild(i).gameObject, layerIndex);
        }
    }

    public static Bounds GetFullBounds(this GameObject gameObject)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Bounds bounds = new Bounds();

        if (renderer)
            bounds = renderer.bounds;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            bounds.Encapsulate(GetFullBounds(gameObject.transform.GetChild(i).gameObject));
        }

        return bounds;
    }

    public static T GetLocalComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component != null)
            return component;

        return gameObject.GetComponentInChildren<T>();
    }
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (!gameObject.GetComponent<T>())
            return gameObject.AddComponent<T>();

        return gameObject.GetComponent<T>();
    }

    public static void SetCollision(GameObject obj1, GameObject obj2, bool enabled)
    {
        Collider[] col1 = obj1.GetComponentsInChildren<Collider>();
        Collider[] col2 = obj2.GetComponentsInChildren<Collider>();

        for (int x = 0; x < col1.Length; x++)
        {
            for (int y = 0; y < col2.Length; y++)
            {
                Physics.IgnoreCollision(col1[x], col2[y], !enabled);
            }
        }
    }
    #endregion

    public static void DestroyChildern(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    public static T GetBehaviour<T>(this Animator anim, string stateName) where T : ImprovedStateMachineBehaviour
    {
        return anim.GetBehaviour<T>(stateName, anim.GetBehaviours<T>());
    }
    public static T GetBehaviour<T>(this Animator anim, string stateName, ImprovedStateMachineBehaviour[] behaviours) where T : ImprovedStateMachineBehaviour
    {
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i].Name == stateName)
                return (T)behaviours[i];
        }

        return null;
    }

    public static byte[] GetBytes(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();

        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }
    }

    public static T GetObject<T>(byte[] data)
    {
        BinaryFormatter bf = new BinaryFormatter();

        using (MemoryStream ms = new MemoryStream(data))
        {
            object obj = bf.Deserialize(ms);

            return (T)obj;
        }
    }
}

public class EventProperty<TData>
{
    public TData Property
    {
        set
        {
            if (onProperty != null)
                onProperty(value);
        }
    }

    event Action<TData> onProperty;
    public event Action<TData> OnProperty { add { onProperty += value; } remove { onProperty -= value; } }

    public void Clear()
    {
        onProperty = null;
    }
}

public class ActionEventBase<TActionList>
{
    protected List<TActionList> actions;
    public List<TActionList> Actions { get { return actions; } }

    public void Add(TActionList action)
    {
        actions.Add(action);
    }
    public void Remove(TActionList action)
    {
        actions.Remove(action);
    }

    public void Clear()
    {
        actions.Clear();
    }
}
public class ActionEvent<T1> : ActionEventBase<Action<T1>>
{
    public void Invoke(T1 t1)
    {
        actions.ForEach(delegate (Action<T1> action)
        {
            if (actions == null)
                actions.Remove(action);
            else
                action(t1);
        });
    }
}
public class ActionEvent<T1, T2> : ActionEventBase<Action<T1, T2>>
{
    public void Invoke(T1 t1, T2 t2)
    {
        actions.ForEach(delegate (Action<T1, T2> action)
        {
            if (actions == null)
                actions.Remove(action);
            else
                action(t1, t2);
        });
    }
}
public class ActionEvent<T1, T2, T3> : ActionEventBase<Action<T1, T2, T3>>
{
    public void Invoke(T1 t1, T2 t2, T3 t3)
    {
        actions.ForEach(delegate (Action<T1, T2, T3> action)
        {
            if (actions == null)
                actions.Remove(action);
            else
                action(t1, t2, t3);
        });
    }
}
public class ActionEvent<T1, T2, T3, T4> : ActionEventBase<Action<T1, T2, T3, T4>>
{
    public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
    {
        actions.ForEach(delegate (Action<T1, T2, T3, T4> action)
        {
            if (actions == null)
                actions.Remove(action);
            else
                action(t1, t2, t3, t4);
        });
    }
}

public class ActionEventProperty<TData>
{
    public TData Property
    {
        set
        {
            ActionEvent.Invoke(value);
        }
    }

    public ActionEvent<TData> ActionEvent { get; private set; }

    public ActionEventProperty()
    {
        ActionEvent = new ActionEvent<TData>();
    }

    public void Clear()
    {
        ActionEvent.Clear();
    }
}

[Serializable]
public class ImprovedStateMachineBehaviour : StateMachineBehaviour
{
    [SerializeField]
    new string name;
    public string Name
    {
        get
        {
            return name;
        }
    }
}

[Serializable]
public struct DualInt
{
    [SerializeField]
    internal int x;

    [SerializeField]
    internal int y;

    public DualInt(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class AutoCoroutine
{
    MonoBehaviour behaviour;
    Coroutine coroutine;

    Func<IEnumerator> function;

    public bool Running
    {
        get
        {
            return coroutine != null;
        }
        set
        {
            if (value)
            {
                if (Running)
                    Running = false;

                coroutine = behaviour.StartCoroutine(function());
            }
            else
            {
                if (coroutine != null)
                    behaviour.StopCoroutine(coroutine);

                coroutine = null;
            }
        }
    }

    public void Start()
    {
        Running = true;
    }

    public void End()
    {
        Running = false;
    }

    public AutoCoroutine(MonoBehaviour behaviour, Func<IEnumerator> function)
    {
        this.behaviour = behaviour;
        this.function = function;
    }
}

public class ListIndexer<TAccessor, TData>
{
    public Dictionary<TAccessor, int> Indexer { get; protected set; }

    public int this[TAccessor accessor]
    {
        get
        {
            if (!Contains(accessor))
                throw new ArgumentException(accessor + " Not Definded In ListIndexer");

            return Indexer[accessor];
        }
    }

    public bool Contains(TAccessor name)
    {
        return Indexer.ContainsKey(name);
    }

    public void Add(TAccessor accessor, int value)
    {
        Indexer.Add(accessor, value);
    }
    public void Remove(TAccessor accessor)
    {
        Indexer.Remove(accessor);
    }

    public ListIndexer()
    {
        Indexer = new Dictionary<TAccessor, int>();
    }

    public ListIndexer(IList<TData> list, Func<TData, TAccessor> AccessorProvider)
    {
        Indexer = new Dictionary<TAccessor, int>();

        for (int i = 0; i < list.Count; i++)
            Indexer.Add(AccessorProvider(list[i]), i);
    }
}

public class AutoResource<T> where T : Object
{
    string path;
    T resource;
    public T Resource
    {
        get
        {
            if (resource == null)
                GetResource();

            return resource;
        }
    }

    public T GetResource()
    {
        resource = Resources.Load<T>(path);
        return resource;
    }

    public AutoResource(string path)
    {
        this.path = path;
    }
}

public class ComplexMethodBase<TAction>
{
    public TAction Default;
    public TAction Add;
    public TAction Override;

    public ComplexMethodBase(TAction Default)
    {
        this.Default = Default;
    }
}
public class ComplexMethod : ComplexMethodBase<Action>
{
    public ComplexMethod(Action Default) : base(Default)
    {

    }

    public void Invoke()
    {
        if (Override != null)
            Override();
        else
        {
            Default();

            if (Add != null)
                Add();
        }
    }
}
public class ComplexMethod<T1> : ComplexMethodBase<Action<T1>>
{
    public ComplexMethod(Action<T1> Default) : base(Default)
    {

    }

    public void Invoke(T1 data)
    {
        if (Override != null)
            Override(data);
        else
        {
            Default(data);

            if (Add != null)
                Add(data);
        }
    }
}
public class ComplexMethod<T1, T2> : ComplexMethodBase<Action<T1, T2>>
{
    public ComplexMethod(Action<T1, T2> Default) : base(Default)
    {

    }

    public void Invoke(T1 data1, T2 data2)
    {
        if (Override != null)
            Override(data1, data2);
        else
        {
            Default(data1, data2);

            if (Add != null)
                Add(data1, data2);
        }
    }
}

[Serializable]
public class ObjectSpawner
{
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get { return prefab; } }

    public GameObject Spawn(Action<GameObject> action = null)
    {
        GameObject prefabI = Object.Instantiate(prefab);

        prefabI.name = prefab.name;

        if (action != null)
            action(prefabI);

        return prefabI;
    }

    public T Spawn<T>(Action<GameObject> action = null)
    {
        GameObject prefabI = Object.Instantiate(prefab);

        prefabI.name = prefab.name;

        if(action != null)
            action(prefabI);

        return prefabI.GetComponent<T>();
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ObjectSpawner))]
    public class PropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(GUILayoutArea.ProgressSingleLineHeight(ref position), property.FindPropertyRelative("prefab"), new GUIContent(label));
        }
    }
#endif
}