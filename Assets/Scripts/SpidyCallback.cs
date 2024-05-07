using UnityEngine;
public class SpidyCallback : MonoBehaviour
{
  public Spidy spidy;

  //-------------------------------------------------------------------------------------------------
  //Script used to forward animator callback when a jump attack is complete,
  //this way character knows a jump attack is complete. It is triggered at the beginning of the jump attack animation
  public void DidJumpAttack()
  {
    spidy.DidJumpAttack();
  }
  //Script used to forward animator callback when a jump attack is complete,
  //this way character knows a jump attack is complete. It is triggered at the beginning of the jump attack animation
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Callback when Spidy completes transformation so it can follow the stand position after transformed
  public void DidTransform()
  {
    spidy.firstTransformation = false;
    spidy.returnTransformation = false;
    if (!StaticVar.transformed)
    {
      StaticVar.transformed = true;
      spidy.waitToRedPow = true;
    }
    else
    {
      //StaticVar.transformed = false;
    }
  }
  //Callback when Spidy completes transformation so it can follow the stand position after transformed
  //-------------------------------------------------------------------------------------------------

  public void DidFinishTransform()
  {
    spidy.firstTransformation = false;
    spidy.returnTransformation = false;
    StaticVar.transformed = false;
    
  }
  public void SpidyDidDie()
  {
    Time.timeScale = 0.0f;
    spidy.deathAnimFinish = true;
  }
}
