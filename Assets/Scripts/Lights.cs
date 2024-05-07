using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lights : MonoBehaviour
{
  //public SpriteRenderer sprite;
  public Light2D lightVar;

  //---------------------------------------------------------------------------------------------
  //Public variables that are only defined over the UI
  public float firstDelayer;          //Delay before starting tilting lights
  public float upDelayer;             //Waiting time before reducing light bright
  public float lowDelayer;            //Waiting time before increasing light bright
  public float timeDelimiter;         //Defines how low lights will go
  public float maxLight;              //Varible that sets the max light of a sprite
  public float lightJump;             //Jump on every iteration of change of light
  public float timeDelay;             //Waiting time before making a jump in light
  //Public variables that are only defined over the UI
  //---------------------------------------------------------------------------------------------

  bool waitingChangeLights;           //Once code has triggered coroutine to allow go up or down, this variable will not let code run same coroutine again
  bool allowGoUp;                     //Boolean that once upDelayer is over, it flags light bright con go down
  bool allowGoDown;                   //Boolean that once upDelayer is over, it flags light bright con go up
  float variation;                    //Defines percentage of starting point of lights
  bool count;                         //Boolean that advices if program is under delay
  bool lower;                         //Boolean that advices if lights are counting up or down
  bool starter;                       //Boolean that defines when the lights will start tilting

  //-------------------------------------------------------------------------------
  //Defines non public variables, public variables are defined under UI (Inspector)
  void Start()
  {
        if (lightVar.intensity == 0)
        {
            lower = false;
            variation = 0.0f;

        }
        else
        {
            lower = true;
            variation = 1.0f;

        }
        count = true;
        starter = false;
        StartCoroutine(Delay());

  }
  //Defines non public variables, public variables are defined under UI (Inspector)
  //-------------------------------------------------------------------------------

  void Update()
  {
    if (starter)        //Allows access to tilt lights when firstDelayer reaches 0
    {
      //-------------------------------------------------
      //Counts when lights are turning down
      if (count && variation > timeDelimiter && lower && !StaticVar.turnLights)
      {
        variation -= lightJump;
        StartCoroutine(TiltLight(variation));

      }
      else if (count && !StaticVar.turnLights)
      {

        if (allowGoDown)
        {
                    lower = false;
                    allowGoDown = false;
                    waitingChangeLights = false;

        }
        else if (!waitingChangeLights)
        {
                    StartCoroutine(WaitGoDown());
                    waitingChangeLights = true;

        }

      }
      //Counts when lights are turning down
      //-------------------------------------------------

      //-------------------------------------------------
      //Counts when lights are turning up
      if (count && variation < maxLight && !lower && !StaticVar.turnLights)
      {
        variation += lightJump;
        StartCoroutine(TiltLight(variation));

      }
      else if (count && !StaticVar.turnLights)
      {
                if (allowGoUp)
                {
                    lower = true;
                    allowGoUp = false;
                    waitingChangeLights = false;

                }
                else if (!waitingChangeLights)
                {
                    StartCoroutine(WaitGoUp());
                    waitingChangeLights = true;

                }

            }
      //Counts when lights are turning up
      //-------------------------------------------------

      if (StaticVar.turnLights && variation > 0)
            {
                variation -= lightJump;
                StartCoroutine(TiltLight(variation));

            }

    }

  }

  //------------------------------------------------------------
  //Waiting and changing of light on the sprite
  private IEnumerator TiltLight(float alpha)
  {
    count = false;
    lightVar.intensity = alpha;
    yield return new WaitForSecondsRealtime(timeDelay);
    count = true;

  }
  //Waiting and changing of light on the sprite
  //------------------------------------------------------------

  //------------------------------------------------------------
  //Waiting before start tilting delay
  private IEnumerator Delay()
  {
        if (StaticVar.CurrentLevel == 0 && StaticVar.firstTimeOnLevel)
        {
            yield return new WaitForSecondsRealtime(2.5f);

        }
        yield return new WaitForSecondsRealtime(firstDelayer);
        starter = true;

  }
  //Waiting before start tilting delay
  //------------------------------------------------------------

  //------------------------------------------------------------
  //Coroutines used to wait time before allowing lights change bright
  private IEnumerator WaitGoUp()
  {
        yield return new WaitForSecondsRealtime(lowDelayer);
        allowGoUp = true;

  }

  private IEnumerator WaitGoDown()
  {
        yield return new WaitForSecondsRealtime(upDelayer);
        allowGoDown = true;

  }
  //Coroutines used to wait time before allowing lights change bright
  //------------------------------------------------------------

}
