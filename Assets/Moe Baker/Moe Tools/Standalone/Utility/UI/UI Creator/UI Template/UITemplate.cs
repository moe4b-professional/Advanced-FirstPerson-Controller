using System;
using System.IO;
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
    public class UITemplate<TData> : MonoBehaviour
    {
        public TData Data { get; protected set; }

        public virtual string LabelPrefix { get { return "-UI Template"; } }

        public virtual void SetData(TData data)
        {
            this.Data = data;

            gameObject.name = GetLabel(data);
        }

        public virtual string GetLabel(TData data)
        {
            return GetLabelSuffix(data) + LabelPrefix;
        }
        public virtual string GetLabelSuffix(TData data)
        {
            return data.ToString();
        }
    }
}