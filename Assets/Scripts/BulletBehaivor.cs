using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaivor : MonoBehaviour
{
  
  [SerializeField]
  GameObject bulletObj;

  [SerializeField]
  Transform spawnPoint;

  [SerializeField]
  Actor actorEnemy;

  private GameObject instantiatedBullet;

  private bool doOnece;

  [SerializeField] 
  float bulletSpeed = 1000f;

  private void Start()
  {
    doOnece = true;
  }

  private void FixedUpdate()
  {
    if (actorEnemy.isBulletSpawned)
    {
      instantiatedBullet = Instantiate(bulletObj);
      doOnece = true;
      instantiatedBullet.transform.position = spawnPoint.position;
      actorEnemy.isBulletSpawned = false;
      instantiatedBullet.GetComponentInChildren<HitForwarder>().actor = actorEnemy;
      
    }
    else if (instantiatedBullet != null)
    {
      if (doOnece)
      {
        if (actorEnemy.spriteDirection)
        {
          instantiatedBullet.transform.localScale = new Vector3(-1, 1, 1);
          instantiatedBullet.GetComponent<Rigidbody>().velocity = -transform.right * bulletSpeed;
        }
        else
        {
          instantiatedBullet.transform.localScale = new Vector3(1, 1, 1);
          instantiatedBullet.GetComponent<Rigidbody>().velocity = transform.right * bulletSpeed;
        }
        doOnece = false;
      }
      
    }
  }
}
