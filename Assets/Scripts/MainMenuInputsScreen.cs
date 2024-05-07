using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuInputsScreen : MonoBehaviour
{
  void OnEnable()
  {
    Keyboard.current.onTextInput += GetKeyInput;
  }

  void OnDisable()
  {
    Keyboard.current.onTextInput -= GetKeyInput;
  }

  private void GetKeyInput(char obj)
  {
    if (StaticVar.waitingNewInput && StaticVar.newInputBinding == " ")
    {
      //UnityEngine.Debug.Log(obj);
      StaticVar.newInputBinding = obj.ToString();
    }

  }

  void Update()
  {
    if (StaticVar.waitingNewInput && StaticVar.newInputBinding == " ")
    {
      
      if (Keyboard.current[Key.UpArrow].wasPressedThisFrame)
      {
        //Debug.Log("Up Arrow Pressed");
        StaticVar.newInputBinding = "UpArrow";
      }
      else if (Keyboard.current[Key.DownArrow].wasPressedThisFrame)
      {
        //Debug.Log("Down Arrow Pressed");
        StaticVar.newInputBinding = "DownArrow";
      }
      else if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame)
      {
        //Debug.Log("Left Arrow Pressed");
        StaticVar.newInputBinding = "LeftArrow";
      }
      else if (Keyboard.current[Key.RightArrow].wasPressedThisFrame)
      {
        //Debug.Log("Right Arrow Pressed");
        StaticVar.newInputBinding = "RightArrow";
      }

      
    }
  }
}
