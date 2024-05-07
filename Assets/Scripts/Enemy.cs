using System.Collections;
using UnityEngine;

public class Enemy : Actor
{
    public GameObject enemy;
    public Walker walker;                               //Reference to the walker script
    public bool stopMovementWhenHit = true;             //Boolean that references when a character is hit and needs to stop movement
    public EnemyAI ai;                                  //Reference to the AI Script
    public GameObject[] Money;                          //List that present the three different ways Money can be collected from an enemy
    public Vector3 moneyPosititon;                      //Sets the position in where the money will appear after an enemy is beaten
    private bool enemyKilled = true;                    //Sets if by gamemanager enemy needs to be killed
    public float yPositionHealthBar;
    public float zPositionHealthBar;
    public EnemyHealthBar healthBar;
    public float behindDistance;
    public float frontDistance;
    public bool attackBehind;
    public bool upper;
    public bool surroundHero;
    public bool canAttack;
    public bool canShoot;
    public Vector3 destinyPoint;
    public EnemyManager eManager;
    public bool isWaiting;
    public bool lookAround;
    public Vector3 corner1;
    public Vector3 corner2;
    public bool isShooter = false;
    public int currentState;
    public float specialKnockdown = 60;
    public float noKnockdown = 40;
    public Vector3 spidyFrontPoint;
    public Vector3 spidyBehindPoint;
    protected override void Start()
    {
        base.Start();
        walker.enabled = false;
        ai.enabled = false;
        StartCoroutine(Flicker());
        healthBar.setLife(currentLife, maxLife);
        eManager = FindObjectOfType<EnemyManager>();
    }
    public override void Update()
    {
        base.Update();
        if ((StaticVar.turnLights || StaticVar.acceptTheKills) && StaticVar.killEnemies && enemyKilled)
        {
            enemyKilled = false;
            Die();
        }
        if(null != healthBar)
        {
            healthBar.setPosition(transform.localPosition, yPositionHealthBar, zPositionHealthBar);
        }
    }
    public void SetHealthBar(ref EnemyHealthBar healthEnemy)
    {
        healthBar = healthEnemy;
    }
    //-------------------------------------------------------------------------------------------------
    //Adds a new enemy to the totalEnemies list
    public void RegisterEnemy()
    {
        StaticVar.TotalEnemies++;
    }
    //Adds a new enemy to the totalEnemies list
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //When a character dies, turns off the walker, reduce the amount of totalenemies by 1 and disable AI. Show money left to collect for Spidy
    protected override void Die()
    {
        base.Die();
        ai.enabled = false;                             //Disable the AI
        walker.enabled = false;                         //Disable the walker
        if (healthBar.gameObject != null)
        {
            Destroy(healthBar.gameObject);
        }
        StaticVar.TotalEnemies--;
        eManager.EraseEnemies(enemy);
        if (StaticVar.acceptTheKills)
        {
            GameObject animator = transform.Find("Animator").gameObject;
            animator.SetActive(false);
            Destroy(walker.gameObject);
        }
        else
        {
            soundManager.PlaySharedSound(priority, deathClip);
            GameObject showMoney;
            moneyPosititon = walker.transform.position;
            moneyPosititon.y += 300;
            showMoney = Instantiate(Money[Random.Range(0, 3)]);
            showMoney.transform.position = moneyPosititon;
            StaticVar.TotalKills++;
            Destroy(walker.gameObject, 3);
        }
    }
    //When a character dies, turns off the walker, reduce the amount of totalenemies by 1 and disable AI. Show money left to collect for Spidy
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Calls for the walker MoveTo Method
    public void MoveTo(Vector3 targetPosition, bool roaming = false)
    {
        walker.MoveTo(targetPosition);
        walker.lookAround = roaming;
    }
    //Calls for the walker MoveTo Method
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Method to stop character's movement
    public void Wait()
    {
        isWaiting = true;
        walker.StopMovement();
    }
    //Method to stop character's movement
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //TakeDamage Method that only adds the syntax so the character won´t move after being hit
    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        if (stopMovementWhenHit)
        {
            walker.StopMovement();
        }
        StartCoroutine(SmallStunDelay());
        base.TakeDamage(value, hitVector, knockdown);
        healthBar.setLife(currentLife, maxLife);
    }
    //TakeDamage Method that only adds the syntax so the character won´t move after being hit
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Method that determines if an actor´s current state allows walking
    public override bool CanWalk()
    {
        return !isHurtAnim && !isKnockedOut && !isAttackingAnim;
    }
    //Method that determines if an actor´s current state allows walking
    //-------------------------------------------------------------------------------------------------

    public void KeepMovement(bool doNotStop)
    {
        walker.keepAnim = doNotStop;
    }
    public virtual void SpecialAttack1()
    {
        baseAnim.SetTrigger("SpecialAttack1");
    }
    public virtual void SpecialAttack2()
    {
        baseAnim.SetTrigger("SpecialAttack2");
    }
    public virtual void RunAttack()
    {
        speed = walkSpeed;
        baseAnim.SetTrigger("RunAttack");
        baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
    }
    private IEnumerator Flicker()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        ai.enabled = true;
        walker.enabled = true;
    }

    private IEnumerator SmallStunDelay()
    {
        smallStun = true;
        currentAttackChain = 0;
        evaluatedAttackChain = 0;
        baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
        baseAnim.SetInteger("CurrentChain", currentAttackChain);
        yield return new WaitForSecondsRealtime(1.5f);
        smallStun = false;
    }
}

//-------------------------------------------------------------------------------------------------
//Configuration of all the possible aspects of an Enemy
public enum EnemyRange
{
    type1 = 0,
    type2 = 1,
    type3 = 2,
    type4 = 3,
    Random = 4
}
//Configuration of all the possible aspects of an Enemy
//-------------------------------------------------------------------------------------------------
