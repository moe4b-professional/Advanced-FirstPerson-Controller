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
using URandom = UnityEngine.Random;
using SEnum = System.Enum;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Enum
        {
            public static T Parse<T>(string value)
                where T : IFormattable, IConvertible, IComparable
            {
                return (T)SEnum.Parse(typeof(T), value);
            }

            public static T GetNextValue<T>(T value)
                where T : IFormattable, IConvertible, IComparable
            {
                Array values = SEnum.GetValues(typeof(T));

                for (int i = 0; i < values.Length; i++)
                {
                    if (values.GetValue(i).Equals(value))
                    {
                        return (T)values.GetValue(Math.ClampRewind(i + 1, 0, values.Length - 1));
                    }
                }

                throw new ArgumentException();
            }
            public static T GetPreviousValue<T>(T value)
                where T : IFormattable, IConvertible, IComparable
            {
                Array values = SEnum.GetValues(typeof(T));

                for (int i = 0; i < values.Length; i++)
                {
                    if (values.GetValue(i).Equals(value))
                    {
                        return (T)values.GetValue(Math.ClampRewind(i - 1, 0, values.Length - 1));
                    }
                }

                throw new ArgumentException();
            }

            public static int ToIndex<T>(T value)
                where T : IFormattable, IConvertible, IComparable
            {
                Array values = SEnum.GetValues(typeof(T));

                for (int i = 0; i < values.Length; i++)
                {
                    if (values.GetValue(i).Equals(value))
                        return i;
                }

                throw new ArgumentException("Enum Value " + value + " Not Defined");
            }
            public static T FromIndex<T>(int index)
                where T : IFormattable, IConvertible, IComparable
            {
                return (T)SEnum.GetValues(typeof(T)).GetValue(index);
            }

            public static int GetLength<T>()
                where T : IFormattable, IConvertible, IComparable
            {
                return SEnum.GetValues(typeof(T)).Length;
            }

            public static bool HasFlag<T>(T value, T flag)
                where T : IFormattable, IConvertible, IComparable
            {
                return (Convert.ToInt64(value) & Convert.ToInt64(flag)) == Convert.ToInt64(flag);
            }

            public static string ToFriendlyString<T>(T value)
                where T : IFormattable, IConvertible, IComparable
            {
                return ToFriendlyString(value, delegate (T eValue) { return String.Format(eValue.ToString()); });
            }
            public static string ToFriendlyString<T>(T value, Func<T, string> nameGetter)
                where T : IFormattable, IConvertible, IComparable
            {
                return nameGetter(value);
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static T GetNextValue<T>(this T value)
                where T : IFormattable, IConvertible, IComparable
        {
            return MoeTools.Enum.GetNextValue(value);
        }
        public static T GetPreviousValue<T>(this T value)
            where T : IFormattable, IConvertible, IComparable
        {
            return MoeTools.Enum.GetPreviousValue(value);
        }

        public static bool HasFlag<T>(this T value, T flag)
                where T : IFormattable, IConvertible, IComparable
        {
            return MoeTools.Enum.HasFlag(value, flag);
        }

        public static string ToFriendlyString<T>(this T value)
                where T : IFormattable, IConvertible, IComparable
        {
            return MoeTools.Enum.ToFriendlyString(value);
        }
        public static string ToFriendlyString<T>(this T value, Func<T, string> nameGetter)
            where T : IFormattable, IConvertible, IComparable
        {
            return MoeTools.Enum.ToFriendlyString(value, nameGetter);
        }
    }
}