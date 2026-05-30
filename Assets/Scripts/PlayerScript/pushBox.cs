using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushBox : MonoBehaviour
{
    public float pushForce = 5f;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("CanPush"))
        {
            Rigidbody boxRb = collision.rigidbody;
            if (boxRb != null && !boxRb.isKinematic)
            {
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                pushDirection.y = 0; // Keep it horizontal
                boxRb.AddForce(pushDirection * pushForce, ForceMode.Force);
            }
        }
    }
}
