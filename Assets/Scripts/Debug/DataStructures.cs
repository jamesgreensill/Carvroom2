using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Carvroom.Data
{
    public abstract class MetaData
    {
        public string DataName { get; set; }
    }

    public unsafe class MetaData<T> : MetaData where T : unmanaged
    {
        public T* Value { get; set; }

        public MetaData(T* ptr, string dataName)
        {
            Value = ptr;
            DataName = dataName;
        }

        public override string ToString()
        {
            return $"{DataName} : {Value->ToString()}";
        }

        public void Modify(T data)
        {
            *Value = data;
        }
    }

    public struct DebugObjectData
    {
        public MetaData Data;
        public DebugForms Form;
        public Vector2 SliderMinMax;
        public UnityAction Action;
    }

    public enum DebugForms
    {
        Text,
        Slider,
    }
}