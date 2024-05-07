using System.Collections;
using UnityEngine;

public class MoveLayer : MonoBehaviour
{
    private GameObject gameManager;
    public Transform layer;
    GameObject spidy;
    float xPosition;                    //Stores the X position of the GamemManager posisiton based on the space this layer can move
    float xLayerPosition;               //Stores the X position of the layer associated based on the GameManager position
    private SpriteRenderer frontImage;
    private float variation;
    private bool count;
    private readonly float lightJump = 0.02f;
    private bool isBehind;
    private bool isFar;
    public bool turnImage = true;
    public float xMovement = 300f;
    public float xContinuousMovement = 2800.0f;

    //-------------------------------------------------------------------------------------------------
    //Locate GameManager and stores its data to gameManager variable
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        spidy = GameObject.FindGameObjectWithTag("Spidy");
        frontImage = GetComponent<SpriteRenderer>();
        count = true;
        isBehind = true;
        isFar = true;
    }
    //Locate GameManager and stores its data to gameManager variable
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Updates the Xposition of the layer based on the GameManager position
    void Update()
    {
        xPosition = (gameManager.transform.position.x - 960f) * (xMovement / 1920f);
        xLayerPosition = xContinuousMovement + xPosition;
        layer.position = new Vector3(xLayerPosition, layer.position.y, layer.position.z);
        if (turnImage)
        {
            if (spidy.transform.position.z <= 4300.0f)
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
    //Updates the Xposition of the layer based on the GameManager position
    //-------------------------------------------------------------------------------------------------
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
