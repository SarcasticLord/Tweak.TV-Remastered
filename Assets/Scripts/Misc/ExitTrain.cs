using UnityEngine;

public class ExitTrain : MonoBehaviour
{
    public float speed = 20f;

    private float flipSpeed = 100f;
    public float timeToWait = 0f;
    public float timeBetweenTrains = 145;
    public float exitWaitTime = 60f;

    public float elapsedTime = 0f;
    private Rigidbody rb;

    public Transform[] waypoints;
    private int waypointIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (waypoints.Length == 0)
            return;

        Transform target = waypoints[waypointIndex].transform;
        Vector3 targetPos = target.position;
        if (waypointIndex != 2)
        {
            timeToWait = timeBetweenTrains / waypoints.Length;
        }
        else
        {
            timeToWait = exitWaitTime;
        }
        if(elapsedTime > timeToWait)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            Vector3 nextPos = Vector3.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
            Vector3 euler = transform.eulerAngles;
            euler.y = target.eulerAngles.y;
            transform.rotation = Quaternion.Euler(euler);
            rb.MovePosition(nextPos);
            transform.rotation = Quaternion.Euler(euler);
            if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                elapsedTime = 0;
            }
        }
       
    }

}
