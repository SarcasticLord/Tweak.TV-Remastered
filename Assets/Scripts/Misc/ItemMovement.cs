// this is what makes the box move up and down and spin in circles minecraft style

using System.Collections;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    public float distance = .3f;
    public float movement = .25f;
    private int counter = 5; // how many times to mive in a given direction
    void Start()
    {
        StartCoroutine("MoveObject");
    }

    void Update() // spins the box in circles
    {
        transform.Rotate (new Vector3 (0, 30, 0) * Time.deltaTime);
    }
    

    IEnumerator MoveObject()  // moves the box up and down
    {
        while (true)
        {
            transform.Translate(new Vector3(0, distance * movement));
            counter--;
            if (counter <= 0) // if it moved 5 times
            {
                movement *= -1; // now move in the oposite directoon
                counter = 5; // reset counter
            }
            yield return new WaitForSeconds(.09f);
        }
    }

    public void StopRotating()
    {
        StopAllCoroutines();
        enabled = false;
    }
}
