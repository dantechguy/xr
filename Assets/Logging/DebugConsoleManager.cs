using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleManager : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup textGroup;
    [SerializeField] private GameObject textPrefab;

    public void AddText(string _text, Color _color)
    {
        GameObject textObject = Instantiate(textPrefab, textGroup.transform);
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = _text;
        text.color = _color;
    }
    
    public void AddText(string _text)
    {
        AddText(_text, Color.white);
    }
}
