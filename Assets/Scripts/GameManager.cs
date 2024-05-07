using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEditor;
public class GameManager : MonoBehaviour
{
  //-------------------------------------------------------------------------------------------------
  //Initial setup
  public EnemyManager enemiesInstanced;
  public Spidy actor;                                         //Reference to main character
  public Erin nerin;
  public MusicBetweenScenes music;
  public bool cameraFollows = true;                           //Allows to activate camera follows main character
  public CameraBounds cameraBounds;                           //Reference to CameraBounds Script
  public CameraShake cameraShake;
  public UIDialogue UIDialogue;
  public UIAdvice UIMessage;
  public DarkScreen darkScreen;
  public GameObject pauseMenu;
  public GameObject resumeButton;
  public GameObject quitButton;
  public GameObject restartGameButton;
  public GameObject restartLevelButton;
  public GameObject gameOverQuitButton;
  private Vector3 mainCharacterPosition;
  private float valueYOffset;
  private bool waitingYMeasure;
  private float newCameraYValue;
  private bool isJumping;
  private float minZSpaceValue = 1080;
  private float maxZSpaceValue = 1480;
  private bool hiddenWall = true;
  private int zFix = 0;
  public bool bossPlay = false;
  private bool spidyBoolSupport;
  private bool nerinBoolSupport;
  private float xLook;
  private float minVisibleX = 0;                              //Position in X around game space background layer starts to appear
  private float maxVisibleX = 0;                              //Position in X around game space background layer finishes to appear
  public Transform walkOutTarget;
  //Initial setup
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Level data configuration
  private LevelData currentLevelData;                         //Stores the current level being played
  private Level currentBattleEvent;                           //Stores the current battle event
  private int nextEventIndex;                                 //Counter of the total amount of battle events in a level
  public bool hasRemainingEvents;                             //Determines if there are more battle events in a level or not
  public GameObject currentLevelBackground;
  public float typingTime = 0.05f;              //Variable for text writing speed
  public InputHandler input;
  //Level data configuration
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Different levels with all enemies in game added
  public LevelData levelData;                                  //Stores all the levels of a scene
  public GameObject[] Enemies;                                //Stores all the enemies of the game
  public GameObject[] Bosses;
  public EnemyHealthBar healthBarPrefab;
  private int enemyCounter;                                   //Stores the number of enemies that have appeared in a battle event
  private int allowedEnemies;                                 //Stores the total number of enemies to fight to finish a battle event
  public bool allowedForBattleEvent;                         //Flag that allows to add new enemies on a battle event
  private int allowedPresentEnemies;                          //Stores the total of enemies that can appear while playing game
  private bool timeToWaitEnemy;                               //Waiting Signal time that allows when a new enemy can be added to the game
  private bool waitingTimeForEnemy;                           //Sets that there is a timing to add a new enemy
  private int enemyID;                                        //Sets random value to select what kind of enemy to show on game
  private float waitDelayTime = 6;                            //Sets the time to wait before adding a new enemy
                                                              //Different levels with all enemies in game added
                                                              //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Change of scenario data setup
  private bool timeToLeave;                                   //Used to signal that screen has been darkened and is ready to move to a different scenario
  private bool stopTime;                                      //Used to set variables to pause game and initiate process to dark screen
                                                              //Change of scenario data setup
                                                              //-------------------------------------------------------------------------------------------------

  private Vector3 neededPosition;
  public bool destroyMusic;
  public bool gameOverScreen;
  public bool gamePaused;
  private bool[] buttonSelector = new bool[2];
  private bool[] gameOverButtonSelector = new bool[3];
  [SerializeField] ChainsEvent chains;
  private float waitTime = 120.0f; //NEW
  private int countTimeReduced = 0;
  float showDuration = 5.0f;
  float timePassed = 0.0f;

  void Start()
  {
    actor = Instantiate(actor);
    nextEventIndex = 0;                                             //Signals the start of battle events in an area
    gameOverScreen = true;
    StartCoroutine(LoadLevelData(levelData));                       //Load a new level to play
    StaticVar.TotalEnemies = 0;                                     //Restart the amount of enemies in a new level
    StaticVar.TotalKills = 0;
    allowedForBattleEvent = false;                                  //Set to avoid enemies from a battle event
    if (StaticVar.CurrentLevel <= 1 || StaticVar.CurrentLevel == 4 || StaticVar.CurrentLevel == 5)
    {
      allowedPresentEnemies = 3;                                  //Set to avoid more enemies than the player can handle
      allowedEnemies = 6;                                         //This number can change based on the level being played
    }
    else if (StaticVar.CurrentLevel == 6)
    {
      allowedPresentEnemies = 0;                                  //Set to avoid more enemies than the player can handle
      allowedEnemies = 0;                                         //This number can change based on the level being played
    }
    else
    {
      allowedPresentEnemies = 4;                                  //Set to avoid more enemies than the player can handle
      allowedEnemies = 8;                                         //This number can change based on the level being played
      waitDelayTime = 3;
    }
    timeToWaitEnemy = false;                                        //Set to not allow enemies during the specified time
    timeToLeave = false;                                            //Set so the game can start normally
    stopTime = true;                                                //Set so the time is not stopped starting game
    switch (StaticVar.CurrentLevel)
    {
      case 3:
        GameObject signaling;
        valueYOffset = mainCharacterPosition.y - 729.0084f;
        newCameraYValue = cameraBounds.cameraRoot.position.y + valueYOffset;
        signaling = GameObject.FindGameObjectWithTag("WildCard");
        signaling.GetComponent<Renderer>().enabled = false;
        signaling = GameObject.FindGameObjectWithTag("WildCard2");
        signaling.GetComponent<Renderer>().enabled = false;
        StaticVar.wildCard = signaling.GetComponent<Renderer>().enabled;
        UIMessage.SetArrowsSprites(GameObject.Find("ButtonArrow"),
                                            GameObject.Find("ShowButtonArrow2"),
                                            GameObject.Find("RightArrow"),
                                            GameObject.Find("RightArrow2"),
                                            GameObject.Find("RightArrowSecondFloor"),
                                            GameObject.Find("CenterArrow"),
                                            GameObject.Find("DownArrow"),
                                            GameObject.Find("DownArrow2"),
                                            GameObject.Find("UpArrow"),
                                            GameObject.Find("LeftArrow"),
                                            GameObject.Find("RightArrowSecondFloor2"),
                                            GameObject.Find("RightArrow3"));
        if (StaticVar.barrierArea0Level3_1)
        {
          signaling = GameObject.Find("WildCard");
          signaling.SetActive(false);
        }
        else
        {
          signaling = GameObject.Find("PLT_09_K1");
          signaling.GetComponent<Renderer>().enabled = false;
          signaling = GameObject.Find("PLT_09_K1_1");
          signaling.GetComponent<Collider>().enabled = false;
          signaling = GameObject.Find("PLT_10_K2");
          signaling.GetComponent<Renderer>().enabled = false;
          signaling = GameObject.Find("PLT_10_K2_1");
          signaling.GetComponent<Collider>().enabled = false;
        }
        if (StaticVar.barrierArea0Level3_2)
        {
          signaling = GameObject.Find("WildCard2");
          signaling.SetActive(false);
          signaling = GameObject.Find("SC04_prop_APLANADORA_0001");
          signaling.GetComponent<Animator>().enabled = false;
          signaling = GameObject.Find("SC04_prop_APLANADORA_0002");
          signaling.GetComponent<Animator>().enabled = false;
          signaling = GameObject.Find("Trash fall left");
          signaling.GetComponent<Animator>().enabled = false;
          signaling.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SC04_prop_trash_0005");
          signaling = GameObject.Find("Trash fall right");
          signaling.GetComponent<Animator>().enabled = false;
          signaling.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SC04_prop_trash_0005");
          signaling = GameObject.Find("Shadow steamroller left");
          signaling.GetComponent<Animator>().enabled = false;
          signaling = GameObject.Find("Shadow steamroller right");
          signaling.GetComponent<Animator>().enabled = false;
          signaling = GameObject.Find("Reel left");
          signaling.GetComponent<Animator>().enabled = false;
          signaling = GameObject.Find("Reel right");
          signaling.GetComponent<Animator>().enabled = false;
        }
        else
        {
          StartCoroutine(DelayRoller());
        }
        break;
      case 4:
        mainCharacterPosition.y = 767.0167f;
        newCameraYValue = 0;
        isJumping = false;
        break;
    }
    waitingYMeasure = false;
    StaticVar.turnLights = false;                                   //Set to false so lights do not go off
    StaticVar.defenseUp = false;
  }

  void Update()
  {
    if (StaticVar.gameCompleted)
    {
      return;
    }
    if (!actor.isAlive)
    {
      if (gameOverScreen)
      {
        if (actor.deathAnimFinish)
        {
          darkScreen.DeadDarkenScreen();
          destroyMusic = true;
          gameOverScreen = false;
        }
      }
      else if (actor.restartGame)
      {
        if (destroyMusic)
        {
          StaticVar.CurrentLevel = 0;
          darkScreen.DarkenScreen();
          darkScreen.DeadUndarkenScreen();
          StaticVar.spidyAllowed = false;
          StaticVar.turnLights = true;
          music = GameObject.Find("BackgroundMusic").GetComponent<MusicBetweenScenes>();
          music.Destroyer();
          StartCoroutine(WaitingBeforeLeaving());
          destroyMusic = false;
        }
        if (timeToLeave)
        {
          timeToLeave = false;
          actor.restartGame = false;
          SceneManager.LoadScene("InitialIntro");
        }
      }
      return;
    }
    if (StaticVar.showAdvice)
    {
      UIMessage.ShowAdvice();
      return;
    }
    if (StaticVar.isGamePaused)
    {
      return;
    }
    if (actor.isAlive && actor.reborn)
    {
      if (destroyMusic)
      {
        darkScreen.DarkenScreen();
        darkScreen.DeadUndarkenScreen();
        StaticVar.spidyAllowed = false;
        StaticVar.turnLights = true;
        music = GameObject.Find("BackgroundMusic").GetComponent<MusicBetweenScenes>();
        music.Destroyer();
        StartCoroutine(WaitingBeforeLeaving());
        destroyMusic = false;
        gameOverScreen = true;
        if (currentLevelData.level[StaticVar.CurrentLevel].newEnemy) StaticVar.newEnemyCounter -= 1;
      }
      //Add coroutine to dark screen and stop game while moving to a different scenario
      //-------------------------------------------------------------------------------------------------
      if (timeToLeave)
      {
        actor.reborn = false;
        timeToLeave = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      }
      return;
    }
    //-------------------------------------------------------------------------------------------------
    //Check if there are battle events to play
    if (currentBattleEvent == null && !StaticVar.signal1 && (hasRemainingEvents || (StaticVar.CurrentLevel == 6 && StaticVar.zimaLevel)))
    {
      if (currentLevelData.level[StaticVar.CurrentLevel].mapLocation.Count != 0)
      {
        if (Mathf.Abs(currentLevelData.level[StaticVar.CurrentLevel].mapLocation[nextEventIndex].yLocation - cameraBounds.activeCamera.transform.position.y) < 20f)
        {
          if (Mathf.Abs(currentLevelData.level[StaticVar.CurrentLevel].mapLocation[nextEventIndex].xLocation - cameraBounds.activeCamera.transform.position.x) < 20f)
          {
            PlayBattleEvent(currentLevelData.level[StaticVar.CurrentLevel]);
          }
        }
      }
      else
      {
        allowedEnemies = 0;
        bossPlay = true;
        PlayBattleEvent(currentLevelData.level[StaticVar.CurrentLevel]);
      }
    }
    //Check if there are battle events to play
    //-------------------------------------------------------------------------------------------------

    else if (currentBattleEvent != null)
    {
      if (StaticVar.CurrentLevel == 6)
      {
        if (StaticVar.TotalEnemies == 0 && enemyCounter >= allowedEnemies && !StaticVar.zimaLevel)
        {
          spidyBoolSupport = true;
          StartCoroutine(SmallDelay());
          CompleteCurrentEvent();
        }
      }
      else if (StaticVar.TotalKills >= allowedEnemies)
      {
        StaticVar.TotalKills = 0;
        CompleteCurrentEvent();
      }
    }
    if (StaticVar.CurrentLevel == 3)
    {
      if (!StaticVar.barrierArea0Level3_1 || !StaticVar.barrierArea0Level3_2)
      {

        if (!StaticVar.showImages)
        {
          StartCoroutine(TimeToShowImages());
        }
        else if (StaticVar.initTimer)
        {
          StartCoroutine(TimeToWait());
        }

      }
    }
    //-------------------------------------------------------------------------------------------------
    //Move camera in axis X, Y and Z to have in the center main character
    if (cameraFollows)
    {
      cameraBounds.SetXPosition(actor.transform.position.x, minVisibleX, maxVisibleX);                    //Updates X camera position based on Spidy position
    }
    cameraBounds.SetYPosition(newCameraYValue);
    cameraBounds.SetZPosition(actor.transform.position.z - 3500 + zFix, minZSpaceValue, maxZSpaceValue);   //Updates Z camera position based on Spidy position
    if (!waitingYMeasure)
    {
      minVisibleX = 0;
      maxVisibleX = 3840;
      switch (StaticVar.CurrentLevel)
      {
        case 2:
          {
            maxVisibleX = 5760;
            break;
          }
        case 3:
          {
            valueYOffset = actor.transform.position.y - mainCharacterPosition.y;
            if (Mathf.Abs(valueYOffset) > 100)
            {
              StartCoroutine(ValidatingYChange());
            }
            if (newCameraYValue >= 900 && actor.transform.position.x < 2200)
            {
              maxVisibleX = 2130;
              minZSpaceValue = 2380;
              maxZSpaceValue = 2480;
              if (hiddenWall)
              {
                StartCoroutine(HideWall());
              }
            }
            else if (newCameraYValue >= 900 && actor.transform.position.x > 2200)
            {
              minVisibleX = 2130;
              maxVisibleX = 7160;
              minZSpaceValue = 2480;
              maxZSpaceValue = 2480;
              if (hiddenWall)
              {
                StartCoroutine(HideWall());
              }
            }
            else
            {
              minZSpaceValue = 1080;
              maxZSpaceValue = 1480;
              if (!hiddenWall)
              {
                StartCoroutine(ShowWall());
              }
              if (actor.transform.position.x > 3300)
              {
                minVisibleX = 3440;
                maxVisibleX = 7160;
              }
              else
              {
                maxVisibleX = 3440;
              }
            }
            break;
          }
        case 4:
          {
            if (!actor.isJumpingAnim && !actor.isJumpLandAnim)
            {
              newCameraYValue = actor.transform.position.y - mainCharacterPosition.y;
              if (isJumping)
              {
                cameraBounds.CalculateOffsetY(newCameraYValue);
                isJumping = false;
              }
              cameraBounds.cameraYPosition = actor.transform.position.y - mainCharacterPosition.y;
            }
            else
            {
              isJumping = true;
            }
            maxVisibleX = 7160;
            maxZSpaceValue = 2480;
            break;
          }
        case 5:
          {
            maxVisibleX = 5760;
            break;
          }
        case 6:
          {
            maxVisibleX = 3840;
            maxZSpaceValue = 2760;
            zFix = 780;
            break;
          }
      }
    }
    //Move camera in axis X, Y and Z to have in the center main character
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Check if there is an event to play to not allow new enemies to appear, this sentence needs to change
    //for future levels where it needs to mix normal appearance and when battle events need to appear
    if (StaticVar.firstTimeOnLevel)
    {
      //-------------------------------------------------------------------------------------------------
      //Syntax that validates if it is possible to play a battle event and add enemies for the battle event
      if (allowedForBattleEvent && StaticVar.TotalEnemies < allowedPresentEnemies && Time.timeScale == 1.0f
          && ((bossPlay && StaticVar.zimaLevel) || (StaticVar.CurrentLevel >= 2 && StaticVar.CurrentLevel != 6) ||
          enemyCounter < allowedEnemies))
      {
        TimerSpawner();
      }
      //Syntax that validates if it is possible to play a battle event and add enemies for the battle event
      //-------------------------------------------------------------------------------------------------
    }
    //Check if there is an event to play to not allow new enemies to appear, this sentence needs to change
    //for future levels where it needs to mix normal appearance and when battle events need to appear
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Adds enemies randomly since there are no battle events to play
    else
    {
      if (StaticVar.TotalEnemies < allowedPresentEnemies && !StaticVar.readyToLeave)
      {
        TimerSpawner();
      }
    }
    //Adds enemies randomly since there are no battle events to play
    //-------------------------------------------------------------------------------------------------

    if (StaticVar.nextExit)
    {
      StaticVar.nextExit = false;
      StartCoroutine(NerinAppearance());
    }

    //-------------------------------------------------------------------------------------------------
    //Syntax to allow Spidy to leave the level for a different one
    if (StaticVar.readyToLeave)
    {
      //-------------------------------------------------------------------------------------------------
      //Add coroutine to dark screen and stop game while moving to a different scenario
      if (stopTime)
      {
        stopTime = false;
        Time.timeScale = 0;                             //Stop the game so characters of the game stop any attack or pending movement
        darkScreen.DarkenScreen();
        StaticVar.spidyAllowed = false;
        StaticVar.turnLights = true;
        StartCoroutine(WaitingBeforeLeaving());
      }
      //Add coroutine to dark screen and stop game while moving to a different scenario
      //-------------------------------------------------------------------------------------------------
      if (timeToLeave)
      {
        StaticVar.killEnemies = false;
        if (currentLevelData.level[StaticVar.CurrentLevel].newEnemy) currentLevelData.level[StaticVar.CurrentLevel].newEnemy = false;
        //-------------------------------------------------------------------------------------------------
        //If signal 1 is triggered, that means Spidy is going to a lower level
        if (StaticVar.signal1)
        {
          StaticVar.CurrentLevel--;
        }
        //If signal 1 is triggered, that means Spidy is going to a lower level
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //If signal 2 is triggered, that means Spidy is going to a upper level
        else if (StaticVar.signal2)
        {
          StaticVar.CurrentLevel++;
        }
        //If signal 2 is triggered, that means Spidy is going to a upper level
        //-------------------------------------------------------------------------------------------------
        else
        {
          //There might be some code to add in here when not both signals are triggered
        }

        //-------------------------------------------------------------------------------------------------
        //Check if there are no more levels on this scene, if not, restart game
        if (StaticVar.CurrentLevel > 6)
        {
          Time.timeScale = 0.0f;
          StaticVar.gameCompleted = true;
          darkScreen.FinalScreen();
        }
        //Check if there are no more levels on this scene, if not, restart game
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //If there are still more levels to move, animate the next level
        else
        {
          StartCoroutine(AnimateNextLevel());
        }
        //If there are still more levels to move, animate the next level
        //-------------------------------------------------------------------------------------------------
      }
    }
    //Syntax to allow Spidy to leave the level for a different one
    //-------------------------------------------------------------------------------------------------
  }

  //-------------------------------------------------------------------------------------------------
  //Code that insert a new enemy on the level being payed, either as in a battle event or no battle event
  private void TimerSpawner()
  {
    if (timeToWaitEnemy && !StaticVar.acceptTheKills)
    {
      //-------------------------------------------------------------------------------------------------
      //Create and Position the new enemy in the map
      if (StaticVar.CurrentLevel == 2)
      {
        int i = Random.Range(0, 3);
        if (cameraBounds.cameraRoot.position.x < 2500f)
        {
          if (i == 0) neededPosition = StaticVar.doors[0];
          else if (i == 1) neededPosition = StaticVar.doors[1];
          else neededPosition = StaticVar.doors[2];
        }
        else
        {
          if (i == 0) neededPosition = StaticVar.doors[0];
          else if (i == 1) neededPosition = StaticVar.doors[1];
          else neededPosition = StaticVar.doors[3];
        }
      }
      else if (StaticVar.CurrentLevel == 3)
      {
        if (actor.transform.position.y <= 1600.0f)
        {
          if (actor.transform.position.x <= 3440)
          {
            neededPosition = StaticVar.doors[0];
          }
          else
          {
            neededPosition = StaticVar.doors[3];
          }
        }
        else
        {
          if (actor.transform.position.x <= 2000)
          {
            neededPosition = StaticVar.doors[2];
          }
          else
          {
            neededPosition = StaticVar.doors[1];
          }
        }
      }
      else if (StaticVar.CurrentLevel == 5)
      {
        neededPosition = StaticVar.doors[0];
        if (cameraBounds.cameraRoot.position.x <= 2400 &&
            Vector3.Distance(actor.transform.position, StaticVar.doors[1]) <=
                Vector3.Distance(actor.transform.position, neededPosition))
        {
          neededPosition = StaticVar.doors[1];
        }
        else if (cameraBounds.cameraRoot.position.x > 2400 && cameraBounds.cameraRoot.position.x <= 4700)
        {
          neededPosition = StaticVar.doors[1];
        }
        else if (cameraBounds.cameraRoot.position.x > 4700)
        {
          neededPosition = StaticVar.doors[2];
        }
      }
      else
      {
        neededPosition = StaticVar.doors[Random.Range(0, 2)];
        for (int i = 0; i < 4; i++)
        {
          if (Mathf.Abs(Mathf.Abs(actor.transform.position.y - StaticVar.doors[i].y) - Mathf.Abs(actor.transform.position.y - neededPosition.y)) <= 10)
          {
            if (Vector3.Distance(actor.transform.position, StaticVar.doors[i]) <= Vector3.Distance(actor.transform.position, neededPosition))
            {
              neededPosition = StaticVar.doors[i];
            }
          }
          else if (Mathf.Abs(actor.transform.position.y - StaticVar.doors[i].y) <= 100)
          {
            neededPosition = StaticVar.doors[i];
          }
        }
      }
      GameObject enemyObj;
      EnemyHealthBar temp = Instantiate(healthBarPrefab);
      enemyID = Random.Range(0, StaticVar.newEnemyCounter);
      enemyObj = Instantiate(Enemies[enemyID]);
      enemyObj.transform.position = neededPosition;
      if (StaticVar.CurrentLevel == 0 && hasRemainingEvents)
        enemyObj.transform.position = new Vector3(2340.0f, 729.0084f, 4450);
      SelectEnemy(enemyID, enemyObj);
      //Create and Position the new enemy in the map
      //-------------------------------------------------------------------------------------------------
      enemyObj.GetComponent<NavMeshAgent>().enabled = false;
      enemyObj.GetComponent<Enemy>().RegisterEnemy();
      enemyObj.GetComponent<Enemy>().SetHealthBar(ref temp);
      enemiesInstanced.SetInstancedEnemies(enemyObj);
      enemyCounter += 1;                                      //Add to the counter a new enemy
      waitingTimeForEnemy = true;                             //Sets true to wait for a new enemy
      timeToWaitEnemy = false;                                //Sets to false to not allow to add a new enemy after the time has passed
    }
    else if (waitingTimeForEnemy)
    {
      waitingTimeForEnemy = false;
      StartCoroutine(TimerForNewEnemy());
    }
  }
  //Code that insert a new enemy on the level being payed, either as in a battle event or no battle event
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Start a battle event
  private void PlayBattleEvent(Level battleEventData)
  {
    //-------------------------------------------------------------------------------------------------
    //Load and start a new a battle event
    if ((nextEventIndex == 0 && (StaticVar.CurrentLevel == 0 ||
        StaticVar.CurrentLevel == 1)) || StaticVar.CurrentLevel == 6)
    {
      enemyCounter = 0;
      UIDialogue.DidDialogueStart();
      if (StaticVar.CurrentLevel == 0)
      {
        int i = 0;
        int z = 4100;
        while (i < 3)
        {
          GameObject enemyObj;
          EnemyHealthBar temp = Instantiate(healthBarPrefab);
          enemyObj = Instantiate(Enemies[0]);
          enemyObj.transform.position = new Vector3(2340.0f, 729.0084f, z);
          enemyObj.GetComponent<Soldier>().SetRange((EnemyRange)Random.Range(0, 0));
          enemyObj.GetComponent<Enemy>().RegisterEnemy();
          enemyObj.GetComponent<Enemy>().SetHealthBar(ref temp);
          enemyCounter += 1;                                      //Add to the counter a new enemy
          i++;
          z += 350;
          enemiesInstanced.SetInstancedEnemies(enemyObj);
        }
      }
      else if (StaticVar.CurrentLevel == 1)
      {
        int i = 0;
        int z = 4100;
        bool r = false;
        while (i < 3)
        {
          int x = 2950;
          if (r)
          {
            x = 2950 - 2350;
            r = false;
          }
          else r = true;
          GameObject enemyObj;
          EnemyHealthBar temp = Instantiate(healthBarPrefab);
          enemyObj = Instantiate(Enemies[1]);
          enemyObj.transform.position = new Vector3(x, 729.0084f, z);
          enemyObj.GetComponent<Gorilla>().SetRange((EnemyRange)Random.Range(0, 0));
          enemyObj.GetComponent<Enemy>().RegisterEnemy();
          enemyObj.GetComponent<Enemy>().SetHealthBar(ref temp);
          enemyCounter += 1;                                      //Add to the counter a new enemy
          i++;
          z += 350;
          enemiesInstanced.SetInstancedEnemies(enemyObj);
        }
      }
      //else if (StaticVar.CurrentLevel == 6)
      //{
      //          int i = 0;
      //          int z = 3820;
      //          while (i < 4)
      //          {
      //              GameObject enemyObj;
      //              EnemyHealthBar temp = Instantiate(healthBarPrefab);
      //              enemyID = Random.Range(0, StaticVar.newEnemyCounter);
      //              enemyObj = Instantiate(Enemies[enemyID]);
      //              enemyObj.transform.position = new Vector3(2100.0f, 1263.0f, z);
      //              SelectEnemy(enemyID, enemyObj);
      //              enemyObj.GetComponent<Enemy>().RegisterEnemy();
      //              enemyObj.GetComponent<Enemy>().SetHealthBar(ref temp);
      //              enemyCounter += 1;                                      //Add to the counter a new enemy
      //              i++;
      //              z += 400;
      //              enemiesInstanced.SetInstancedEnemies(enemyObj);
      //          }
      //}
    }
    if (StaticVar.CurrentLevel == 4)
    {
      string layerName = "WallBattleEvent";
      GameObject[] objs = (from go in FindObjectsOfType<GameObject>()
                           where go.layer == LayerMask.NameToLayer(layerName)
                           select go).ToArray();
      foreach (GameObject obj in objs)
      {
        obj.GetComponent<Collider>().enabled = true;
      }
    }
    currentBattleEvent = battleEventData;
    if (!bossPlay)
    {
      cameraFollows = false;
      cameraBounds.CalculateOffsetX(battleEventData.mapLocation[nextEventIndex].xLocation);  //Slowly move camera to reach its X destination
      ShowChains();
    }
    allowedForBattleEvent = true;
    nextEventIndex++;
    //Load and start a new a battle event
    //-------------------------------------------------------------------------------------------------
  }
  //Start a battle event
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method to complete the current battle event
  private void CompleteCurrentEvent()
  {
    currentBattleEvent = null;                                  //Sets variable to null to validate on update if there are more battlevents coming
    cameraFollows = true;                                       //Allows camera to follow over x
    hasRemainingEvents = false;
    if (!StaticVar.zimaLevel)
    {
      cameraBounds.CalculateOffsetX(actor.transform.position.x);  //Slowly move camera to reach its X destination
      hasRemainingEvents = currentLevelData.level[StaticVar.CurrentLevel].mapLocation.Count >
      nextEventIndex && StaticVar.firstTimeOnLevel;
      enemyCounter = 0;
      bossPlay = false;
    }
    if (!hasRemainingEvents)
    {
      currentLevelData.level[StaticVar.CurrentLevel].firstTimeAccess = false;               //Change variable signal to Spidy has passed over this level
      if (StaticVar.CurrentLevel == 0)
      {
        GameObject door = GameObject.Find("ExitPoint");
        door.GetComponent<Renderer>().enabled = true;
      }
      else if (StaticVar.CurrentLevel == 6)
      {
        StartCoroutine(ReadyToLeave());
      }
    }
    if (StaticVar.CurrentLevel == 4)
    {
      string layerName = "WallBattleEvent";
      GameObject[] objs = (from go in FindObjectsOfType<GameObject>()
                           where go.layer == LayerMask.NameToLayer(layerName)
                           select go).ToArray();
      foreach (GameObject obj in objs)
      {
        obj.GetComponent<Collider>().enabled = false;
      }
    }
    if (!bossPlay)
    {
      HideChains();
    }
  }
  //Method to complete the current battle event
  //-------------------------------------------------------------------------------------------------

  protected virtual void SetOpacity(float value)
  {
    Color color = GameObject.Find("WallRocks").GetComponent<SpriteRenderer>().color;
    color.a = value;
    GameObject.Find("WallRocks").GetComponent<SpriteRenderer>().color = color;
  }

  private void SelectEnemy(int enemyID, GameObject enemyObj)
  {
    //-------------------------------------------------------------------------------------------------
    //Selection of the new Enemy based on the enemyID, all enemies will be stored in here
    switch (enemyID)
    {
      case 0:
        enemyObj.GetComponent<Soldier>().SetRange((EnemyRange)Random.Range(0, 5));
        break;
      case 1:
        enemyObj.GetComponent<Gorilla>().SetRange((EnemyRange)Random.Range(0, 5));
        break;
      case 2:
        enemyObj.GetComponent<Sniper>().SetRange((EnemyRange)Random.Range(0, 5));
        break;
    }
    //Selection of the new Enemy based on the enemyID, all enemies will be stored in here
    //-------------------------------------------------------------------------------------------------
  }
  private void DidFinishWalkout()
  {
    actor.baseAnim.SetBool("IsRunning", false);
    actor.UseAutopilot(false);
    actor.controllable = false;
    StartCoroutine(DelayBeforeLeaving());
  }
  private void DidArrivePosition()
  {
    actor.baseAnim.SetBool("IsRunning", false);
    actor.transform.localScale = new Vector3(-1, 1, 1);
    if (xLook < 0)
    {
      actor.transform.localScale = new Vector3(1, 1, 1);
    }
    spidyBoolSupport = false;
  }
  private void NerinArrived()
  {
    nerin.baseAnim.SetBool("IsRunning", false);
    Vector3 refVec = actor.transform.localScale;
    nerin.transform.localScale = new Vector3(-1, 1, 1);
    if (refVec.x < 0)
    {
      nerin.transform.localScale = new Vector3(1, 1, 1);
    }
    StartCoroutine(NerinDelayAtArrival());
  }
  private void NerinDidLeave()
  {
    nerin.baseAnim.SetBool("IsRunning", false);
    UIDialogue.DidDialogueStart();
    StartCoroutine(NerinLeaving());
    StartCoroutine(SpidyWalkout());
  }
  //-------------------------------------------------------------------------------------------------
  //Coroutine used to load a new level data of the game
  public void GameOverScreen()
  {
    gameOverButtonSelector[0] = true;
    restartLevelButton.SetActive(true);
    restartGameButton.SetActive(false);
    gameOverQuitButton.SetActive(false);
    Time.timeScale = 0.0f;
  }
  public void GameOverToggleButtons(int button)
  {
    switch (button)
    {
      case 0:
        {
          gameOverButtonSelector[0] = true;
          gameOverButtonSelector[1] = false;
          gameOverButtonSelector[2] = false;
          restartLevelButton.SetActive(true);
          restartGameButton.SetActive(false);
          gameOverQuitButton.SetActive(false);
          break;
        }
      case 1:
        {
          gameOverButtonSelector[0] = false;
          gameOverButtonSelector[1] = true;
          gameOverButtonSelector[2] = false;
          restartLevelButton.SetActive(false);
          restartGameButton.SetActive(true);
          gameOverQuitButton.SetActive(false);
          break;
        }
      case 2:
        {
          gameOverButtonSelector[0] = false;
          gameOverButtonSelector[1] = false;
          gameOverButtonSelector[2] = true;
          restartLevelButton.SetActive(false);
          restartGameButton.SetActive(false);
          gameOverQuitButton.SetActive(true);
          break;
        }
    }
  }
  public void SelectButtonsGameOver()
  {
    if (gameOverButtonSelector[0])
    {
      StaticVar.currentLife = StaticVar.maxLife;
      actor.isAlive = true;
      actor.reborn = true;
      actor.deathAnimFinish = false;
      StaticVar.showGameOverMenu = false;
      if (StaticVar.CurrentLevel == 0 || StaticVar.CurrentLevel == 1) StaticVar.newEnemyCounter -= 1;
    }
    else if (gameOverButtonSelector[1])
    {
      actor.restartGame = true;
    }
    else
    {
      //EditorApplication.isPlaying = false;
      Application.Quit();
    }
  }
  public void PauseGame()
  {
    buttonSelector[0] = false;
    StaticVar.isGamePaused = true;
    pauseMenu.SetActive(true);
    resumeButton.SetActive(true);
    quitButton.SetActive(false);
    Time.timeScale = 0;
  }
  public void ResumeGame()
  {
    StaticVar.isGamePaused = false;
    pauseMenu.SetActive(false);
    Time.timeScale = 1;
  }
  public void ToggleButtons()
  {
    if (buttonSelector[0])
    {
      buttonSelector[0] = false;
      buttonSelector[1] = true;
      resumeButton.SetActive(true);
      quitButton.SetActive(false);
    }
    else
    {
      buttonSelector[0] = true;
      buttonSelector[1] = false;
      resumeButton.SetActive(false);
      quitButton.SetActive(true);
    }
  }
  public void SelectButtons()
  {
    if (!buttonSelector[0])
    {
      buttonSelector[0] = true;
      ResumeGame();
    }
    else if (!buttonSelector[1])
    {
      buttonSelector[1] = true;
      ExitGame();
    }
  }
  public void ExitGame()
  {
    music = GameObject.Find("BackgroundMusic").GetComponent<MusicBetweenScenes>();
    music.Destroyer();
    StaticVar.isGamePaused = false;
    StaticVar.CurrentLevel = 0;         //this variable can be avoided
    Time.timeScale = 1;
    StaticVar.gameCompleted = false;
    SceneManager.LoadScene("MainMenu");
  }
  private void ShowChains()
  {
    chains.chainsEdge.SetActive(true);
    StartCoroutine(StartDelay());
  }
  private void HideChains()
  {
    chains.chainsEdgeAnim.SetBool("Finish", true);
    chains.chainsUpAnim.SetBool("Finish", true);
    StartCoroutine(RemovingChains());
  }
  public Vector3 GetPosition()
  {
    return cameraBounds.cameraRoot.transform.position;
  }
  private IEnumerator LoadLevelData(LevelData data)
  {
    currentLevelData = data;                                        //Loads the new level into the variable
    actor.body.useGravity = false;                                  //Turn off Spidy's gravity so it doesn´t fall from game
                                                                    //-------------------------------------------------------------------------------------------------
                                                                    //Check if this is the first time Spidy access the level, if it is there will be battle events
    StaticVar.firstTimeOnLevel = currentLevelData.level[StaticVar.CurrentLevel].firstTimeAccess;
    //Check if this is the first time Spidy access the level, if it is there will be battle events
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Updates the battle event list and performs a yield,
    //also updates the minimum position in X to move over the level and the maximum value in X
    hasRemainingEvents = currentLevelData.level[StaticVar.CurrentLevel].mapLocation.Count > 0 &&
            StaticVar.firstTimeOnLevel;
    cameraBounds.AllignBoundaries();                                //Calls to allign camera to background and Spidy position
    if (currentLevelBackground != null) Destroy(currentLevelBackground);
    currentLevelBackground = Instantiate(currentLevelData.level[StaticVar.CurrentLevel].levelPrefab);
    yield return null;
    mainCharacterPosition = actor.transform.position;
    mainCharacterPosition.y = 729.0084f;
    //Updates the battle event list and performs a yield,
    //also updates the minimum position in X to move over the level and th emaximum value in X
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    // Turn off the signals that allow Spidy to leave the level for the nex one,
    // there are two exits on each level (except level 1, that one only has one,
    // exit1 is going level down, exit2 is going level up
    if (StaticVar.CurrentLevel != 0)
    {
      GameObject signaling;
      signaling = GameObject.FindGameObjectWithTag("Exit1");
      signaling.GetComponent<Renderer>().enabled = false;
      if (StaticVar.CurrentLevel == 2 && hasRemainingEvents) StartCoroutine(WaitMessagesTime());
    }
    else
    {
      //-------------------------------------------------------------------------------------------------
      //Check if there are no remaining events to show the exit carpet from first scenario
      if (!hasRemainingEvents)
      {
        GameObject door = GameObject.Find("ExitPoint");
        door.GetComponent<Renderer>().enabled = true;
      }
      //Check if there are no remaining events to show carpet
      //-------------------------------------------------------------------------------------------------

      //-------------------------------------------------------------------------------------------------
      //if there are remaining events, turn off the exit carpet from first scenario
      else
      {
        yield return new WaitForSecondsRealtime(3);
        GameObject door = GameObject.Find("ExitPoint");
        door.GetComponent<Renderer>().enabled = false;
        StartCoroutine(WaitMessagesTime());
      }
      //if there are remaining events, turn off the exit carpet
      //-------------------------------------------------------------------------------------------------
    }
    GameObject signaling2;
    signaling2 = GameObject.FindGameObjectWithTag("Exit2");
    signaling2.GetComponent<Renderer>().enabled = false;
    // Turn off the signals that allow Spidy to leave the level for the next one,
    // there are two exits on each level (except level 1, that one only has one,
    // exit1 is going level down, exit2 is going level up
    //-------------------------------------------------------------------------------------------------

    if (StaticVar.CurrentLevel == 6)
    {
      if (StaticVar.firstTimeOnLevel)
      {
        if (currentLevelData.level[StaticVar.CurrentLevel].cinematic)
        {
          currentLevelData.level[StaticVar.CurrentLevel].cinematic = false;
          music = GameObject.Find("BackgroundMusic").GetComponent<MusicBetweenScenes>();
          music.StopMusic();
          SceneManager.LoadScene("Zima");
        }
        EnemyHealthBar temp = Instantiate(healthBarPrefab);
        GameObject enemyObj;
        enemyObj = Instantiate(Bosses[0]);
        enemyObj.transform.position = new Vector3(1500, 1263.011f, 4220);
        enemyObj.transform.localScale = new Vector3(-1, 1, 1);
        enemyObj.GetComponent<Enemy>().SetHealthBar(ref temp);
        enemiesInstanced.SetInstancedEnemies(enemyObj);
        StartCoroutine(MusicStop());
        StaticVar.waitingMusic = true;
        StartCoroutine(MusicStart());
      }
    }
    actor.body.useGravity = true;                   //Turn on Spidy's gravity so it can be used back

    //-------------------------------------------------------------------------------------------------
    //Add coroutine to light screen
    yield return new WaitForSecondsRealtime(2.0f);
    darkScreen.UndarkenScreen();
    Time.timeScale = 1;                             //Starts the game so characters of the game can perform any attack or pending movement
    yield return new WaitForSecondsRealtime(1.0f);
    StaticVar.spidyAllowed = true;
    //Add coroutine to light screen
    //-------------------------------------------------------------------------------------------------

    timeToWaitEnemy = true;                         //Allows to enter the first enemy to the area

    //-------------------------------------------------------------------------------------------------
    //Check if a new enemy type will appear in this level
    if (currentLevelData.level[StaticVar.CurrentLevel].newEnemy) StaticVar.newEnemyCounter += 1;
    //Check if a new enemy type will appear in this level
    //-------------------------------------------------------------------------------------------------
  }
  //Coroutine used to load a new level data of the game
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Coroutine used to restart the scene in case there are more levels to pass
  private IEnumerator AnimateNextLevel()
  {
    StaticVar.readyToLeave = false;
    //-------------------------------------------------------------------------------------------------
    //I need to add the sequence for the nex scenario to open, based on the current scenario being played
    SceneManager.LoadScene("Game");
    //I need to add the sequence for the nex scenario to open, based on the current scenario being played
    //-------------------------------------------------------------------------------------------------
    yield return null;
  }
  //Coroutine used to restart the scene in case there are more levels to pass
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Coroutine used to delay the entry of a new enemy
  private IEnumerator TimerForNewEnemy()
  {
    yield return new WaitForSecondsRealtime(waitDelayTime);
    timeToWaitEnemy = true;
  }
  //Coroutine used to delay the entry of a new enemy
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Coroutine used as delay when scenario gets dark to change to a different scenario
  private IEnumerator WaitingBeforeLeaving()
  {
    yield return new WaitForSecondsRealtime(1);
    StaticVar.killEnemies = true;                       //Signals that all enemies of the scenario will die so Spidy can move to the next level
    timeToLeave = true;
    Time.timeScale = 1;
  }
  //Coroutine used as delay when scenario gets dark to change to a different scenario
  //-------------------------------------------------------------------------------------------------
  private IEnumerator ValidatingYChange()
  {
    waitingYMeasure = true;
    yield return new WaitForSecondsRealtime(0.25f);
    waitingYMeasure = false;
    if (Mathf.Abs((mainCharacterPosition.y + valueYOffset) - actor.transform.position.y) <= 1)
    {
      newCameraYValue = cameraBounds.cameraYPosition + valueYOffset;
      cameraBounds.CalculateOffsetY(newCameraYValue);
      mainCharacterPosition.y = actor.transform.position.y;
    }
  }
  private IEnumerator HideWall()
  {
    StaticVar.acceptTheKills = true;
    StaticVar.killEnemies = true;                       //Signals that all enemies of the scenario will die so Spidy can move to the next level
    int i = 10;
    while (i >= 0)
    {
      SetOpacity(i * 0.1f);
      yield return new WaitForFixedUpdate();
      i--;
    }
    StaticVar.killEnemies = false;
    hiddenWall = false;
    StaticVar.acceptTheKills = false;
    StaticVar.secondFloorImage = true;
    waitTime = 120.0f;
    countTimeReduced = 0;
  }
  private IEnumerator ShowWall()
  {
    StaticVar.acceptTheKills = true;
    StaticVar.killEnemies = true;                       //Signals that all enemies of the scenario will die so Spidy can move to the next level        int i = 0;
    int i = 0;
    while (i <= 10)
    {
      SetOpacity(i * 0.1f);
      yield return new WaitForFixedUpdate();
      i++;
    }
    StaticVar.killEnemies = false;
    hiddenWall = true;
    StaticVar.acceptTheKills = false;
    StaticVar.secondFloorImage = false;
    waitTime = 120.0f;
    countTimeReduced = 0;
  }
  private IEnumerator DelayRoller()
  {
    GameObject steamRoller;
    GameObject garbage;
    GameObject rollerShadow;
    steamRoller = GameObject.Find("Steamroller base rigth");
    garbage = GameObject.Find("Trash fall right");
    rollerShadow = GameObject.Find("Shadow steamroller right");
    steamRoller.SetActive(false);
    garbage.SetActive(false);
    rollerShadow.SetActive(false);
    yield return new WaitForSecondsRealtime(0.60f);
    steamRoller.SetActive(true);
    garbage.SetActive(true);
    rollerShadow.SetActive(true);
  }
  private IEnumerator SmallDelay()
  {
    while (spidyBoolSupport)
    {
      yield return null;
    }
    UIDialogue.DidDialogueStart();
    yield return new WaitForSecondsRealtime(0.5f);
    StaticVar.bossReadyToDie = true;
    StartCoroutine(music.FadeMusic());
    nerinBoolSupport = true;
    while (nerinBoolSupport)
    {
      yield return null;
    }
    UIDialogue.DidDialogueStart();
    yield return null;
    while (Time.timeScale == 0)
    {
      yield return null;
    }
    nerin.AnimateTo(StaticVar.doors[1], true, NerinDidLeave);
  }
  private IEnumerator SpidyWalkout()
  {
    actor.UseAutopilot(true);
    actor.controllable = false;
    yield return null;
    while (Time.timeScale == 0)
    {
      yield return null;
    }
    actor.AnimateTo(StaticVar.doors[1], true, DidFinishWalkout);
  }
  private IEnumerator DelayBeforeLeaving()
  {
    yield return new WaitForSecondsRealtime(0.5f);
    StaticVar.readyToLeave = true;
    StaticVar.signal2 = true;
  }
  private IEnumerator ReadyToLeave()
  {
    actor.Stop();
    actor.UseAutopilot(true);
    actor.controllable = false;
    yield return new WaitForSecondsRealtime(2.0f);
    actor.walker.enabled = true;
    yield return null;
    GameObject Boss = GameObject.Find("Zima(Clone)");
    Vector3 spidyPosition = Boss.transform.position;
    xLook = Boss.transform.localScale.x;
    spidyPosition.x += 400 * xLook;
    spidyPosition.z -= 100;
    actor.AnimateTo(spidyPosition, true, DidArrivePosition);
  }
  private IEnumerator NerinAppearance()
  {
    Vector3 reqPosition = actor.transform.position;
    Vector3 facePosition = actor.transform.localScale;
    reqPosition.x += 400 * facePosition.x;
    nerin = Instantiate(nerin);
    nerin.transform.position = StaticVar.doors[1];
    while (!nerin.allowedToRun)
    {
      yield return null;
    }
    nerin.allowedToRun = false;
    nerin.AnimateTo(reqPosition, true, NerinArrived);
  }
  private IEnumerator NerinLeaving()
  {
    yield return new WaitForSeconds(0.5f);
    nerin.walker.enabled = false;
  }
  private IEnumerator NerinDelayAtArrival()
  {
    yield return new WaitForSeconds(0.5f);
    nerinBoolSupport = false;
  }
  private IEnumerator MusicStop()
  {
    yield return null;
    music = GameObject.Find("BackgroundMusic").GetComponent<MusicBetweenScenes>();
    music.StopMusic();
  }
  private IEnumerator MusicStart()
  {
    while (StaticVar.waitingMusic) yield return null;
    StartCoroutine(music.StartMusic());
  }
  private IEnumerator StartDelay()
  {
    yield return new WaitForSeconds(0.5f);
    chains.chainsUp.SetActive(true);
  }
  private IEnumerator RemovingChains()
  {
    yield return new WaitForSecondsRealtime(2.0f);
    chains.chainsEdge.SetActive(false);
    chains.chainsUp.SetActive(false);
  }
  private IEnumerator WaitMessagesTime()
  {
    yield return new WaitForSecondsRealtime(5.0f);
    while (Time.timeScale == 0.0f)
    {
      yield return new WaitForSecondsRealtime(3.0f);
    }
    Time.timeScale = 0.0f;
    StaticVar.showAdvice = true;
    if (StaticVar.CurrentLevel == 0) StaticVar.message = 3;
    else StaticVar.message = 11;
  }
  private IEnumerator TimeToShowImages()
  {
    StaticVar.showImages = true;
    yield return new WaitForSecondsRealtime(waitTime);
    StaticVar.initTimer = true;
  }
  private IEnumerator TimeToWait()
  {
    timePassed = 0.0f;
    StaticVar.initTimer = false;
    if (countTimeReduced != 3)
    {
      waitTime /= 2;
      countTimeReduced++;
    }
    else waitTime = 10f;
    while (true)
    {
      if (!StaticVar.secondFloorImage && actor.transform.position.x <= 1200.0f && !StaticVar.barrierArea0Level3_1)
      {
        UIMessage.ShowImage(0); //Case to show center up arrow in first floor
      }
      else if (!StaticVar.secondFloorImage && actor.transform.position.x >= 1200.0f && !StaticVar.barrierArea0Level3_1)
      {
        UIMessage.ShowImage(1);  //Case to show arrow in first floor to move to the left 
      }
      else if (StaticVar.secondFloorImage && !StaticVar.barrierArea0Level3_1)
      {
        UIMessage.ShowImage(2); //Case to show arrow in second floor to push the button 
      }
      else if (StaticVar.secondFloorImage && StaticVar.barrierArea0Level3_1 && actor.transform.position.x <= 2400.0f)
      {
        UIMessage.ShowImage(3); //Case to show down arrow in second floor to go down
      }
      else if (!StaticVar.secondFloorImage && StaticVar.barrierArea0Level3_1 && actor.transform.position.x <= 2000.0f)
      {
        UIMessage.ShowImage(4);
      }
      else if (!StaticVar.secondFloorImage && StaticVar.barrierArea0Level3_1 && actor.transform.position.x <= 2900.0f)
      {
        UIMessage.ShowImage(5);
      }
      else if (StaticVar.secondFloorImage && actor.transform.position.x <= 5300.0f)
      {
        UIMessage.ShowImage(6);  //Case to show arrow in second floor to move to right
      }
      else if (StaticVar.secondFloorImage && !StaticVar.barrierArea0Level3_2)
      {
        UIMessage.ShowImage(7);  //Case to show arrow in second floor get down
      }
      else if (!StaticVar.secondFloorImage && actor.transform.position.x >= 4500.0f)
      {
        UIMessage.ShowImage(8); //Case to show up arrow in first floor
      }
      else if (!StaticVar.secondFloorImage && !StaticVar.barrierArea0Level3_2 && actor.transform.position.x <= 4500.0f)
      {
        UIMessage.ShowImage(9); //Case to show arrow to push button 2
      }
      timePassed += Time.deltaTime;
      if (timePassed >= showDuration)
      {
        StaticVar.showImages = false;
        UIMessage.alphaEnable = false;
        UIMessage.HideImage();
        break;
      }
      yield return new WaitForFixedUpdate();
    }
  }
  [System.Serializable]
  public struct ChainsEvent
  {
    public GameObject chainsEdge;
    public GameObject chainsUp;
    [SerializeField] public Animator chainsEdgeAnim;
    [SerializeField] public Animator chainsUpAnim;
  }
}
