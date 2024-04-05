using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField] Rigidbody2D bobber;
    [SerializeField] GameObject Player;
    public bool inwater = false;
    private void Update()
    {
        gameObject.transform.eulerAngles = Vector3.zero;
        if (inwater) { bobber.gravityScale = 0.25f; bobber.mass = 0.25f; }
        else { bobber.mass = 1; }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) { inwater = true; }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) { inwater = false; bobber.gravityScale = 1; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall")) 
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
        }
    }
}
