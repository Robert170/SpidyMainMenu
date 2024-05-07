using System.Collections;
using UnityEngine;

public class MusicBetweenScenes : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    private MusicBetweenScenes instance;
    public LevelData levelData;

    public MusicBetweenScenes Instance
    {
        get
        {
            return instance;
        }
    }
    void Start()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);

        }
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;

        }
        else
        {
            instance = this;

        }
        DontDestroyOnLoad(gameObject);
        audioSource.clip = audioClip1;
        if (StaticVar.CurrentLevel == 6 && levelData.level[StaticVar.CurrentLevel].firstTimeAccess)
        {
            audioSource.Stop();
        }
        else
        {
            StartCoroutine(StartMusic());

        }

    }

    public void StopMusic()
    {
        audioSource.Stop();
        audioSource.clip = audioClip2;
    }

    public IEnumerator StartMusic()
    {
        yield return null;
        audioSource.Play();
    }
    public IEnumerator FadeMusic()
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < 2.0f)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / 2.0f);
            yield return null;
        }
        yield break;
    }
    public void Destroyer()
    {
        Destroy(gameObject);
    }
}
