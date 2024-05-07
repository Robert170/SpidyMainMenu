using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.IO;
using System;
using System.Linq;

public class ReadAndWriteSettings : MonoBehaviour
{

  void SetScreenResolution(int resolutionIndex)
  {
    switch (resolutionIndex)
    {
      case 0:
        Screen.SetResolution(1920, 1080, true);

        break;
      case 1:
        Screen.SetResolution(1600, 900, false);
        break;
      case 2:
        Screen.SetResolution(1280, 720, false);
        break;
      default:
        break;
    }
  }

  [SerializeField]
  private TextAsset settingsText;  //The txt file 


  string content; //Variable to save all the content of the txt

  string[] lines; //Variable to store the lines of the txt

  string[] wordsInLine; //Variable to store the words of the current line

  string settingName; //The name of the setting

  int settingValue; //The value of the setting

  string filePath;


  // Start is called before the first frame update
  void Start()
  {
    
    //Get and save the path of the file
    filePath = Application.dataPath + "/Resources" + "/SettingSipyAdventureTXT.txt";

    //Check if is the fist time that is loaded
    if (!StaticVar.isSettingsStarted)
    {
      StaticVar.isSettingsStarted = true;

      if (settingsText != null) //Check that txt is loaded
      {
        content = settingsText.text; //Get all text

        // Divide the content in sections using --- as a separator
        string[] sections = content.Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string section in sections) //Process each section
        {
          ProcessSection(section.Trim());
        }
      }
    }

    switch (StaticVar.ResolutionValue)
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

  }


  void ProcessSection(string section)
  {
    //Divide the sections in lines
    string[] lines = section.Split('\n');

    //Check which section is
    if (lines.Length > 0)
    {
      string sectionName = lines[0].Trim().ToUpper();

      switch (sectionName)
      {
        case "SETTINGS":
          ProcessSettingsSection(lines.Skip(1).ToArray()); // Pasa el resto de las líneas a ProcessSettingsSection
          break;
        case "CONTROLS":
          ProcessControlsSection(lines.Skip(1).ToArray()); // Pasa el resto de las líneas a ProcessControlsSection
          break;
        default:
          Debug.LogWarning("Unknown section name: " + sectionName);
          break;
      }
    }
  }

  void ProcessSettingsSection(string[] lines)
  {
    //Cheack all lines
    foreach (string line in lines)
    {
      //Divide the current line in words
      string[] wordsInLine = line.Split(' ');

      if (wordsInLine.Length == 2) //Check that has 2 words (Name and value)
      {
        settingName = wordsInLine[0]; //Get and save the name of the setting

        if (int.TryParse(wordsInLine[1], out settingValue)) //Try to convert string to int to obtain the value
        {
          //Using the name to set the correspond value
          switch (settingName)
          {
            case "VFX":
              StaticVar.VFXValue = settingValue;
              break;
            case "Music":
              StaticVar.Musicvalue = settingValue;
              break;
            case "Voice":
              StaticVar.VoiceValue = settingValue;
              break;
            case "Resolution":
              StaticVar.ResolutionValue = settingValue;
              break;
            case "Lenguage":
              StaticVar.LenguageValue = settingValue;
              break;
            case "Subtitles":
              StaticVar.SubtitleValue = (settingValue != 0);
              break;
            case "Vibration":
              StaticVar.VibrationValue = (settingValue != 0);
              break;
            case "Hints":
              StaticVar.HintsValue = (settingValue != 0);
              break;
            case "EnemiesLifeBar":
              StaticVar.EnemiesLifeBarValue = (settingValue != 0);
              break;
            default:
              Debug.LogWarning("Unknown setting name: " + settingName);
              break;
          }
        }
        else
        {
          Debug.LogError("Error trying to convert value into int in the line: " + line);
        }
      }
      else
      {
        Debug.LogError("Invalid line format in SETTINGS section: " + line);
      }
    }
  }

  void ProcessControlsSection(string[] lines)
  {
    // Create a list to save values
    List<string> controlValues = new List<string>();

    // Check all lines
    foreach (string line in lines)
    {
      // Divide the current line into words
      string[] wordsInLine = line.Split(' ');

      if (wordsInLine.Length == 2) // Check that it has 2 words (Name and value)
      {
        // Get the value to save in the list
        controlValues.Add(wordsInLine[1]);
      }
      else
      {
        Debug.LogError("Invalid line format in CONTROLS section: " + line);
      }
    }

    // Save the list in StaticVar
    StaticVar.ControlsUI = controlValues;

  }
  public void ChangeSettingValue(string _settingName, int newValue)
  {
    if (File.Exists(filePath))
    {
      //Read file
      content = File.ReadAllText(filePath);

      // Divide text in lines
      lines = content.Split('\n');

      for (int i = 0; i < lines.Length; i++)
      {

        wordsInLine = lines[i].Split(' ');

        settingName = wordsInLine[0];

        if (settingName == _settingName)
        {

          lines[i] = _settingName + " " + newValue.ToString();
          break;
        }
      }


      try
      {
        File.WriteAllText(filePath, string.Join("\n", lines));
      }
      catch (Exception ex)
      {
        Debug.LogError("Error al sobrescribir el archivo: " + ex.Message);
      }
    }
    else
    {
      Debug.LogError("Fail to try read the file in the path: " + filePath);
    }
  }


  public void ChangeSettingValue(string _settingName, string newValue)
  {
    if (File.Exists(filePath))
    {
      //Read file
      content = File.ReadAllText(filePath);

      // Divide text in lines
      lines = content.Split('\n');

      for (int i = 0; i < lines.Length; i++)
      {

        wordsInLine = lines[i].Split(' ');

        settingName = wordsInLine[0];

        if (settingName == _settingName)
        {

          lines[i] = _settingName + " " + newValue;
          break;
        }
      }


      try
      {
        File.WriteAllText(filePath, string.Join("\n", lines));
      }
      catch (Exception ex)
      {
        Debug.LogError("Error al sobrescribir el archivo: " + ex.Message);
      }
    }
    else
    {
      Debug.LogError("Fail to try read the file in the path: " + filePath);
    }
  }
}




