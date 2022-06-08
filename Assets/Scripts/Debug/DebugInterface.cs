using System;
using Carvroom.Data;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DebugInterface : Singleton<DebugInterface>
{
    public GridLayoutGroup Group;
    public List<DebugElement> DebugObjects = new List<DebugElement>();

    public GameObject SliderTemplate;
    public GameObject TextTemplate;

    public UnityEvent<DebugObjectData> AddObjectToGroup;

    public override void Awake()
    {
        base.Awake();

        // Add Listeners.
        AddObjectToGroup.AddListener(AddObject);
    }

    public void AddObject(DebugObjectData data)
    {
        switch (data.Form)
        {
            case DebugForms.Text:
                {
                    var text = Instantiate(TextTemplate, Group.transform);
                    text.SetActive(true);
                    text.name = data.Data.DataName;
                    var component = text.GetComponent<DebugText>();
                    component.Initialize(data);
                    DebugObjects.Add(component);
                }
                break;

            case DebugForms.Slider:
                {
                    var slider = Instantiate(SliderTemplate, Group.transform);
                    slider.SetActive(true);
                    slider.name = data.Data.DataName;
                    var component = slider.GetComponent<DebugSlider>();
                    component.Initialize(data);
                    component.SetMinMax(data.SliderMinMax.x, data.SliderMinMax.y);
                    DebugObjects.Add(component);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(data.Form), data.Form, null);
        }

    }
}