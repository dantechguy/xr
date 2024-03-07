using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIClickSounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clickSounds;
    [SerializeField] private AudioMixerGroup mixer;
    
    private AudioSource source_;

    private void Start()
    {
        CreateAudioSource();
        AttachSoundEffect(GetComponent<Selectable>());
    }

    private void CreateAudioSource()
    {
        var audioSourceHolder = new GameObject("AudioSourceHolder");
        Transform holder = GameObject.Find("Audio Source Holder").transform;
        if (holder == null)
        {
            holder = new GameObject("Audio Source Holder").transform;
        }
        audioSourceHolder.transform.SetParent(holder);

        source_ = audioSourceHolder.AddComponent<AudioSource>();
        source_.outputAudioMixerGroup = mixer;
        source_.priority = 0;
        source_.playOnAwake = false;
    }

    private void AttachSoundEffect(Selectable selectable)
    {
        if (selectable is Button button)
        {
            button.onClick.AddListener(PlayClickSound);
        }
        else if (selectable is Toggle toggle)
        {
            toggle.onValueChanged.AddListener(_ => PlayClickSound());
        }
    }

    private void PlayClickSound()
    {
        source_.clip = clickSounds[Random.Range(0, clickSounds.Count)];
        source_.Play();
    }
}
