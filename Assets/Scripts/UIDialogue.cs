using System.Collections;
using UnityEngine;
using TMPro;
//using UnityEngine.InputSystem;
public class UIDialogue : MonoBehaviour
{
    //--------------------------------------------------------------------------------
    //variable to detect input

    public InputHandler input;
    public LevelData levelData;
    private int lineIndex;                        //Variable to know which text line is writing 
    private int dialogueBoxIndex;                 //Variable to know which dialog box is writing
    private int boxCharacter;
    private float typingTime = 0.05f;             //Variable for text writing speed
    public DialogueBoxes[] dialogueBoxes;         //Setup of variables related to the dialog box and the text

    //-------------------------------------------------------------------------------------------------
    //Enable the input
    public void Start()
    {
        dialogueBoxIndex = 0;
    }
    //Enable the input
    //-------------------------------------------------------------------------------------------------

    public void DidDialogueStart()
    {
        boxCharacter = levelData.level[StaticVar.CurrentLevel].dialogueBoxes[dialogueBoxIndex].boxCharacter;
        dialogueBoxes[boxCharacter].boxDialogue.SetActive(true);
        dialogueBoxes[boxCharacter].dialogueBoxAnim.SetBool("Appear", true);
        lineIndex = 0;                                                  //Set 0 to start with the first dialog line
        Time.timeScale = 0.0f;                                          // Set in 0 to stop the game
    }
    public void DidStartText()
    {
        StartCoroutine(ShowLineText());
    }

    //--------------------------------------------------------------------------------
    //Code that start show the dialog
    private IEnumerator ShowLineText()
    {
        dialogueBoxes[boxCharacter].dialogueText.text = string.Empty;
        foreach (char ch in levelData.level[StaticVar.CurrentLevel].dialogueBoxes[dialogueBoxIndex].dialogueLines[lineIndex])        //Loop that goes through the entire dialog to display it
        {
            dialogueBoxes[boxCharacter].dialogueText.text += ch;                                                                     //The variable is filled character by character
            yield return new WaitForSecondsRealtime(typingTime);
        }
        while (!input.GetAttackButtonDown())
        {
            yield return null;
        }
        lineIndex++;
        if (lineIndex < levelData.level[StaticVar.CurrentLevel].dialogueBoxes[dialogueBoxIndex].dialogueLines.Length)
        {
            DidStartText();
        }
        else
        {
            dialogueBoxes[boxCharacter].dialogueText.text = string.Empty;
            dialogueBoxes[boxCharacter].dialogueBoxAnim.SetBool("Appear", false);
            StartCoroutine(RemovingDialogueBox());
        }
    }
    //Code that start show the dialog
    //--------------------------------------------------------------------------------

    private IEnumerator RemovingDialogueBox()
    {
        yield return new WaitForSecondsRealtime(0.35f);
        dialogueBoxes[boxCharacter].boxDialogue.SetActive(false);
        dialogueBoxIndex++;
        if (dialogueBoxIndex < levelData.level[StaticVar.CurrentLevel].dialogueBoxes.Length && !levelData.level[StaticVar.CurrentLevel].dialogueBoxes[dialogueBoxIndex].stopText)
        {
            DidDialogueStart();
        }
        else
        {
            Time.timeScale = 1.0f;
            StaticVar.waitingMusic = false;
            if (dialogueBoxIndex >= dialogueBoxes.Length)
            {
                dialogueBoxIndex = 0;
            }
        }
    }

    //--------------------------------------------------------------------------------
    //Setup of variables related to the dialog box and the text
    [System.Serializable]
    public struct DialogueBoxes
    {
        public GameObject boxDialogue;
        [SerializeField] public Animator dialogueBoxAnim;
        [SerializeField] public TMP_Text dialogueText;
    }
    //Setup of variables related to the dialog box and the text
    //--------------------------------------------------------------------------------

}
