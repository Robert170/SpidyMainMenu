using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixerGroup[] mixerGroups;
    private AudioSource activeSoloAudioSource;
    private AudioSource activeShareAudioSource;

    private void Awake()
    {
        GameObject soundGameObject = new GameObject("Sound");
        activeSoloAudioSource = soundGameObject.AddComponent<AudioSource>();
        activeShareAudioSource = soundGameObject.AddComponent<AudioSource>();
    }

    public void PlaySoloSound(AudioClip soundClip)
    {
        PlayClip(mixerGroups[0], soundClip, 0);
    }

    public void PlaySharedSound(int priority, AudioClip soundClip)
    {
        if (activeShareAudioSource.priority < priority)
        {
            PlaySharedClip(mixerGroups[0], soundClip, priority);
        }
        else if (!activeShareAudioSource.isPlaying)
        {
            PlaySharedClip(mixerGroups[0], soundClip, priority);
        }
    }

    private void PlayClip(AudioMixerGroup group, AudioClip clip, int priority)
    {
        activeSoloAudioSource.outputAudioMixerGroup = group;
        activeSoloAudioSource.clip = clip;
        activeSoloAudioSource.priority = priority;

        activeSoloAudioSource.Play();
    }

    private void PlaySharedClip(AudioMixerGroup group, AudioClip clip, int priority)
    {
        activeShareAudioSource.outputAudioMixerGroup = group;
        activeShareAudioSource.clip = clip;
        activeShareAudioSource.priority = priority;

        activeShareAudioSource.Play();
        StartCoroutine(DestroyAfterClipLength(clip.length));
    }

    private IEnumerator DestroyAfterClipLength(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        activeShareAudioSource.Stop();
    }
}
