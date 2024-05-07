using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLife : MonoBehaviour
{

  [SerializeField]
  float lifeTime;
 
  void Start()
  {
    Invoke("DestroyMyObject", lifeTime);
  }
  void DestroyMyObject()
  {
    Destroy(this.gameObject);
  }
}
