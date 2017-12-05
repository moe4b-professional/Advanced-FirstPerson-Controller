using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public abstract class InputFieldSlider<TData> : MonoBehaviour
    {
        [SerializeField]
        protected TData value;
        public TData Value { get { return value; } set { SetValue(value); } }

        public event Action<TData> OnValueChanged;

        [SerializeField]
        InputField inputField;
        public InputField InputField { get { return inputField; } }
        protected abstract InputField.ContentType ContentType { get; }

        [SerializeField]
        Slider slider;
        public Slider Slider { get { return slider; } }
        public float MinValue
        {
            get
            {
                return slider.minValue;
            }
            set
            {
                slider.minValue = value;
            }
        }
        public float MaxValue
        {
            get
            {
                return slider.maxValue;
            }
            set
            {
                slider.maxValue = value;
            }
        }

        protected virtual void Start()
        {
            inputField.contentType = ContentType;

            inputField.onEndEdit.AddListener((string text) =>
            {
                SetValue(GetData(text));
            });
            slider.onValueChanged.AddListener((float value) =>
            {
                SetValue(GetData(value));
            });
        }

        protected virtual void SetValue(TData newData)
        {
            value = ClampValue(newData);

            EditValue();

            inputField.text = GetText(value);
            slider.value = GetFloat(value);

            if (OnValueChanged != null)
                OnValueChanged(value);
        }

        protected virtual void EditValue()
        {

        }

        protected abstract TData GetData(string text);
        public abstract TData GetData(float value);

        public abstract TData ClampValue(TData data);

        public virtual string GetText(TData data)
        {
            return data.ToString();
        }
        public abstract float GetFloat(TData data);
    }
}