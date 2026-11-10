using UnityEngine;
using UnityEngine.InputSystem;

public class Crowbar : BaseItem
{
    public Transform cam;
    private int randAnim;

    public LayerMask ignoreLayer;

    public float maxDistance;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            Debug.Log("Crowbare input read");
            Vector3 rayOrigin = cam.position;
            Vector3 rayDirection = cam.forward;

            Debug.DrawRay(rayOrigin, rayDirection * 10f, Color.red, 5f);
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection * 10f, out hit, maxDistance, ~ignoreLayer))
            {
                GameObject target = hit.transform.gameObject;
                Debug.Log(target.name);

                if (target.CompareTag("Crowable"))
                {
                    Collider targetCollider = hit.collider;
                    Rigidbody targetRb = targetCollider.GetComponent<Rigidbody>();

                    if (targetCollider != null)
                    {
                        targetCollider.enabled = true;
                    }

                    if (targetRb != null)
                    {
                        // Enable collider if needed
                        //hit.collider.enabled = true;
                        // Remove constraints
                        targetRb.constraints = RigidbodyConstraints.None;
                        // Calculate direction from target to caster
                        Vector3 directionToCaster = (cam.position - hit.transform.position).normalized;
                        // Apply impulse force toward the caster
                        targetRb.AddForce(directionToCaster * 5, ForceMode.Impulse);

                    }
                    Destroy(hit.collider.gameObject, 5f);                
                }
            }
        }
    }
}