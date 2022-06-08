using Carvroom.Data;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

public class DebugSlider : DebugElement
{
    public Slider Slider;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Value;

    public override void Initialize(DebugObjectData data)
    {
        Data = data;
        Title.text = Data.Data.DataName;
        Slider.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetMinMax(float min, float max)
    {
        Slider.minValue = min;
        Slider.maxValue = max;
    }

    public void Update()
    {
        Value.text = Slider.value.ToString(CultureInfo.InvariantCulture);
    }

    public unsafe void OnValueChanged(float value)
    {
        var data = (MetaData<float>)Data.Data;
        if (data == null)
            return;

        *data.Value = value;

        Data.Action?.Invoke();
    }
}