using System;
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
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject BobberCamera;
    [SerializeField] GameObject reelSign;
    [SerializeField] GameObject inventorypanel;
    [SerializeField] GameObject[] inventoryslots;
    [SerializeField] GameObject[] fishicons;
    [SerializeField] bool enableextracam;
    public string[] Caughtfish;
    public string[] Fish;
    public string progressfish = null;
    public float chargetime;
    float allowedtime = 0;
    float reelsigntimer;
    float reeltimer;
    float fishstrength;
    bool reel;
    bool charging;
    bool hascast;
    System.Random rng;
    // Start is called before the first frame update
    void Start()
    {
        rng = new System.Random();
        bobberstart.transform.position = bobber.transform.position;
        progressfish = null;
        for (int i = 0; i < Caughtfish.Length; i++)
        {
            Caughtfish[i] = null;
        }
    }
    private void Update()
    {
        for (int i = 0; i < Caughtfish.Length; i++)
        {
            if (Caughtfish[i] == Fish[0]) Caughtfish[i] = Fish[0];
            else if (Caughtfish[i] == Fish[1]) Caughtfish[i] = Fish[1];
            else if (Caughtfish[i] == Fish[2]) Caughtfish[i] = Fish[2];
        }
        if (bobber.GetComponent<Bobber>().inwater)
        {
            reeltimer += Time.deltaTime;
            if (reeltimer >= 2) { Fishing(); reeltimer = 0; }
        }
        else if (progressfish != null) 
        {
            for (int i = 0; i < Caughtfish.Length; i++)
            {
                if (Caughtfish[i] == null)
                {
                    Caughtfish[i] = progressfish;
                    break;
                }
            }
            progressfish = null;
        }
        if (reel && bobber.GetComponent<Bobber>().inwater)
        {
            if (reelSign.activeInHierarchy) 
            {
                reelsigntimer += Time.deltaTime;
                if (reelsigntimer >= 2) { reelSign.SetActive(false); reelsigntimer = 0; }
            }
            else 
            {
                reelsigntimer += Time.deltaTime;
                if (reelsigntimer >= 1) { reelSign.SetActive(true); reelsigntimer = 0; }
            }
            Vector2 targetPos = transform.position;
            Vector3 dir = targetPos - (Vector2)bobber.transform.position;
            bobber.transform.up = dir;
            bobber.GetComponent<Rigidbody2D>().gravityScale = 0;
            if (bobber.GetComponent<Bobber>().inwater) bobber.GetComponent<Rigidbody2D>().AddForce(-bobber.transform.up * 0.375f * (fishstrength/2));
        }
        else if (reel && !bobber.GetComponent<Bobber>().inwater)
        {
            bobber.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            bobber.GetComponent<Rigidbody2D>().gravityScale = 0;
            if (enableextracam)
            {
                MainCamera.SetActive(true);
                BobberCamera.SetActive(false);
            }
            bobber.transform.position = bobberstart.transform.position;
            reelSign.SetActive(false);
            reel = false;
        }
        if (chargetime >= 3) { chargetime = 3; reel = false; }
        if (charging)
        {
            reel = false;
            if (hascast) 
            {
                bobber.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                if (enableextracam)
                {
                    MainCamera.SetActive(true);
                    BobberCamera.SetActive(false);
                }
            }
            bobber.GetComponent<Rigidbody2D>().gravityScale = 0;
            hascast = false;
            bobber.transform.position = bobberstart.transform.position;
            chargetime += Time.deltaTime;
            fishingrod.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (chargetime >= .5f)
        {
            reel = false;
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
        if (reel)
        {
            Vector2 targetPos = transform.position;
            Vector3 dir = targetPos - (Vector2)bobber.transform.position;
            bobber.transform.up = dir;
            bobber.GetComponent<Rigidbody2D>().AddForce(bobber.transform.up * 28);
        }
        else
        {
            if (context.performed)
            {
                charging = true;
            }
            else
            {
                charging = false;
            }
        }
    }
    private void Cast()
    {
        Vector2 targetPos = -transform.position;
        Vector3 dir = targetPos - (Vector2)bobber.transform.position;
        bobber.transform.up = dir;
        bobber.GetComponent<Rigidbody2D>().AddForce(bobber.transform.up * (chargetime * 20));
        bobber.GetComponent<Rigidbody2D>().gravityScale = 1;
        if (enableextracam)
        {
            MainCamera.SetActive(false);
            BobberCamera.SetActive(true);
        }
        hascast = true;
    }
    private void Fishing()
    {
        int temp = rng.Next(0, 1000000);
        if (temp > 10 && !reel)
        {
            int fish = rng.Next(0, Fish.Length);
            fishstrength = fish + 1;
            progressfish = Fish[fish];
            reel = true;
            reelSign.SetActive(true);
        }
    }
    public  void Inventory()
    {
        Debug.Log("Inventory");
        if (!inventorypanel.activeInHierarchy) inventorypanel.SetActive(true);
        else inventorypanel.SetActive(false);
    }
}
