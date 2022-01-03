using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int target = 0;
    [SerializeField] private Transform exitPoint;

    [SerializeField] private float navigationUpdate;
    [SerializeField] private int healthPoints;
    [SerializeField] private int rewardAmount;

    private Transform enemy;       //enemy location
    private float navigationTime = 0;
    private bool isDead;
    public bool IsDead{
        get{
            return isDead;
        }
    }

    private Collider2D enemyCollider;
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Transform>();
        GameManager.Instance.RegisterEnemy(this);
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.waypoints != null && !isDead)
        {
            
            navigationTime += Time.deltaTime;
            if (navigationTime < navigationUpdate)
            {
                if (target < GameManager.Instance.waypoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, GameManager.Instance.waypoints[target].position, navigationTime);
                    
                }

                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);

                }
                navigationTime = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "checkpoint")
        {
            target += 1;
        }
        else if (collision.tag == "Finish")
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.isWaveOver();
        } 
        else if (collision.tag == "projectile"){
            Projectile newP = collision.gameObject.GetComponent<Projectile>();
            enemyHit(newP.AttackStrength);
            Destroy(collision.gameObject);
        }
    }

    public void enemyHit(int hitpoints){
        if (healthPoints - hitpoints > 0){
            healthPoints -= hitpoints;
            //call hurt animation
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
            anim.Play("Hurt");

        }
        else{
            //die animation here
            //enemy should die
            anim.SetTrigger("didDie");
            die();

        }
    }

    public void die(){
        isDead = true;
        enemyCollider.enabled = false;
        GameManager.Instance.TotalKilled += 1;
        GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        GameManager.Instance.addMoney(rewardAmount);
        GameManager.Instance.isWaveOver();
    }
   
}
