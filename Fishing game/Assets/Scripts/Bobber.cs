using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField] Rigidbody2D bobber;
    public bool inwater = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) { bobber.gravityScale = 0.25f; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall")) { gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; }
    }
}
