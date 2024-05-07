using System;
using System.Collections;
using UnityEngine;

public class Erin : Actor
{
    //-------------------------------------------------------------------------------------------------
    //Variables that control the run effects of the character
    public GameObject startRunning;
    private bool runningEffect;
    //Variables that control the run effects of the character
    //-------------------------------------------------------------------------------------------------
    public Walker walker;
    public bool isAutoPiloting;
    public bool controllable = true;
    public bool allowedToRun;
    //-------------------------------------------------------------------------------------------------
    //Initial setup of Nerin with its variables along to the UI
    protected override void Start()
    {
        base.Start();
        allowedToRun = false;
        StartCoroutine(Delay());
    }
    //Initial setup of Nerin with its variables along to the UI
    //-------------------------------------------------------------------------------------------------
    public override void Update()
    {
        shadowYPosition = transform.position.y - 0.2f;
        shadowZPosition = transform.position.z - 0.9f;
    }
    public void Walk()
    {
        speed = walkSpeed;         //Updates speed character to make him walk
        isRunning = false;                          //Updates variable to change animator from running
        baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
        baseAnim.SetBool("IsRunning", isRunning);   //Sets the boolean that player is running from animator
    }
    //Code that makes character to move and updates animator based on the movement requirements
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    //Code that makes character to stop and updates animator based on the stop requirements
    public void Stop()
    {
        speed = 0;                                  //Updates speed character to stop
        isRunning = false;                          //Updates variable to change animator from running
        baseAnim.SetFloat("Speed", speed);          //Updates variable from animator with the speed
        baseAnim.SetBool("IsRunning", isRunning);   //Sets the boolean that player is running from animator
    }
    //Code that makes character to stop and updates animator based on the stop requirements
    //-------------------------------------------------------------------------------------------------
    public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
    {
        if (shouldRun)
        {
            Run();
        }
        else
        {
            Walk();
        }
        walker.MoveTo(position, callback, shouldRun);
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        allowedToRun = true;
        walker.enabled = true;
    }
}
