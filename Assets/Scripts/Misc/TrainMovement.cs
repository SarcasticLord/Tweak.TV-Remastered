using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public float speed = 5f;
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
        if (waypoints.Length == 0)
            return;

        Transform target = waypoints[waypointIndex];
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);

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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Enemy Hit
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Enemy Hit!");
        }
        //Player Hit 
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Hit!");
        }
    }
}
