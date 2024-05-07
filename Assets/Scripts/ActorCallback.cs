using UnityEngine;
public class ActorCallback : MonoBehaviour
{
    private EnemyManager enemy;
    public Actor actor;
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
    public AudioClip shootClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip knockdownClip;
    public AudioClip hurtClip;
    void Start()
    {
        enemy = FindObjectOfType<EnemyManager>();
    }
    //-------------------------------------------------------------------------------------------------
    //Forward the call that character indeed has getup
    public void DidGetUp()
    {
        actor.DidGetUp();
    }
    //Forward the call that character indeed has getup
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Script used to forward animator callback when an attack is complete,
    //this way character knows an attack is complete. It is triggered at the beginning of the attack animation
    public void DidChain(int chain)
    {
        actor.DidChain(chain);
    }
    //Script used to forward animator callback when an attack is complete,
    //this way character knows an attack is complete. It is triggered at the beginning of the attack animation
    //-------------------------------------------------------------------------------------------------
    public void ShowKnockdownAttack()
    {
        actor.knockdownAttackObj.SetActive(true);
    }
    public void HideKnockdownAttack()
    {
        actor.knockdownAttackObj.SetActive(false);
    }
    public void ShowSpecialAttack2_Left()
    {
        actor.specialAttack_2_Obj_Left.SetActive(true);
        actor.specialAttack_2_Obj_Left.transform.position = enemy.SetSpidyPosition(actor.transform.position.y);
        actor.specialAttack_2_Obj_Left.GetComponentInChildren<Animator>().enabled = true;
    }
    public void ShowSpecialAttack2_Right()
    {
        actor.specialAttack_2_Obj_Right.SetActive(true);
        actor.specialAttack_2_Obj_Right.transform.position = enemy.SetSpidyPosition(actor.transform.position.y);
        actor.specialAttack_2_Obj_Right.GetComponentInChildren<Animator>().enabled = true;
    }
    public void HideSpecialAttack2_Left()
    {
        actor.specialAttack_2_Obj_Left.GetComponentInChildren<Animator>().enabled = false;
        actor.specialAttack_2_Obj_Left.SetActive(false);
    }
    public void HideSpecialAttack2_Right()
    {
        actor.specialAttack_2_Obj_Right.GetComponentInChildren<Animator>().enabled = false;
        actor.specialAttack_2_Obj_Right.SetActive(false);
    }
    public void ShowSpecialAttack1()
    {
        actor.specialAttack_1_Obj.SetActive(true);
    }
    public void HideSpecialAttack1()
    {
        actor.specialAttack_1_Obj.SetActive(false);
    }
    public void RunAttackMomentum()
    {
        actor.body.AddForce((Vector3.up + (actor.frontVector * 5)) * actor.runAttackForce, ForceMode.Impulse);
    }
    public void SpawnBullet()
    {
        actor.isBulletSpawned = true;
    }
    public void DidDie()
    {
        actor.DidDie();
    }
    public void IsDead()
    {
        StaticVar.nextExit = true;
    }
    public void Hit1Clip()
    {
        audioSource.PlayOneShot(hitClip1);
    }
    public void Hit2Clip()
    {
        audioSource.PlayOneShot(hitClip2);
    }
    public void Hit3Clip()
    {
        audioSource.PlayOneShot(hitClip3);
    }
    public void JumpClip()
    {
        audioSource.PlayOneShot(jumpClip);
    }
    public void ShootClip()
    {
        actor.soundManager.PlaySoloSound(shootClip);
    }
    public void StopRuning()
    {
        actor.isRunning = false;
        actor.baseAnim.SetBool("IsRunning", actor.isRunning);
    }
    public void EnableFlip()
    {
        actor.canFlip = true;
    }
    public void DisableFlip()
    {
        actor.canFlip = false;
    }
}
