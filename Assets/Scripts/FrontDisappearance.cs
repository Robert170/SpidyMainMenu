using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDisappearance : MonoBehaviour
{
    private Spidy actor;                                         //Reference to main character
    private bool waitingYMeasure;
    private float valueYOffset;
    private Vector3 mainCharacterPosition;
    public bool hiddenWall;
    public float thresholdValue = 900.0f;
    public bool lower = true;
    private int counter;
    // Start is called before the first frame update
    void Start()
    {
        waitingYMeasure = false;
        if (!lower) hiddenWall = true;
        StartCoroutine(SelectHero());
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingYMeasure && actor != null)
        {
            valueYOffset = actor.transform.position.y - mainCharacterPosition.y;
            if (Mathf.Abs(valueYOffset) > 100)
            {
                StartCoroutine(ValidatingYChange());
            }
        }
    }
    protected virtual void SetOpacity(float value)
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = value;
        GetComponent<SpriteRenderer>().color = color;
    }
    protected virtual void ShowHide()
    {
        if (lower)
        {
            if (actor.transform.position.y > thresholdValue && hiddenWall)
            {
                StartCoroutine(HideWall());
            }
            else if (actor.transform.position.y <= thresholdValue && !hiddenWall)
            {
                StartCoroutine(ShowWall());
            }
        }
        else
        {
            if (actor.transform.position.y < thresholdValue && hiddenWall)
            {
                StartCoroutine(HideWall());
            }
            else if (actor.transform.position.y >= thresholdValue && !hiddenWall)
            {
                StartCoroutine(ShowWall());
            }
        }
    }
    private IEnumerator HideWall()
    {
        int i = 10;
        while (i >= 0)
        {
            SetOpacity(i * 0.1f);
            yield return new WaitForFixedUpdate();
            i--;
        }
        hiddenWall = false;
    }
    private IEnumerator ShowWall()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (!lower) counter = 5;   else counter = 10;
        int i = 0;
        while (i <= counter)
        {
            SetOpacity(i * 0.1f);
            yield return new WaitForFixedUpdate();
            i++;
        }
        hiddenWall = true;
    }
    private IEnumerator ValidatingYChange()
    {
        waitingYMeasure = true;
        yield return new WaitForSecondsRealtime(0.25f);
        waitingYMeasure = false;
        if (Mathf.Abs(mainCharacterPosition.y + valueYOffset - actor.transform.position.y) <= 1)
        {
            mainCharacterPosition.y = actor.transform.position.y;
            ShowHide();
        }
    }
    private IEnumerator SelectHero()
    {
        yield return new WaitForFixedUpdate();
        actor = FindObjectOfType<Spidy>();
        mainCharacterPosition = actor.transform.position;
        ShowHide();
    }
}
