using UnityEngine;

public class ExitTrain : MonoBehaviour
{
    public float speed = 5f;
    public float timeToWait = 30f;
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

        if(elapsedTime > timeToWait)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            Vector3 nextPos = Vector3.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
            rb.MovePosition(nextPos);
            if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                elapsedTime = 0;
            }
        }
       
    }

}
