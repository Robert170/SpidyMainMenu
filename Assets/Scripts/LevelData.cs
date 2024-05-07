using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Spidy Adventure/LevelData")]
public class LevelData : ScriptableObject
{
    public List<Level> level;
}

[Serializable]
public class Level
{
	public string levelName;                //Sets the level name
	public bool firstTimeAccess;            //Defines if it is first time character passes over scenario
	public bool newEnemy;                   //Defines if in this scenario, there is a new enemy to appear, only set to true before entering the firts time, then it set to zero and does not change after that
	public GameObject levelPrefab;
    public List<Column> mapLocation;
	public bool cinematic;
    public DialogBoxes[] dialogueBoxes;            //Setup of variables related to the dialog box and the text
}

[Serializable]
public class Column
{
	public int xLocation;
	public int yLocation;
}

//--------------------------------------------------------------------------------
//Setup of variables related to the dialog box and the text
[System.Serializable]
public struct DialogBoxes
{
    public int boxCharacter;
    [SerializeField, TextArea(4, 6)] public string[] dialogueLines;
    public bool stopText;
}
//Setup of variables related to the dialog box and the text
//--------------------------------------------------------------------------------
