using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAdvice : MonoBehaviour
{
    [SerializeField]
    Image backgroundMessage;
    public List<IMessage> initialMessage;
    public List<Message> message;
    SpriteRenderer showButtonArrowSprite;
    SpriteRenderer showButtonArrow2Sprite;
    SpriteRenderer rightArrowSprite;
    SpriteRenderer rightArrow2Sprite;
    SpriteRenderer rightArrow2SpriteCopy;
    SpriteRenderer rightArrowSecondFloorSprite;
    SpriteRenderer rightArrowSecondFloorSpriteCopy;
    SpriteRenderer centerArrowSprite;
    SpriteRenderer downArrowSprite;
    SpriteRenderer downArrow2Sprite;
    SpriteRenderer LeftArrowSprite;
    SpriteRenderer UpArrowSprite;
    private readonly float lightJump = 0.02f;
    float variation;
    float alphaColor = 0;
    readonly float tiltDuration = 0.25f;
    float timePassed = 0.0f;
    public bool alphaEnable = false;
    public void ShowAdvice()
    {
        switch (StaticVar.message)
        {
            case 1:
                StartCoroutine(EnableIMessage(initialMessage[0]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 1;
            break;
            case 2:
                StartCoroutine(DisableIMessage(initialMessage[0]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
            break;
            case 3:
                StartCoroutine(EnableMessage(message[0]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 2;
            break;
            case 4:
                StartCoroutine(DisableMessage(message[0]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                StaticVar.message = 5;
                StartCoroutine(NextMessageDelay());
                break;
            case 5:
                StartCoroutine(EnableMessage(message[1]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 3;
            break;
            case 6:
                StartCoroutine(DisableMessage(message[1]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                StaticVar.message = 7;
                StartCoroutine(NextMessageDelay());
            break;
            case 7:
                StartCoroutine(EnableMessage(message[2]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 4;
            break;
            case 8:
                StartCoroutine(DisableMessage(message[2]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                StaticVar.message = 9;
                StartCoroutine(NextMessageDelay());
                break;
            case 9:
                StartCoroutine(EnableMessage(message[3]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 5;
                break;
            case 10:
                StartCoroutine(DisableMessage(message[3]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                break;
            case 11:
                StartCoroutine(EnableMessage(message[4]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 6;
                break;
            case 12:
                StartCoroutine(DisableMessage(message[4]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                StaticVar.message = 13;
                StartCoroutine(NextMessageDelay());
                break;
            case 13:
                StartCoroutine(EnableMessage(message[5]));
                StaticVar.message = 0;
                StaticVar.messageSelector = 7;
                break;
            case 14:
                StartCoroutine(DisableMessage(message[5]));
                StaticVar.showAdvice = false;
                StaticVar.messageSelector = 0;
                break;
            default:
            break;
        }
    }
    public void SetArrowsSprites(GameObject showButtonArrowSpriteParam, GameObject showButtonArrow2SpriteParam, GameObject rightArrowSpriteParam,
                               GameObject rightArrow2SpriteParam, GameObject rightArrowSecondFloorSpriteParam, GameObject centerArrowSpriteParam,
                               GameObject downArrowSpriteParam, GameObject downArrow2SpriteParam, GameObject UpArrowSpriteParam, GameObject LeftArrowSpriteParam,
                               GameObject rightArrowSecondFloorSpriteCopyParam, GameObject rightArrow2SpriteCopyParam)
    {
        showButtonArrowSprite = showButtonArrowSpriteParam.GetComponent<SpriteRenderer>();
        showButtonArrow2Sprite = showButtonArrow2SpriteParam.GetComponent<SpriteRenderer>();
        rightArrowSprite = rightArrowSpriteParam.GetComponent<SpriteRenderer>();
        rightArrow2Sprite = rightArrow2SpriteParam.GetComponent<SpriteRenderer>();
        rightArrow2SpriteCopy = rightArrow2SpriteCopyParam.GetComponent<SpriteRenderer>();
        rightArrowSecondFloorSprite = rightArrowSecondFloorSpriteParam.GetComponent<SpriteRenderer>();
        rightArrowSecondFloorSpriteCopy = rightArrowSecondFloorSpriteCopyParam.GetComponent<SpriteRenderer>();
        centerArrowSprite = centerArrowSpriteParam.GetComponent<SpriteRenderer>();
        downArrowSprite = downArrowSpriteParam.GetComponent<SpriteRenderer>();
        downArrow2Sprite = downArrow2SpriteParam.GetComponent<SpriteRenderer>();
        UpArrowSprite = UpArrowSpriteParam.GetComponent<SpriteRenderer>();
        LeftArrowSprite = LeftArrowSpriteParam.GetComponent<SpriteRenderer>();

        showButtonArrowSprite.enabled = false;
        showButtonArrow2Sprite.enabled = false;
        rightArrowSprite.enabled = false;
        rightArrow2Sprite.enabled = false;
        rightArrow2SpriteCopy.enabled = false;
        rightArrowSecondFloorSprite.enabled = false;
        rightArrowSecondFloorSpriteCopy.enabled = false;
        centerArrowSprite.enabled = false;
        downArrowSprite.enabled = false;
        downArrow2Sprite.enabled = false;
        UpArrowSprite.enabled = false;
        LeftArrowSprite.enabled = false;
    }
    public void ShowImage(int value)
    {
        switch (value)
        {
            case 0: //Case to show up arrow in first floor
            {
                HideImage();
                centerArrowSprite.enabled = true;
                TiltImage(centerArrowSprite);
                break;
            }
            case 1: //Case to show arrow in first floor to move to the left
            {
                HideImage();
                rightArrowSprite.enabled = true;
                TiltImage(rightArrowSprite);
                break;
            }
            case 2: //Case to show arrow in second floor to push the button 
            {
                HideImage();
                showButtonArrowSprite.enabled = true;
                TiltImage(showButtonArrowSprite);
                break;
            }
            case 3: //Case to show down arrow in second floor to go down to the first floor 
            {
                HideImage();
                downArrowSprite.enabled = true;
                TiltImage(downArrowSprite);
                break;
            }
            case 4: //Case to show down arrow in second floor to go down to the first floor 
            {
                HideImage();
                LeftArrowSprite.enabled = true;
                TiltImage(LeftArrowSprite);
                break;
            }
            case 5: //Case to show up arrow in first floor after push the first button in second floor 
            {
                HideImage();
                UpArrowSprite.enabled = true;
                TiltImage(UpArrowSprite);
                break;
            }
            case 6: //Case to show arrow in second floor to move to right
            {
                HideImage();
                rightArrowSecondFloorSprite.enabled = true;
                rightArrowSecondFloorSpriteCopy.enabled = true;
                TiltImage(rightArrowSecondFloorSprite, rightArrowSecondFloorSpriteCopy);
                break;
            }
            case 7:  //Case to show down arrow in second floor to go down to the first floor 
            {
                HideImage();
                downArrow2Sprite.enabled = true;
                TiltImage(downArrow2Sprite);
                break;
            }
            case 8: //Show which button press in the first floor
            {
                HideImage();
                rightArrow2Sprite.enabled = true;
                rightArrow2SpriteCopy.enabled = true;
                TiltImage(rightArrow2Sprite, rightArrow2SpriteCopy);
                break;
            }
            case 9: //Show which button press in the first floor
            {
                HideImage();
                showButtonArrow2Sprite.enabled = true;
                TiltImage(showButtonArrow2Sprite);
                break;
            }
        }
    }
    public void HideImage()
    {
        showButtonArrowSprite.enabled = false;
        showButtonArrow2Sprite.enabled = false;
        rightArrowSprite.enabled = false;
        rightArrow2Sprite.enabled = false;
        rightArrow2SpriteCopy.enabled = false;
        rightArrowSecondFloorSprite.enabled = false;
        rightArrowSecondFloorSpriteCopy.enabled = false;
        centerArrowSprite.enabled = false;
        downArrowSprite.enabled = false;
        downArrow2Sprite.enabled = false;
        UpArrowSprite.enabled = false;
        LeftArrowSprite.enabled = false;
    }
    private void TiltImage(SpriteRenderer arrowSprite, SpriteRenderer arrowSpriteCopy = null)
    {
        Color originalColor = arrowSprite.color;
        if (arrowSprite.color.a == 1.0f && !alphaEnable)
        {
            alphaEnable = true;
            timePassed = 0.0f;
            alphaColor = 1.0f;
        }
        else if (arrowSprite.color.a == 1.0f && alphaEnable)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= tiltDuration)
            {
                alphaColor = 0.0f;
                alphaEnable = false;
                timePassed = 0.0f;
            }
        }
        else
        {
            timePassed += Time.deltaTime;
            if (timePassed >= tiltDuration)
            {
                alphaColor = 1.0f;
            }
        }
        arrowSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaColor);
        if (arrowSpriteCopy != null)
        {
            arrowSpriteCopy.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaColor);
        }
    }
    private void SetMessage(Message screen, float variation)
    {
        backgroundMessage.color = new Color(backgroundMessage.color.r,
                        backgroundMessage.color.g,
                        backgroundMessage.color.b,
                        variation);
        screen.boxMessage.color = new Color(screen.boxMessage.color.r,
                        screen.boxMessage.color.g,
                        screen.boxMessage.color.b,
                        variation);
        screen.buttonMessage.color = new Color(screen.buttonMessage.color.r,
                        screen.buttonMessage.color.g,
                        screen.buttonMessage.color.b,
                        variation);
        screen.text1Message.color = new Color(screen.text1Message.color.r,
                        screen.text1Message.color.g,
                        screen.text1Message.color.b,
                        variation);
        screen.text2Message.color = new Color(screen.text2Message.color.r,
                        screen.text2Message.color.g,
                        screen.text2Message.color.b,
                        variation);
    }
    private void SetIMessage(IMessage screen, float variation)
    {
        screen.boxMessage.color = new Color(screen.boxMessage.color.r,
                        screen.boxMessage.color.g,
                        screen.boxMessage.color.b,
                        variation);
        screen.titleMessage.color = new Color(screen.titleMessage.color.r,
                        screen.titleMessage.color.g,
                        screen.titleMessage.color.b,
                        variation);
        screen.text1Message.color = new Color(screen.text1Message.color.r,
                        screen.text1Message.color.g,
                        screen.text1Message.color.b,
                        variation);
        screen.text2Message.color = new Color(screen.text2Message.color.r,
                        screen.text2Message.color.g,
                        screen.text2Message.color.b,
                        variation);
    }
    private IEnumerator EnableIMessage(IMessage screen)
    {
        variation = 0.0f;
        while (variation <= 1)
        {
            variation += lightJump;
            SetIMessage(screen, variation);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    private IEnumerator EnableMessage(Message screen)
    {
        variation = 0.0f;
        while (variation <= 1)
        {
            variation += lightJump;
            SetMessage(screen, variation);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    private IEnumerator DisableIMessage(IMessage screen)
    {
        variation = 1;
        while (variation >= 0)
        {
            variation -= lightJump;
            SetIMessage(screen, variation);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    private IEnumerator DisableMessage(Message screen)
    {
        variation = 1;
        while (variation >= 0)
        {
            variation -= lightJump;
            SetMessage(screen, variation);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    private IEnumerator NextMessageDelay()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        while (Time.timeScale == 0.0f)
        {
            yield return new WaitForSecondsRealtime(3.0f);
        }
        Time.timeScale = 0.0f;
        StaticVar.showAdvice = true;
    }
}
[Serializable]
public class IMessage
{
    public Image boxMessage;
    public Text titleMessage;
    public Text text1Message;
    public Text text2Message;
}

[Serializable]
public class Message
{
    public Image boxMessage;
    public Image buttonMessage;
    public Text text1Message;
    public Text text2Message;
}
