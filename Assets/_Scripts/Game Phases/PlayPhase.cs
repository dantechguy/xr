using Logging;
using UnityEngine;

public class PlayPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Play phase enabled");
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Play phase disabled");
    }
}