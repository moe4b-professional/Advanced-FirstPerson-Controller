using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moe.Tools
{
    public partial class OptionsBox : Selectable
    {
        [SerializeField]
        OptionsBoxArrow next;
        public OptionsBoxArrow Next { get { return next; } }

        [SerializeField]
        OptionsBoxArrow previous;
        public OptionsBoxArrow Previous { get { return previous; } }

        [SerializeField]
        Text label;
        public Text Label { get { return label; } }

        [SerializeField]
        int value;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                SetValue(value);
            }
        }

        [SerializeField]
        List<string> options;
        public List<string> Options
        {
            get
            {
                return options;
            }
            set
            {
                if (value == null)
                    options.Clear();
                else
                    options = value;

                UpdateText();
            }
        }

        [SerializeField]
        OptionBoxEvent onValueChanged;
        public OptionBoxEvent OnValueChanged
        {
            get
            {
                return onValueChanged;
            }
        }

        public virtual bool CanGoForward
        {
            get
            {
                return (value != options.Count - 1 || options.Count <= 2) && options.Count != 1;
            }
        }
        public virtual bool CanGoBackwards
        {
            get
            {
                return (value != 0 || options.Count <= 2) && options.Count != 1;
            }
        }

        public bool AtRuntime { get { return Application.isPlaying; } }

        protected override void Awake()
        {
            base.Awake();

            if(AtRuntime)
            {
                next.Init(() => { if (CanGoForward) Value++; });
                previous.Init(() => { if (CanGoBackwards) Value--; });
            }
        }

        protected override void Start()
        {
            base.Start();

            if(AtRuntime)
            {
                UpdateInteractability();

                UpdateText();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.button == PointerEventData.InputButton.Left && CanGoForward)
                Value++;
            else if (eventData.button == PointerEventData.InputButton.Right && CanGoBackwards)
                Value--;
        }

        public virtual void SetValue(int newValue)
        {
            value = MoeTools.Math.ClampRewind(newValue, 0, options.Count - 1);

            UpdateInteractability();
            UpdateText();

            onValueChanged.Invoke(value);
        }
        public void UpdateInteractability()
        {
            next.Interactable = CanGoForward;

            previous.Interactable = CanGoBackwards;
        }
        public void UpdateText()
        {
            if (options.Count == 0)
                label.text = "";
            else
                label.text = options[value];
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (eventData.moveDir == MoveDirection.Right)
            {
                if (CanGoForward)
                {
                    Value++;
                }

                return;
            }
            else if (eventData.moveDir == MoveDirection.Left)
            {
                if (CanGoBackwards)
                {
                    Value--;
                }

                return;
            }

            base.OnMove(eventData);
        }

        public bool AddValue(string value)
        {
            if (options.Contains(value))
                return false;

            options.Add(value);
            return true;
        }

        public void RemoveValue(string value)
        {
            if (options.Contains(value))
                RemoveValue(options.IndexOf(value));
        }
        public void RemoveValue(int index)
        {
            options.RemoveAt(index);

            if (index <= value)
                Value--;
        }

        public void SetValues<EnumType>(int index = 0)
        {
            options.Clear();

            string[] names = Enum.GetNames(typeof(EnumType));

            for (int i = 0; i < names.Length; i++)
            {
                options.Add(names[i]);
            }

            Value = index;
            UpdateText();
        }

        [Serializable]
        public class OptionBoxEvent : UnityEvent<int>
        {

        }

        public static class Tools
        {
            public static void SetFromArray(OptionsBox optionsBox, string[] options, int value)
            {
                SetFromList(optionsBox, options, value, delegate (string sValue) { return sValue; });
            }
            public static void SetFromList<TData>(OptionsBox optionsBox, IList<TData> list, int value, Func<TData, string> nameGetter)
            {
                optionsBox.options.Clear();

                optionsBox.options = list.GetArrayOf(nameGetter).ToList();

                optionsBox.SetValue(value);
            }

            public static void SetFromEnum<TEnum>(OptionsBox optionsBox, TEnum value)
                where TEnum : IFormattable, IConvertible, IComparable
            {
                SetFromEnum(optionsBox, value, delegate (TEnum eValue) { return eValue.ToString(); });
            }

            public static void SetFromEnum<TEnum>(OptionsBox optionsBox, TEnum value, Func<TEnum, string> nameGetter)
                where TEnum : IFormattable, IConvertible, IComparable
            {
                optionsBox.options.Clear();

                Array values = Enum.GetValues(value.GetType());

                for (int i = 0; i < values.Length; i++)
                {
                    optionsBox.options.Add(nameGetter((TEnum)values.GetValue(i)));

                    if (values.GetValue(i).Equals(value))
                        optionsBox.SetValue(i);
                }

                optionsBox.UpdateText();
            }
        }
    }
}