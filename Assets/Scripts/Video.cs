using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class Video : MonoBehaviour
{
    public string mapToLoad;
    public VideoPlayer cinematic;
    private bool startLeaving;
    private bool waitingToLeave;
    public InputAction Submit;
    private void Start()
    {
        startLeaving = false;
        waitingToLeave = true;
        Submit.Enable();
    }
    void Update()
    {
        if (((cinematic.frame > 0 && cinematic.isPlaying == false) ||
            Submit.ReadValue<float>() > 0) && !startLeaving)
        {
            startLeaving = true;
        }
        else if (startLeaving && waitingToLeave)
        {
            waitingToLeave = false;
            StartCoroutine(UpdateVariables());
        }
    }
    private IEnumerator UpdateVariables()
    {
        yield return null;
        SceneManager.LoadScene(mapToLoad);
    }
}
