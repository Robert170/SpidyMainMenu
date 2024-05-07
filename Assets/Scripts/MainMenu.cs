using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
  public Animator imageAnim;      //Variable to have a reference of e animator
  public Animator imageExitAnim;  //Variable to have a reference of animator

  public Animator exitMainMenuAnim;      //Variable to have a reference of e animator
  public Animator settingsAnim;          //Variable to have a reference of animator

  public LevelData levelData;
  public float blinkTime;         //Variable to set the time of blink of the image 
  public float staticTime;        //Variable to set the time of static of the image 
  public DarkScreen darkScreen;
  private bool starting = true;
  protected float v;
  protected float h;
  protected float preseed;
  int buttonSelected = 0;
  public bool moveVerticalPressed = false;
  public bool moveHorizontalPressed = false;
  public InputAction Submit;
  public InputAction MoveAction;
  public InputAction MoveHorizontalAction;



  public GameObject mainMenu;
  public GameObject savedFileMenu;
  public GameObject settingsMenu;
  public GameObject settingsUI;
  public GameObject settingsControllUI;

  public RectTransform mainMenuRectTransform;
  public RectTransform savedFilesRectTransform;
  public RectTransform settingsRectTransform;
  public RectTransform settingsUIRectTransform;
  public RectTransform settingsUIContolsRectTransform;

  public Vector2 showPosition;
  public Vector2 mainMeuHidePosition;

  public Vector2 savedFilesHidePosition;
  public Vector2 savedFilesHidePosition2;

  public Vector2 settingsHidePosition;
  public Vector2 settingsUIHidePosition;
  public Vector2 settingsControlUIHidePosition;

  float minimunDistance = 1f;
  public float movementSpeed;


  private int currentScreen = 0;
  private int columSelected = 0;

  public GameObject saveFile1ButtonOn;
  public GameObject saveFile2ButtonOn;
  public GameObject saveFile3ButtonOn;
  public GameObject saveFile1ButtonOff;
  public GameObject saveFile2ButtonOff;
  public GameObject saveFile3ButtonOff;

  RectTransform tempRectTransformButton1;
  RectTransform tempRectTransformButton3;
  //public GameObject exitToMainMenuButton;
  //public GameObject settingsButton;

  private int maxNumButton = 1;

  private bool movenextUI = false;

  public GameObject[] settingsButtons;


  public TextMeshProUGUI[] texts;

  private bool subtitleEnable = true;
  public GameObject subtitleYesOn;
  public GameObject subtitleYesOff;
  public GameObject subtitleNoOn;
  public GameObject subtitleNoOff;

  private bool vibrationEnable = true;
  public GameObject vibrationYesOn;
  public GameObject vibrationYesOff;
  public GameObject vibrationNoOn;
  public GameObject vibrationNoOff;

  private bool hintEnable = true;
  public GameObject hintYesOn;
  public GameObject hintYesOff;
  public GameObject hintNoOn;
  public GameObject hintNoOff;

  private bool lifeBarEnable = true;
  public GameObject lifeBarYesOn;
  public GameObject lifeBarYesOff;
  public GameObject lifeBarNoOn;
  public GameObject lifeBarNoOff;


  public float tamañoMaximo = 70f;
  float velocidadCrecimiento = 50f;

  public Slider vfxVolumeSlider;
  public Slider musicVolumeSlider;
  public Slider voiceVolumeSlider;

  bool increasSliderValue;
  bool decreaseSliderValue;

  private int resolutionSelected = 0;
  public TextMeshProUGUI[] resolutionsTexts;
  public GameObject resolutionsLeftArrow;
  public GameObject resolutionsRightArrow;

  private int lenguageSelected = 0;
  public TextMeshProUGUI[] lenguageTexts;
  public GameObject lenguageLeftArrow;
  public GameObject lenguageRightArrow;

  float currentVFXVolume;
  float selectedVFXVolume;

  float currentMusicVolume;
  float selectedMusicVolume;

  float currentVoiceVolume;
  float selectedVoiceVolume;

  int currentResolution;
  int selectedResolution;

  int currentLenguage;
  int selectedLenguage;

  bool currentSubtitle;
  bool selectedSubtitle;

  bool currentVibration;
  bool selectedVibration;

  bool currentHints;
  bool selectedHints;

  bool currentEnemisLifebar;
  bool selectedEnemisLifeBar;

  public GameObject confirmationMessage;

  bool waitingConfirmation;
  bool waitingConfirmationPressed;

  float confirmation = 0;
  bool confirmationReady;
  int typeOfConfirmation;

  [SerializeField]
  ReadAndWriteSettings readAndWriteSettings;



  List<settingToChange> listSettingsInt = new List<settingToChange>();
  List<settingToChangeBool> listSettingsBool = new List<settingToChangeBool>();


  public TextMeshProUGUI[] inputUIText;

  public GameObject[] hoveredInputButton;

  bool inputEnable = true;

  private string lastTextInput = "";

  int countDown = 0;



  void Start()
  {
    //Limit the gameplay to 60 FPS
    Application.targetFrameRate = 60;
    StartCoroutine(ApplyAnimation());

    enableInputs();
    //imageExitAnim.SetBool("IsSelected", false);
    tempRectTransformButton1 = saveFile1ButtonOff.GetComponent<RectTransform>();
    tempRectTransformButton3 = saveFile3ButtonOff.GetComponent<RectTransform>();


    resolutionsLeftArrow.SetActive(false);
    resolutionsRightArrow.SetActive(false);
    lenguageLeftArrow.SetActive(false);
    lenguageRightArrow.SetActive(false);

    currentResolution = StaticVar.ResolutionValue;
    for (int i = 0; i < resolutionsTexts.Length; i++)
    {
      if (resolutionsTexts[i].enabled)
      {
        selectedResolution = i;
        break;
      }
    }

    currentLenguage = StaticVar.LenguageValue;
    for (int i = 0; i < lenguageTexts.Length; i++)
    {
      if (lenguageTexts[i].enabled)
      {
        selectedLenguage = i;
        break;
      }
    }

    setSettings();

    setUIInputs();

  }


  void Update()
  {
    UnityEngine.Debug.Log(StaticVar.waitingNewInput);
    if (waitingConfirmation)
    {
      confirmation = Submit.ReadValue<float>();
      switch (typeOfConfirmation)
      {
        case 0:
          {

            if (waitingConfirmationPressed)
            {
              switch (currentResolution)
              {
                case 0:
                  {

                    Screen.SetResolution(1920, 1080, true);
                    break;
                  }
                case 1:
                  {

                    Screen.SetResolution(1600, 900, false);
                    break;
                  }
                case 2:
                  {

                    Screen.SetResolution(1280, 720, false);
                    break;
                  }
              }
              StaticVar.ResolutionValue = currentResolution;
              waitingConfirmation = false;
              waitingConfirmationPressed = false;
              confirmationMessage.SetActive(false);
              readAndWriteSettings.ChangeSettingValue("Resolution", currentResolution);
            }
            if (confirmation == 0.0f || confirmationReady)
            {
              confirmationReady = true;
              if (confirmation != 0)
              {
                waitingConfirmationPressed = true;
                confirmationReady = false;
              }
            }
            break;
          }
        case 1:
          {
            if (waitingConfirmationPressed)
            {
              StaticVar.LenguageValue = currentLenguage;
              waitingConfirmation = false;
              waitingConfirmationPressed = false;
              confirmationMessage.SetActive(false);
              readAndWriteSettings.ChangeSettingValue("Lenguage", currentLenguage);
            }
            if (confirmation == 0.0f || confirmationReady)
            {
              confirmationReady = true;
              if (confirmation != 0)
              {
                waitingConfirmationPressed = true;
                confirmationReady = false;
              }
            }
            break;
          }
        case 2:
          {
            if (waitingConfirmationPressed)
            {

              if (listSettingsInt != null)
              {
                foreach (settingToChange setting in listSettingsInt)
                {
                  readAndWriteSettings.ChangeSettingValue(setting.settingName, setting.settingValue);
                  changeSettings(setting.settingName, setting.settingValue);
                }


                listSettingsInt.Clear();
              }

              if (listSettingsBool != null)
              {
                foreach (settingToChangeBool setting in listSettingsBool)
                {
                  readAndWriteSettings.ChangeSettingValue(setting.settingName, setting.settingBoolValue ? 1 : 0);
                  changeSettings(setting.settingName, setting.settingBoolValue ? 1 : 0);
                }
                listSettingsBool.Clear();
              }
              waitingConfirmation = false;
              waitingConfirmationPressed = false;
              confirmationMessage.SetActive(false);
              savedFileMenu.SetActive(true);
              //To hide
              StartCoroutine(MoveUIScreen(settingsRectTransform, settingsHidePosition, true, settingsMenu));
              //To show
              StartCoroutine(MoveUIScreen(savedFilesRectTransform, showPosition, false));
              currentScreen = 1;
              columSelected = 0;
              buttonSelected = 0;
              maxNumButton = 2;
            }
            if (confirmation == 0.0f || confirmationReady)
            {
              confirmationReady = true;
              if (confirmation != 0)
              {
                waitingConfirmationPressed = true;
                confirmationReady = false;
              }
            }
            break;
          }
      }




      return;
    }
    if (StaticVar.waitingNewInput)
    {
      changueInpuntBinding();
      return;
    }

    v = MoveAction.ReadValue<float>();
    h = MoveHorizontalAction.ReadValue<float>();

    if(StaticVar.newInputBinding != "10" && countDown == 0)
    {
      preseed = Submit.ReadValue<float>();
    }
    else
    {
      if(countDown == 0)
      {
        
        StaticVar.newInputBinding = " ";
      }
      else if(countDown > 0)
      {
        preseed = 0;
        countDown--;
      }
      
    }
    
    if (moveHorizontalPressed)
    {
      if (h != 0) v = 0;
      else
      {
        moveHorizontalPressed = false;
        increasSliderValue = false;
        decreaseSliderValue = false;

      }
    }
    else if (h != 0)
    {
      moveHorizontalPressed = true;
      if (h > 0)
      {
        increasSliderValue = true;
        if (currentScreen == 2)
        {
          switch (buttonSelected)
          {
            case 3:
              {
                resolutionSelected++;
                resolutionsRightArrow.SetActive(true);
                if (resolutionSelected > resolutionsTexts.Length - 1)
                {
                  resolutionSelected = 0;
                }

                break;
              }
            case 4:
              {
                lenguageSelected++;
                lenguageRightArrow.SetActive(true);
                if (lenguageSelected > lenguageTexts.Length - 1)
                {
                  lenguageSelected = 0;
                }
                break;
              }

            case 5:
              {
                subtitleEnable = false;
                break;
              }
            case 7:
              {
                vibrationEnable = false;
                break;
              }
            case 8:
              {
                hintEnable = false;
                break;
              }
            case 9:
              {
                lifeBarEnable = false;
                break;
              }
          }
        }
        if (currentScreen != 2 && currentScreen != 3)
        {
          if (columSelected == 0)
          {

            buttonSelected = 0;
            columSelected = 1;
          }
          else
          {
            exitMainMenuAnim.SetBool("IsSelectedExit", false);
            settingsAnim.SetBool("IsSelectedSettings", false);
            columSelected--;
          }
        }

      }
      else
      {

        decreaseSliderValue = true;
        if (currentScreen == 2)
        {
          switch (buttonSelected)
          {
            case 3:
              {
                resolutionSelected--;
                resolutionsLeftArrow.SetActive(true);
                if (resolutionSelected < 0)
                {
                  resolutionSelected = resolutionsTexts.Length - 1;
                }

                break;
              }
            case 4:
              {
                lenguageSelected--;
                lenguageLeftArrow.SetActive(true);
                if (lenguageSelected < 0)
                {
                  lenguageSelected = lenguageTexts.Length - 1;
                }
                break;
              }
            case 5:
              {
                subtitleEnable = true;
                break;
              }
            case 7:
              {
                vibrationEnable = true;
                break;
              }
            case 8:
              {
                hintEnable = true;
                break;
              }
            case 9:
              {
                lifeBarEnable = true;
                break;
              }
          }
        }
        if (currentScreen != 2 && currentScreen != 3)
        {
          if (columSelected == 1)
          {
            columSelected = 0;
            buttonSelected = 0;
            exitMainMenuAnim.SetBool("IsSelectedExit", false);
            settingsAnim.SetBool("IsSelectedSettings", false);
          }
          else
          {
            buttonSelected = 0;
            columSelected++;
          }
        }



      }

    }

    ////////////////////////////////////


    if (moveVerticalPressed)
    {
      if (v != 0) v = 0;
      else moveVerticalPressed = false;
    }
    if (preseed > 0 && !starting)
    {
      switch (currentScreen)
      {
        case 0:
          {
            disableInputs();
            StartCoroutine(ApplyAnimation());
            break;
          }
        case 1:
          {
            disableInputs();
            buttonSelecttedFiles();
            break;
          }
        case 2:
          {
            disableInputs();
            buttonSelectteSettings();
            break;
          }
        case 3:
          {

            disableInputs();
            changueInpuntBinding();

            break;
          }
      }

    }
    else if (v != 0)
    {
      moveVerticalPressed = true;
      if (v > 0)
      {
        if (buttonSelected == 0)
        {
          buttonSelected = maxNumButton;
        }
        else
        {
          buttonSelected--;
        }
      }
      else
      {
        if (buttonSelected == maxNumButton)
        {
          buttonSelected = 0;
        }
        else
        {
          buttonSelected++;
        }
      }
      //SetButton();
    }
    else if (movenextUI)
    {
      savedFileMenu.SetActive(true);

      //To hide main menu
      StartCoroutine(MoveUIScreen(mainMenuRectTransform, mainMeuHidePosition, true, mainMenu));

      //To show saved files menu
      StartCoroutine(MoveUIScreen(savedFilesRectTransform, showPosition, false));
      currentScreen = 1;
      buttonSelected = 0;
      maxNumButton = 2;
      columSelected = 0;
    }


    switch (currentScreen)
    {
      case 0:
        {
          SetButton();
          break;
        }
      case 1:
        {
          if (columSelected == 0)
          {
            maxNumButton = 2;

            SetButtonSavedFile();
          }
          else
          {
            maxNumButton = 1;
            saveFile1ButtonOn.SetActive(false);
            saveFile2ButtonOn.SetActive(false);
            saveFile3ButtonOn.SetActive(false);

            saveFile1ButtonOff.SetActive(true);
            saveFile2ButtonOff.SetActive(true);
            saveFile3ButtonOff.SetActive(true);
            tempRectTransformButton1.anchoredPosition = new Vector2(638f, 239);
            tempRectTransformButton3.anchoredPosition = new Vector2(638f, -239f);
            SetButtonSavedFile2();
          }
          break;
        }
      case 2:
        {
          SetButtonSettings();
          break;
        }
      case 3:
        {

          SetButtonInputs();
          break;
        }
    }
  }
  //Public method that can be called from a button
  public void GoToGame()
  {
    if (!starting)
    {
      StartCoroutine(ApplyAnimation());
    }
  }
  private void SetButton()
  {
    if (buttonSelected == 0)
    {
      imageExitAnim.SetBool("IsSelected", false);
      imageAnim.SetBool("timePassed", false);
    }
    else
    {
      imageExitAnim.SetBool("IsSelected", true);
      imageAnim.SetBool("timePassed", true);
    }
  }

  private void SetButtonSavedFile()
  {
    saveFile1ButtonOn.SetActive(false);
    saveFile2ButtonOn.SetActive(false);
    saveFile3ButtonOn.SetActive(false);

    saveFile1ButtonOff.SetActive(true);
    saveFile2ButtonOff.SetActive(true);
    saveFile3ButtonOff.SetActive(true);

    if (buttonSelected == 0)
    {
      saveFile1ButtonOn.SetActive(true);
      saveFile1ButtonOff.SetActive(false);
      tempRectTransformButton1.anchoredPosition = new Vector2(638f, 239);
      tempRectTransformButton3.anchoredPosition = new Vector2(638f, -239f);
    }
    else if (buttonSelected == 1)
    {
      saveFile2ButtonOn.SetActive(true);
      saveFile2ButtonOff.SetActive(false);
      tempRectTransformButton1.anchoredPosition = new Vector2(638f, 319f);
      tempRectTransformButton3.anchoredPosition = new Vector2(638f, -279f);

    }
    else
    {
      saveFile3ButtonOn.SetActive(true);
      saveFile3ButtonOff.SetActive(false);
      tempRectTransformButton1.anchoredPosition = new Vector2(638f, 239);
      tempRectTransformButton3.anchoredPosition = new Vector2(638f, -239f);
    }
  }

  private void SetButtonSavedFile2()
  {


    if (buttonSelected == 0)
    {

      exitMainMenuAnim.SetBool("IsSelectedExit", true);
      settingsAnim.SetBool("IsSelectedSettings", false);
    }
    else
    {
      exitMainMenuAnim.SetBool("IsSelectedExit", false);
      settingsAnim.SetBool("IsSelectedSettings", true);
    }

  }

  private void SetButtonSettings()
  {

    foreach (GameObject _object in settingsButtons)
    {

      _object.SetActive(false);


    }
    for (int i = 0; i < texts.Length; i++)
    {
      if (texts[i].fontSize > 60 && buttonSelected != i)
      {
        texts[i].fontSize = 60;
      }
    }

    for (int i = 0; i < lenguageTexts.Length; i++)
    {
      if (i == lenguageSelected)
      {
        lenguageTexts[i].enabled = true;
        selectedLenguage = i;
      }
      else
      {
        lenguageTexts[i].enabled = false;
      }
    }


    for (int i = 0; i < resolutionsTexts.Length; i++)
    {
      if (i == resolutionSelected)
      {
        resolutionsTexts[i].enabled = true;
        selectedResolution = i;

      }
      else
      {
        resolutionsTexts[i].enabled = false;
      }
    }



    if (resolutionsLeftArrow.activeSelf || resolutionsRightArrow.activeSelf ||
        lenguageLeftArrow.activeSelf || lenguageRightArrow.activeSelf)
    {
      Invoke("deactivateArrow", 0.1f);
    }



    settingsButtons[buttonSelected].SetActive(true);


    if (texts[buttonSelected].fontSize < tamañoMaximo)
    {
      texts[buttonSelected].fontSize += velocidadCrecimiento * Time.deltaTime;
    }



    subtitleYesOn.SetActive(false);
    subtitleNoOn.SetActive(false);
    vibrationYesOn.SetActive(false);
    vibrationNoOn.SetActive(false);
    hintYesOn.SetActive(false);
    hintNoOn.SetActive(false);
    lifeBarYesOn.SetActive(false);
    lifeBarNoOn.SetActive(false);

    switch (buttonSelected)
    {

      case 0:
        {
          if (increasSliderValue)
          {
            vfxVolumeSlider.value += 0.01f;
          }
          else if (decreaseSliderValue)
          {
            vfxVolumeSlider.value -= 0.01f;
          }
          selectedVFXVolume = vfxVolumeSlider.value;
          break;
        }
      case 1:
        {
          if (increasSliderValue)
          {
            musicVolumeSlider.value += 0.01f;
          }
          else if (decreaseSliderValue)
          {
            musicVolumeSlider.value -= 0.01f;
          }
          selectedMusicVolume = musicVolumeSlider.value;
          break;
        }
      case 2:
        {
          if (increasSliderValue)
          {
            voiceVolumeSlider.value += 0.01f;
          }
          else if (decreaseSliderValue)
          {
            voiceVolumeSlider.value -= 0.01f;
          }
          selectedVoiceVolume = voiceVolumeSlider.value;
          break;
        }
      case 5:
        {
          if (subtitleEnable)
          {
            subtitleYesOn.SetActive(true);
            subtitleYesOff.SetActive(true);
            subtitleNoOn.SetActive(false);
            subtitleNoOff.SetActive(false);
          }
          else
          {
            subtitleNoOn.SetActive(true);
            subtitleNoOff.SetActive(true);
            subtitleYesOn.SetActive(false);
            subtitleYesOff.SetActive(false);
          }
          selectedSubtitle = subtitleEnable;
          break;
        }
      case 7:
        {
          if (vibrationEnable)
          {
            vibrationYesOn.SetActive(true);
            vibrationYesOff.SetActive(true);
            vibrationNoOn.SetActive(false);
            vibrationNoOff.SetActive(false);
          }
          else
          {
            vibrationNoOn.SetActive(true);
            vibrationNoOff.SetActive(true);
            vibrationYesOn.SetActive(false);
            vibrationYesOff.SetActive(false);
          }
          selectedVibration = vibrationEnable;
          break;
        }
      case 8:
        {
          if (hintEnable)
          {
            hintYesOn.SetActive(true);
            hintYesOff.SetActive(true);
            hintNoOn.SetActive(false);
            hintNoOff.SetActive(false);
          }
          else
          {
            hintNoOn.SetActive(true);
            hintNoOff.SetActive(true);
            hintYesOn.SetActive(false);
            hintYesOff.SetActive(false);
          }
          selectedHints = hintEnable;
          break;
        }
      case 9:
        {
          if (lifeBarEnable)
          {
            lifeBarYesOn.SetActive(true);
            lifeBarYesOff.SetActive(true);
            lifeBarNoOn.SetActive(false);
            lifeBarNoOff.SetActive(false);
          }
          else
          {
            lifeBarNoOn.SetActive(true);
            lifeBarNoOff.SetActive(true);
            lifeBarYesOn.SetActive(false);
            lifeBarYesOff.SetActive(false);
          }
          selectedEnemisLifeBar = lifeBarEnable;
          break;
        }
      default:
        {

          break;
        }
    }

  }

  private void SetButtonInputs()
  {
    foreach (GameObject _object in hoveredInputButton)
    {

      _object.SetActive(false);


    }
    hoveredInputButton[buttonSelected].SetActive(true);
  }


  private void enableInputs()
  {
    inputEnable = true;
    Submit.Enable();
    MoveAction.Enable();
    MoveHorizontalAction.Enable();
    
  }

  private void disableInputs()
  {
    inputEnable = false;
    Submit.Disable();
    MoveAction.Disable();
    MoveHorizontalAction.Disable();
    
  }

  private void buttonSelecttedFiles()
  {
    if (columSelected == 0)
    {

    }
    else
    {
      if (buttonSelected == 0)
      {
        mainMenu.SetActive(true);
        //To hide saved files UI
        StartCoroutine(MoveUIScreen(savedFilesRectTransform, savedFilesHidePosition, true, savedFileMenu));
        //To show Main menu
        StartCoroutine(MoveUIScreen(mainMenuRectTransform, showPosition, false));
        currentScreen = 0;
        buttonSelected = 0;

      }
      else
      {
        settingsMenu.SetActive(true);
        //To hide saved files UI
        StartCoroutine(MoveUIScreen(savedFilesRectTransform, savedFilesHidePosition2, true, savedFileMenu));
        //To show settings Menu
        StartCoroutine(MoveUIScreen(settingsRectTransform, showPosition, false));
        currentScreen = 2;
        buttonSelected = 0;
        maxNumButton = 11;
      }
    }
  }
  private void buttonSelectteSettings()
  {

    switch (buttonSelected)
    {
      case 0:
        {
          break;
        }
      case 1:
        {
          break;
        }
      case 2:
        {
          break;
        }
      case 3: //resolution
        {
          if (currentResolution != selectedResolution)
          {
            currentResolution = selectedResolution;
            confirmationMessage.SetActive(true);
            waitingConfirmation = true;
            confirmation = Submit.ReadValue<float>();
            typeOfConfirmation = 0;
          }
          enableInputs();
          break;
        }
      case 4: //Lenguage
        {
          if (currentLenguage != selectedLenguage)
          {
            currentLenguage = selectedLenguage;
            confirmationMessage.SetActive(true);
            waitingConfirmation = true;
            confirmation = Submit.ReadValue<float>();
            typeOfConfirmation = 1;
          }
          enableInputs();
          break;
        }
      case 5:
        {
          break;
        }
      case 6: //Controls
        {
          settingsControllUI.SetActive(true);
          //To hide
          StartCoroutine(MoveUIScreen(settingsUIRectTransform, savedFilesHidePosition, true, settingsUI));
          //To show
          StartCoroutine(MoveUIScreen(settingsUIContolsRectTransform, showPosition, false));
          currentScreen = 3;
          buttonSelected = 0;
          maxNumButton = 10;
          //enableInputs();
          break;
        }
      case 7:
        {
          break;
        }
      case 8:
        {
          break;
        }
      case 9:
        {
          break;
        }
      case 10:
        {
          break;
        }
      case 11: //Return
        {
          if (checkSettingsChange())
          {

            confirmationMessage.SetActive(true);
            waitingConfirmation = true;
            typeOfConfirmation = 2;
          }
          else
          {
            savedFileMenu.SetActive(true);
            //To hide
            StartCoroutine(MoveUIScreen(settingsRectTransform, settingsHidePosition, true, settingsUI));
            //To show
            StartCoroutine(MoveUIScreen(savedFilesRectTransform, showPosition, false));
            currentScreen = 1;
            columSelected = 0;
            buttonSelected = 0;
            maxNumButton = 2;
          }
          enableInputs();
          break;
        }
    }

  }

  private void changueInpuntBinding()
  {
    if (buttonSelected != 10)
    {
      enableInputs();

      StaticVar.waitingNewInput = true;

      if (StaticVar.newInputBinding != " ")
      {
        inputUIText[buttonSelected].text = StaticVar.newInputBinding;
        switch (buttonSelected)
        {
          case 0:
            {
              readAndWriteSettings.ChangeSettingValue("Attack", StaticVar.newInputBinding);
              break;
            }
          case 1:
            {
              readAndWriteSettings.ChangeSettingValue("HeavyAttack", StaticVar.newInputBinding);
              break;
            }
          case 2:
            {
              readAndWriteSettings.ChangeSettingValue("Kick", StaticVar.newInputBinding);
              break;
            }
          case 3:
            {
              readAndWriteSettings.ChangeSettingValue("Berserker", StaticVar.newInputBinding);
              break;
            }
          case 4:
            {
              readAndWriteSettings.ChangeSettingValue("Run", StaticVar.newInputBinding);
              break;
            }
          case 5:
            {
              readAndWriteSettings.ChangeSettingValue("Jump", StaticVar.newInputBinding);
              break;
            }
          case 6:
            {
              readAndWriteSettings.ChangeSettingValue("Up", StaticVar.newInputBinding);
              break;
            }
          case 7:
            {
              readAndWriteSettings.ChangeSettingValue("Down", StaticVar.newInputBinding);
              break;
            }
          case 8:
            {
              readAndWriteSettings.ChangeSettingValue("Left", StaticVar.newInputBinding);
              break;
            }
          case 9:
            {
              readAndWriteSettings.ChangeSettingValue("Right", StaticVar.newInputBinding);
              break;
            }

        }
        StaticVar.waitingNewInput = false;
        StaticVar.newInputBinding = "10";
        preseed = 0;
        countDown = 50;
      }


    }
    else
    {
      settingsUI.SetActive(true);
      //hide
      StartCoroutine(MoveUIScreen(settingsUIContolsRectTransform, settingsControlUIHidePosition, true, settingsControllUI));
      //show
      StartCoroutine(MoveUIScreen(settingsUIRectTransform, showPosition, false));
      currentScreen = 2;
      buttonSelected = 0;
      maxNumButton = 11;
      enableInputs();
    }
  }



  void deactivateArrow()
  {

    if (resolutionsLeftArrow.activeSelf)
    {
      resolutionsLeftArrow.SetActive(false);
    }
    if (resolutionsRightArrow.activeSelf)
    {
      resolutionsRightArrow.SetActive(false);
    }
    if (lenguageLeftArrow.activeSelf)
    {
      lenguageLeftArrow.SetActive(false);
    }
    else
    {
      lenguageRightArrow.SetActive(false);
    }
  }

  void setSettings()
  {
    //VFX volume
    vfxVolumeSlider.value = StaticVar.VFXValue / 100.0f;
    currentVFXVolume = StaticVar.VFXValue;

    //Music volume
    musicVolumeSlider.value = StaticVar.Musicvalue / 100.0f;
    currentMusicVolume = StaticVar.Musicvalue;

    //Voice
    voiceVolumeSlider.value = StaticVar.VoiceValue / 100.0f;
    currentVoiceVolume = StaticVar.VoiceValue;

    //Resolution
    foreach (TextMeshProUGUI texts in resolutionsTexts)
    {
      texts.enabled = false;
    }
    resolutionsTexts[StaticVar.ResolutionValue].enabled = true;
    resolutionSelected = StaticVar.ResolutionValue;

    //Lenguage
    foreach (TextMeshProUGUI texts in lenguageTexts)
    {
      texts.enabled = false;
    }
    lenguageTexts[StaticVar.LenguageValue].enabled = true;
    lenguageSelected = StaticVar.LenguageValue;

    //Subtitle
    if (StaticVar.SubtitleValue)
    {
      subtitleYesOn.SetActive(true);
      subtitleYesOff.SetActive(true);
      subtitleNoOn.SetActive(false);
      subtitleNoOff.SetActive(false);

    }
    else
    {
      subtitleNoOn.SetActive(true);
      subtitleNoOff.SetActive(true);
      subtitleYesOn.SetActive(false);
      subtitleYesOff.SetActive(false);
    }

    currentSubtitle = StaticVar.SubtitleValue;
    subtitleEnable = StaticVar.SubtitleValue;

    //Vibration
    if (StaticVar.VibrationValue)
    {
      vibrationYesOn.SetActive(true);
      vibrationYesOff.SetActive(true);
      vibrationNoOn.SetActive(false);
      vibrationNoOff.SetActive(false);
    }
    else
    {
      vibrationNoOn.SetActive(true);
      vibrationNoOff.SetActive(true);
      vibrationYesOn.SetActive(false);
      vibrationYesOff.SetActive(false);
    }
    currentVibration = StaticVar.VibrationValue;
    vibrationEnable = StaticVar.VibrationValue;

    //Hints
    if (StaticVar.HintsValue)
    {
      hintYesOn.SetActive(true);
      hintYesOff.SetActive(true);
      hintNoOn.SetActive(false);
      hintNoOff.SetActive(false);
    }
    else
    {
      hintNoOn.SetActive(true);
      hintNoOff.SetActive(true);
      hintYesOn.SetActive(false);
      hintYesOff.SetActive(false);
    }
    currentHints = StaticVar.HintsValue;
    hintEnable = StaticVar.HintsValue;

    //Enemies Life bar
    if (StaticVar.EnemiesLifeBarValue)
    {
      lifeBarYesOn.SetActive(true);
      lifeBarYesOff.SetActive(true);
      lifeBarNoOn.SetActive(false);
      lifeBarNoOff.SetActive(false);
    }
    else
    {
      lifeBarNoOn.SetActive(true);
      lifeBarNoOff.SetActive(true);
      lifeBarYesOn.SetActive(false);
      lifeBarYesOff.SetActive(false);
    }
    currentEnemisLifebar = StaticVar.EnemiesLifeBarValue;
    lifeBarEnable = StaticVar.EnemiesLifeBarValue;
  }
  void setUIInputs()
  {
    for (int i = 0; i < inputUIText.Length - 1; i++)
    {
      inputUIText[i].text = StaticVar.ControlsUI[i];
    }
  }

  private bool checkSettingsChange()
  {
    bool anyChancge = false;

    float temp = selectedVFXVolume * 100.0f;
    float temp1 = selectedMusicVolume * 100.0f;
    float temp2 = selectedVoiceVolume * 100.0f;
    if (currentVFXVolume != temp)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "VFX",
        settingValue = (int)(selectedVFXVolume * 100.0f)
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (Mathf.Abs(currentMusicVolume - temp1) > 0.001)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "Music",
        settingValue = (int)(selectedMusicVolume * 100.0f)
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (currentVoiceVolume != temp2)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "Voice",
        settingValue = (int)(selectedVoiceVolume * 100.0f)
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (currentVoiceVolume != temp2)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "Voice",
        settingValue = (int)(selectedVoiceVolume * 100.0f)
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (currentResolution != selectedResolution)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "Resolution",
        settingValue = selectedResolution
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (currentLenguage != selectedLenguage)
    {
      settingToChange newValues = new settingToChange
      {
        settingName = "Lenguage",
        settingValue = selectedLenguage
      };

      listSettingsInt.Add(newValues);
      anyChancge = true;
    }

    if (currentSubtitle != selectedSubtitle)
    {
      settingToChangeBool newValues = new settingToChangeBool
      {
        settingName = "Subtitles",
        settingBoolValue = subtitleEnable
      };

      listSettingsBool.Add(newValues);
      anyChancge = true;
    }

    if (currentVibration != selectedVibration)
    {
      settingToChangeBool newValues = new settingToChangeBool
      {
        settingName = "Vibration",
        settingBoolValue = selectedVibration
      };

      listSettingsBool.Add(newValues);
      anyChancge = true;
    }

    if (currentHints != selectedHints)
    {
      settingToChangeBool newValues = new settingToChangeBool
      {
        settingName = "Hints",
        settingBoolValue = selectedHints
      };

      listSettingsBool.Add(newValues);
      anyChancge = true;
    }

    if (currentEnemisLifebar != selectedEnemisLifeBar)
    {
      settingToChangeBool newValues = new settingToChangeBool
      {
        settingName = "EnemiesLifeBar",
        settingBoolValue = selectedEnemisLifeBar
      };

      listSettingsBool.Add(newValues);
      anyChancge = true;
    }




    return anyChancge;
  }
  private void changeSettings(string settingNameParam, int settingValueParam)
  {
    switch (settingNameParam)
    {
      case "VFX":
        {
          currentVFXVolume = settingValueParam;
          StaticVar.VFXValue = settingValueParam;
          break;
        }
      case "Music":
        {
          currentMusicVolume = settingValueParam;
          StaticVar.Musicvalue = settingValueParam;
          break;
        }
      case "Voice":
        {
          currentVoiceVolume = settingValueParam;
          StaticVar.VoiceValue = settingValueParam;
          break;
        }
      case "Resolution":
        {
          switch (selectedResolution)
          {
            case 0:
              {

                Screen.SetResolution(1920, 1080, true);
                break;
              }
            case 1:
              {

                Screen.SetResolution(1600, 900, false);
                break;
              }
            case 2:
              {

                Screen.SetResolution(1280, 720, false);
                break;
              }
          }
          currentResolution = settingValueParam;
          StaticVar.ResolutionValue = settingValueParam;
          break;
        }
      case "Lenguage":
        {
          currentLenguage = settingValueParam;
          StaticVar.LenguageValue = settingValueParam;
          break;
        }
      case "Subtitles":
        {
          currentSubtitle = settingValueParam != 0;
          StaticVar.SubtitleValue = settingValueParam != 0;
          break;
        }
      case "Vibration":
        {

          currentVibration = settingValueParam != 0;
          StaticVar.VibrationValue = settingValueParam != 0;
          break;
        }
      case "Hints":
        {

          currentHints = settingValueParam != 0;
          StaticVar.HintsValue = settingValueParam != 0;
          break;
        }
      case "EnemiesLifeBar":
        {

          currentEnemisLifebar = settingValueParam != 0;
          StaticVar.EnemiesLifeBarValue = settingValueParam != 0;
          break;
        }
      default:
        break;
    }
  }


  private IEnumerator MoveUIScreen(RectTransform menuRectTransform,
                                   Vector2 finalPosition,
                                   bool movenextUIPram,
                                   GameObject deactivateUI = null)
  {

    while (Vector2.Distance(menuRectTransform.anchoredPosition, finalPosition) > minimunDistance)
    {
      menuRectTransform.anchoredPosition = Vector2.Lerp(menuRectTransform.anchoredPosition,
                                                        finalPosition,
                                                        Time.deltaTime * movementSpeed);
      yield return null;
    }



    menuRectTransform.anchoredPosition = finalPosition;

    if (null != deactivateUI)
    {
      deactivateUI.SetActive(false);
      enableInputs();
    }
    movenextUI = movenextUIPram;
  }


  private IEnumerator ApplyAnimation()
  {
    if (buttonSelected != 0)
    {
      //imageExitAnim.SetBool("Apressed", true);
      //yield return new WaitForSecondsRealtime(blinkTime);
      //imageExitAnim.SetBool("timePassed", true);
      Application.Quit();
    }
    else if (starting)
    {
      yield return new WaitForSecondsRealtime(2.0f);
      darkScreen.UndarkenScreen();
      yield return new WaitForSecondsRealtime(1.0f);
      starting = false;
    }
    else if (buttonSelected == 0)
    {
      imageAnim.SetBool("Apressed", true);
      yield return new WaitForSecondsRealtime(blinkTime);
      imageAnim.SetBool("timePassed", true);
      yield return new WaitForSecondsRealtime(staticTime);
      movenextUI = true;

      //savedFileMenu.SetActive(true);

      ////To hide main menu
      //StartCoroutine(MoveUIScreen(mainMenuRectTransform, mainMeuHidePosition, mainMenu));

      ////To show saved files menu
      //StartCoroutine(MoveUIScreen(savedFilesRectTransform, savedFilesShowPosition));
      //currentScreen = 1;
      //buttonSelected = 0;
      //maxNumButton = 2;
      //columSelected = 0;
    }

    //else if (buttonSelected == 0)
    //{
    //    StaticVar.signal1 = false;
    //    StaticVar.signal2 = false;
    //    StaticVar.maxExperienceLevel = 10f;
    //    StaticVar.characterLevel = 0;
    //    StaticVar.levelExperience = 0f;
    //    StaticVar.maxLife = 100f;
    //    StaticVar.currentLife = 100f;
    //    StaticVar.staminaValue = 2f;
    //    StaticVar.maxStamina = 2f;
    //    StaticVar.countComboMax = 100f;
    //    StaticVar.countCombo = 0f;
    //    StaticVar.money = 0;
    //    StaticVar.CurrentLevel = 0;
    //    StaticVar.newEnemyCounter = 0;
    //    StaticVar.showGameOverMenu = false;
    //    for (int i = 0; i <= 6; i++)
    //    {
    //        if (i != 6)
    //        {
    //            levelData.level[i].cinematic = false;
    //            if (i != 3)
    //            {
    //                levelData.level[i].firstTimeAccess = true;
    //                if (i == 0 || i == 1)
    //                {
    //                    levelData.level[i].newEnemy = true;
    //                }
    //                else
    //                {
    //                    levelData.level[i].newEnemy = false;
    //                }
    //            }
    //            else
    //            {
    //                levelData.level[i].firstTimeAccess = false;
    //                levelData.level[i].newEnemy = true;
    //            }
    //        }
    //        else
    //        {
    //            levelData.level[i].cinematic = true;
    //            levelData.level[i].firstTimeAccess = true;
    //            levelData.level[i].newEnemy = false;
    //        }
    //    }
    //    imageAnim.SetBool("Apressed", true);
    //    yield return new WaitForSecondsRealtime(blinkTime);
    //    imageAnim.SetBool("timePassed", true);
    //    yield return new WaitForSecondsRealtime(staticTime);
    //    darkScreen.DarkenScreen();
    //    yield return new WaitForSecondsRealtime(3);
    //    SceneManager.LoadScene("IntroGame");
    //}
  }


  public struct settingToChange
  {
    public string settingName;
    public int settingValue;
  }

  public struct settingToChangeBool
  {
    public string settingName;
    public bool settingBoolValue;
  }

}
