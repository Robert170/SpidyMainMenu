using System;
using System.Collections;
using UnityEngine;

public class Spidy : Actor
{
  //-------------------------------------------------------------------------------------------------
  //Setup of variables related to combo attacks
  public AttackData heavyAttack1;         //Attack data of the first combo
  public AttackData heavyAttack2;         //Attack data of the second combo
  public AttackData heavyAttack3;         //Attack data of the third combo
  public AttackData firstTransAttack1;    //Attack data of the first attack for the normal first transformation
  public AttackData firstTransAttack2;    //Attack data of the second attack for the normal first transformation
  public AttackData firstTransAttack3;    //Attack data of the third attack for the normal first transformation
  public float chainHeavyComboLimit;      //Waiting time to allow enter next hit as heavy combo, if not, the hit does not count as combo anymore
  const int maxCombo = 3;                 //Variable defines the max amount of different attacks in a combo
                                          //Setup of variables related to first Normal combo attack
                                          //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Setup of variables related to jump attack and kick
  public bool canJumpAttack = true;       //Tracks the use of the jump attack once per jump
  public AttackData jumpAttack;           //Contains the hit data of the jump attack
  public AttackData jumpKick;             //Contains the hit data of the jump kick
  public float jumpKickForce;             //Dictates how far Spidy lunges forward when performing jump kick
                                          //Setup of variables related to jump attack and kick
                                          //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Setup of variables related to run attack
  public AttackData kickAttack;           //Store Spidy's run attack value
  public bool kickAttackRef;              //Boolean that defines player is using a kick attack
                                          //Setup of variables related to run attack
                                          //-------------------------------------------------------------------------------------------------

  public InputHandler input;              //Attach an InputHandler Script to Character
  public UIDialogue UIDialogue;
  protected float h;                      //horizontal move
  protected float v;                      //vertical move
  protected bool z;                       //run botton
  protected bool jump;                    //jump botton
  protected bool attack;                  //attack botton
  protected bool heavyAttack;             //heavy attack botton
  protected bool kickAttackButton;        //kick attack botton
  protected bool softAttack = false;      //Boolean that defines if you are using a soft or heavy attack
  protected bool defense = false;
  protected bool trans = false;
  Vector3 currentDir;                     //Determines the direction character will be moving
  Vector3 moveVector;                     //Determines at what speed character will move through scenario
  bool isFacingLeft;                      //Sets to True when character faces left, false when faces right

  public bool isJumpLandAnim;             //Store the status of a jump, if after the jump, character has landed, this variable will be set to true
  public bool isJumpingAnim;              //Store the status of a jump, if a character is in the air and is running an animation related to jump state, it will be set to true
  public float jumpForce = 42000000;      //Force needed by Spidy to jump
  private float lastJumpTime;             //Stores the time the last jump character jumped
  private bool canJumpAgain = true;
  public int tireForce;
  private Vector3 originPoint;
  private RaycastHit shadowPosition;

  float lastAttackTime;                   //Stores the time when the last attack was triggered
  readonly float attackLimit = 0.14f;     //Time limit when a new normal attack trigger is allowed
  readonly float heavyAttackLimit = 0.3f; //Time limit when a new heavy attack trigger is allowed

  //-------------------------------------------------------------------------------------------------
  //Setup to start Spidy with the First Transformation
  public bool firstTransformation;        //Boolean that triggers the first transformation
  public bool returnTransformation;       //Boolean that triggers the return from transformation
  
  bool fullPower = false;                 //Signals Spidy can perform transform
  bool noMorePower = false;               //Signals Spidy has exhausted all his combo power
                                          //  float triggerTransformTime = 0.2f;      //Sets the time Spidy has to perform transformation
  public bool waitToRedPow = false;       //Waiting time used to reduce combo power before Spidy losses the transformation
                                          //Setup to start Spidy with the First Transformation
                                          //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Variables that control the run effects of the character
  public GameObject startRunning;
  private bool runningEffect;
  //Variables that control the run effects of the character
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Variables that control the tolerance to attacks effect of the character
  public float hurtTolerance;
  public float hurtLimit = 20;
  public float recoveryRate = 5;
  //Variables that control the tolerance to attacks effect of the character
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Set of variables related to the UI and connection to Spidy
  public LifeBar lifeBar;                 //Set the connection between Spidy and its Life Bar
  private int timerRefCombo;              //It is a reference value used to define if timer combo will increase value
                                          //Set of variables related to the UI and connection to Spidy
                                          //-------------------------------------------------------------------------------------------------

  public Walker walker;
  public bool isAutoPiloting;
  public bool controllable = true;
  readonly float probeDistance = 235f;
  private float jumpHeight = 100000f;
  [SerializeField] LayerMask probeMask = -1;
  public bool reborn;
  public bool restartGame = false;
  public bool moveVerticalPressed = false;
  public bool deathAnimFinish = false;
  int buttonSelected = 0;

  //-------------------------------------------------------------------------------------------------
  //Initial setup of Spidy with its variables along to the UI
  protected override void Start()
  {
    input = FindObjectOfType<InputHandler>();
    lifeBar = FindObjectOfType<LifeBar>();
    UIDialogue = FindObjectOfType<UIDialogue>();
    base.Start();
    tireForce = 1;
    currentLife = StaticVar.currentLife;
    lifeBar.SetCurrentLife(currentLife / StaticVar.maxLife);
    lifeBar.SetLevel(StaticVar.levelExperience / StaticVar.maxExperienceLevel);
    StaticVar.readyToLeave = false;
    ComboCounter(StaticVar.countCombo, StaticVar.countComboMax);
    StartCoroutine(InitialDelay());
    reborn = false;
    canJumpAgain = true;
    if (StaticVar.transformed)
    {
      baseAnim.SetTrigger("FirstTransformation");
      lifeBar.ActivateFullPowerImage();
      waitToRedPow = true;
    }
  }
  //Initial setup of Spidy with its variables along to the UI
  //-------------------------------------------------------------------------------------------------

  public override void Update()
  {
    if (StaticVar.gameCompleted)
    {
      attack = input.GetAttackButtonDown();               //Store in attack if player wants character to attack
      if (attack)
      {
        GameObject.Find("MyGameManager").GetComponent<GameManager>().ExitGame();
        Application.OpenURL("https://greenbound.net/game/");
      }
    }
    base.Update();                                        //Performs the update code coming from actor script

    //-------------------------------------------------------------------------------------------------
    //Check if character is alive, if not, it will finish the update code
    //Also checks if the scene is starting so Spidy cannot move until everything has been loaded
    if (!isAlive)
    {
      if (deathAnimFinish && !StaticVar.showGameOverMenu)
      {
        StaticVar.showGameOverMenu = true;
        GameObject.Find("MyGameManager").GetComponent<GameManager>().GameOverScreen();
      }
      else if (deathAnimFinish && StaticVar.showGameOverMenu)
      {
        v = input.GetVerticalAxis();
        attack = input.GetAttackButtonDown();
        if (moveVerticalPressed)
        {
          if (v != 0) v = 0;
          else moveVerticalPressed = false;
        }
        if (attack)
          GameObject.Find("MyGameManager").GetComponent<GameManager>().SelectButtonsGameOver();
        else if (v != 0)
        {
          moveVerticalPressed = true;
          if (v > 0)
          {
            if (buttonSelected == 0) buttonSelected = 2;
            else buttonSelected--;
          }
          else
          {
            if (buttonSelected == 2) buttonSelected = 0;
            else buttonSelected++;
          }
          GameObject.Find("MyGameManager").GetComponent<GameManager>().GameOverToggleButtons(buttonSelected);
        }
      }
      return;
    }
    else if (StaticVar.showAdvice)
    {
      switch (StaticVar.messageSelector)
      {
        case 1:
          trans = input.GetTransButton();
          if (trans)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 2;
          }
          break;
        case 2:
          attack = input.GetAttackButtonDown();               //Store in attack if player wants character to attack
          if (attack)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 4;
          }
          break;
        case 3:
          heavyAttack = input.GetHeavyAttackButtonDown();
          if (heavyAttack)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 6;
          }
          break;
        case 4:
          kickAttackButton = input.GetKickAttackButtonDown();
          if (kickAttackButton)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 8;
          }
          break;
        case 5:
          jump = input.GetJumpButtonDown();
          if (jump)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 10;
          }
          break;
        case 6:
          z = input.GetRunButton();
          if (z)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 12;
          }
          break;
        case 7:
          attack = input.GetAttackButtonDown();               //Store in attack if player wants character to attack
          if (attack)
          {
            Time.timeScale = 1.0f;
            StaticVar.message = 14;
          }
          break;
      }
      return;
    }
    else if (!StaticVar.spidyAllowed) return;
    if (StaticVar.isGamePaused)
    {
      v = input.GetVerticalAxis();
      attack = input.GetAttackButtonDown();
      if (moveVerticalPressed)
      {
        if (v != 0)
        {
          v = 0;
        }
        else
        {
          moveVerticalPressed = false;
        }
      }
      if (v != 0)
      {
        moveVerticalPressed = true;
        GameObject.Find("MyGameManager").GetComponent<GameManager>().ToggleButtons();
      }
      else if (attack)
      {
        GameObject.Find("MyGameManager").GetComponent<GameManager>().SelectButtons();
      }
      if (input.GetPauseButtonDown())
      {
        StaticVar.isGamePaused = false;
        GameObject.Find("MyGameManager").GetComponent<GameManager>().ResumeGame();
      }
      return;
    }
    else if (input.GetPauseButtonDown() && !StaticVar.isGamePaused)
    {
      GameObject.Find("MyGameManager").GetComponent<GameManager>().PauseGame();
    }
    //Also checks if the scene is starting so Spidy cannot move until everything has been loaded
    //Check if character is alive, if not, it will finish the update code
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Update the variables that store whether character is jumping or not, combo attacking or not, being hurt or not, etc...
    isAttackingAnim =
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack2") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack3") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("run_attack") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack2") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack3") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_kick") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("kick") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack2") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack3");
    isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
    isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") ||
      baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");
    isHurtAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_hurt");
    //Update the variables that store whether character is jumping or not, attacking or not, being hurt or not, etc...
    //-------------------------------------------------------------------------------------------------

    originPoint = transform.position;
    originPoint.y += 175.0f;
    shadowZPosition = transform.position.z - 16.0f;
    if (isGrounded)
    {
      shadowYPosition = transform.position.y - 25.0f;
    }
    if (!isGrounded)
    {
      if (Physics.Raycast(originPoint, Vector3.down, out shadowPosition, Mathf.Infinity, probeMask))
      {
        shadowYPosition = shadowPosition.point.y - 25.0f;
      }
    }
    if (isAutoPiloting)
    {
      return;
    }
    if (Time.timeScale == 1.0f)
    {
      h = input.GetHorizontalAxis();                      //Stores in h if player wants to move character horizontally
      v = input.GetVerticalAxis();                        //Stores in v if player wants to move character vertically
      z = input.GetRunButton();                           //Store in z if player wants to make character to run
      jump = input.GetJumpButtonDown();                   //Store in jump if player wants character to jump
      attack = input.GetAttackButtonDown();               //Store in attack if player wants character to attack
      heavyAttack = input.GetHeavyAttackButtonDown();     //Store in heavyAttack if player wants character to attack heavily
      kickAttackButton = input.GetKickAttackButtonDown(); //Store in kickAttackButton if player wants character to kick
      trans = input.GetTransButton();
    }
    else
    {
      return;
    }

    //-------------------------------------------------------------------------------------------------
    //Store and normalize to 1 the request from player to move character through the game

    //-------------------------------------------------------------------------------------------------
    //Validate that Spidy is under transformation to stop vertical and horizontal movement from Spidy
    if (firstTransformation || returnTransformation)
    {
      v = 0;
      h = 0;
    }
    //Validate that Spidy is under transformation to stop vertical and horizontal movement from Spidy
    //-------------------------------------------------------------------------------------------------

    currentDir = new Vector3(h, 0, v);
    if (Vector3.Angle(Vector3.up, groundNormal) != 0 && Vector3.Angle(Vector3.up, groundNormal) != 90)
    {
      var slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
      currentDir = slopeRotation * currentDir;
    }
    if (!Physics.Raycast(originPoint, Vector3.down, probeDistance, probeMask) || jumpingRef)
    {
      jumpingRef = true;
    }
    currentDir.Normalize();

    //Store and normalize to 1 the request from player to move character through the game
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //When character is not attacking, program will review if character is walking, running or stopping
    //and after the program decides what player has requested, it will only update speed variable
    //and the animator with the corresponding animations to show over the UI
    if (!isAttackingAnim || baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_kick") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack"))
    {
      if (v == 0 && h == 0)               //If player is not pressing the moving bottons, character will stop
      {
        Stop();
        runningEffect = true;
        if (StaticVar.staminaValue < StaticVar.maxStamina)
        {
          StaticVar.staminaValue += Time.deltaTime / (2f - (StaticVar.characterLevel * 0.0175f));
          lifeBar.SetStamina(StaticVar.staminaValue / StaticVar.maxStamina);
        }
      }
      else if (v != 0 || h != 0)          //If player is pressing the moving bottons, character will move
      {
        if (z && !StaticVar.transformed)          //Added transformed variable so Spidy will not run when under transformation
        {
          if (StaticVar.staminaValue > 0)
          {
            Run();
          }
          else
          {
            Walk();
          }
        }

        else
        {
          Walk();                     //Character will walk
          runningEffect = true;
          if (StaticVar.staminaValue < StaticVar.maxStamina)
          {
            StaticVar.staminaValue = StaticVar.staminaValue + (Time.deltaTime / (2f - (StaticVar.characterLevel * 0.0175f)));
            lifeBar.SetStamina(StaticVar.staminaValue / StaticVar.maxStamina);
          }
        }
      }
    }
    else
    {
      if (StaticVar.staminaValue < StaticVar.maxStamina)
      {
        StaticVar.staminaValue = StaticVar.staminaValue + (Time.deltaTime / (2f - (StaticVar.characterLevel * 0.0175f)));
        lifeBar.SetStamina(StaticVar.staminaValue / StaticVar.maxStamina);
      }
    }

    //When character is not attacking, program will review if character is walking, running or stopping
    //and after the program decides what player has requested, it will only update speed variable
    //and the animator with the corresponding animations to show over the UI
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Jump sentence that will only happen if player presses the jump input and character is not in the
    //midst of landing and character isGrounded, and of course jumpDuration hasn´t expired
    if (canJumpAgain && jump && !StaticVar.transformed && !isKnockedOut && !isJumpLandAnim && !isAttackingAnim &&
            (isGrounded || (isJumpingAnim && Time.time < (lastJumpTime + StaticVar.jumpDuration))))
    {
      Jump();                   //Character will jump
    }
    else if (isJumpingAnim && !isGrounded)
    {
      canJumpAgain = false;
    }

    //Jump sentence that will only happen if player presses the jump input and character is not in the
    //midst of landing and character isGrounded, and of course jumpDuration hasn´t expired
    //-------------------------------------------------------------------------------------------------

    if (!defense)
    {
      //-------------------------------------------------------------------------------------------------
      //Validates if Spidy is close to an exit and leave the room, if not, follows a normal attack
      if ((attack && (StaticVar.signal1 || StaticVar.signal2)) || StaticVar.readyToLeave)
      {
        if (StaticVar.CurrentLevel == 5 && !StaticVar.Scene5Card && StaticVar.signal2)
        {
          UIDialogue.DidDialogueStart();
        }
        else
        {
          StaticVar.readyToLeave = true;
          StaticVar.currentLife = currentLife;
        }
      }
      else if (attack && (StaticVar.wildCard || StaticVar.wildCard2))
      {
        switch (StaticVar.CurrentLevel)
        {
          case 3:
            {
              GameObject signaling;
              if (StaticVar.wildCard)
              {
                signaling = GameObject.Find("PLT_09_K1");
                signaling.GetComponent<Renderer>().enabled = true;
                signaling = GameObject.Find("PLT_09_K1_1");
                signaling.GetComponent<Collider>().enabled = true;
                signaling = GameObject.Find("PLT_10_K2");
                signaling.GetComponent<Renderer>().enabled = true;
                signaling = GameObject.Find("PLT_10_K2_1");
                signaling.GetComponent<Collider>().enabled = true;
                signaling = GameObject.Find("WildCard");
                signaling.SetActive(false);
                StaticVar.barrierArea0Level3_1 = true;
                StaticVar.wildCard = false;
              }
              else if (StaticVar.wildCard2)
              {
                signaling = GameObject.Find("WildCard2");
                signaling.SetActive(false);
                signaling = GameObject.Find("SC04_prop_APLANADORA_0001");
                signaling.GetComponent<Animator>().enabled = false;
                signaling = GameObject.Find("SC04_prop_APLANADORA_0002");
                signaling.GetComponent<Animator>().enabled = false;
                signaling = GameObject.Find("Trash fall left");
                signaling.GetComponent<Animator>().enabled = false;
                bool getAnimName = signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0001" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0002" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0003" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0004";
                if (getAnimName)
                {
                  signaling.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SC04_prop_trash_0005");
                }
                signaling = GameObject.Find("Trash fall right");
                signaling.GetComponent<Animator>().enabled = false;
                getAnimName = signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0001" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0002" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0003" ||
                signaling.GetComponent<SpriteRenderer>().sprite.name == "SC04_prop_trash_0004";
                if (getAnimName)
                {
                  signaling.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SC04_prop_trash_0005");
                }
                signaling = GameObject.Find("Shadow steamroller left");
                signaling.GetComponent<Animator>().enabled = false;
                signaling = GameObject.Find("Shadow steamroller right");
                signaling.GetComponent<Animator>().enabled = false;
                signaling = GameObject.Find("Reel left");
                signaling.GetComponent<Animator>().enabled = false;
                signaling = GameObject.Find("Reel right");
                signaling.GetComponent<Animator>().enabled = false;
                StaticVar.barrierArea0Level3_2 = true;
                StaticVar.wildCard2 = false;
              }
              break;
            }
        }
      }
      else if (attack && StaticVar.signal3)
      {
        UIDialogue.DidDialogueStart();
      }
      else
      {
        //-------------------------------------------------------------------------------------------------
        //Code that triggers the First Transformation of Spidy
        if (fullPower && trans && !firstTransformation)
        {
          fullPower = false;
          firstTransformation = true;
          baseAnim.SetTrigger("FirstTransformation");
          //I will set sound for Spidy transformation
          //I will set sound for Spidy transformation
          //I will set sound for Spidy transformation
          //I will set sound for Spidy transformation
          //I will set sound for Spidy transformation
          return;
        }
        //Code that triggers the First Transformation of Spidy
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //Triggers the animation attack from animator, adds a timer to when is allowed to punch again
        if (attack && Time.time >= lastAttackTime + attackLimit && !isKnockedOut)
        {
          lastAttackTime = Time.time;
          //-------------------------------------------------------------------------------------------------
          //Check if previous attack was a heavy attack, if it was will reset the combo chain and timing
          if (!softAttack || kickAttackRef)
          {
            chainComboTimer = 0;
            currentAttackChain = 0;
            evaluatedAttackChain = 0;
            softAttack = true;
            kickAttackRef = false;
          }
          //Check if previous attack was a heavy attack, if it was will reset the combo chain and timing
          //-------------------------------------------------------------------------------------------------

          CounterAttack();
        }
        //Triggers the animation attack from animator, adds a timer to when is allowed to punch again
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //Triggers the animation heavy attack from animator, adds a timer to when is allowed to punch again
        else if (heavyAttack && Time.time >= lastAttackTime + heavyAttackLimit && !isKnockedOut && !StaticVar.transformed)
        {
          lastAttackTime = Time.time;

          //-------------------------------------------------------------------------------------------------
          //Check if previous attack was a normal attack, if it was will reset the combo chain and timing
          if (softAttack || kickAttackRef)
          {
            chainComboTimer = 0;
            currentAttackChain = 0;
            evaluatedAttackChain = 0;
            softAttack = false;
            kickAttackRef = false;
          }
          //Check if previous attack was a normal attack, if it was will reset the combo chain and timing
          //-------------------------------------------------------------------------------------------------

          CounterAttack();
        }
        //Triggers the animation heavy attack from animator, adds a timer to when is allowed to punch again
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //Triggers the kick attack from Spidy, this one does not posses a combo attack
        else if (kickAttackButton && Time.time >= lastAttackTime + attackLimit && !isKnockedOut)
        {
          lastAttackTime = Time.time;
          kickAttackRef = true;
          currentAttackChain = 1;
          evaluatedAttackChain = 0;
          CounterAttack();
        }
        //Triggers the kick attack from Spidy, this one does not posses a combo attack
        //-------------------------------------------------------------------------------------------------
      }
      //Validates if Spidy is close to an exit and leave the room, if not, follows a normal attack
      //-------------------------------------------------------------------------------------------------
    }

    //-------------------------------------------------------------------------------------------------
    //Code used to return Spidy back to normal. When power is back to 0, Spidy returns to normal
    if (StaticVar.transformed)
    {
      if (noMorePower)
      {
        noMorePower = false;
        returnTransformation = true;
        baseAnim.SetTrigger("ReturnTransformation");
        ComboCounter(StaticVar.countCombo, StaticVar.countComboMax);
      }
      else
      {
        if (waitToRedPow)
        {
          waitToRedPow = false;
          StartCoroutine(WaitTimeToRedPow());
          StaticVar.countCombo -= 1;
          ComboCounter(StaticVar.countCombo, StaticVar.countComboMax);
          if (StaticVar.countCombo == 0)
          {
            noMorePower = true;
            lifeBar.DeactivateFullPowerImage();
          }
        }
      }
    }
    //Code used to return Spidy back to normal. When power is back to 0, Spidy returns to normal
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Time tolerance before Spidy is knockdown for several hits
    if (hurtTolerance < hurtLimit && !StaticVar.transformed)
    {
      hurtTolerance += Time.deltaTime * recoveryRate;
      hurtTolerance = Mathf.Clamp(hurtTolerance, 0, hurtLimit);
    }
    //Time tolerance before Spidy is knockdown for several hits
    //-------------------------------------------------------------------------------------------------

    //Set the experience level of Spidy with respect to its current level, once reached max value, update all character power
    if (StaticVar.levelExperience < StaticVar.maxExperienceLevel)
    {
      lifeBar.SetLevel(StaticVar.levelExperience / StaticVar.maxExperienceLevel);
    }
    else
    {
      //-------------------------------------------------------------------------------------------------
      //Update all Spidy variables related to the UI once it has increased level
      if (StaticVar.characterLevel < 20)  // CURRENTLY THE MAX LEVEL HAS 20 IT IS THE DEMO, MAX IS 100
      {
        StaticVar.maxLife += 10;
        StaticVar.maxExperienceLevel += 10;
        StaticVar.levelExperience = 0;
        lifeBar.SetLevel(StaticVar.levelExperience / StaticVar.maxExperienceLevel);
        currentLife = StaticVar.maxLife;
        lifeBar.SetCurrentLife(currentLife / StaticVar.maxLife);
        StaticVar.characterLevel += 1;
        StaticVar.maxStamina = 2f + (StaticVar.characterLevel * 0.18f);
        StaticVar.staminaValue = StaticVar.maxStamina;
        lifeBar.SetLevelText(StaticVar.characterLevel);
        timerRefCombo = StaticVar.characterLevel;
        while (timerRefCombo > 0)
        {
          timerRefCombo -= 10;
          if (timerRefCombo >= 0)
          {
            StaticVar.comboTimer += 0.1f;
          }
        }
      }
      //Update all Spidy variables related to the UI once it has increased level
      //-------------------------------------------------------------------------------------------------
      else
      {
        StaticVar.levelExperience = StaticVar.maxExperienceLevel;
        lifeBar.SetLevel(StaticVar.levelExperience / StaticVar.maxExperienceLevel);
      }
    }
    //Set the experience level of Spidy with respect to its current level, once reached max value, update all character power

    if (isJumpingAnim && (!jump || body.velocity.y < 0))
    {
      body.velocity += Physics.gravity.y * 0.5f * Time.deltaTime * Vector3.up;
    }
  }

  //-------------------------------------------------------------------------------------------------
  //Code that makes character to stop and updates animator based on the stop requirements
  public void Stop()
  {
    if (!isAutoPiloting)
    {
      speed = 0;                                  //Updates speed character to stop
      isRunning = false;                          //Updates variable to change animator from running
      baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
      baseAnim.SetBool("IsRunning", isRunning);   //Sets the boolean that player is running from animator
    }
  }
  //Code that makes character to stop and updates animator based on the stop requirements
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Code that makes character to move and updates animator based on the movement requirements
  public void Walk()
  {
    if (!isAutoPiloting)
    {
      speed = walkSpeed;                          //Updates speed character to make him walk
      isRunning = false;                          //Updates variable to change animator from running
      baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
      baseAnim.SetBool("IsRunning", isRunning);   //Sets the boolean that player is running from animator
    }
  }
  //Code that makes character to move and updates animator based on the movement requirements
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Jump sequence
  void Jump()
  {
    //-------------------------------------------------------------------------------------------------
    //When jump method has access, it needs to verify if it is not jumping already, if not, anymator
    //enters into the jump animation
    if (!isJumpingAnim)
    {
      baseAnim.SetTrigger("Jump");
      lastJumpTime = Time.time;
      jumpingRef = true;
    }
    //When jump method has access, it needs to verify if it is not jumping already, if not, anymator
    //enters into the jump animation
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Applies the force and movement to the rigidbody of the character to jump
    //This will occur as long as the jump button is pressed and the max time to allow to press button is fulfilled
    Vector3 verticalVector = jumpForce * jumpHeight * Time.deltaTime * Vector3.up;
    verticalVector *= tireForce;
    body.AddForce(verticalVector, ForceMode.Force);
    //Applies the force and movement to the rigidbody of the character to jump
    //This will occur as long as the jump button is pressed and the max time to allow to press button is fulfilled
    //-------------------------------------------------------------------------------------------------

  }
  //Jump sequence
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Attack sequence, triggers the two variables needed from the animator to start an attack
  public override void CounterAttack()
  {
    //-------------------------------------------------------------------------------------------------
    //Checks that the chain is inside combo and is not 3 or higher, this way does not allow an extra combo or an attack until chain timming is zero
    if (currentAttackChain <= maxCombo)
    {
      //-------------------------------------------------------------------------------------------------
      //Check if character is on the air to be able to attack
      if (!isGrounded)
      {
        //-------------------------------------------------------------------------------------------------
        //Fully validates the jump attack has not occurred before and it is not throwing a normal kick
        if (isJumpingAnim && canJumpAttack && !kickAttackButton)
        {
          canJumpAttack = false;                                      //Stops character from being able to attack after this attack in the air until he falls to floor

          //-------------------------------------------------------------------------------------------------
          //Sets conditions for the jump attack and kick attack animation to occur
          currentAttackChain = 1;
          evaluatedAttackChain = 0;
          baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
          baseAnim.SetInteger("CurrentChain", currentAttackChain);
          baseAnim.SetBool("SoftAttack", softAttack);
          if (softAttack) { } //PlaySFX(hitClip10);
          else { }//PlaySFX(hitClip11);
                  //Sets conditions for the jump attack and kick attack animation to occur
                  //-------------------------------------------------------------------------------------------------
        }
        //Fully validates the jump attack has not ocurred before and it is not throwing a normal kick
        //-------------------------------------------------------------------------------------------------
      }
      //Check if character is on the air to be able to attack
      //-------------------------------------------------------------------------------------------------

      //-------------------------------------------------------------------------------------------------
      //If character is not on the air, performs a normal or a run attack
      else
      {
        //-------------------------------------------------------------------------------------------------
        //Applies the run attack setup animation
        if (isRunning)
        {
          if (softAttack && !kickAttackRef)
          {
            body.AddForce((Vector3.up + (frontVector * 5)) * runAttackForce, ForceMode.Impulse);
            currentAttackChain = 1;
            evaluatedAttackChain = 0;
            baseAnim.SetInteger("CurrentChain", currentAttackChain);
            baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
            baseAnim.SetBool("SoftAttack", softAttack);
          }
        }
        //Applies the run attack setup animation
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //Applies the normal attack setup animation
        else
        {
          if (kickAttackRef)
          {
            baseAnim.SetTrigger("KickAttack");
          }
          else
          {
            //-------------------------------------------------------------------------------------------------
            //If there has been a reset on the chainComboTimer, this code will aply and AttackChain will reset
            if (currentAttackChain == 0 || chainComboTimer == 0)
            {
              currentAttackChain = 1;
              evaluatedAttackChain = 0;
            }
            //If there has been a reset on the chainComboTimer, this code will aply and AttackChain will reset
            //-------------------------------------------------------------------------------------------------
            baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
            baseAnim.SetInteger("CurrentChain", currentAttackChain);
            baseAnim.SetBool("SoftAttack", softAttack);
          }
        }
      }
      //Applies the normal attack setup animation
      //-------------------------------------------------------------------------------------------------
      //If character is not on the air, performs a normal or a run attack
      //-------------------------------------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------------------------------
    //Checks that the chain is inside combo and is not 3 or higher, this way does not allow an extra combo or an attack until chain timming is zero
  }
  //Attack sequence, triggers the two variables needed from the animator to start an attack
  //-------------------------------------------------------------------------------------------------

  void FixedUpdate()
  {
    if (!isAutoPiloting)
    {
      //-------------------------------------------------------------------------------------------------
      //Check if character is alive, if not, it will finish the FixedUpdate code
      if (!isAlive)
      {
        return;
      }
      //Check if character is alive, if not, it will finish the FixedUpdate code
      //-------------------------------------------------------------------------------------------------

      moveVector = currentDir * speed;    //Multiply normalized values with the required speed

      //-----------------------------------------------------------------------------
      //Updates Rigidbody from character to make him move
      //Updates animator according to speed parameters and isRunning boolean
      //Added isJumpingAnim to allow character to move when it is on the air
      //Added isJumpLandAnim to not allow character to move when is landing
      if ((isGrounded || isJumpingAnim) && (!isAttackingAnim || baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_kick") ||
      baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack")) && !isJumpLandAnim && !isKnockedOut && !isHurtAnim)
      {
        baseAnim.SetFloat("Speed", moveVector.magnitude);                           //Updates animator speed variable
        body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);   //Sets the speed of the RigidBody to make character move over the area
        if (runningEffect && speed > 600.01f)
        {
          StartCoroutine(DidRun());
        }
        else if (speed > 600.01f)
        {
          StaticVar.staminaValue -= Time.deltaTime;
          lifeBar.SetStamina(StaticVar.staminaValue / StaticVar.maxStamina);
        }
      }
      //Added isJumpLandAnim to not allow character to move when is landing
      //Updates Rigidbody from character to make him move
      //Updates animator according to speed parameters and isRunning boolean
      //Added isJumpingAnim was removed to allow character to move when it is on the air
      //-----------------------------------------------------------------------------

      //-----------------------------------------------------------------------------
      //Check if Spidy is facing left or right, updates Spidy's sprite calling FlipSprite method
      if (moveVector != Vector3.zero && isGrounded && !isKnockedOut && !isAttackingAnim)
      {
        //-----------------------------------------------------------------------------
        //Updates boolean isFacingLeft if character is moving
        if (moveVector.x != 0)
        {
          isFacingLeft = moveVector.x < 0;    //Updates isFacingLeft according to Spidy movement
        }
        //Updates boolean isFacingLeft if character is moving
        //-----------------------------------------------------------------------------
        FlipSprite(isFacingLeft);               //Call method FlipSprite
      }
      //Check if Spidy is facing left or right, updates Spidy's sprite with FlipSprite method
      //-----------------------------------------------------------------------------
    }
  }

  //-------------------------------------------------------------------------------------------------
  //Load the actor's script code of OnCollisionEnter and then sets canJumpAttack variable back to true
  protected override void OnCollisionEnter(Collision collision)
  {
    base.OnCollisionEnter(collision);
    if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Treadmill") || collision.collider.CompareTag("Treadmill2"))
    {
      canJumpAttack = true;
      canJumpAgain = true;
    }
  }
  //Load the actor's script code of OnCollisionEnter and then sets canJumpAttack variable back to true
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Code used when Spidy is inside the boundaries of an exit point, it will light a signal that he may leave, previous rooms should work, further rooms should not a llow untill battles events are complete
  private void OnTriggerEnter(Collider item)
  {
    if (item.gameObject.layer == LayerMask.NameToLayer("Exit1") && ((!StaticVar.zimaLevel &&
        StaticVar.CurrentLevel == 6) ||
        !GameObject.Find("MyGameManager").GetComponent<GameManager>().hasRemainingEvents))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("Exit1");
      signaling.GetComponent<Renderer>().enabled = true;
      StaticVar.signal1 = signaling.GetComponent<Renderer>().enabled;
    }

    //-------------------------------------------------------------------------------------------------
    //Controls when the exit output will appear, box collider is allways on but verify there is no missing event to start
    switch (StaticVar.CurrentLevel)
    {
      case 3:
        {
          if (item.gameObject.layer == LayerMask.NameToLayer("Exit2"))
          {
            GameObject signaling;
            signaling = GameObject.FindGameObjectWithTag("Exit2");
            signaling.GetComponent<Renderer>().enabled = true;
            if (StaticVar.barrierArea0Level3_2)
            {
              StaticVar.signal2 = signaling.GetComponent<Renderer>().enabled;
              StaticVar.signal3 = false;
            }
            else
            {
              StaticVar.signal3 = signaling.GetComponent<Renderer>().enabled;
              StaticVar.signal2 = false;
            }
          }
          break;
        }
      default:
        {
          if (!GameObject.Find("MyGameManager").GetComponent<GameManager>().hasRemainingEvents && !StaticVar.zimaLevel &&
                      !GameObject.Find("MyGameManager").GetComponent<GameManager>().bossPlay)
          {
            if (item.gameObject.layer == LayerMask.NameToLayer("Exit2"))
            {
              GameObject signaling;
              signaling = GameObject.FindGameObjectWithTag("Exit2");
              signaling.GetComponent<Renderer>().enabled = true;
              StaticVar.signal2 = signaling.GetComponent<Renderer>().enabled;
            }
          }
          break;
        }
    }

    //Controls when the exit output will appear, box collider is allways on but verify there is no missing event to start
    //-------------------------------------------------------------------------------------------------

    if (item.gameObject.layer == LayerMask.NameToLayer("WildCard"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("WildCard");
      signaling.GetComponent<Renderer>().enabled = true;
      StaticVar.wildCard = signaling.GetComponent<Renderer>().enabled;
    }
    if (item.gameObject.layer == LayerMask.NameToLayer("WildCard2"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("WildCard2");
      signaling.GetComponent<Renderer>().enabled = true;
      StaticVar.wildCard2 = signaling.GetComponent<Renderer>().enabled;
    }
    if (item.gameObject.layer == LayerMask.NameToLayer("CardScene0_5"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("CardScene0_5");
      signaling.GetComponent<Renderer>().enabled = false;
      StaticVar.Scene5Card = true;
    }


  }
  //Code used when Spidy is inside the boundaries of an exit point, it will light a signal that he may leave, previous rooms should work, further rooms should not a llow untill battles events are complete
  //-------------------------------------------------------------------------------------------------

  private void OnTriggerExit(Collider otherItem)
  {

    //-------------------------------------------------------------------------------------------------
    //Turn off exit output when you are not close to it
    if (otherItem.gameObject.layer == LayerMask.NameToLayer("Exit1"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("Exit1");
      signaling.GetComponent<Renderer>().enabled = false;
      StaticVar.signal1 = signaling.GetComponent<Renderer>().enabled;
    }
    else if (otherItem.gameObject.layer == LayerMask.NameToLayer("Exit2"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("Exit2");
      signaling.GetComponent<Renderer>().enabled = false;
      StaticVar.signal2 = signaling.GetComponent<Renderer>().enabled;
      StaticVar.signal3 = false;
    }
    //Turn off exit output when you are not close to it
    //-------------------------------------------------------------------------------------------------

    if (otherItem.gameObject.layer == LayerMask.NameToLayer("WildCard"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("WildCard");
      signaling.GetComponent<Renderer>().enabled = false;
      StaticVar.wildCard = signaling.GetComponent<Renderer>().enabled;
    }
    if (otherItem.gameObject.layer == LayerMask.NameToLayer("WildCard2"))
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("WildCard2");
      signaling.GetComponent<Renderer>().enabled = false;
      StaticVar.wildCard2 = signaling.GetComponent<Renderer>().enabled;
    }
  }

  //-------------------------------------------------------------------------------------------------
  //Handles what happens after jump attack is performed, rigidbody is affected back by gravity
  public void DidJumpAttack()
  {
  }
  //Handles what happens after jump attack is performed, rigidbody is affected back by gravity
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method that helps to process the combo of the normal attack
  private void AnalyzeNormalAttack(AttackData attackData, int attackChain, Actor actor, Vector3 hitPoint, Vector3 hitVector)
  {
    actor.EvaluateAttackData(attackData, hitVector, hitPoint, gameObject.name);      //Same as AnalizeSpecialAttack
    currentAttackChain = attackChain;                                                //Updates variable according to the combo being played

    //-------------------------------------------------------------------------------------------------
    //Depending on the attack, validates what time limit will apply, if for a normal attack or a heavy attack
    if (softAttack)
    {
      chainComboTimer = chainComboLimit;                          //Set the time for the player to attack before losses opportunity to play normal combo
    }
    else
    {
      chainComboTimer = chainHeavyComboLimit;                     //Set the time for the player to attack before losses opportunity to play heavy combo
    }
    //Depending on the attack, validates what time limit will apply, if for a normal attack or a heavy attack
    //-------------------------------------------------------------------------------------------------
  }
  //Method that helps to process the combo of the normal attack
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method that helps to process special attacks like jump attacks, run attacks, etc...
  private void AnalyzeSpecialAttack(AttackData attackData, Actor actor, Vector3 hitPoint, Vector3 hitVector)
  {
    actor.EvaluateAttackData(attackData, hitVector, hitPoint, gameObject.name);
    chainComboTimer = chainHeavyComboLimit;                         //Set the timer for other attacks to be played
  }
  //Method that helps to process special attacks like jump attacks, run attacks, etc...
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //This code process all the different attacks Spidy has, depending of the animation loaded
  protected override void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
  {
    if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack1"))
    {
      StaticVar.countCombo += 1;
      AnalyzeNormalAttack(normalAttack, 2, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack2"))
    {
      StaticVar.countCombo += 1;
      AnalyzeNormalAttack(normalAttack2, 3, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack3"))
    {
      StaticVar.countCombo += 2;
      AnalyzeNormalAttack(normalAttack3, 1, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack1"))
    {
      StaticVar.countCombo += 1;
      AnalyzeNormalAttack(heavyAttack1, 2, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack2"))
    {
      StaticVar.countCombo += 1;
      AnalyzeNormalAttack(heavyAttack2, 3, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("heavy_attack3"))
    {
      StaticVar.countCombo += 2;
      AnalyzeNormalAttack(heavyAttack3, 1, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack1"))
    {
      AnalyzeNormalAttack(firstTransAttack1, 2, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack2"))
    {
      AnalyzeNormalAttack(firstTransAttack2, 3, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("first_transformation_attack3"))
    {
      AnalyzeNormalAttack(firstTransAttack3, 1, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack"))
    {
      StaticVar.countCombo += 1;
      AnalyzeSpecialAttack(jumpAttack, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_kick"))
    {
      StaticVar.countCombo += 2;
      AnalyzeSpecialAttack(jumpKick, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("run_attack"))
    {
      StaticVar.countCombo += 2;
      AnalyzeSpecialAttack(runAttack, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("kick"))
    {
      StaticVar.countCombo += 1;
      AnalyzeSpecialAttack(kickAttack, actor, hitPoint, hitVector);
    }
    if (StaticVar.countCombo > StaticVar.countComboMax)
    {
      StaticVar.countCombo = StaticVar.countComboMax;
    }
    ComboCounter(StaticVar.countCombo, StaticVar.countComboMax);        //Sets the combocounter bar from UI based on the combo count
  }
  //This code process all the different attacks Spidy has, depending of the animation loaded
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Take damage method is used when character receives an attack
  public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
  {
    //-------------------------------------------------------------------------------------------------
    //If Spidy is under transformation, he will not receive any damage during this time
    if (!firstTransformation && !returnTransformation && !StaticVar.transformed)
    {

      //-------------------------------------------------------------------------------------------------
      //Check if Spidy is transformed to apply this code of being hurt
      //if (StaticVar.transformed)
      //{
      //  FlipSprite(hitVector.x > 0);
      //  currentLife -= value;
      //  lifeBar.EnableLifeBar(true);
      //  lifeBar.SetCurrentLife(currentLife / StaticVar.maxLife);
      //  if (isAlive && currentLife <= 0)
      //  {
      //    Die();

      //  }
      //  baseAnim.SetTrigger("IsHurtTransformed");
      //}
      //Check if Spidy is transformed to apply this code of being hurt
      //-------------------------------------------------------------------------------------------------

      //else
      //{
        hurtTolerance -= value;
        //-------------------------------------------------------------------------------------------------
        //Validates if spidy is on the air and will start the knockdown routine when attacked
        if (hurtTolerance <= 0 || !isGrounded)
        {
          hurtTolerance = hurtLimit;
          knockdown = true;

        }
        //Validates if spidy is on the air and will start the knockdown routine when attacked
        //-------------------------------------------------------------------------------------------------
        base.TakeDamage(value, hitVector, knockdown);
        lifeBar.EnableLifeBar(true);
        lifeBar.SetCurrentLife(currentLife / StaticVar.maxLife);
        if (currentLife <= 0)
        {
          lifeBar.SetThumbnail();
        }
      //}
    }
    //If Spidy is under transformation, he will not receive any damage during this time
    //-------------------------------------------------------------------------------------------------
  }
  //Take damage method is used when character receives an attack
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //This code prevents for spidy to move while being knocked down or hurt
  public override bool CanWalk()
  {
    return isGrounded && !isAttackingAnim && !isJumpLandAnim && !isKnockedOut && !isHurtAnim;
  }
  //This code prevents for spidy to move while being knocked down or hurt
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Allows for the character to be affected by the gravity when rigidbody is set to false
  protected override IEnumerator KnockdownRoutine()
  {
    body.useGravity = true;
    return base.KnockdownRoutine();
  }
  //Allows for the character to be affected by the gravity when rigidbody is set to false
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sentence to make character walk back again
  protected override void DidLand()
  {
    base.DidLand();                             //Performs the DidLand code coming from actor script
    Walk();
  }
  //Sentence to make character walk back again
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //This method is used to set the combo counter from the UI
  private void ComboCounter(float setCombo, float maxComboValue)
  {
    if (setCombo < maxComboValue)
    {
      lifeBar.SetCombo(setCombo / maxComboValue);
    }
    else
    {
      if (StaticVar.firstTime)
      {
        Time.timeScale = 0.0f;
        StaticVar.showAdvice = true;
        StaticVar.message = 1;
        StaticVar.firstTime = false;
      }
      fullPower = true;
      setCombo = maxComboValue;
      lifeBar.SetCombo(setCombo / maxComboValue);
      lifeBar.ActivateFullPowerImage();
    }
  }
  //This method is used to set the combo counter from the UI
  //-------------------------------------------------------------------------------------------------

  public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
  {
    if (shouldRun)
    {

      Run();
    }
    else
    {
      Walk();
    }
    walker.MoveTo(position, callback, shouldRun);
  }
  public void UseAutopilot(bool useAutopilot)
  {
    isAutoPiloting = useAutopilot;
  }
  //-------------------------------------------------------------------------------------------------
  //Coroutine to show effect of start running from character
  private IEnumerator DidRun()
  {
    runningEffect = false;
    yield return null;
    GameObject runObj = Instantiate(startRunning);
    Vector3 runPosition = transform.position;
    runObj.transform.localScale = transform.localScale;
    runPosition.x += 150 * transform.localScale.x;
    runObj.transform.position = runPosition;
  }
  //Coroutine to show effect of start running from character
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Coroutine used to make delay on the time that each combo counter will be reduced to one
  private IEnumerator WaitTimeToRedPow()
  {
    yield return new WaitForSecondsRealtime(StaticVar.comboTimer);
    waitToRedPow = true;

  }
  //Coroutine used to make delay on the time that each combo counter will be reduced to one
  //-------------------------------------------------------------------------------------------------

  private IEnumerator InitialDelay()
  {
    yield return null;
    shadowYPosition = transform.position.y - 25.0f;
    shadowZPosition = transform.position.z - 16.0f;
  }
  public IEnumerator StairsDelay()
  {
    yield return null;
    yield return null;
    currentDir.y = 1;
    if (body.velocity.y < 0) currentDir.y = -1;
  }
}
