using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject fishingrod;
    [SerializeField] GameObject bobber;
    [SerializeField] GameObject bobberstart;
    public List<string> Caughtfish = new List<string>();
    public float chargetime;
    bool charging;
    bool hascast;
    float allowedtime = 0;
    // Start is called before the first frame update
    void Start()
    {
        while (Caughtfish.Count > 0) { Caughtfish.RemoveAt(0); }
        bobberstart.transform.position = bobber.transform.position;
    }
    private void Update()
    {
        if (chargetime >= 3) chargetime = 3;
        if (charging)
        {
            if (hascast) 
            {
                bobber.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                bobber.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
            hascast = false;
            bobber.transform.position = bobberstart.transform.position;
            chargetime += Time.deltaTime;
            fishingrod.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (chargetime >= .5f)
        {
            fishingrod.transform.eulerAngles = new Vector3(0, 0, 75);
            Cast();
            chargetime -= 0.05f;
        }
        else if (hascast)
        {
            fishingrod.transform.eulerAngles = new Vector3(0, 0, 0);
            chargetime = 0;
        }
        if (chargetime > 0 && !charging)
        {
            allowedtime += Time.deltaTime;
            if (allowedtime > 1)
            {
                fishingrod.transform.eulerAngles = new Vector3(0, 0, 0);
                chargetime = 0;
                allowedtime = 0;
            }
        }
    }
    public void ChargeandReel(InputAction.CallbackContext context)
    {
        Debug.Log("charge");
        if (context.performed)
        {
            charging = true;
        }
        else
        {
            charging = false;
        }
    }
    private void Cast()
    {
        Vector2 targetPos = -transform.position;
        Vector3 dir = targetPos - (Vector2)bobber.transform.position;
        bobber.transform.up = dir;
        bobber.GetComponent<Rigidbody2D>().AddForce(bobber.transform.up * (chargetime * 20));
        bobber.GetComponent<Rigidbody2D>().gravityScale = 1;
        hascast = true;
    }
}
