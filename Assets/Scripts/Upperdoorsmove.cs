using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upperdoorsmove : MonoBehaviour
{
  Vector3 initPos;
  // Start is called before the first frame update
  void Start()
  {
    initPos = transform.localPosition;
  }

  // Update is called once per frame
  void Update()
  {
    transform.localPosition = new Vector3(initPos.x, Mathf.Sin(Time.time), initPos.z);
  }
}
