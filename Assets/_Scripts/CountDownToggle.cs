using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CountDownToggle : MonoBehaviour
{
    [SerializeField] private GeneralSettings generalSettings;
    private Toggle toggle_;
    
    private void OnEnable()
    {
        toggle_ = GetComponent<Toggle>();
        toggle_.isOn = generalSettings.countDown;
    }
    

    public void ToggleCountDown()
    {
        generalSettings.countDown = toggle_.isOn;
    }
}