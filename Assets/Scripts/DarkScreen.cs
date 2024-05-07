using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DarkScreen : MonoBehaviour
{
    /**
       * @Variable darkScreen, reference to an image
     */
    [SerializeField]
    private Image darkScreen;
    [SerializeField]
    private GameObject loadingSpidy;
    /**
       * @Variable gameOver, reference to an image
     */
    [SerializeField]
    private Image gameOver;
    [SerializeField]
    private Image endDemoGame;
    [SerializeField]
    private Image blueImage;
    /**
       * @Variable lightJump, public variable to jump on every iteration of change of light 
     */
    private readonly float lightJump = 0.02f;
    /**
       * @Variable variation, to set the alpha value in the image
     */
    float variation;
    float gameOverVariation;
    float endGamevariation;
    /**
       * @Variable count
     */
    bool count;
    void Start()
    {
        count = true;
    }
    /**
     * @brief      darkenScreen function, darken the screen 
     * @bug		     No know Bugs
     * @return     Returns nothing
     */
    public void DarkenScreen()
    {
        loadingSpidy.SetActive(true);
        StartCoroutine(InitializeDarken());
    }
    /**
     * @brief      undarkenScreen function, undarken the screen 
     * @bug		     No know Bugs
     * @return     Returns nothing
     */
    public void UndarkenScreen()
    {
        loadingSpidy.SetActive(false);
        StartCoroutine(InitializeUndarken());
    }
    /**
     * @brief      undarkenScreen TiltLight, to set new alpha in the image 
     * @param      alpha parameter, value to set in alpha
     * @bug		     No know Bugs
     * @return     Returns nothing
     */
    public void DeadDarkenScreen()
    {
        StartCoroutine(InitializeDeadDarkenScreen());
    }
    public void DeadUndarkenScreen()
    {
        StartCoroutine(InitializeDeadUndarkenScreen());
    }
    public void FinalScreen()
    {
        StartCoroutine(InitializeFinalScreen());
    }
    private IEnumerator InitializeFinalScreen()
    {
        endGamevariation = 0;
        while (endGamevariation <= 1)
        {
            if (count)
            {
                endGamevariation += lightJump;
                StartCoroutine(TiltLight(endDemoGame, variation));
            }
            yield return null;
        }
    }
    private IEnumerator InitializeUndarken()
    {
        variation = 1;
        while (variation >= 0)
        {
            if (count)
            {
                StartCoroutine(TiltLight(darkScreen, variation));
                variation -= lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator InitializeDeadUndarkenScreen()
    {
        gameOverVariation = 1;
        while (gameOverVariation >= 0)
        {
            if (count)
            {
                StartCoroutine(TiltLight(gameOver, gameOverVariation));
                StartCoroutine(TiltLight(blueImage, gameOverVariation));
                gameOverVariation -= lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator InitializeDarken()
    {
        variation = 0;
        while (variation <= 1)
        {
            if (count)
            {
                StartCoroutine(TiltLight(darkScreen, variation));
                variation += lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator InitializeDeadDarkenScreen()
    {
        gameOverVariation = 0;
        while (gameOverVariation <= 1)
        {
            if (count)
            {
                StartCoroutine(TiltLight(gameOver, gameOverVariation));
                StartCoroutine(TiltLight(blueImage, gameOverVariation));
                gameOverVariation += lightJump;
            }
            yield return null;
        }
    }
    private IEnumerator TiltLight(Image screen, float alpha)
    {
        count = false;
        //Set the param alpha
        screen.color = new Color(screen.color.r,
                                     screen.color.g,
                                     screen.color.b,
                                     alpha);
        yield return new WaitForSecondsRealtime(0.01f);
        count = true;
    }
}
