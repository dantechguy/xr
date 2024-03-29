using System;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using Logging;
using TMPro;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SpawnSelectionManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private SpawnSettings spawnSettings;

    [Header("Select Item")] [SerializeField]
    private GameObject selectItemPrefab;

    [Header("Scan")] [SerializeField] private ScanMesh scanMesh;
    [SerializeField] private string fileName;

    [Header("3D UI")] [SerializeField] private float baseSize;
    [SerializeField] private float selectedSizeMultiplier;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float deselectRotationSpeed = 50f;

    [SerializeField] private float itemMeshSize;
    // [SerializeField] private Color highlightedColor;

    private void Start()
    {
        // reset active index
        spawnSettings.activePrefabIndex = 0;
    }

    private void OnEnable()
    {
        XLogger.Log(Category.UI, "SpawnSelectionManager enabled");

        GameObject customCar = scanMesh.OpenCarFile(fileName);
        if (customCar != null)
        {
            customCar.name = "Scanned Car";
            spawnSettings.customCar = customCar;
        }

        PopulateSelectItems();
    }

    void Update()
    {
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            GameObject selectItem = layoutGroup.transform.GetChild(i).gameObject;

            // Sorry about the code below lol
            Transform meshWrapperTransform = selectItem.GetNamedChild("MeshWrapper").transform;
            if (meshWrapperTransform.childCount <= 0)
                continue;

            Transform meshTransform = meshWrapperTransform.GetChild(0);
            if (i == spawnSettings.activePrefabIndex)
            {
                meshTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else
            {
                meshTransform.localPosition = Vector3.zero;
                // selectItem.GetNamedChild("MeshWrapper").transform.GetChild(0).localRotation = Quaternion.identity;
                meshTransform.localRotation =
                    Quaternion.RotateTowards(meshTransform.localRotation, quaternion.identity, deselectRotationSpeed);
            }
        }

        HighlightSelected();
    }

    private void PopulateSelectItems()
    {
        // clear layout group (note: backward loop to prevent deletion resulting in changes)
        for (int i = layoutGroup.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }

        // populate layout group
        for (int i = 0; i < spawnSettings.prefabs.Count + 1; i++)
        {
            bool last = i == spawnSettings.prefabs.Count;
            if (last && spawnSettings.customCar == null) continue;

            GameObject selectItem = Instantiate(selectItemPrefab, layoutGroup.transform);
            var button = selectItem.GetComponent<Button>();
            GameObject meshWrapper = selectItem.GetNamedChild("MeshWrapper");

            GameObject prefab = last ? spawnSettings.customCar : spawnSettings.prefabs[i];
            GameObject prefabObject = Instantiate(prefab, meshWrapper.transform);
            prefabObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));

            if (prefabObject.TryGetComponent(out Rigidbody prefabRigidbody))
                Destroy(prefabRigidbody);
            if (prefabObject.TryGetComponent(out Collider prefabCollider))
                Destroy(prefabCollider);
            if (prefabObject.TryGetComponent(out ARSpawnedSelectable selectable))
                Destroy(selectable);
            var prefabLight = prefabObject.GetComponentInChildren<Light>();
            if (prefabLight != null)
                Destroy(prefabLight.gameObject);

            // // set material
            // var meshRenderer = selectItem.GetComponentInChildren<MeshRenderer>();
            // var materials = prefab.GetComponentInChildren<MeshRenderer>().sharedMaterials;
            // for (int j = 0; j < materials.Length; j++)
            // {
            //     meshRenderer.AddMaterial(materials[j]);
            // }

            // // set mesh


            // scale the mesh to fit the button
            Bounds bounds = Utils.GetBounds(prefabObject);
            Vector3 maxBounds = new Vector3(0.08f, 0.08f, 0.08f) * itemMeshSize;
            float scaleFactor = Mathf.Min(maxBounds.x / bounds.size.x, maxBounds.y / bounds.size.y,
                maxBounds.z / bounds.size.z);
            prefabObject.transform.localScale *= scaleFactor;

            selectItem.GetComponentInChildren<TextMeshProUGUI>().text = prefab.name;

            var iCopy = i; // closure, otherwise i is passed as a reference
            button.onClick.AddListener(() => OnSelectItem(prefab, iCopy));
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

            // var image = selectItem.GetComponent<Image>();
            // image.color = i == index ? highlightedColor : Color.clear;

            selectItem.GetChild(0).localScale = i == index ? Vector3.one * selectedSizeMultiplier : Vector3.one;
            selectItem.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                i == index ? baseSize * selectedSizeMultiplier : baseSize);
        }
    }
}