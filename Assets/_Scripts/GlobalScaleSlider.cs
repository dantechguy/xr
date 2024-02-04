using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalScaleSlider : MonoBehaviour
{
    [SerializeField] private SpawnSettings spawnSettings;
    
    private Slider slider_;

    private void Start()
    {
        slider_ = GetComponent<Slider>();
    }

    public void SetScale()
    {
        var value = slider_.value;
        spawnSettings.globalScale = value;
    }
}
