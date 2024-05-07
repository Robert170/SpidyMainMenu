using System.Collections;
using UnityEngine;
public class Soldier : Enemy
{
  public EnemyRange typeRange;                                    //Sets the strength of the enemy
  public int characterLevel;                                      //Sets the level of the enemy based on Spidy level
  private float nRed;
  private float nGreen;
  private float nBlue;

  //-------------------------------------------------------------------------------------------------
  //Method to tint soldier in different color, different life and damage power
  public void SetRange(EnemyRange typeRange)
  {
    characterLevel = StaticVar.characterLevel;
    this.typeRange = typeRange;
    switch (typeRange)
    {
      case EnemyRange.type1:
        maxLife = 100.0f;
        normalAttack.attackDamage = 1;
        break;

      case EnemyRange.type2:
        nRed = 173.0f / 255.0f;
        nGreen = 166.0f / 255.0f;
        nBlue = 166.0f / 255.0f;
        baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
        maxLife = 150.0f;
        normalAttack.attackDamage = 4;
        break;

      case EnemyRange.type3:
        nRed = 176.0f / 255.0f;
        nGreen = 160.0f / 255.0f;
        nBlue = 147.0f / 255.0f;
        baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
        maxLife = 200.0f;
        normalAttack.attackDamage = 5;
        break;

      case EnemyRange.type4:
        nRed = 171.0f / 255.0f;
        nGreen = 159.0f / 255.0f;
        nBlue = 164.0f / 255.0f;
        baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
        maxLife = 250.0f;
        normalAttack.attackDamage = 6;
        break;

      case EnemyRange.Random:
        nRed = 169.0f / 255.0f;
        nGreen = 159.0f / 255.0f;
        nBlue = 144.0f / 255.0f;
        baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
        maxLife = Random.Range(100, 300);
        normalAttack.attackDamage = Random.Range(4, 10);
        break;
    }
    currentLife = maxLife;
    normalAttack.attackDamage += characterLevel * 0.3f;
    shadowYPosition = transform.position.y - 21.0f;
    shadowZPosition = transform.position.z - 13.0f;

  }
  //Method to tint soldier in different color, different life and damage power
  //-------------------------------------------------------------------------------------------------

  public override void Update()
  {
    base.Update();
    shadowYPosition = transform.position.y - 21.0f;
    shadowZPosition = transform.position.z - 13.0f;
  }

  //-------------------------------------------------------------------------------------------------
  //ContextMenu attributes when selecting the attributes of the soldier,
  //calls for the SoldierColor option and then calls for SetColor
  [ContextMenu("Type Range: Type1")]
  void SetToType1()
  {
    SetRange(EnemyRange.type1);
  }

  [ContextMenu("Type Range: Type2")]
  void SetToType2()
  {
    SetRange(EnemyRange.type2);
  }

  [ContextMenu("Type Range: Type3")]
  void SetToType3()
  {
    SetRange(EnemyRange.type3);
  }

  [ContextMenu("Type Range: Type4")]
  void SetToType4()
  {
    SetRange(EnemyRange.type4);
  }

  [ContextMenu("Type Range: Random")]
  void SetToRandom()
  {
    SetRange(EnemyRange.Random);
  }
  //ContextMenu attributes when selecting the attributes of the soldier,
  //calls for the SoldierColor option and then calls for SetColor
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Overrides the coroutine from actor to enable different actions for the soldier
  protected override IEnumerator KnockdownRoutine()
  {
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

}
