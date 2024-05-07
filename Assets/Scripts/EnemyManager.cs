using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> fighters;
    List<GameObject> longRange;
    List<GameObject> boss;
    public GameManager gameManager;
    GameObject spidyObj;
    public float minimumDistance = 300.0f;
    Vector3 cameraPosition;
    public Vector3 lowerLeftDistance;
    public Vector3 upperLeftDistance;
    public Vector3 upperDistance;
    public Vector3 lowerDistance;
    public Vector3 lowerRightDistance;
    public Vector3 upperRightDistance;
    public List<Vector3> defineCornerPoints = new List<Vector3>();
    public float surface = 10.0f;
    private Vector3 finalPosition;
    public Vector3 spidyFarFrontPoint;
    public Vector3 spidyFarBehindPoint;
    private readonly float farPointDistance = 600.0f;
    private float sqrDistance;
    void Start()
    {
        fighters = new List<GameObject>();      //Create the list to save instanced fighters
        longRange = new List<GameObject>();     //Create the list to save instanced longRange enemies
        boss = new List<GameObject>();          //Create the list to save instanced Bosses
        StartCoroutine(WaitSpidy());            //Coroutine to wait for Spidey to instantiate
    }
    public void SetInstancedEnemies(GameObject instancedEnemy)
    {
        if (instancedEnemy.CompareTag("Fighter")) fighters.Add(instancedEnemy);   //Add instanced enemy to the list
        else if (instancedEnemy.CompareTag("Boss")) boss.Add(instancedEnemy);
        else longRange.Add(instancedEnemy);
    }
    public void EraseEnemies(GameObject deadEnemy)
    {
        if (deadEnemy.CompareTag("Fighter")) fighters.Remove(deadEnemy); //Remove enemy when died
        else if (deadEnemy.CompareTag("Boss")) boss.Remove(deadEnemy);
        else longRange.Remove(deadEnemy);
    }
    public bool CheckNavMesh(Vector3 comparePosition)
    {
        bool pointNav = NavMesh.SamplePosition(comparePosition, out NavMeshHit hit, surface, NavMesh.AllAreas);
        if (pointNav) return true;
        return false;
    }
    void FixedUpdate()
    {
        if (spidyObj != null && boss != null && boss.Count != 0)
        {
            UpdateData(boss); //Call the function updateData
        }
        //-------------------------------------------------------------------------------------------------
        //Check if spidy is in the scene and vector of enemies is not null
        if (spidyObj != null && (fighters != null || longRange != null) && (fighters.Count != 0 || longRange.Count != 0))
        {
            if (fighters != null && fighters.Count != 0)
            {
                PositionFighters();
            }
            if (longRange != null && longRange.Count != 0)
            {
                cameraPosition = gameManager.GetPosition();
                cameraPosition.z = spidyObj.transform.position.z;
                cameraPosition.y = spidyObj.transform.position.y;
                //Update the far front point to attack spidy
                spidyFarFrontPoint = new Vector3(cameraPosition.x + farPointDistance,
                                                 cameraPosition.y,
                                                 cameraPosition.z);

                //Update the far behind point to attack spidy
                spidyFarBehindPoint = new Vector3(cameraPosition.x - farPointDistance,
                                                  cameraPosition.y,
                                                  cameraPosition.z);
                PositionShooters();
            }
        }
        //Check if spidy is in the scene and vector of enemies is not null
        //-------------------------------------------------------------------------------------------------
    }
    public void UpdateData(List<GameObject> vectorEnemies)
    {
        for (int i = 0; i < vectorEnemies.Count; i++) //Loop to update enemies distances and attack point 
        {
            if (vectorEnemies[i] != null)
            {
                //Update the front point to attack spidy
                vectorEnemies[i].GetComponent<Enemy>().spidyFrontPoint = new Vector3(spidyObj.transform.position.x + vectorEnemies[i].GetComponent<Enemy>().personalSpace,
                                                spidyObj.transform.position.y,
                                                spidyObj.transform.position.z);
                //Update the behind point to attack spidy
                vectorEnemies[i].GetComponent<Enemy>().spidyBehindPoint = new Vector3(spidyObj.transform.position.x - vectorEnemies[i].GetComponent<Enemy>().personalSpace,
                                                spidyObj.transform.position.y,
                                                spidyObj.transform.position.z);
                if (vectorEnemies[i].CompareTag("RangeEnemy"))
                {
                    //Set distance between current enemy position and behind attack point of spidy
                    vectorEnemies[i].GetComponent<Enemy>().frontDistance = Vector3.SqrMagnitude(spidyFarFrontPoint - vectorEnemies[i].GetComponent<Enemy>().transform.position);
                    //Set distance between current enemy position and front attack point of spidy
                    vectorEnemies[i].GetComponent<Enemy>().behindDistance = Vector3.SqrMagnitude(spidyFarBehindPoint - vectorEnemies[i].GetComponent<Enemy>().transform.position);
                }
                else
                {
                    //Set distance between current enemy position and behind attack point of spidy
                    vectorEnemies[i].GetComponent<Enemy>().frontDistance = Vector3.SqrMagnitude(vectorEnemies[i].GetComponent<Enemy>().spidyFrontPoint - 
                        vectorEnemies[i].GetComponent<Enemy>().transform.position);
                    //Set distance between current enemy position and front attack point of spidy
                    vectorEnemies[i].GetComponent<Enemy>().behindDistance = Vector3.SqrMagnitude(vectorEnemies[i].GetComponent<Enemy>().spidyBehindPoint - 
                        vectorEnemies[i].GetComponent<Enemy>().transform.position);
                }

                //-------------------------------------------------------------------------------------------------
                //Check which is the nearest attack point
                if (vectorEnemies[i].GetComponent<Enemy>().behindDistance <
                    vectorEnemies[i].GetComponent<Enemy>().frontDistance)
                {
                    if (vectorEnemies[i].CompareTag("RangeEnemy")) vectorEnemies[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                    else vectorEnemies[i].GetComponent<Enemy>().destinyPoint = vectorEnemies[i].GetComponent<Enemy>().spidyBehindPoint; //Set position to attack
                    vectorEnemies[i].GetComponent<Enemy>().attackBehind = true;
                }
                else
                {
                    if (vectorEnemies[i].CompareTag("RangeEnemy")) vectorEnemies[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                    else vectorEnemies[0].GetComponent<Enemy>().destinyPoint = vectorEnemies[i].GetComponent<Enemy>().spidyFrontPoint; //Set position to attack
                    vectorEnemies[i].GetComponent<Enemy>().attackBehind = false;
                }
                //Check which is the nearest attack point
                //-------------------------------------------------------------------------------------------------
            }
        }
    }
    public void CornerPoints(Vector3 cameraPos)
    {
        defineCornerPoints.Add(new Vector3(cameraPos.x - 800, cameraPos.y, cameraPos.z + 500));
        defineCornerPoints.Add(new Vector3(cameraPos.x + 800, cameraPos.y, cameraPos.z + 500));
        defineCornerPoints.Add(new Vector3(cameraPos.x - 800, cameraPos.y, cameraPos.z - 500));
        defineCornerPoints.Add(new Vector3(cameraPos.x + 800, cameraPos.y, cameraPos.z - 500));
    }
    public void GetSpidyPositions(float spidyDistance)
    {
        upperLeftDistance = new Vector3(spidyObj.transform.position.x - spidyDistance, spidyObj.transform.position.y, spidyObj.transform.position.z + spidyDistance);
        upperDistance = new Vector3(spidyObj.transform.position.x, spidyObj.transform.position.y, spidyObj.transform.position.z + spidyDistance);
        upperRightDistance = new Vector3(spidyObj.transform.position.x + spidyDistance, spidyObj.transform.position.y, spidyObj.transform.position.z + spidyDistance);
        lowerLeftDistance = new Vector3(spidyObj.transform.position.x - spidyDistance, spidyObj.transform.position.y, spidyObj.transform.position.z - spidyDistance);
        lowerRightDistance = new Vector3(spidyObj.transform.position.x + spidyDistance, spidyObj.transform.position.y, spidyObj.transform.position.z - spidyDistance);
        lowerDistance = new Vector3(spidyObj.transform.position.x, spidyObj.transform.position.y, spidyObj.transform.position.z - spidyDistance);
    }
    private void PositionFighters()
    {
        UpdateData(fighters); //Call the function updateData
        if (fighters.Count > 1)
        {
            for (int i = 0; i < fighters.Count; i++) //Loop to cycle through the vector of enemies
            {
                fighters[i].GetComponent<Enemy>().canAttack = true; //Set canAttack in true to the current enemy
                //-------------------------------------------------------------------------------------------------
                //Check if the current enemy is attacking the player from behind
                if (fighters[i].GetComponent<Enemy>().attackBehind)
                {
                    for (int j = 0; j < fighters.Count; j++) //Loop to cycle through the vector of enemies to compare distances
                    {
                        if (i == j) continue;
                        //-------------------------------------------------------------------------------------------------
                        //Check if current enemy can attack or if change attack position
                        if (!fighters[i].GetComponent<Enemy>().canAttack ||
                            !fighters[i].GetComponent<Enemy>().attackBehind) break; //Finish loop
                        //Check if current enemy can attack or if change attack position
                        //-------------------------------------------------------------------------------------------------

                        //-------------------------------------------------------------------------------------------------
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance behind the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's front
                        if (!fighters[j].GetComponent<Enemy>().attackBehind)
                            fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyBehindPoint; //Set position to attack
                        else if (fighters[i].GetComponent<Enemy>().behindDistance <
                                 fighters[j].GetComponent<Enemy>().behindDistance)
                            fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyBehindPoint; //Set position to attack
                        else
                        {
                            fighters[i].GetComponent<Enemy>().attackBehind = false; //Set the attack position opposite to the original
                            for (int k = 0; k < fighters.Count; k++)  //Loop to cycle again
                            {
                                fighters[i].GetComponent<Enemy>().surroundHero = true;
                                if (i == k) continue;
                                //-------------------------------------------------------------------------------------------------
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance front the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                if (fighters[k].GetComponent<Enemy>().attackBehind)
                                    fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyFrontPoint; //Set position to attack
                                else if (fighters[i].GetComponent<Enemy>().frontDistance <
                                         fighters[k].GetComponent<Enemy>().frontDistance)
                                    fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyFrontPoint; //Set position to attack
                                else
                                {
                                    fighters[i].GetComponent<Enemy>().canAttack = false; ///Set canAttack in false
                                    fighters[i].GetComponent<Enemy>().surroundHero = false;
                                    break; //Finish loop
                                }
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance front the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                //-------------------------------------------------------------------------------------------------
                            }
                        }
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance behind the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's front
                        //-------------------------------------------------------------------------------------------------
                    }
                }
                else
                {
                    for (int j = 0; j < fighters.Count; j++) //Loop to cycle through the vector of enemies to compare distances
                    {
                        if (i == j) continue;
                        //-------------------------------------------------------------------------------------------------
                        //Check if current enemy can attack or if change attack position
                        if (!fighters[i].GetComponent<Enemy>().canAttack ||
                            fighters[i].GetComponent<Enemy>().attackBehind) break; //Finish loop
                        //Check if current enemy can attack or if change attack position
                        //-------------------------------------------------------------------------------------------------

                        //-------------------------------------------------------------------------------------------------
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance front the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's behind
                        if (fighters[j].GetComponent<Enemy>().attackBehind)
                            fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyFrontPoint; //Set position to attack
                        else if (fighters[i].GetComponent<Enemy>().frontDistance <
                                 fighters[j].GetComponent<Enemy>().frontDistance)
                            fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyFrontPoint;
                        else
                        {
                            fighters[i].GetComponent<Enemy>().surroundHero = true;
                            fighters[i].GetComponent<Enemy>().attackBehind = true; //Set the attack position opposite to the original
                            for (int k = 0; k < fighters.Count; k++) //Loop to cycle again 
                            {
                                if (i == k) continue;
                                //-------------------------------------------------------------------------------------------------
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance behind the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                if (!fighters[k].GetComponent<Enemy>().attackBehind)
                                    fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyBehindPoint; //Set position to attack
                                else if (fighters[i].GetComponent<Enemy>().behindDistance <
                                         fighters[k].GetComponent<Enemy>().behindDistance)
                                    fighters[i].GetComponent<Enemy>().destinyPoint = fighters[i].GetComponent<Enemy>().spidyBehindPoint; //Set position to attack
                                else
                                {
                                    fighters[i].GetComponent<Enemy>().canAttack = false; ///Set canAttack in false
                                    fighters[i].GetComponent<Enemy>().surroundHero = false;
                                    break; //Finish loop
                                }
                            }
                        }
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance front the player
                        //In case of not having the least distance, check if the current enemy can attack the player's behind
                        //-------------------------------------------------------------------------------------------------
                    }
                }
                //Check if the current enemy is attacking the player from behind
                //-------------------------------------------------------------------------------------------------
            }
            defineCornerPoints.Clear();
            CornerPoints(cameraPosition);
            int pointer = 0;
            for (int a = 0; a < fighters.Count; a++)
            {
                if (fighters[a].GetComponent<Enemy>().surroundHero)
                {
                    GetSpidyPositions(fighters[a].GetComponent<Enemy>().personalSpace);
                    fighters[a].GetComponent<Enemy>().upper = false;
                    if (fighters[a].GetComponent<Enemy>().destinyPoint.x > spidyObj.transform.position.x)
                    {
                        if (Mathf.Abs(fighters[a].GetComponent<Enemy>().transform.position.z - upperRightDistance.z) <
                            Mathf.Abs(fighters[a].GetComponent<Enemy>().transform.position.z - lowerRightDistance.z))
                        {
                            fighters[a].GetComponent<Enemy>().corner2 = upperRightDistance;
                            fighters[a].GetComponent<Enemy>().corner1 = upperLeftDistance;
                            fighters[a].GetComponent<Enemy>().upper = true;
                        }
                        else
                        {
                            fighters[a].GetComponent<Enemy>().corner2 = lowerRightDistance;
                            fighters[a].GetComponent<Enemy>().corner1 = lowerLeftDistance;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(fighters[a].GetComponent<Enemy>().transform.position.z - upperLeftDistance.z) <
                            Mathf.Abs(fighters[a].GetComponent<Enemy>().transform.position.z - lowerLeftDistance.z))
                        {
                            fighters[a].GetComponent<Enemy>().corner2 = upperLeftDistance;
                            fighters[a].GetComponent<Enemy>().corner1 = upperRightDistance;
                            fighters[a].GetComponent<Enemy>().upper = true;
                        }
                        else
                        {
                            fighters[a].GetComponent<Enemy>().corner2 = lowerLeftDistance;
                            fighters[a].GetComponent<Enemy>().corner1 = lowerRightDistance;
                        }
                    }
                }
                else if (!fighters[a].GetComponent<Enemy>().canAttack)
                {
                    finalPosition = Vector3.zero;
                    for (int b = 0; b < defineCornerPoints.Count; b++)
                    {
                        float firstDistance = Vector3.SqrMagnitude(defineCornerPoints[b] - fighters[a].transform.position);
                        float secondDistance = Vector3.SqrMagnitude(finalPosition - fighters[a].transform.position);
                        if (firstDistance < secondDistance)
                        {
                            if (CheckNavMesh(defineCornerPoints[b]))
                            {
                                finalPosition = defineCornerPoints[b];
                                pointer = b;
                            }
                        }
                    }
                    if (finalPosition == Vector3.zero) fighters[a].GetComponent<Enemy>().destinyPoint = finalPosition;
                    else
                    {
                        fighters[a].GetComponent<Enemy>().destinyPoint = finalPosition;
                        fighters[a].GetComponent<Enemy>().surroundHero = false;
                        defineCornerPoints.Remove(defineCornerPoints[pointer]);
                    }
                }
            }
        }
        else fighters[0].GetComponent<Enemy>().surroundHero = false;
    }
    private void PositionShooters()
    {
        UpdateData(longRange);  //Call the function updateData
        if (longRange.Count > 1)
        {
            for (int i = 0; i < longRange.Count; i++) //Loop to cycle through the vector of enemies
            {
                longRange[i].GetComponent<Enemy>().canShoot = true; //Set canShoot in true to the current enemy
                //-------------------------------------------------------------------------------------------------
                //Check if the current enemy is attacking the player from behind
                if (longRange[i].GetComponent<Enemy>().attackBehind)
                {
                    for (int j = 0; j < longRange.Count; j++) //Loop to cycle through the vector of enemies to compare distances
                    {
                        if (i == j) continue;
                        //-------------------------------------------------------------------------------------------------
                        //Check if current enemy can attack or if change attack position
                        if (!longRange[i].GetComponent<Enemy>().canShoot ||
                            !longRange[i].GetComponent<Enemy>().attackBehind) break; //Finish loop
                        //Check if current enemy can attack or if change attack position
                        //-------------------------------------------------------------------------------------------------

                        //-------------------------------------------------------------------------------------------------
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance behind the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's front
                        if (!longRange[j].GetComponent<Enemy>().attackBehind)
                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint; //Set position to SHOOT
                        else if (longRange[i].GetComponent<Enemy>().behindDistance <
                                 longRange[j].GetComponent<Enemy>().behindDistance)
                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint; //Set position to attack
                        else
                        {
                            longRange[i].GetComponent<Enemy>().attackBehind = false; //Set the attack position opposite to the original
                            for (int k = 0; k < longRange.Count; k++)  //Loop to cycle again
                            {
                                longRange[i].GetComponent<Enemy>().surroundHero = true;
                                if (i == k) continue;
                                //-------------------------------------------------------------------------------------------------
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance front the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                if (longRange[k].GetComponent<Enemy>().attackBehind)
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint; //Set position to shoot
                                else if (longRange[i].GetComponent<Enemy>().frontDistance <
                                         longRange[k].GetComponent<Enemy>().frontDistance)
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint; //Set position to shoot
                                else
                                {
                                    longRange[i].GetComponent<Enemy>().canShoot = false; ///Set canShoot in false
                                    longRange[i].GetComponent<Enemy>().surroundHero = false;
                                    break; //Finish loop
                                }
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance front the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                //-------------------------------------------------------------------------------------------------
                            }
                        }
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance behind the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's front
                        //-------------------------------------------------------------------------------------------------
                    }
                }
                else
                {
                    for (int j = 0; j < longRange.Count; j++) //Loop to cycle through the vector of enemies to compare distances
                    {
                        if (i == j) continue;
                        //-------------------------------------------------------------------------------------------------
                        //Check if current enemy can attack or if change attack position
                        if (!longRange[i].GetComponent<Enemy>().canShoot ||
                            longRange[i].GetComponent<Enemy>().attackBehind) break; //Finish loop
                        //Check if current enemy can attack or if change attack position
                        //-------------------------------------------------------------------------------------------------

                        //-------------------------------------------------------------------------------------------------
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance front the player.
                        //In case of not having the least distance, check if the current enemy can attack the player's behind
                        if (longRange[j].GetComponent<Enemy>().attackBehind)
                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint; //Set position to shoot
                        else if (longRange[i].GetComponent<Enemy>().frontDistance <
                                 longRange[j].GetComponent<Enemy>().frontDistance)
                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                        else
                        {
                            longRange[i].GetComponent<Enemy>().surroundHero = true;
                            longRange[i].GetComponent<Enemy>().attackBehind = true; //Set the attack position opposite to the original
                            for (int k = 0; k < longRange.Count; k++) //Loop to cycle again 
                            {
                                if (i == k) continue;
                                //-------------------------------------------------------------------------------------------------
                                //Check if another enemy will attack the same position as current enemy
                                //If attacking in the same position, compare who is a shorter distance behind the player.
                                //In case of not having the least distance, set can attack in false and break the loop
                                if (!longRange[k].GetComponent<Enemy>().attackBehind)
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint; //Set position to shoot
                                else if (longRange[i].GetComponent<Enemy>().behindDistance <
                                         longRange[k].GetComponent<Enemy>().behindDistance)
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint; //Set position to shoot
                                else
                                {
                                    longRange[i].GetComponent<Enemy>().canShoot = false; ///Set canShoot in false
                                    longRange[i].GetComponent<Enemy>().surroundHero = false;
                                    break; //Finish loop
                                }
                            }
                        }
                        //Check if another enemy will attack the same position as current enemy
                        //If attacking in the same position, compare who is a shorter distance front the player
                        //In case of not having the least distance, check if the current enemy can attack the player's behind
                        //-------------------------------------------------------------------------------------------------
                    }
                }
                //Check if the current enemy is attacking the player from behind
                //-------------------------------------------------------------------------------------------------
            }
            for (int a = 0; a < longRange.Count; a++)
            {
                if (longRange[a].GetComponent<Enemy>().surroundHero)
                {
                    GetSpidyPositions(longRange[a].GetComponent<Enemy>().personalSpace);
                    longRange[a].GetComponent<Enemy>().upper = false;
                    if (longRange[a].GetComponent<Enemy>().destinyPoint.x > spidyObj.transform.position.x)
                    {
                        if (Mathf.Abs(longRange[a].GetComponent<Enemy>().transform.position.z - upperRightDistance.z) <
                            Mathf.Abs(longRange[a].GetComponent<Enemy>().transform.position.z - lowerRightDistance.z))
                        {
                            longRange[a].GetComponent<Enemy>().corner2 = upperRightDistance;
                            longRange[a].GetComponent<Enemy>().corner1 = upperLeftDistance;
                            longRange[a].GetComponent<Enemy>().upper = true;
                        }
                        else
                        {
                            longRange[a].GetComponent<Enemy>().corner2 = lowerRightDistance;
                            longRange[a].GetComponent<Enemy>().corner1 = lowerLeftDistance;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(longRange[a].GetComponent<Enemy>().transform.position.z - upperLeftDistance.z) <
                            Mathf.Abs(longRange[a].GetComponent<Enemy>().transform.position.z - lowerLeftDistance.z))
                        {
                            longRange[a].GetComponent<Enemy>().corner2 = upperLeftDistance;
                            longRange[a].GetComponent<Enemy>().corner1 = upperRightDistance;
                            longRange[a].GetComponent<Enemy>().upper = true;
                        }
                        else
                        {
                            longRange[a].GetComponent<Enemy>().corner2 = lowerLeftDistance;
                            longRange[a].GetComponent<Enemy>().corner1 = lowerRightDistance;
                        }
                    }
                }
                else if (!longRange[a].GetComponent<Enemy>().canShoot)
                {
                    finalPosition = Vector3.zero;
                    //Set distance between current enemy position and behind attack point of spidy
                    longRange[a].GetComponent<Enemy>().frontDistance = Vector3.SqrMagnitude(longRange[a].GetComponent<Enemy>().spidyFrontPoint -
                        longRange[a].GetComponent<Enemy>().transform.position);
                    //Set distance between current enemy position and front attack point of spidy
                    longRange[a].GetComponent<Enemy>().behindDistance = Vector3.SqrMagnitude(longRange[a].GetComponent<Enemy>().spidyBehindPoint -
                        longRange[a].GetComponent<Enemy>().transform.position);
                    if (longRange[a].GetComponent<Enemy>().frontDistance < longRange[a].GetComponent<Enemy>().behindDistance) //FRONT DISTANCE
                    {
                        longRange[a].GetComponent<Enemy>().attackBehind = false;
                        finalPosition = longRange[a].GetComponent<Enemy>().spidyFrontPoint;
                        longRange[a].GetComponent<Enemy>().destinyPoint = finalPosition;
                        if (fighters != null && fighters.Count != 0)
                        {
                            if (finalPosition == fighters[0].GetComponent<Enemy>().destinyPoint)
                            {
                                longRange[a].GetComponent<Enemy>().destinyPoint = longRange[a].GetComponent<Enemy>().spidyBehindPoint;
                                longRange[a].GetComponent<Enemy>().surroundHero = true;
                                longRange[a].GetComponent<Enemy>().attackBehind = true;
                            }
                        }
                    }
                    else
                    {
                        longRange[a].GetComponent<Enemy>().attackBehind = true;
                        finalPosition = longRange[a].GetComponent<Enemy>().spidyBehindPoint;
                        longRange[a].GetComponent<Enemy>().destinyPoint = finalPosition;
                        if (fighters != null && fighters.Count != 0)
                        {
                            if (finalPosition == fighters[0].GetComponent<Enemy>().destinyPoint)
                            {
                                longRange[a].GetComponent<Enemy>().destinyPoint = longRange[a].GetComponent<Enemy>().spidyFrontPoint;
                                longRange[a].GetComponent<Enemy>().surroundHero = true;
                                longRange[a].GetComponent<Enemy>().attackBehind = false;
                            }
                        }
                    }
                }
            }
            for (int c = 0; c < longRange.Count; c++)
            {
                if (!longRange[c].GetComponent<Enemy>().canShoot)
                {
                    for(int d = 0; d < longRange.Count; d++)
                    {
                        if (c == d) continue;
                        if (!longRange[c].GetComponent<Enemy>().attackBehind)
                        {
                            if (!longRange[d].GetComponent<Enemy>().attackBehind && !longRange[d].GetComponent<Enemy>().canShoot)
                            {
                                if (longRange[c].GetComponent<Enemy>().frontDistance > longRange[d].GetComponent<Enemy>().frontDistance)
                                {
                                    longRange[c].GetComponent<Enemy>().destinyPoint = longRange[c].GetComponent<Enemy>().spidyBehindPoint;
                                    longRange[c].GetComponent<Enemy>().attackBehind = true;
                                    longRange[c].GetComponent<Enemy>().surroundHero = true;
                                }
                            }
                        }
                        else
                        {
                            if (longRange[d].GetComponent<Enemy>().attackBehind && !longRange[d].GetComponent<Enemy>().canShoot)
                            {
                                if (longRange[c].GetComponent<Enemy>().behindDistance > longRange[d].GetComponent<Enemy>().behindDistance)
                                {
                                    longRange[c].GetComponent<Enemy>().destinyPoint = longRange[c].GetComponent<Enemy>().spidyFrontPoint;
                                    longRange[c].GetComponent<Enemy>().attackBehind = false;
                                    longRange[c].GetComponent<Enemy>().surroundHero = true;
                                }
                            }
                        }
                    }
                    if (longRange[c].GetComponent<Enemy>().surroundHero)
                    {
                        GetSpidyPositions(longRange[c].GetComponent<Enemy>().personalSpace);
                        longRange[c].GetComponent<Enemy>().upper = false;
                        if (longRange[c].GetComponent<Enemy>().destinyPoint.x > spidyObj.transform.position.x)
                        {
                            if (Mathf.Abs(longRange[c].GetComponent<Enemy>().transform.position.z - upperRightDistance.z) <
                                Mathf.Abs(longRange[c].GetComponent<Enemy>().transform.position.z - lowerRightDistance.z))
                            {
                                longRange[c].GetComponent<Enemy>().corner2 = upperRightDistance;
                                longRange[c].GetComponent<Enemy>().corner1 = upperLeftDistance;
                                longRange[c].GetComponent<Enemy>().upper = true;
                            }
                            else
                            {
                                longRange[c].GetComponent<Enemy>().corner2 = lowerRightDistance;
                                longRange[c].GetComponent<Enemy>().corner1 = lowerLeftDistance;
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(longRange[c].GetComponent<Enemy>().transform.position.z - upperLeftDistance.z) <
                                Mathf.Abs(longRange[c].GetComponent<Enemy>().transform.position.z - lowerLeftDistance.z))
                            {
                                longRange[c].GetComponent<Enemy>().corner2 = upperLeftDistance;
                                longRange[c].GetComponent<Enemy>().corner1 = upperRightDistance;
                                longRange[c].GetComponent<Enemy>().upper = true;
                            }
                            else
                            {
                                longRange[c].GetComponent<Enemy>().corner2 = lowerLeftDistance;
                                longRange[c].GetComponent<Enemy>().corner1 = lowerRightDistance;
                            }
                        }
                    }
                }
            }
            for (int e = 0; e < longRange.Count; e++)
            {
                if (!longRange[e].GetComponent<Enemy>().canShoot)
                {
                    for (int f = 0; f < longRange.Count; f++)
                    {
                        if (e == f) continue;
                        else if (longRange[e].GetComponent<Enemy>().attackBehind)
                        {
                            if (longRange[f].GetComponent<Enemy>().attackBehind)
                            {
                                if (longRange[f].GetComponent<Enemy>().currentLife > longRange[e].GetComponent<Enemy>().currentLife)
                                {
                                    longRange[e].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                                    longRange[e].GetComponent<Enemy>().canShoot = true;
                                    longRange[f].GetComponent<Enemy>().destinyPoint = longRange[f].GetComponent<Enemy>().spidyBehindPoint;
                                    longRange[f].GetComponent<Enemy>().canShoot = false;
                                }
                            }
                        }
                        else
                        {
                            if (!longRange[f].GetComponent<Enemy>().attackBehind)
                            {
                                if (longRange[f].GetComponent<Enemy>().currentLife > longRange[e].GetComponent<Enemy>().currentLife)
                                {
                                    longRange[e].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                                    longRange[e].GetComponent<Enemy>().canShoot = true;
                                    longRange[f].GetComponent<Enemy>().destinyPoint = longRange[f].GetComponent<Enemy>().spidyFrontPoint;
                                    longRange[f].GetComponent<Enemy>().canShoot = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (!spidyObj.GetComponent<Spidy>().isKnockedOut)
        {
            if (longRange.Count < 4 && fighters.Count < 2)
            {
                for (int i = 0; i < longRange.Count; i++)
                {
                    sqrDistance = Vector3.SqrMagnitude(spidyObj.transform.position - longRange[i].GetComponent<Enemy>().transform.position);
                    if (sqrDistance >= 62500 && sqrDistance <= 250000)
                    {
                        if (longRange[i].GetComponent<Enemy>().currentLife > longRange[i].GetComponent<Enemy>().maxLife / 2)
                        {
                            if (longRange[i].GetComponent<Enemy>().destinyPoint == spidyFarFrontPoint)
                            {
                                longRange[i].GetComponent<Enemy>().destinyPoint = longRange[i].GetComponent<Enemy>().spidyFrontPoint;
                                for (int j = 0; j < longRange.Count; j++)
                                {
                                    if (i == j) continue;
                                    else
                                    {
                                        if (longRange[i].GetComponent<Enemy>().spidyFrontPoint == longRange[j].GetComponent<Enemy>().destinyPoint)
                                        {
                                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                                            break;
                                        }
                                    }
                                }
                                if (fighters.Count > 0)
                                {
                                    if (fighters[0].GetComponent<Enemy>().destinyPoint == longRange[i].GetComponent<Enemy>().spidyFrontPoint)
                                    {
                                        longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                                    }
                                }
                            }
                            else if (longRange[i].GetComponent<Enemy>().destinyPoint == spidyFarBehindPoint)
                            {
                                longRange[i].GetComponent<Enemy>().destinyPoint = longRange[i].GetComponent<Enemy>().spidyBehindPoint;
                                for (int j = 0; j < longRange.Count; j++)
                                {
                                    if (i == j) continue;
                                    else
                                    {
                                        if (longRange[i].GetComponent<Enemy>().spidyBehindPoint == longRange[j].GetComponent<Enemy>().destinyPoint)
                                        {
                                            longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                                            break;
                                        }
                                    }
                                }
                                if (fighters.Count > 0)
                                {
                                    if (fighters[0].GetComponent<Enemy>().destinyPoint == longRange[i].GetComponent<Enemy>().spidyBehindPoint)
                                    {
                                        longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                                    }
                                }
                            }
                        }
                    }
                    else if(sqrDistance < 62500)
                    {
                        if (longRange[i].GetComponent<Enemy>().destinyPoint == spidyFarFrontPoint)
                        {
                            longRange[i].GetComponent<Enemy>().destinyPoint = longRange[i].GetComponent<Enemy>().spidyFrontPoint;
                            for (int j = 0; j < longRange.Count; j++)
                            {
                                if (i == j) continue;
                                else
                                {
                                    if (longRange[i].GetComponent<Enemy>().spidyFrontPoint == longRange[j].GetComponent<Enemy>().destinyPoint)
                                    {
                                        longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                                        break;
                                    }
                                }
                            }
                            if (fighters.Count > 0)
                            {
                                if (fighters[0].GetComponent<Enemy>().destinyPoint == longRange[i].GetComponent<Enemy>().spidyFrontPoint)
                                {
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarFrontPoint;
                                }
                            }
                        }
                        else if (longRange[i].GetComponent<Enemy>().destinyPoint == spidyFarBehindPoint)
                        {
                            longRange[i].GetComponent<Enemy>().destinyPoint = longRange[i].GetComponent<Enemy>().spidyBehindPoint;
                            for (int j = 0; j < longRange.Count; j++)
                            {
                                if (i == j) continue;
                                else
                                {
                                    if (longRange[i].GetComponent<Enemy>().spidyBehindPoint == longRange[j].GetComponent<Enemy>().destinyPoint)
                                    {
                                        longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                                        break;
                                    }
                                }
                            }
                            if (fighters.Count > 0)
                            {
                                if (fighters[0].GetComponent<Enemy>().destinyPoint == longRange[i].GetComponent<Enemy>().spidyBehindPoint)
                                {
                                    longRange[i].GetComponent<Enemy>().destinyPoint = spidyFarBehindPoint;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public Vector3 SetSpidyPosition(float Yposition)
    {
        return new Vector3(spidyObj.transform.position.x, Yposition, spidyObj.transform.position.z);
    }
    private IEnumerator WaitSpidy()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        spidyObj = GameObject.FindGameObjectWithTag("Spidy");
    }
}
