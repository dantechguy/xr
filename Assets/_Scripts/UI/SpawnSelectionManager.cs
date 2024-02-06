using System;
using Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSelectionManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private GameObject selectItemPrefab;
    [SerializeField] private SpawnSettings spawnSettings;
    [SerializeField] private Color highlightedColor;

    private void Start()
    {
        // reset active index
        spawnSettings.activePrefabIndex = 0;

        PopulateSelectItems();
    }

    private void PopulateSelectItems()
    {
        // clear layout group (note: backward loop to prevent deletion resulting in changes)
        for (int i = layoutGroup.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }

        // populate layout group
        for (int i = 0; i < spawnSettings.prefabs.Count; i++)
        {
            GameObject prefab = spawnSettings.prefabs[i];
            GameObject selectItem = Instantiate(selectItemPrefab, layoutGroup.transform);
            var button = selectItem.GetComponent<Button>();
            var iCopy = i; // closure, otherwise i is passed as a reference
            button.onClick.AddListener(() => OnSelectItem(prefab, iCopy));
            // TODO: use image/icon as well
            selectItem.GetComponentInChildren<TextMeshProUGUI>().text = prefab.name;
            if (i == spawnSettings.activePrefabIndex)
            {
                var image = selectItem.GetComponent<Image>();
                image.color = highlightedColor;
            }
        }
    }

    private void OnSelectItem(GameObject _prefab, int _i)
    {
        XLogger.Log(Category.UI, $"Selected prefab: {_i}");
        spawnSettings.activePrefabIndex = _i;
        HighlightSelected();
    }

    // set the UI element to highlighted, others to normal 
    private void HighlightSelected()
    {
        var index = spawnSettings.activePrefabIndex;
        // XLogger.Log(Category.UI, $"Highlighting prefab: {index}");
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Transform selectItem = layoutGroup.transform.GetChild(i);
            var button = selectItem.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = i == index ? highlightedColor : Color.white;
            colors.selectedColor = i == index ? highlightedColor : Color.white;
            button.colors = colors;
            var image = selectItem.GetComponent<Image>();
            image.color = i == index ? highlightedColor : Color.white;
        }
    }
}