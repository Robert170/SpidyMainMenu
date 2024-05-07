using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//-------------------------------------------------------------------------------------------------
//Code used by the AI to make an actor move and walk over the map
[RequireComponent(typeof(Actor))]
public class Walker : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;                     //Reference to the NavMesh Agent attached to the character
    private NavMeshPath navPath;                          //NavMesPath used to calculate where the character will move
    List<Vector3> corners = new List<Vector3>();          //The points where character will move to reach its destiny
    public float currentSpeed;                            //Speed that character currently has
    float speed;                                          //Constant speed variable of character
    private Actor actor;                                  //Reference to the character that will walk
    private Enemy enemy;
    private System.Action didFinishWalk;                  //A callback that is called when walker reaches its destination
    public GameObject heroe;
    public bool keepAnim;
    public bool lookAround;
    private bool pathFound;
    private List<Vector3> pathReference;
    private bool canRun;

    //-------------------------------------------------------------------------------------------------
    //Initial setup to avoid any conflict with the initial updates of the NavMeshAgent
    void Start()
    {
        heroe = GameObject.FindGameObjectWithTag("Spidy");
        actor = GetComponent<Actor>();
        navMeshAgent.enabled = true;
        navMeshAgent.updateRotation = false;
        navMeshAgent.isStopped = true;
        if (!actor.CompareTag("Spidy"))
        {
            enemy = actor.GetComponent<Enemy>();
        }
    }
    //Initial setup to avoid any conflict with the initial updates of the NavMeshAgent
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Method used to find a path of how character will reach its destiny point
    //Return true if a path is found, false if path was not found
    public bool MoveTo(Vector3 targetPosition, System.Action callback = null, bool shouldRun = false)
    {
        if (Vector3.SqrMagnitude(transform.position - targetPosition) < 100.0f)
        {
            StopMovement();
            return true;
        }
        navMeshAgent.Warp(transform.position);
        didFinishWalk = callback;
        speed = actor.speed;
        canRun = shouldRun;
        navPath = new NavMeshPath();
        pathFound = navMeshAgent.CalculatePath(targetPosition, navPath);
        if (actor.CompareTag("Spidy"))
        {
            if (pathFound)
            {
                corners = navPath.corners.ToList();
                navMeshAgent.isStopped = false;
                return true;
            }
        }
        else
        {
            if (pathFound)
            {
                corners = navPath.corners.ToList();
                navMeshAgent.isStopped = false;
                if (!enemy.surroundHero) return true;
            }
            if (enemy.surroundHero)
            {
                if (corners != null)
                {
                    corners.Clear();
                    DefineSurroundingPath();
                    return true;
                }
            }
        }
        return false;
    }
    //Method used to find a path of how character will reach its destiny point
    //Return true if a path is found, false if path was not found
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Stop immediately the movement of the character, clears the navpath and the point where this character should move
    public void StopMovement()
    {
        if (GetComponent<NavMeshAgent>().enabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }
        navPath = null;
        corners = null;
        currentSpeed = 0;
    }
    //Stop immediately the movement of the character, clears the navpath and the point where this character should move
    //-------------------------------------------------------------------------------------------------
    public void AddDeadImpulse(Vector3 vectorDir)
    {
        navMeshAgent.velocity = 3000 * -vectorDir.x * transform.right;
    }
    //-------------------------------------------------------------------------------------------------
    //This code is used to move character over the map, check if character canWalk and there are still corners to move
    protected void FixedUpdate()
    {
        bool canWalk = actor.CanWalk();
        if (canWalk && !navMeshAgent.isStopped && corners != null && corners.Count > 0)
        {
            //-------------------------------------------------------------------------------------------------
            //Check if character has arrived to its first target position, then remove that corner from the list
            if (Vector3.SqrMagnitude(transform.position - corners[0]) < 25.0f)
            {
                corners.RemoveAt(0);
            }
            //Check if character has arrived to its first target position, then remove that corner from the list
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //Check if there are more corners for character to move, if not, immediately character stops moving
            if (corners.Count > 0)
            {
                Vector3 direction = actor.transform.position - heroe.transform.position;
                float walkDirection = actor.transform.position.x - corners[0].x;
                currentSpeed = speed;
                if (lookAround || canRun)
                {
                    direction.x = walkDirection;
                }
                if (walkDirection < 0)
                {
                    if (direction.x >= 0) currentSpeed = -400;
                }
                else if (direction.x < 0) currentSpeed = -400;
                actor.FlipSprite(direction.x >= 0);
                if (actor.isOverTreadmill)
                {
                    navMeshAgent.velocity = transform.right * 1000;
                }
                else if (actor.isOverTreadmill2)
                {
                    navMeshAgent.velocity = -transform.right * 1000;
                }
                navMeshAgent.SetDestination(corners[0]);
            }
            //Check if there are more corners for character to move, if not, immediately character stops moving
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //Check if character has arrived to its first target position, then remove that corner from the list
            else
            {
                currentSpeed = 0.0f;
                if (actor.isOverTreadmill)
                {
                    navMeshAgent.velocity = transform.right * 300;
                }
                else if (actor.isOverTreadmill2)
                {
                    navMeshAgent.velocity = -transform.right * 300;
                }
                navMeshAgent.isStopped = true;
                if (!actor.CompareTag("Spidy"))
                    enemy.surroundHero = false;
                if (didFinishWalk != null)
                {
                    didFinishWalk.Invoke();
                    didFinishWalk = null;
                }
            }
            //Check if character has arrived to its first target position, then remove that corner from the list
            //-------------------------------------------------------------------------------------------------
        }
        else
        {
            navPath = null;
            corners = null;
            currentSpeed = 0.0f;
            if (actor.isOverTreadmill)
            {
                navMeshAgent.velocity = transform.right * 300;
            }
            else if (actor.isOverTreadmill2)
            {
                navMeshAgent.velocity = -transform.right * 300;
            }
            navMeshAgent.isStopped = true;
        }
        if (keepAnim)
        {
            currentSpeed = 400;
        }
        actor.baseAnim.SetFloat("Speed", currentSpeed);
    }
    //This code is used to move character over the map, check if character canWalk and there are still corners to move
    //-------------------------------------------------------------------------------------------------

    private void DefineSurroundingPath()
    {
        if ((enemy.attackBehind && actor.transform.position.x >= enemy.corner1.x) || (!enemy.attackBehind && actor.transform.position.x <= enemy.corner1.x))
        {
            DefinePath(actor.transform.position, enemy.corner1);
            DefinePath(enemy.corner1, enemy.corner2);
            DefinePath(enemy.corner2, enemy.destinyPoint);
        }
        else if ((enemy.upper && actor.transform.position.z >= enemy.corner1.z) || (!enemy.upper && actor.transform.position.z <= enemy.corner1.z))
        {
            DefinePath(actor.transform.position, enemy.corner2);
            DefinePath(enemy.corner2, enemy.destinyPoint);
        }
        else if ((enemy.attackBehind && actor.transform.position.x >= heroe.transform.position.x) || (!enemy.attackBehind && actor.transform.position.x <= heroe.transform.position.x))
        {
            DefinePath(transform.position, new Vector3(actor.transform.position.x, enemy.corner1.y, enemy.corner1.z));
            DefinePath(new Vector3(actor.transform.position.x, enemy.corner1.y, enemy.corner1.z), enemy.corner2);
            DefinePath(enemy.corner2, enemy.destinyPoint);
        }
        else DefinePath(transform.position, enemy.destinyPoint);
    }
    private void DefinePath(Vector3 point1, Vector3 point2)
    {
        navPath = new NavMeshPath();
        pathFound = NavMesh.CalculatePath(point1, point2, NavMesh.AllAreas, navPath);
        if (pathFound)
        {
            pathReference = navPath.corners.ToList();
            while (pathReference.Count > 0)
            {
                if (corners.Count == 0 || corners[corners.Count - 1] != pathReference[0]) corners.Add(pathReference[0]);
                pathReference.RemoveAt(0);
            }
        }
    }
}
//Code used by the AI to make an actor move and walk over the map
//-------------------------------------------------------------------------------------------------
