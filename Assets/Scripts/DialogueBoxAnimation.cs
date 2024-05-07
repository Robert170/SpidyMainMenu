using UnityEngine;

public class DialogueBoxAnimation : MonoBehaviour
{
    public UIDialogue UIDialogue;
  public void StartAnimation()
  {
    UIDialogue.DidStartText();
  }
}
