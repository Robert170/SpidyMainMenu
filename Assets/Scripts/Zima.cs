using System.Collections;
using UnityEngine;
public class Zima : Enemy
{
  public int characterLevel;                                      //Sets the level of the enemy based on Spidy level

  //-------------------------------------------------------------------------------------------------
  //Method to tint soldier in different color, different life and damage power
  protected override void Start()
  {
    base.Start();
    characterLevel = StaticVar.characterLevel;
    normalAttack.attackDamage = 1;
    currentLife = maxLife;
    normalAttack.attackDamage += characterLevel * 3.0f;
    shadowYPosition = transform.position.y - 18.0f;
    shadowZPosition = transform.position.z - 11.0f;
    currentState = 0;
    specialKnockdown = 0;
    noKnockdown = 100;
    StaticVar.zimaLevel = false;
    ai.enabled = false;                             //Disable the AI
    walker.enabled = false;                         //Disable the walker
    StartCoroutine(InitialWaiting());
  }
  //Method to tint soldier in different color, different life and damage power
  //-------------------------------------------------------------------------------------------------

  public override void Update()
  {
    base.Update();
    if (!StaticVar.zimaLevel)
    {
      ai.enabled = false;                             //Disable the AI
      walker.enabled = false;                         //Disable the walker
    }
    else
    {
      ai.enabled = true;                             //Disable the AI
      walker.enabled = true;                         //Disable the walker
    }
    if (StaticVar.bossReadyToDie)
    {
      baseAnim.SetTrigger("ReadyToDie");
      StaticVar.bossReadyToDie = false;
    }
    if (currentLife < (2.0f / 3.0f * maxLife) && currentLife >= (1.0f / 3.0f * maxLife))
    {
      currentState = 1;
      specialKnockdown = 30;
      noKnockdown = 70;
    }
    else if (currentLife < (1.0f / 3.0f * maxLife))
    {
      currentState = 2;
      specialKnockdown = 60;
      noKnockdown = 40;
    }
    shadowYPosition = transform.position.y - 18.0f;
    shadowZPosition = transform.position.z - 11.0f;
  }

  //-------------------------------------------------------------------------------------------------
  //Overrides the takedamage from actor to not let the gorilla to get knockdown
  public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
  {
    float totalWeight = specialKnockdown + noKnockdown;
    float randomValue = Random.Range(0, totalWeight);
    if (randomValue < specialKnockdown)
    {
      base.TakeDamage(value, hitVector, true);
    }
    else
    {
      base.TakeDamage(value, hitVector, false);
    }
  }
  //Overrides the takedamage from actor to not let the gorilla to get knockdown
  //-------------------------------------------------------------------------------------------------
  protected override void Die()
  {
    StaticVar.zimaLevel = false;
    soundManager.PlaySharedSound(priority, deathClip);
    isAlive = false;
    baseAnim.SetBool("IsAlive", isAlive);
    walker.AddDeadImpulse(frontVector);
    eManager.EraseEnemies(enemy);
  }
  //-------------------------------------------------------------------------------------------------
  //Overrides the coroutine from actor to enable different actions for the soldier
  protected override IEnumerator KnockdownRoutine()
  {
    specialAttack_1_Obj.SetActive(false);
    specialAttack_2_Obj_Right.SetActive(false);
    specialAttack_2_Obj_Left.SetActive(false);
    knockdownAttackObj.SetActive(false);
    isKnockedOut = true;
    baseAnim.SetTrigger("Knockdown");       //Start knockout routine
    ai.enabled = false;                     //Disable the AI
    yield return new WaitForSeconds(2.0f);
    baseAnim.SetTrigger("GetUp");           //Start the getup routine
    ai.enabled = true;                      //Enable the AI
    knockdownRoutine = null;
  }
  //Overrides the coroutine from actor to enable different actions for the soldier
  //-------------------------------------------------------------------------------------------------

  private IEnumerator InitialWaiting()
  {
    yield return new WaitForSecondsRealtime(2f);
    StaticVar.zimaLevel = true;
  }

}
