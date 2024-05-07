using UnityEngine;
using System.Collections.Generic;

public class StaticVar : MonoBehaviour
{

  //-------------------------------------------------------------------------------------------------
  //Initial variables used to configure behaviour of character
  public static float maxExperienceLevel = 10f;       //Current max experience needed to move to the next level
  public static float levelExperience = 0f;           //Signals current experience of character before increasing level
  public static int characterLevel = 0;               //Current level character has
  public static int money = 0;                        //Variable that stores the money a character has collected
  public static float countCombo = 195f;                //Make the count on the increase of the combo counter till the max combo count
  public static float countComboMax = 200f;           //Variable used to set the max value of the combo counter
  public static float comboTimer = 0.1f;              //Timer used to define how much time character will have combo working
  public static float staminaValue = 2f;              //Stores the resistance character has when running before it stops
  public static float maxStamina = 2f;                //Stores the max resistance character has when running before stops
                                                      //Initial variables used to configure behaviour of character
                                                      //-------------------------------------------------------------------------------------------------

  public static bool firstTimeOnLevel = true;         //States if Spidy has passed for the first time over a level or not
  public static int CurrentLevel = 0;                 //Stores current background player is on
  public static float maxLife = 100f;                 //Stores the max life Spidy can have
  public static float currentLife = 100f;             //Stores the current life Spidy has
  public static float jumpDuration = 0.2f;            //Validate the time player can keep pushed the jump button to vary the force to jump
  public static int TotalEnemies = 0;                 //Variable that stores the total amount of enemies of this type
  public static int newEnemyCounter = 0;              //This variable releases new type of enemies as you move through each room
  public static bool readyToLeave = false;            //Signals that Spidy will leave the level
  public static bool signal1 = false;                 //Sets if Spidy is touching around the area to leave the room, one level down
  public static bool signal2 = false;                 //Sets if Spidy is touching around the area to leave the room, one level up
  public static bool signal3 = false;                 //Extra signal used to tell player, there is something missing on the scenario before he moves to the next level
  public static bool wildCard = false;
  public static bool wildCard2 = false;
  public static bool barrierArea0Level3_1 = false;    //Variable used to signal that stairs have been activated
  public static bool barrierArea0Level3_2 = false;    //Variable used to allow the second door open once the machine has been stopped

  //-------------------------------------------------------------------------------------------------
  //Variables used to select scenario
  public static int level;
  public static int area;
  //Variables used to select scenario
  //-------------------------------------------------------------------------------------------------

  public static bool zimaLevel = false;
  public static bool bossReadyToDie = false;
  public static bool nextExit = false;
  public static bool nerinArrived = false;
  public static bool waitingMusic = false;
  public static bool turnLights = true;              //Used to set light off from a scenario when leaving it
  public static bool killEnemies = false;             //Used to kill enemies when leaving a scenario
  public static bool spidyAllowed = false;            //Used to allow spidy to move after a scenario has been loaded
  public static bool defenseUp;                       //Flags if defense is up of a character being attacked
  public static float actorScale;                     //Check if attacking character is facing left or right
  public static float hitActorScale;                  //Check if character attacked is facing left or right
  public static bool Scene5Card = false;

  public static List<Vector3> doors = new List<Vector3>();
  public static bool acceptTheKills;
  public static int TotalKills;

  public static bool isGamePaused = false;
  public static bool gameCompleted = false;

  //---------------------------
  public static bool showText = false;
  //---------------------------

  public static bool showAdvice = false;
  public static int message = 0;
  public static bool firstTime = true;
  public static int messageSelector = 0;

  public static bool initTimer = false;
  public static bool showImages = false;
  public static bool secondFloorImage = false;

  public static bool showGameOverMenu = false;
  public static bool transformed;                //Boolean that signals Spidy is transformed

  public static bool shakeCamera = false;

  public static bool isSettingsStarted = false;

  public static int VFXValue = 0;
  public static int Musicvalue = 0;
  public static int VoiceValue = 0;
  public static int ResolutionValue = 0;
  public static int LenguageValue = 0;
  public static bool SubtitleValue = false;
  public static bool VibrationValue = false;
  public static bool HintsValue = false;
  public static bool EnemiesLifeBarValue = false;

  public static List<string> ControlsUI;

  public static string newInputBinding = " ";
  public static bool waitingNewInput = false;

}
