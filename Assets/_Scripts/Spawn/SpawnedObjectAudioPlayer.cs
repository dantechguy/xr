using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using Random = UnityEngine.Random;

public class SpawnedObjectAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;

    public void PlaySpawnAudio(Vector3 _spawnPosition, GameObject _spawnedObject)
    {
        audioSource.transform.position = _spawnPosition;
        
        audioSource.clip = clips[Random.Range(0, clips.Count)];
        var size = Utils.GetBounds(_spawnedObject).size.magnitude;
        audioSource.volume = Mathf.Lerp(0.2f, 1.5f , size);
        audioSource.pitch = Mathf.Lerp(1.0f, 0.2f, size);
        
        audioSource.Play();
    }
}