using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moe.Tools
{
    public class InputModule : ScriptableObject
    {
        public virtual void Init()
        {
            Clear();
        }

        public virtual void Process()
        {

        }

        public virtual void Clear()
        {

        }
    }
}