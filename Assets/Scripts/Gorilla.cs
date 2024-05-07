using System.Collections;
using UnityEngine;
public class Gorilla : Enemy
{
    public EnemyRange typeRange;                                    //Sets the strength of the enemy
    public int characterLevel;                                      //Sets the level of the enemy based on Spidy level
    private float nRed;
    private float nGreen;
    private float nBlue;
    private bool readyToLeave = false;

    public override void Update()
    {
        base.Update();
        if (walker.currentSpeed == 0 && readyToLeave)
        {
            StartCoroutine(GorillaLeaving());
        }
        shadowYPosition = transform.position.y - 16.0f;
        shadowZPosition = transform.position.z - 9.0f;
    }

    //-------------------------------------------------------------------------------------------------
    //Method to tint Gorilla in different color, different life and damage power
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
                //nRed = 235.0f / 255.0f;
                //nGreen = 123.0f / 255.0f;
                //nBlue = 44.0f / 255.0f;
                //baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
                maxLife = 150.0f;
                normalAttack.attackDamage = 4;
                break;
            case EnemyRange.type3:
                //nRed = 90.0f / 255.0f;
                //nGreen = 49.0f / 255.0f;
                //nBlue = 27.0f / 255.0f;
                //baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
                maxLife = 200.0f;
                normalAttack.attackDamage = 5;
                break;
            case EnemyRange.type4:
                //nRed = 144.0f / 255.0f;
                //nGreen = 128.0f / 255.0f;
                //nBlue = 120.0f / 255.0f;
                //baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
                maxLife = 250.0f;
                normalAttack.attackDamage = 6;
                break;
            case EnemyRange.Random:
                //nRed = 51.0f / 255.0f;
                //nGreen = 64.0f / 255.0f;
                //nBlue = 79.0f / 255.0f;
                //baseSprite.color = new Color(nRed, nGreen, nBlue, 1.0f);
                maxLife = Random.Range(100, 300);
                normalAttack.attackDamage = Random.Range(4, 10);
                break;
        }
        currentLife = maxLife;
        normalAttack.attackDamage += characterLevel * 0.3f;
        shadowYPosition = transform.position.y - 16.0f;
        shadowZPosition = transform.position.z - 10.0f;
    }
    //Method to tint Gorilla in different color, different life and damage power
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //ContextMenu attributes when selecting the attributes of the gorilla,
    //calls for the GorillaColor option and then calls for SetColor
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
    //ContextMenu attributes when selecting the attributes of the Gorilla,
    //calls for the GorillaColor option and then calls for SetColor
    //-------------------------------------------------------------------------------------------------

    protected override void Die()
    {
        isAlive = false;
        baseAnim.SetBool("IsAlive", isAlive);
        ai.enabled = false;                             //Disable the AI
        walker.enabled = false;                         //Disable the walker
        StaticVar.TotalEnemies--;
        if (StaticVar.acceptTheKills)
        {
            GameObject animator = transform.Find("Animator").gameObject;
            animator.SetActive(false);
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
        }
        if (healthBar.gameObject != null)
        {
            Destroy(healthBar.gameObject);
        }
        StartCoroutine(GorillaRun());
        eManager.EraseEnemies(enemy);
    }
    public override bool CanWalk()
    {
        bool keepWalking;
        if (!ai.enabled)
        {
            keepWalking = true;
        }
        else
        {
            keepWalking = !isHurtAnim && !isKnockedOut && !isAttackingAnim;
        }
        return keepWalking;
    }

    //-------------------------------------------------------------------------------------------------
    //Overrides the takedamage from actor to not let the gorilla to get knockdown
    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        base.TakeDamage(value, hitVector, false);
    }
    //Overrides the takedamage from actor to not let the gorilla to get knockdown
    //-------------------------------------------------------------------------------------------------

    protected virtual IEnumerator GorillaRun()
    {
        yield return new WaitForSeconds(0.2f);
        baseAnim.SetBool("IsAlive", true);
        yield return new WaitForSeconds(1.0f);
        walker.enabled = true;                         //Enable the walker
        speed = 800;
        Vector3 neededPosition;
        if (StaticVar.CurrentLevel == 3)
        {
            if (transform.position.y <= 1600.0f)
            {
                if (transform.position.x <= 3440)
                {
                    neededPosition = StaticVar.doors[0];
                }
                else
                {
                    neededPosition = StaticVar.doors[3];
                }
            }
            else
            {
                if (transform.position.x <= 2000)
                {
                    neededPosition = StaticVar.doors[2];
                }
                else
                {
                    neededPosition = StaticVar.doors[1];
                }
            }
        }
        else
        {
            neededPosition = Vector3.zero;
            for (int i = 0; i < 4; i++)
            {
                if (Mathf.Abs(Mathf.Abs(transform.position.y - StaticVar.doors[i].y) - Mathf.Abs(transform.position.y - neededPosition.y)) <= 10)
                {
                    if (Vector3.Distance(transform.position, StaticVar.doors[i]) <= Vector3.Distance(transform.position, neededPosition))
                    {
                        neededPosition = StaticVar.doors[i];
                    }
                }
                else if (Mathf.Abs(transform.position.y - StaticVar.doors[i].y) <= 100)
                {
                    neededPosition = StaticVar.doors[i];
                }
            }
        }
        surroundHero = false;
        walker.MoveTo(neededPosition, null, true);
        yield return new WaitForSeconds(0.5f);
        readyToLeave = true;
    }
    protected virtual IEnumerator GorillaLeaving()
    {
        baseAnim.SetTrigger("IsLeaving");
        yield return new WaitForSeconds(0.5f);
        Destroy(walker.gameObject);
    }
}
