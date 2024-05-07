using System.Collections;
using UnityEngine;

public class MoveLayerY : MonoBehaviour
{
    private GameObject gameManager;
    public Transform layer;
    GameObject spidy;
    float zPosition;
    float zLayerPosition;
    float xPosition;                    //Stores the X position of the GamemManager posisiton based on the space this layer can move
    float xLayerPosition;               //Stores the X position of the layer associated based on the GameManager position
    private Color enableAlpha;
    private Color disableAlpha;
    private SpriteRenderer frontImage;
    private float variation;
    private bool count;
    private readonly float lightJump = 0.02f;
    private bool isBehind;
    private bool isFar;
    public float xMovement = 500f;
    public float xScreenSize = 1920;
    public float xContinuousMovement = 1720.0f;
    public float zInitial = 4013.6f;
    public float zThreshold = 4300.0f;
    private FrontDisappearance front;
    private void Start()
    {
        front = GetComponent<FrontDisappearance>();
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        spidy = GameObject.FindGameObjectWithTag("Spidy");
        enableAlpha = new Color(1, 1, 1, 1);
        disableAlpha = new Color(1, 1, 1, 0.5f);
        frontImage = GetComponent<SpriteRenderer>();
        count = true;
        isBehind = true;
        isFar = true;
    }
    void Update()
    {
        float deface = xMovement / xScreenSize;                                        //Ratio on how image will move through axis X
        xPosition = (gameManager.transform.position.x - 960f) * deface;
        xLayerPosition = xPosition + xContinuousMovement;
        zPosition = (gameManager.transform.position.z - 1080) / 2;
        zLayerPosition = zPosition + zInitial;
        layer.position = new Vector3(xLayerPosition, layer.position.y, zLayerPosition);
        if (front != null)
        {
            if (front.lower & spidy.transform.position.y < front.thresholdValue && front.hiddenWall)
            {
                if (spidy.transform.position.z <= zThreshold)
                {
                    if (isFar)
                    {
                        StartCoroutine(InitializeUndarken(frontImage));
                        isBehind = true;
                        isFar = false;
                    }
                }
                else
                {
                    if (isBehind)
                    {
                        StartCoroutine(InitializeDarken(frontImage));
                        isBehind = false;
                        isFar = true;
                    }
                }
            }
            else if (!front.lower & spidy.transform.position.y >= front.thresholdValue && front.hiddenWall)
            {
                if (spidy.transform.position.z <= zThreshold)
                {
                    if (isFar)
                    {
                        StartCoroutine(InitializeUndarken(frontImage));
                        isBehind = true;
                        isFar = false;
                    }
                }
                else
                {
                    if (isBehind)
                    {
                        StartCoroutine(InitializeDarken(frontImage));
                        isBehind = false;
                        isFar = true;
                    }
                }
            }
        }
        else
        {
            if (spidy.transform.position.z <= zThreshold)
            {
                if (isFar)
                {
                    StartCoroutine(InitializeUndarken(frontImage));
                    isBehind = true;
                    isFar = false;
                }
            }
            else
            {
                if (isBehind)
                {
                    StartCoroutine(InitializeDarken(frontImage));
                    isBehind = false;
                    isFar = true;
                }
            }
        }
    }
    private IEnumerator InitializeDarken(SpriteRenderer frontLayer)
    {
        variation = 0.5f;
        while (variation <= 1)
        {
            if (count)
            {
                StartCoroutine(TiltLight(frontLayer, variation));
                variation += lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator InitializeUndarken(SpriteRenderer frontLayer)
    {
        variation = 1.0f;
        while (variation >= 0.5f)
        {
            if (count)
            {
                StartCoroutine(TiltLight(frontLayer, variation));
                variation -= lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator TiltLight(SpriteRenderer screen, float alpha)
    {
        count = false;
        screen.color = new Color(screen.color.r,
                                     screen.color.g,
                                     screen.color.b,
                                     alpha);
        yield return new WaitForSecondsRealtime(0.01f);
        count = true;
    }
}
