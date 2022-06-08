using System.Collections;
using System.Collections.Generic;
using Carvroom.Data;
using TMPro;
using UnityEngine;

public class DebugText : DebugElement
{
    [SerializeField] public TextMeshProUGUI Text;

    public override void Initialize(DebugObjectData data)
    {
        Data = data;
    }

    public void Update()
    {
        Text.text = Data.Data.ToString();
    }
}
