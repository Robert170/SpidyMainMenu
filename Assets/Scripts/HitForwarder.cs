using System.Collections;
using UnityEngine;
public class HitForwarder : MonoBehaviour
{

  //-------------------------------------------------------------------------------------------------
  //Code that helps character to detect a hit has occurred
  public Actor actor;                                 //Reference to the actor script
  public Collider triggerCollider;                    //Reference to the character collider

  //-------------------------------------------------------------------------------------------------
  //A method to handle trigger events using the hitCollider variable, estimates the start point of collision and direction
  void OnTriggerEnter(Collider hitCollider)
  {

    if (hitCollider.gameObject.CompareTag("Spidy") && StaticVar.transformed)
    {
      return;
    }
    //-------------------------------------------------------------------------------------------------
    //First make a check if defense is triggered so character being hit reduces attack to 1/4
    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Defense"))
    {
      GameObject objSecondLevel = hitCollider.transform.parent.gameObject;
      GameObject objFirstLevel = objSecondLevel.transform.parent.gameObject;
      StaticVar.defenseUp = true;
      StaticVar.actorScale = actor.transform.localScale.x;            //Check if attacking character is facing left or right
      StaticVar.hitActorScale = objFirstLevel.transform.localScale.x; //Check if character attacked is facing left or right
    }
    //First make a check if defense is triggered so character being hit reduces attack to 1/4
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //After checking if character has defense, the normal attack follows
    else
    {
      Vector3 direction = new Vector3(hitCollider.transform.position.x - actor.transform.position.x, 0, 0);
      direction.Normalize();
      BoxCollider collider = triggerCollider as BoxCollider;
      Vector3 centerPoint = this.transform.position;
      if (collider)
      {
        centerPoint = transform.TransformPoint(collider.center);
      }
      Vector3 startPoint = hitCollider.ClosestPointOnBounds(centerPoint);
      actor.DidHitObject(hitCollider, startPoint, direction);
    }
    //After checking if character has defense, the normal attack follows
    //-------------------------------------------------------------------------------------------------
  }
  //A method to handle trigger events using the hitCollider variable, estimates the start point of collision and direction
  //-------------------------------------------------------------------------------------------------

  //Code that helps character to detect a hit has occurred
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Normal process to validate a normal attack
  protected IEnumerator DelayAttack(Collider hitCollider)
  {
    yield return null;                              //Makes a frame delay to check if defense has been triggered
    Vector3 direction = new Vector3(hitCollider.transform.position.x - actor.transform.position.x, 0, 0);
    direction.Normalize();
    BoxCollider collider = triggerCollider as BoxCollider;
    Vector3 centerPoint = this.transform.position;
    if (collider)
    {
      centerPoint = transform.TransformPoint(collider.center);

    }
    Vector3 startPoint = hitCollider.ClosestPointOnBounds(centerPoint);
    actor.DidHitObject(hitCollider, startPoint, direction);

  }
  //Normal process to validate a normal attack
  //-------------------------------------------------------------------------------------------------

}
