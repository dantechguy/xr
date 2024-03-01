using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class GlobalScaleSlider : MonoBehaviour
{
    [SerializeField] private SpawnSettings spawnSettings;
    
    private Slider slider_;

    private void Start()
    {
        slider_ = GetComponent<Slider>();
        slider_.value = spawnSettings.globalScale;
    }

    public void SetScale()
    {
        spawnSettings.globalScale = slider_.value;
    }
}