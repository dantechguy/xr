using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup mixer;
    
    private Button button_;
    private AudioSource source_;
    
    private void Start()
    {
        button_ = GetComponent<Button>();
        button_.onClick.AddListener(PlayClickSound);
        
        // create an empty audio source holder object
        var audioSourceHolder = new GameObject("AudioSourceHolder");
        source_ = audioSourceHolder.AddComponent<AudioSource>();
        source_.outputAudioMixerGroup = mixer;
        source_.priority = 0;
        source_.clip = clickSound; 
        source_.playOnAwake = false;
    }

    private void PlayClickSound()
    {
        source_.Play();
    }
}