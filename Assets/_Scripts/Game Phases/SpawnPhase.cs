using Logging;
using UnityEngine;

public class SpawnPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject spawnUICanvas;
    [SerializeField] private SpawnInputManager spawnInputManager_;

    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Spawn phase enabled");
        spawnUICanvas.SetActive(true);
        spawnInputManager_.enabled = true;
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Spawn phase disabled");
        spawnUICanvas.SetActive(false);
        spawnInputManager_.enabled = false;
    }
}