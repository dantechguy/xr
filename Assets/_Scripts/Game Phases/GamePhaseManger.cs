using System;
using System.Collections.Generic;
using Logging;
using UnityEngine;

[RequireComponent(typeof(SpawnPhase), 
    typeof(SelectPhase), typeof(PlayPhase))]
public class GamePhaseManger : MonoBehaviour
{
    public enum GamePhase
    {
        Spawn = 0,
        Select = 1,
        Play = 2
    }

    public interface IGamePhase
    {
        public void Enable();
        public void Disable();
    }

    private List<IGamePhase> gamePhases_;
    private IGamePhase currentPhase_;

    private void Start()
    {
        gamePhases_ = new List<IGamePhase>
        {
            GetComponent<SpawnPhase>(),
            GetComponent<SelectPhase>(),
            GetComponent<PlayPhase>()
        };
        
        foreach (IGamePhase phase in gamePhases_)
        {
            phase.Disable();
        } 

        currentPhase_ = gamePhases_[(int)GamePhase.Spawn];
        currentPhase_.Enable();
    }


    public void SwitchPhase(GamePhase _phase)
    {
        currentPhase_.Disable();

        var index = (int)_phase;
        if (index < 0 || index >= gamePhases_.Count)
        {
            XLogger.LogError(Category.GamePhase, $"Invalid phase index: {index}");
            return;
        }

        currentPhase_ = gamePhases_[(int)_phase];
        currentPhase_.Enable();
    }
    
    // for events in unity editor (e.g. button click)
    public void SwitchToSpawnPhase()
    {
        SwitchPhase(GamePhase.Spawn);
    }
    
    public void SwitchToSelectPhase()
    {
        SwitchPhase(GamePhase.Select);
    }
    
    public void SwitchToPlayPhase()
    {
        SwitchPhase(GamePhase.Play);
    }
}