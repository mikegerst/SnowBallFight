using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform player;
    private Animator animator;
    private Rigidbody rigidbody;
    private SkinnedMeshRenderer enemyRender;
    private float health = 1.0f;
    private Vector3 runSpot;
    private bool hiding = false;
    private bool running = false;
    
    
    


    private float countDown;

    [SerializeField] Image healthBar;
    [SerializeField] bool isGrounded = false;
    private List<Collider> collisions = new List<Collider>();

    [SerializeField] private Transform shotSpot;
    [SerializeField] private GameObject snowball;
    [SerializeField] private float snowballReset = 1.0f;
    [SerializeField] private float timeToWave;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> hidingSpots;
    [SerializeReference] private EnemyState state;
    [SerializeReference] private string currentHidingSpot;
    [SerializeField] private int ThrowsTillRun = 3;
    [SerializeReference] private int throws = 0;


    private bool isSnowBallResetTimerRunning = true;


    private float snowballResetTime;

    void Start()
    {
        enemyRender = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        countDown = timeToWave;
        snowballResetTime = snowballReset;

        animator.SetFloat("MoveSpeed", 3.0f);

        state = EnemyState.Hide;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", isGrounded);

        /*if (isGrounded)
        {
            if (System.Math.Abs(Vector3.Distance(transform.position, player.position)) > 10.0f)
            {
                animator.SetFloat("MoveSpeed", 3.0f);
                rigidbody.velocity = transform.forward * 6;
                Debug.Log("Distance: " + Vector3.Distance(transform.position, player.position));
            }
            else
            {
                rigidbody.velocity = new Vector3(0, 0, 0);
                animator.SetFloat("MoveSpeed", 0.0f);
            }
        }
       

        if (countDown <= 0)
        {
            animator.SetTrigger("Wave");
            countDown = timeToWave;
            rigidbody.velocity = new Vector3(0, 0, 0);
        }
        else
        {

            countDown -= Time.deltaTime;
        }*/
        CheckState();

        if (state.Equals(EnemyState.Hide))
        {
            Hide();
        }
        else if (state.Equals(EnemyState.Wait))
        {
            Wait();
        }
        else if (state.Equals(EnemyState.Throw))
        {
            CheckThrowSnowball();
        }else if (state.Equals(EnemyState.Run))
        {
            Run();
        }
        
    }

    private void CheckState()
    {
        if(Vector3.Distance(transform.position, player.position) <= 10f)
        {
            state = EnemyState.Throw;
        }
        else
        {
            if (state.Equals(EnemyState.Hide))
            {

                if (Vector3.Distance(transform.position, hidingSpots.Find(t => t.name == currentHidingSpot).position) < 3f)
                {
                    state = EnemyState.Wait;
                    hiding = false;
                }
            }

            if (state.Equals(EnemyState.Throw))
            {
                if (throws >= ThrowsTillRun)
                {
                    state = EnemyState.Run;
                    throws = 0;
                }
            }

            if (state.Equals(EnemyState.Run))
            {
                if (runSpot == transform.position)
                {
                    state = EnemyState.Hide;
                    running = false;
                }
            }
        }
        
    }
     private void CheckThrowSnowball()
        {

            if (isSnowBallResetTimerRunning)
            {

                if (snowballResetTime <= 0)
                {
                    isSnowBallResetTimerRunning = false;
                }
                else
                {
                    snowballResetTime -= Time.deltaTime;
                }

            }

          

            if(!isSnowBallResetTimerRunning)
            {
                 ThrowSnowball();
                 isSnowBallResetTimerRunning = true;
                 snowballResetTime = snowballReset;
                 throws++;
            }
          

           

        }

    private void Hide()
    {
        if (!hiding)
        {
            float closestPosition = 0;
            Vector3 hideSpot = transform.position;
            foreach (var t in hidingSpots)
            {
                float distance = Vector3.Distance(transform.position, t.position);

                if (closestPosition == 0 || distance < closestPosition)
                {
                    hideSpot = t.position;
                    closestPosition = distance;
                    currentHidingSpot = t.name;
                }
            }

            transform.LookAt(hideSpot);
            agent.SetDestination(hideSpot);
            hiding = true;
        }

        
        
        
    }

    private void Run()
    {
        if (!running)
        {
            runSpot = transform.position + (transform.forward * -35);
            animator.SetFloat("MoveSpeed", 3.0f);
            agent.SetDestination(runSpot);
            running = true;
        }
        
    }
    private void Wait()
    {
        animator.SetFloat("MoveSpeed", 0f);
    }


    private void ThrowSnowball()
    {
        transform.LookAt(player);
        var snowballBeingThrown = Instantiate(snowball, shotSpot.position, transform.rotation);
               
        snowballBeingThrown.GetComponent<Rigidbody>().AddForce(transform.forward * 1000, ForceMode.Force);
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        HitBySnowBall(collision);
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
                isGrounded = true;
            }
        }
    }

    private void HitBySnowBall(Collision collision)
    {
        
        if(collision.gameObject.name == "Snowball")
        {
            health -= 0.25f;
            healthBar.fillAmount = health;

            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
        }
        if (collisions.Count == 0) { isGrounded = false; }
    }

    public enum EnemyState
    {
        Hide,
        Wait,
        Throw,
        Move,
        Run
    }


}
