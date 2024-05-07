
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Actor : MonoBehaviour
{
  public float personalSpace;                             //Variable used to allow enemy attack spidy
  public float longSpace;
  public GameObject normalHitSparkPrefab; //This variables loads the effect of a normal hit to the character
  public GameObject strongHitSparkPrefab; //This variables loads the effect of a strong hit to the character
  public GameObject defenseHitSparkPrefab;//This variables loads the effect of a defense hit to the character
  public SpriteRenderer baseSprite;       //Sets the main sprite so program will tilt character for 3 seconds after dying
  public Animator baseAnim;               //Character's animator
  public Rigidbody body;                  //Character's physics
  public SpriteRenderer shadow;           //Character's shadow
  public Vector3 shadowSpritePosition;   //Support variable for the shadow position when character jumps
  public float speed;                     //Main Character speed variable
  public float runSpeed;                  //Actor run speed
  public float walkSpeed;                 //Actor normal walk speed
  public bool isRunning;
  public bool isRunningAttack;
  public float maxLife;                   //Represents the starting max life of a character
  public float currentLife;               //Defines updated life of a character through the game
  public Vector3 frontVector;             //Determines the direction character is facing
  public bool isGrounded;                 //Detects if character is in the air or in the ground
  public bool isAlive;                    //Allows you to know if character is alive or not
  protected Coroutine knockdownRoutine;   //Coroutine to animate knockdown
  public bool isKnockedOut;               //Variable that validates character is knocked down or not
  public AttackData normalAttack;         //Variable data that stores the information of attack of the character
  public AttackData normalAttack2;        //Attack data of the second combo
  public AttackData normalAttack3;        //Attack data of the third combo
  public AttackData specialAttack_1;
  public AttackData specialAttack_2;
  public AttackData knockdownAttack;
  public AttackData runAttack;            //Store Actor run attack value
  public float runAttackForce;            //Dictates how far Actor lunges forward when performing run attack
  private string newAttacker;             //Variable that stores the attacker that is hitting the actor
  private bool noShowDamage;
  public float chainComboTimer;           //Stores the amount of time remaining in the combo chain
  public float chainComboLimit;           //Waiting time to allow enter next hit as normal combo, if not, the hit does not count as combo anymore
  public bool isHurtAnim;                 //Tracks if the current animationis a hurt animation
  public bool isAttackingAnim;            //Boolean that allows you to check if there is an animation of attack started
  public float shadowYPosition;
  public float shadowZPosition;
  public bool jumpingRef;

  //-------------------------------------------------------------------------------------------------
  //Setup for triggering attacks of Character, trigger variables used for each combo or attack played
  public int currentAttackChain = 1;
  public int evaluatedAttackChain = 0;
  //Setup for triggering attacks of Character, trigger variables used for each combo or attack played
  //-------------------------------------------------------------------------------------------------

  public Vector3 groundNormal;
  public AudioSource audioSource;
  public AudioClip deathClip;
  public AudioClip hitClip1;
  public AudioClip hitClip2;
  public AudioClip hitClip3;
  public AudioClip hitClip4;
  public AudioClip hitClip5;
  public AudioClip hitClip6;
  public AudioClip hitClip7;
  public AudioClip hitClip8;
  public AudioClip hitClip9;
  public AudioClip hitClip10;
  public AudioClip hitClip11;
  public AudioClip hitClip12;
  public AudioClip hitClip13;
  public AudioClip jumpClip;
  public AudioClip landClip;
  public AudioClip knockdownClip;
  public AudioClip hurtClip;
  public bool smallStun;
  public GameObject knockdownAttackObj;
  public GameObject specialAttack_2_Obj_Right;
  public GameObject specialAttack_2_Obj_Left;
  public GameObject specialAttack_1_Obj;
  public bool isBulletSpawned;
  public bool spriteDirection;
  public int priority;
  public SoundManager soundManager;
  public float treadmillSpeed = 0;
  public bool isOverTreadmill = false;
  public bool isOverTreadmill2 = false;

  public bool canFlip;



  //-------------------------------------------------------------------------------------------------
  //Setup of new character
  protected virtual void Start()
  {
    currentLife = maxLife;
    isAlive = true;
    baseAnim.SetBool("IsAlive", isAlive);
    isGrounded = true;
    soundManager = FindObjectOfType<SoundManager>();
    canFlip = true;
  }
  //Setup of new character
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Keeps shadow of the character in the floor instead of following character during air when jumping
  public virtual void Update()
  {
    shadowSpritePosition = shadow.transform.position;
    shadowSpritePosition.y = shadowYPosition;
    shadowSpritePosition.z = shadowZPosition;
    shadow.transform.position = shadowSpritePosition;

    isAttackingAnim =
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack2") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack3") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("special_attack1") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("special_attack2");
    isHurtAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt1");

    //-------------------------------------------------------------------------------------------------
    //Code that defines if Character is trying to perform a combo,
    //reduces the time Character has to punch again before leaving the combo time
    if (chainComboTimer > 0)
    {
      chainComboTimer -= Time.deltaTime;

      //-------------------------------------------------------------------------------------------------
      //Once the time has reached 0, this code resets the variables that triggers the combo and the animator
      if (chainComboTimer < 0)
      {
        chainComboTimer = 0;
        currentAttackChain = 0;
        evaluatedAttackChain = 0;
        baseAnim.SetInteger("CurrentChain", currentAttackChain);
        baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
      }
      //Once the time has reached 0, this code resets the variables that triggers the combo and the animator
      //-------------------------------------------------------------------------------------------------

    }
    //Code that defines if Character is trying to perform a combo,
    //reduces the time Character has to punch again before leaving the combo time
    //-------------------------------------------------------------------------------------------------
  }
  //Keeps shadow of the character in the floor instead of following character during air when jumping
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Built-in method for MonoBehaviours, it is called when another collider hits attached collider
  protected virtual void OnCollisionEnter(Collision collision)
  {
    Vector3 contact = collision.GetContact(0).normal;
    //Verifies if the collider is the floor, if it is, it will follow this syntax
    if ((collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Treadmill") || collision.collider.CompareTag("Treadmill2")) && (contact.y > 0))
    {
      if (!isGrounded) soundManager.PlaySoloSound(landClip);
      isGrounded = true;
      baseAnim.SetBool("IsGrounded", isGrounded);     //Sets animator to the land anim of the jump sequence
      DidLand();                                      //Calls for the method DidLand to udpate animator
      jumpingRef = false;
      body.velocity = Vector3.zero;
      groundNormal = collision.contacts[0].normal;
    }
    //Verifies if the collider is the floor, if it is, it will follow this syntax
    //-------------------------------------------------------------------------------------------------
  }
  //Built-in method for MonoBehaviours, it is called when another collider hits attached collider
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Built-in method for MonoBehaviours, it is called when another collider has stopped colliding with it
  protected virtual void OnCollisionExit(Collision collision)
  {
    if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Treadmill") || collision.collider.CompareTag("Treadmill2"))
    {
      if (jumpingRef)
      {
        isGrounded = false;
        baseAnim.SetBool("IsGrounded", isGrounded);
      }
    }
  }
  protected virtual void OnCollisionStay(Collision collision)
  {
    Vector2 contact = collision.GetContact(0).normal;
    if ((collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Treadmill") || collision.collider.CompareTag("Treadmill2")) && (contact.y > 0))
    {
      isGrounded = true;
      baseAnim.SetBool("IsGrounded", isGrounded);
      if (StaticVar.CurrentLevel != 3 || !StaticVar.barrierArea0Level3_2)
      {
        if (collision.collider.CompareTag("Treadmill")) body.velocity = new Vector3(30000, 0, 0) * Time.deltaTime;
        else if (collision.collider.CompareTag("Treadmill2")) body.velocity = new Vector3(-30000, 0, 0) * Time.deltaTime;
      }
      groundNormal = collision.contacts[0].normal;
    }
  }
  //Built-in method for MonoBehaviours, it is called when another collider has stopped colliding with it
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sentence to make character walk back again
  protected virtual void DidLand()
  {
  }
  //Sentence to make character walk back again
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Code that makes character to run and updates animator based on the movement requirements
  public void Run()
  {
    speed = runSpeed;                           //Updates speed character to make him run
    isRunning = true;                           //Updates variable to change animator to running
    baseAnim.SetBool("IsRunning", isRunning);   //Updates animator isRunning variable
    baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
  }
  //Code that makes character to run and updates animator based on the movement requirements
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Flips character left or right, depending the value of isFacingLeft
  public void FlipSprite(bool isFacingLeft)
  {
    spriteDirection = isFacingLeft;
    //Asks if character is facing left
    if (isFacingLeft)
    {
      frontVector = new Vector3(-1, 0, 0);
      transform.localScale = new Vector3(-1, 1, 1);   //Makes character face to the left
    }
    //Asks if character is facing left
    //-------------------------------------------------------------------------------------------------
    //Performs code if character is facing right
    else
    {
      frontVector = new Vector3(1, 0, 0);
      transform.localScale = new Vector3(1, 1, 1);    //Makes character face to the right
    }
    //Performs code if character is facing right
  }
  //Flips character left or right, depending the value of isFacingLeft
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Triggers the Attack variable from the animator
  public virtual void CounterAttack()
  {
    if (!isKnockedOut)
    {
      //-------------------------------------------------------------------------------------------------
      //If there has been a reset on the chainComboTimer, this code will aply and AttackChain will reset
      if (currentAttackChain == 0 || chainComboTimer == 0)
      {
        currentAttackChain = 1;
        evaluatedAttackChain = 0;
      }
      //If there has been a reset on the chainComboTimer, this code will aply and AttackChain will reset
      //-------------------------------------------------------------------------------------------------
      if (currentAttackChain == 1 && evaluatedAttackChain == 0) soundManager.PlaySharedSound(priority, hitClip1);
      else if (currentAttackChain == 2 && evaluatedAttackChain == 1) soundManager.PlaySharedSound(priority, hitClip2);
      else if (currentAttackChain == 3 && evaluatedAttackChain == 2) soundManager.PlaySharedSound(priority, hitClip3);
      baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
      baseAnim.SetInteger("CurrentChain", currentAttackChain);
    }
  }
  //Triggers the Attack variable from the animator
  //-------------------------------------------------------------------------------------------------

  public virtual void NormalAttack()
  {
    if (!isKnockedOut)
    {
      baseAnim.SetTrigger("NormalAttack");
    }
  }

  //-------------------------------------------------------------------------------------------------
  //It is called from the HitForwarder script that a trigger collider occurred,
  //checks if the object that hit has an actor component in it
  public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
  {
    Actor actor = collider.GetComponent<Actor>();
    //-------------------------------------------------------------------------------------------------
    //Check there is an actor in collider hit and this character is alive, if there is,
    //HitActor method is called. Added the option that does not allow actors with same tag to be hit
    if (actor != null && actor.CanBeHit() && collider.gameObject.layer != gameObject.layer)
    {
      if (collider.attachedRigidbody != null)
      {
        HitActor(actor, hitPoint, hitVector);
      }
    }
    //Check there is an actor in collider hit and this character is alive, if there is,
    //HitActor method is called. Added the option that does not allow actors with same tag to be hit
    //-------------------------------------------------------------------------------------------------
  }
  //It is called from the HitForwarder script that a trigger collider occurred,
  //checks if the object that hit has an actor component in it
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Evaluates the completion of an attack coming from Animator's SpidyCallback
  public void DidChain(int chain)
  {
    evaluatedAttackChain = chain;
    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
  }
  //Evaluates the completion of an attack coming from Animator's SpidyCallback
  //-------------------------------------------------------------------------------------------------
  public void DidDie()
  {
    baseAnim.SetBool("IsAlive", true);
    baseAnim.SetTrigger("Waiting");
  }
  //-------------------------------------------------------------------------------------------------
  //Handles the instructions of when this actor hits
  protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
  {
    if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack1"))
    {
      AnalyzeNormalAttack(normalAttack, 2, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack2"))
    {
      AnalyzeNormalAttack(normalAttack2, 3, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("normal_attack3"))
    {
      AnalyzeNormalAttack(normalAttack3, 1, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
    {
      AnalyzeNormalAttack(normalAttack, 0, actor, hitPoint, hitVector);
      soundManager.PlaySharedSound(priority, hitClip1);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("special_attack1"))
    {
      AnalyzeNormalAttack(specialAttack_1, 0, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("special_attack2"))
    {
      AnalyzeNormalAttack(specialAttack_2, 0, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("get_up_attack"))
    {
      AnalyzeNormalAttack(knockdownAttack, 0, actor, hitPoint, hitVector);
    }
    else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("run_attack"))
    {
      AnalyzeNormalAttack(runAttack, 0, actor, hitPoint, hitVector);
    }
  }
  //Handles the instructions of when this actor hits
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method that helps to process the combo of the normal attack
  private void AnalyzeNormalAttack(AttackData attackData, int attackChain, Actor actor, Vector3 hitPoint, Vector3 hitVector)
  {
    actor.EvaluateAttackData(attackData, hitVector, hitPoint);                       //Same as AnalizeSpecialAttack
    currentAttackChain = attackChain;                                                //Updates variable according to the combo being played
    chainComboTimer = chainComboLimit;                                               //Set the time for the player to attack before losses opportunity to play normal combo
  }
  //Method that helps to process the combo of the normal attack
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Verify the character is still alive
  public bool CanBeHit()
  {
    return (isAlive && !isKnockedOut) || !canFlip;    //Checks if actor is alive and is not knockdown to be able to be hit again
  }
  //Verify the character is still alive
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Makes character to flip direction to where the hit came from and reduce character´s life,
  //if it is less than 0, goes to the method Die, if not, updates the animator with the hurt animation
  public virtual void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
  {
    if (canFlip) FlipSprite(hitVector.x > 0);
    currentLife -= value;
    if (isAlive && currentLife <= 0)
    {
      if (newAttacker == "Spidy(Clone)")
      {
        StaticVar.levelExperience += 1;
        
      }
      Die();
    }
    //-------------------------------------------------------------------------------------------------
    //Initiate coroutine if character receives an attack that causes a knockdown
    else if (knockdown && canFlip)
    {
      
      if (knockdownRoutine == null)
      {
        Vector3 pushbackVector = (hitVector + Vector3.up * 0.75f).normalized;
        body.AddForce(pushbackVector * 250);
        soundManager.PlaySharedSound(priority, knockdownClip);
        knockdownRoutine = StartCoroutine(KnockdownRoutine());
      }
    }
    //Initiate coroutine if character receives an attack that causes a knockdown
    //-------------------------------------------------------------------------------------------------
    else
    {
      if (!noShowDamage)
      {
        if (CompareTag("Spidy")) soundManager.PlaySoloSound(hurtClip);
        else soundManager.PlaySharedSound(priority, hurtClip);
        if (canFlip && !StaticVar.transformed) baseAnim.SetTrigger("IsHurt");
      }
      else
      {
        noShowDamage = false;
        baseAnim.SetTrigger("DefenseHurt");
      }
    }
  }
  //Makes character to flip direction t where the hit came from and reduce character´s life,
  //if it is less than 0, goes to the method Die, if not, updates the animator with the hurt animation
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //This code process the attack, also applies the pushback force
  public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint, string attacker = null)
  {
    newAttacker = attacker;
    //-------------------------------------------------------------------------------------------------
    //Normal attack sequence, if character did not have the defense up
    if (canFlip) body.AddForce(data.force * hitVector * 100);
    if (data.knockdown)
    {
      StaticVar.shakeCamera = true;
    }
    TakeDamage(data.attackDamage, hitVector, data.knockdown);
    ShowHitEffects(data.attackDamage, hitPoint, data.knockdown);
    //Normal attack sequence, if character did not have the defense up
    //-------------------------------------------------------------------------------------------------
  }
  //This code process the attack, also applies the pushback force
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method that determines if an actor´s current state allows walking
  public virtual bool CanWalk()
  {
    return !isHurtAnim && !isAttackingAnim && !smallStun;
  }
  //Method that determines if an actor´s current state allows walking
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Changes the direction an actor faces based on its target point
  public virtual void FaceTarget(Vector3 targetPoint)
  {
    if (canFlip) FlipSprite(transform.position.x - targetPoint.x > 0);
  }
  //Changes the direction an actor faces based on its target point
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Calls and sets to false as reference that character has finally got up
  public void DidGetUp()
  {
    isKnockedOut = false;
  }
  //Calls and sets to false as reference that character has finally got up
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Sets the process of the knockdown and getup of the character
  protected virtual IEnumerator KnockdownRoutine()
  {
    isKnockedOut = true;
    baseAnim.SetTrigger("Knockdown");               //Sets the animator to the knockdown process
    yield return new WaitForSecondsRealtime(1.5f);  //After knockdown is triggered, wait for one second
    baseAnim.SetTrigger("GetUp");                   //Start the getup process
    knockdownRoutine = null;
  }
  //Sets the process of the knockdown and getup of the character
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Performs the method to show the particles effect when a character is hit
  protected void ShowHitEffects(float value, Vector3 position, bool knockdown)
  {
    GameObject sparkObj;
    if (knockdown)
    {
      sparkObj = Instantiate(strongHitSparkPrefab);

    }
    else
    {
      sparkObj = Instantiate(normalHitSparkPrefab);

    }
    sparkObj.transform.position = position;
  }
  //Performs the method to show the particles effect when a character is hit
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Die animation
  protected virtual void Die()
  {
    //-------------------------------------------------------------------------------------------------
    //This code stops any knockdown coroutine when the character dies
    if (knockdownRoutine != null)
    {
      StopCoroutine(knockdownRoutine);
    }
    //This code stops any knockdown coroutine when the character dies
    //-------------------------------------------------------------------------------------------------
    isAlive = false;
    baseAnim.SetBool("IsAlive", isAlive);
    StartCoroutine(DeathFlicker());
  }
  //Die animation
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method used to flicker character once that he is dead
  protected virtual void SetOpacity(float value)
  {
    Color color = baseSprite.color;
    color.a = value;
    baseSprite.color = color;
  }
  //Method used to flicker character once that he is dead
  //-------------------------------------------------------------------------------------------------

  //-------------------------------------------------------------------------------------------------
  //Method coroutine instructions that uses SetOpacity to make character flicker
  private IEnumerator DeathFlicker()
  {
    int i = 5;
    while (i > 0)
    {
      SetOpacity(0.5f);
      yield return new WaitForSeconds(0.1f);
      SetOpacity(1.0f);
      yield return new WaitForSeconds(0.1f);
      i--;
    }
  }
  //Method coroutine instructions that uses SetOpacity to make character flicker
  //-------------------------------------------------------------------------------------------------

  private IEnumerator RampDelay()
  {
    yield return new WaitForFixedUpdate();
    if (jumpingRef)
    {
      isGrounded = false;
      baseAnim.SetBool("IsGrounded", isGrounded);

    }
  }

}

//-------------------------------------------------------------------------------------------------
//Attack data information for every character
[System.Serializable]
public class AttackData
{
  public float attackDamage;
  public float force;
  public bool knockdown;
}
//Attack data information for every character
//-------------------------------------------------------------------------------------------------
