using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Vector3 PlayerPosition { get; private set; }

    void Update()

    {

        PlayerPosition = transform.position;

    }
}
