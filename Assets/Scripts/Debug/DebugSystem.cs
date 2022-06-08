using Carvroom.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public unsafe class DebugSystem : Singleton<DebugSystem>
{
    /// <summary>
    /// How to add an item.
    /// </summary>
    //unsafe
    //{
    //    fixed (double* ptr = &weewoo)
    //    {
    //        DebugSystem.Instance.Add(ptr, "weewoo");
    //    }
    //}

    public Dictionary<string, MetaData> Parameters = new Dictionary<string, MetaData>();

    public void Add<T>(T* data, string dataName, DebugForms form = DebugForms.Text, float min = 0, float max = 0, UnityAction action = null) where T : unmanaged
    {
        if (Parameters.ContainsKey(dataName))
            throw new Exception($"Debug Dictionary already contains key: {dataName}");

        MetaData mData = new MetaData<T>(data, dataName);

        Parameters.Add(dataName, mData);

        DebugInterface.Instance.AddObjectToGroup?.Invoke(new DebugObjectData()
        {
            Data = mData,
            SliderMinMax = new Vector2(min, max),
            Form = form,
            Action = action
        });
    }

    public void Remove(string dataName)
    {
        if (!Parameters.ContainsKey(dataName))
        {
            Debug.Log($"Debug Dictionary does not contain key: {dataName}.");
            return;
        }

        Parameters.Remove(dataName);
    }

    public void Modify<T>(string dataName, T value) where T : unmanaged
    {
        if (!Parameters.ContainsKey(dataName))
        {
            Debug.Log($"Debug Dictionary does not contain key: {dataName}.");
            return;
        }

        var data = (MetaData<T>)Parameters[dataName];

        *data.Value = value;
    }
}