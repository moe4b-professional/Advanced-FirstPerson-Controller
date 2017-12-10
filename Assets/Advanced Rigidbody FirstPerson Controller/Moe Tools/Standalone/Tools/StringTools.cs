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
	public static partial class MoeTools
	{
        public static class String
        {
            public static List<string> SeperateViaChar(string text, char parseChar)
            {
                List<string> names = new List<string>();

                string value = string.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    if (i == text.Length - 1)
                    {
                        value += text[i];
                        names.Add(value);
                    }
                    else if (text[i] == parseChar)
                    {
                        names.Add(value);
                        value = string.Empty;
                    }
                    else
                    {
                        value += text[i];
                    }
                }

                return names;
            }

            public static string Enclose(string text)
            {
                return Enclose(text, '(', ')');
            }
            public static string Enclose(string text, char start, char end)
            {
                return start + text + end;
            }

            public static string Format(string text)
            {
                string resault = "";

                for (int i = 0; i < text.Length; i++)
                {
                    if(char.IsUpper(text[i]))
                    {
                        if(i != text.Length - 1)
                        {
                            if (!char.IsUpper(text[i + 1]))
                                resault += " ";
                        }
                    }

                    resault += text[i];
                }

                return resault;
            }
        }
    }
}