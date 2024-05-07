using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

//-------------------------------------------------------------------------------------------------
//Creating dependency to the Enemy Script
[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------------
    //List details of what are the potential actions the character can perform from this script
    public enum EnemyAction
    {
        None,
        Wait,
        NormalAttack,
        CounterAttack,
        SpecialAttack1,
        SpecialAttack2,
        RunAttack,
        Chase,
        Run,
        Roam
    }
    //List details of what are the potential actions the character can perform from this script
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Stores data for the decision weight randomizer, contains the weight and the action
    public class DecisionWeight
    {
        public int weight;
        public EnemyAction action;
        public DecisionWeight(int weight, EnemyAction action)
        {
            this.weight = weight;
            this.action = action;
        }
    }
    //Stores data for the decision weight randomizer, contains the weight and the action
    //-------------------------------------------------------------------------------------------------

    private Enemy enemy;
    GameObject spidyObj;                                    //Reference to Spidy
    public float attackReachMin;                            //The minimum reach of the enemy to attack
    public float attackReachMax;                            //The maximum reach of the enemy to attack
    public SpidyDetector detector;                          //Reference to the SpidyDetector script
    List<DecisionWeight> weights;                           //List of possible actions foun in EnemyAction class
    public EnemyAction currentAction = EnemyAction.None;    //Reference to the action the enemy is currently performing
    private float sqrDistance;                              //Vector between character and Spidy
    private float decisionDuration;                         //The time AI needs to wait between decisions
    private float randomDegree;                             //Random angle used to move character roamly
    private int total;                                      //Used to calculate the summ of all values to give weight
    private int intDecision;                                //Counter to lead to the final decision of the system
    private bool canReach;                                  //Defines if Character is close to Spidy to attack
    private bool samePlane;                                 //Defines if character is in the same Z plane to attack Spidy
    private Vector3 otherDirectionVector;                   //Sets the distance used to take when character is roaming
    Vector3 presentDistance;
    Vector3 pastDistance;

    public EnemyManager eManager;
    public GameManager gameManager;

    //-------------------------------------------------------------------------------------------------
    //Setup of the weights, find the enemy component associated to the script and look for Spidy GameObject
    void Start()
    {
        weights = new List<DecisionWeight>();
        enemy = GetComponent<Enemy>();
        spidyObj = GameObject.FindGameObjectWithTag("Spidy");
        eManager = FindObjectOfType<EnemyManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    //Setup of the weights, find the enemy component associated to the script and look for Spidy GameObject
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //Method that makes character to enter into stand state
    private void Wait()
    {
        decisionDuration = Random.Range(0.2f, 0.5f);
        if (enemy.smallStun)
        {
            decisionDuration = Random.Range(0.8f, 1f);
        }
        enemy.Wait();
    }
    //Method that makes character to enter into stand state
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Method that makes character to punch Spidy
    private void NormalAttack()
    {
        enemy.Wait();
        enemy.smallStun = false;
        enemy.FaceTarget(spidyObj.transform.position);
        enemy.NormalAttack();
        decisionDuration = Random.Range(0.5f, 1.0f);
    }
    //Method that makes character to punch Spidy
    //-------------------------------------------------------------------------------------------------
    private void CounterAttack()
    {
        enemy.Wait();
        enemy.smallStun = false;
        enemy.FaceTarget(spidyObj.transform.position);
        enemy.CounterAttack();
        if (enemy.currentAttackChain == 1 && enemy.evaluatedAttackChain == 3) decisionDuration = Random.Range(0.8f, 1.0f);
        else decisionDuration = Random.Range(0.2f, 0.6f);
    }
    private void SpecialAttack1()
    {
        enemy.Wait();
        enemy.smallStun = false;
        enemy.FaceTarget(spidyObj.transform.position);
        enemy.SpecialAttack1();
        decisionDuration = Random.Range(0.5f, 1.0f);
    }
    private void SpecialAttack2()
    {
        enemy.Wait();
        enemy.smallStun = false;
        enemy.FaceTarget(spidyObj.transform.position);
        enemy.SpecialAttack2();
        decisionDuration = Random.Range(0.5f, 1.0f);
    }
    private void RunAttack()
    {
        enemy.isRunning = false;
        enemy.Wait();
        enemy.smallStun = false;
        enemy.FaceTarget(spidyObj.transform.position);
        enemy.RunAttack();
        decisionDuration = Random.Range(0.5f, 1.0f);
    }
    //-------------------------------------------------------------------------------------------------
    //Chase Method, instructions to chase Spidy
    private void Chase()
    {
        enemy.smallStun = false;
        enemy.isWaiting = false;
        randomDegree = Random.Range(0, 360);
        Vector2 offset = new Vector2(Mathf.Sin(randomDegree), Mathf.Cos(randomDegree));
        float distance = Random.Range(0, 50);
        offset *= distance;
        enemy.MoveTo(new Vector3(enemy.destinyPoint.x + offset.x, enemy.destinyPoint.y, enemy.destinyPoint.z + offset.y));
        decisionDuration = Random.Range(0.1f, 0.3f);
    }
    //Chase Method, instructions to chase Spidy
    //-------------------------------------------------------------------------------------------------
    private void Run()
    {
        if (enemy.isRunningAttack)
        {
            enemy.isRunningAttack = false;
            enemy.baseAnim.SetBool("RunAttack 0", enemy.isRunningAttack);
        }
        enemy.isRunning = true;
        enemy.smallStun = false;
        enemy.isWaiting = false;
        randomDegree = Random.Range(0, 360);
        Vector2 offset = new Vector2(Mathf.Sin(randomDegree), Mathf.Cos(randomDegree));
        float distance = Random.Range(0, 50);
        offset *= distance;
        enemy.Run();
        enemy.MoveTo(new Vector3(enemy.destinyPoint.x + offset.x, enemy.destinyPoint.y, enemy.destinyPoint.z + offset.y));
        decisionDuration = Random.Range(0.1f, 0.3f);
    }
    //-------------------------------------------------------------------------------------------------
    //Method that makes randomly move character in the area
    private void Roam()
    {
        enemy.isWaiting = false;
        enemy.smallStun = false;
        randomDegree = Random.Range(0, 360);
        Vector2 offset = new Vector2(Mathf.Sin(randomDegree), Mathf.Cos(randomDegree));
        float distance = Random.Range(10, 250);
        offset *= distance;
        otherDirectionVector = new Vector3(offset.x, 0, offset.y);
        if (detector.spidyIsNearby)
        {
            float dirFromSpidyX = enemy.transform.position.x - spidyObj.transform.position.x;
            float dirFromSpidyZ = enemy.transform.position.z - spidyObj.transform.position.z;
            if (dirFromSpidyX < 300 && dirFromSpidyX > 0) otherDirectionVector.x = Mathf.Abs(otherDirectionVector.x);
            else if (dirFromSpidyX > -300 && dirFromSpidyX < 0) otherDirectionVector.x = Mathf.Abs(otherDirectionVector.x) * -1.0f;
            if (dirFromSpidyZ < 300 && dirFromSpidyZ > 0) otherDirectionVector.z = Mathf.Abs(otherDirectionVector.z);
            else if (dirFromSpidyZ > -300 && dirFromSpidyZ < 0) otherDirectionVector.z = Mathf.Abs(otherDirectionVector.z) * -1.0f;
            enemy.MoveTo(enemy.transform.position + otherDirectionVector, false);
        }
        else enemy.MoveTo(enemy.transform.position + otherDirectionVector, true);
        decisionDuration = Random.Range(0.3f, 0.6f);
    }
    //Method that makes randomly move character in the area
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Method used to add and calculate each one of the action the character can perform
    private void DecideWithWeights(int wait = 0,
        int normalAttack = 0,
        int counterAttack = 0,
        int specialAttack1 = 0,
        int specialAttack2 = 0,
        int runAttack = 0,
        int chase = 0,
        int run = 0,
        int move = 0)
    {
        //-------------------------------------------------------------------------------------------------
        //Calculate the weighting of each action and if decides if it should add or no the action
        weights.Clear();

        if (wait > 0)
        {
            weights.Add(new DecisionWeight(wait, EnemyAction.Wait));
        }
        if (normalAttack > 0)
        {
            weights.Add(new DecisionWeight(normalAttack, EnemyAction.NormalAttack));
        }
        if (counterAttack > 0)
        {
            weights.Add(new DecisionWeight(counterAttack, EnemyAction.CounterAttack));
        }
        if (specialAttack1 > 0)
        {
            weights.Add(new DecisionWeight(specialAttack1, EnemyAction.SpecialAttack1));
        }
        if (specialAttack2 > 0)
        {
            weights.Add(new DecisionWeight(specialAttack2, EnemyAction.SpecialAttack2));
        }
        if (runAttack > 0)
        {
            weights.Add(new DecisionWeight(runAttack, EnemyAction.RunAttack));
        }
        if (chase > 0)
        {
            weights.Add(new DecisionWeight(chase, EnemyAction.Chase));
        }
        if (run > 0)
        {
            weights.Add(new DecisionWeight(run, EnemyAction.Run));
        }
        if (move > 0)
        {
            weights.Add(new DecisionWeight(move, EnemyAction.Roam));
        }
        //Calculate the weighting of each action and if decides if it should add or no the action
        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------
        //Defining a random index then substract from the random index value untill is less or equal to zero
        //then calls SetDecision method
        total = wait + normalAttack + counterAttack + specialAttack1 + specialAttack2 + runAttack + chase + run + move;
        intDecision = Random.Range(0, total - 1);
        foreach (DecisionWeight weight in weights)
        {
            intDecision -= weight.weight;
            if (intDecision <= 0)
            {
                SetDecision(weight.action);
                break;
            }
        }
        //Defining a random index then substract from the random index value untill is less or equal to zero
        //then calls SetDecision method
        //-------------------------------------------------------------------------------------------------
    }
    //Method used to add and calculate each one of the action the character can perform
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //Once a decision is taken, this method is called to perform the activity on that decision taken
    private void SetDecision(EnemyAction action)
    {
        currentAction = action;
        if (action == EnemyAction.Wait)
        {
            Wait();
        }
        else if (action == EnemyAction.NormalAttack)
        {
            NormalAttack();
        }
        else if (action == EnemyAction.CounterAttack)
        {
            CounterAttack();
        }
        else if (action == EnemyAction.SpecialAttack1)
        {
            SpecialAttack1();
        }
        else if (action == EnemyAction.SpecialAttack2)
        {
            SpecialAttack2();
        }
        else if (action == EnemyAction.RunAttack)
        {
            RunAttack();
        }
        else if (action == EnemyAction.Chase)
        {
            Chase();
        }
        else if (action == EnemyAction.Run)
        {
            Run();
        }
        else if (action == EnemyAction.Roam)
        {
            Roam();
        }
    }
    //Once a decision is taken, this method is called to perform the activity on that decision taken
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //In here, it calculates based on the distance that character is to Spidy what decision to make
    //Gives values and decision weights to each one depending on the distance and then calls DecideWithWeights
    void FixedUpdate()
    {
        bool doNotStop = false;
        if (enemy.isWaiting)
        {
            presentDistance = enemy.transform.position;
            float difference = Vector3.SqrMagnitude(presentDistance - pastDistance);
            if (difference > 5)
            {
                doNotStop = true;
            }
        }
        enemy.KeepMovement(doNotStop);
        pastDistance = presentDistance;
        sqrDistance = Vector3.SqrMagnitude(spidyObj.transform.position - transform.position);
        canReach = attackReachMin * attackReachMin < sqrDistance && sqrDistance < attackReachMax * attackReachMax;// && !doNotStop;
        samePlane = Mathf.Abs(spidyObj.transform.position.z - transform.position.z) < 40;
        if (decisionDuration > 0.0f)
        {
            decisionDuration -= Time.deltaTime;
        }
        else if (spidyObj.GetComponent<Spidy>().isKnockedOut ||
            spidyObj.GetComponent<Spidy>().firstTransformation ||
            spidyObj.GetComponent<Spidy>().returnTransformation)
        {
            if (enemy.CompareTag("RangeEnemy"))
            {
                DecideWithWeights(10, 0, 0, 0, 0, 0, 80, 0, 10);
            }
            else DecideWithWeights(20, 0, 0, 0, 0, 0, 0, 0, 80);
        }
        else
        {
            switch (enemy.tag)
            {
                case "Fighter":
                    if (detector.spidyIsNearby)
                    {
                        if (samePlane)
                        {
                            if (canReach)
                            {
                                if (enemy.currentAttackChain > 0 && enemy.evaluatedAttackChain > 0)
                                {
                                    DecideWithWeights(10, 0, 90);
                                }
                                else
                                {
                                    DecideWithWeights(10, 10, 80, 0, 0, 0, 0, 0, 0);
                                }
                            }
                            else
                            {
                                if (StaticVar.TotalEnemies == 1)
                                {
                                    DecideWithWeights(10, 0, 0, 0, 0, 0, 80, 0, 10);
                                }
                                else if (StaticVar.TotalEnemies >= 2)
                                {
                                    if (enemy.canAttack)
                                    {
                                        DecideWithWeights(10, 0, 0, 0, 0, 0, 80, 0, 10);
                                    }
                                    else
                                    {
                                        if (enemy.destinyPoint == Vector3.zero) DecideWithWeights(10, 0, 0, 0, 0, 0, 0, 0, 90);
                                        else DecideWithWeights(10, 0, 0, 0, 0, 0, 80, 0, 10);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (StaticVar.TotalEnemies == 1)
                            {
                                DecideWithWeights(20, 0, 0, 0, 0, 0, 80);
                            }
                            else if (StaticVar.TotalEnemies >= 2)
                            {
                                if (enemy.canAttack)
                                {
                                    DecideWithWeights(20, 0, 0, 0, 0, 0, 80);
                                }
                                else
                                {
                                    if (enemy.destinyPoint == Vector3.zero) DecideWithWeights(10, 0, 0, 0, 0, 0, 0, 0, 90);
                                    else DecideWithWeights(10, 0, 0, 0, 0, 0, 90);
                                }
                            }
                        }
                    }
                    else if (gameManager.allowedForBattleEvent)
                    {
                        DecideWithWeights(10, 0, 0, 0, 0, 0, 90);
                    }
                    else
                    {
                        DecideWithWeights(40, 0, 0, 0, 0, 0, 10, 0, 50);
                    }
                    break;
                case "RangeEnemy":
                    if (detector.spidyIsNearby)
                    {
                        if (sqrDistance > 250000)
                        {
                            if (((Mathf.Abs(transform.position.x - eManager.spidyFarBehindPoint.x) <= 50) || (Mathf.Abs(transform.position.x - eManager.spidyFarFrontPoint.x) <= 50)) && samePlane)
                            {
                                DecideWithWeights(25, 0, 0, 0, 75); //Special Attack 2 for Sniper is Shoot Attack
                            }
                            else
                            {
                                DecideWithWeights(20, 0, 0, 0, 0, 0, 70, 0, 10);
                            }
                        }
                        else if (samePlane)
                        {
                            if (canReach)
                            {
                                if (enemy.currentLife > enemy.maxLife / 2)
                                {
                                    DecideWithWeights(0, 50, 0, 50); //Special Attack 1 for Sniper is a kick, Normal Attack for Sniper is the hit with the gun
                                }
                                else
                                {
                                    DecideWithWeights(0, 0, 0, 100); //Special Attack 1 for Sniper is a kick
                                }
                            }
                            else
                            {
                                DecideWithWeights(20, 0, 0, 0, 0, 0, 70, 0, 10);
                            }
                        }
                        else DecideWithWeights(20, 0, 0, 0, 0, 0, 70, 0, 10);
                    }
                    else if (gameManager.allowedForBattleEvent)
                    {
                        DecideWithWeights(10, 0, 0, 0, 0, 0, 90);
                    }
                    else
                    {
                        DecideWithWeights(40, 0, 0, 0, 0, 0, 10, 0, 50);
                    }
                    break;
                case "Boss":
                    samePlane = Mathf.Abs(spidyObj.transform.position.z - transform.position.z) < 60;
                    if (StaticVar.zimaLevel)
                    {
                        if (enemy.isRunning)
                        {
                            if (canReach && samePlane)
                                DecideWithWeights(0, 0, 0, 0, 0, 100, 0, 0, 0);
                            else DecideWithWeights(0, 0, 0, 0, 0, 0, 0, 100);
                        }
                        else if (enemy.currentState == 2 && sqrDistance >= 500000)
                            DecideWithWeights(0, 0, 0, 0, 0, 0, 0, 100);
                        else if (enemy.currentState == 1 && sqrDistance >= 1000000)
                            DecideWithWeights(0, 0, 0, 0, 0, 0, 0, 100);
                        else
                        {
                            switch (enemy.currentState)
                            {
                                case 0:
                                    if (canReach && samePlane)
                                    {
                                        if (enemy.currentAttackChain > 0 && enemy.evaluatedAttackChain > 0)
                                            DecideWithWeights(10, 0, 90);
                                        else DecideWithWeights(5, 0, 90, 0, 0, 0, 2, 0, 3);
                                    }
                                    else
                                    {
                                        DecideWithWeights(5, 0, 0, 0, 0, 0, 95);
                                    }
                                    break;
                                case 1:
                                    if (samePlane)
                                    {
                                        if (canReach)
                                        {
                                            if (enemy.currentAttackChain > 0 && enemy.evaluatedAttackChain > 0)
                                                DecideWithWeights(10, 0, 90);
                                            else DecideWithWeights(5, 0, 90, 0, 0, 0, 2, 0, 3);
                                        }
                                        else if (sqrDistance > 250000)
                                            DecideWithWeights(5, 0, 0, 10, 0, 0, 85);
                                        else DecideWithWeights(5, 0, 0, 0, 0, 0, 95);
                                    }
                                    else DecideWithWeights(5, 0, 0, 0, 0, 0, 95);
                                    break;
                                case 2:
                                    if (samePlane)
                                    {
                                        if (canReach)
                                        {
                                            if (enemy.currentAttackChain > 0 && enemy.evaluatedAttackChain > 0)
                                                DecideWithWeights(0, 0, 100);
                                            else DecideWithWeights(0, 0, 100);
                                        }
                                        else if (sqrDistance > 250000)
                                            DecideWithWeights(20, 0, 0, 10, 0, 0, 65);
                                        else DecideWithWeights(5, 0, 0, 0, 0, 0, 95);
                                    }
                                    else if (detector.spidyIsNearby)
                                        DecideWithWeights(5, 0, 0, 0, 50, 0, 45);
                                    else DecideWithWeights(5, 0, 0, 0, 0, 0, 95);
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
    }
    //In here, it calculates based on the distance that character is to Spidy what decision to make
    //Gives values and decision weights to each one depending on the distance and then calls DecideWithWeights
    //-------------------------------------------------------------------------------------------------
}
//Creating dependency to the Enemy Script
//-------------------------------------------------------------------------------------------------
