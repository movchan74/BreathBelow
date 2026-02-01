using UnityEngine;
using UnityEngine.UI;

public class AirBar : MonoBehaviour
{

    public Slider airSlider;
    public MicReader micReader;

    void Update()
    {
        airSlider.value = micReader.air;
    }
}
