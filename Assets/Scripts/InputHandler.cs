using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
  public InputAction moveHorizontalInput;
  public InputAction moveVerticalInput;
  public InputAction runInput;
  public InputAction jumpInput;
  public InputAction normalAttackInput;
  public InputAction heavyAttackInput;
  public InputAction kickInput;
  public InputAction pausebutton;
  public InputAction transButton;
  bool normalAttackPressed;
  bool heavyAttackPressed;
  bool kickPressed;
  bool pauseButtonPressed;
  float horizontal;                                       //Stores the horizontal move if button is pressed
  float vertical;                                         //Stores the vertical move if button is pressed
  bool run;                                               //Stores the state if player is running
  bool jump;                                              //Stores the state if player has requested to jump
  float lastJumpTime;                                     //Stores the time the jump button has been pressed
  bool isJumping;                                         //Stores the boolean that states character is jumping
  bool attack;                                            //STores the boolean that player has requested to attack
  bool heavyAttack;                                       //Stores the boolean that player has requested for heavy attack
  bool KickAttack;                                        //Stores the boolean that player has requested for kick attack
  bool pause;
  bool trans;
  public bool attackChainer;                              //Variable used by Spidy so Spidy can change into berserker mode
  public bool heavyAttackChainer;                         //Variable used by Spidy so Spidy can change into berserker mode
  bool jumping;

  //-------------------------------------------------------------------------------------------------
  //Methods to call from Character Script to return the state of each button during game
  public float GetVerticalAxis()
  {
    return vertical;
  }
  public float GetHorizontalAxis()
  {
    return horizontal;
  }
  public bool GetRunButton()
  {
    return run;
  }
  public bool GetJumpButtonDown()
  {
    return jump;
  }
  public bool GetAttackButtonDown()
  {
    return attack;
  }
  public bool GetHeavyAttackButtonDown()
  {
    return heavyAttack;
  }
  public bool GetKickAttackButtonDown()
  {
    return KickAttack;
  }
  public bool GetPauseButtonDown()
  {
    return pause;
  }
  public bool GetTransButton()
  {
    return trans;
  }
  //Methods to call from Character Script to return the state of each button during game
  //-------------------------------------------------------------------------------------------------
  public void Start()
  {
    moveHorizontalInput.Enable();
    moveVerticalInput.Enable();
    runInput.Enable();
    jumpInput.Enable();
    normalAttackInput.Enable();
    heavyAttackInput.Enable();
    kickInput.Enable();
    pausebutton.Enable();
    transButton.Enable();
  }
    void Update()
  {
    //-------------------------------------------------------------------------------------------------
    //Check each frame the status of the buttons to change the state of the corresponding variables
    horizontal = moveHorizontalInput.ReadValue<float>();
    vertical = moveVerticalInput.ReadValue<float>();
    run = runInput.ReadValue<float>() > 0;
    attack = normalAttackInput.ReadValue<float>() > 0;
    heavyAttack = heavyAttackInput.ReadValue<float>() > 0;
    KickAttack = kickInput.ReadValue<float>() > 0;
    pause = pausebutton.ReadValue<float>() > 0;
    trans = transButton.ReadValue<float>() > 0;
        jumping = jumpInput.ReadValue<float>() > 0;
    //Check each frame the status of the buttons to change the state of the corresponding variables
    //-------------------------------------------------------------------------------------------------
    if (normalAttackPressed)
    {
        if (attack)
        {
            attack = false;
        }
        else
        {
            normalAttackPressed = false;
        }
    }
    else if (attack) normalAttackPressed = true;

    if (heavyAttackPressed)
    {
        if (heavyAttack)
        {
            heavyAttack = false;
        }
        else
        {
            heavyAttackPressed = false;
        }
    }
    else if (heavyAttack) heavyAttackPressed = true;

    if (kickPressed)
    {
        if (KickAttack)
        {
            KickAttack = false;
        }
        else
        {
            kickPressed = false;
        }
    }
    else if (KickAttack) kickPressed = true;
    if (pauseButtonPressed)
    {
        if (pause)
        {
            pause = false;
        }
        else
        {
            pauseButtonPressed = false;
        }
    }
    else if (pause) pauseButtonPressed = true;

    //-------------------------------------------------------------------------------------------------
    //Sets the time jump button is pressed, more force will be applied to character if more
    //time is pressed the button, the max time currently is 0.2 seconds
    //-------------------------------------------------------------------------------------------------
    //It will access at the first moment jump button is pressed
    if (!jump && !isJumping && jumping)
    {
      jump = true;
      lastJumpTime = Time.time;
      isJumping = true;
    }
    //It will access at the first moment jump button is pressed
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //It will just be accessed when jump button is no longer pressed
    else if (!jumping)
    {
      jump = false;
      if (Time.time > lastJumpTime + StaticVar.jumpDuration) isJumping = false;
    }
    //It will just be accessed when jump button is no longer pressed
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //It will access as long as jump button is pressed and the button is pressed more than 0.2 seconds
    if (jump && Time.time > lastJumpTime + StaticVar.jumpDuration)
    {
      jump = false;
    }
    //It will access as long as jump button is pressed and the button is pressed more than 0.2 seconds
    //-------------------------------------------------------------------------------------------------
    //Sets the time jump button is pressed, more force will be applied to character if more
    //time is pressed the button, the max time currently is 0.2 seconds
    //-------------------------------------------------------------------------------------------------
  }
}
