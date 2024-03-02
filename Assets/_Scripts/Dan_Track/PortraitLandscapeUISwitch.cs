using UnityEngine;

public class PortraitLandscapeUISwitch : MonoBehaviour
{
    [SerializeField] private GameObject portraitUI;
    [SerializeField] private GameObject landscapeUI;
    
    private void Update()
    {
        if (Screen.width > Screen.height)
        {
            portraitUI.SetActive(false);
            landscapeUI.SetActive(true);
        }
        else
        {
            portraitUI.SetActive(true);
            landscapeUI.SetActive(false);
        }
    }
    
}