using System.Collections;
using System.Collections.Generic;
using Carvroom.Data;
using UnityEngine;

public abstract class DebugElement : MonoBehaviour
{
    public DebugObjectData Data;

    public abstract void Initialize(DebugObjectData data);
}
