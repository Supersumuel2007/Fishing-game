using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI caughttext;
    [SerializeField] GameObject[] caughtfishicons;
    [SerializeField] GameObject[] inventoryslots;
    [SerializeField] GameObject[] fishicons;
    [SerializeField] GameObject fishingrod;
    [SerializeField] GameObject bobber;
    [SerializeField] GameObject bobberstart;
    [SerializeField] GameObject maincamera;
    [SerializeField] GameObject bobbercamera;
    [SerializeField] GameObject reelSign;
    [SerializeField] GameObject inventorypanel;
    [SerializeField] GameObject caughtpanel;
    [SerializeField] bool enableextracam;
    public string[] caughtfish;
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
        for (int i = 0; i < caughtfish.Length; i++)
        {
            caughtfish[i] = null;
        }
    }
    private void Update()
    {
        if (bobber.GetComponent<Bobber>().inwater)
        {
            reeltimer += Time.deltaTime;
            if (reeltimer >= 2) { Fishing(); reeltimer = 0; }
        }
        else if (progressfish != null) 
        {
            for (int i = 0; i < caughtfish.Length; i++)
            {
                if (caughtfish[i] == null)
                {
                    caughtfish[i] = progressfish;
                    caughttext.text = progressfish;
                    caughtpanel.SetActive(true);
                    if (progressfish == Fish[0]) { caughtfishicons[0].SetActive(true); }
                    else if (progressfish == Fish[1]) { caughtfishicons[1].SetActive(true); }
                    else if (progressfish == Fish[2]) { caughtfishicons[2].SetActive(true); }
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
                maincamera.SetActive(true);
                bobbercamera.SetActive(false);
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
                    maincamera.SetActive(true);
                    bobbercamera.SetActive(false);
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
        if (caughtpanel.activeInHierarchy) caughtpanel.SetActive(false);
        for (int i = 0; i < caughtfishicons.Length; i++)
        {
            caughtfishicons[i].SetActive(false);
        }
        int temp = 0;
        for (int i = 0; i < caughtfish.Length; i++)
        {
            if (caughtfish[i] == null) temp++;
        }
        if (temp >= caughtfish.Length)
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
            maincamera.SetActive(false);
            bobbercamera.SetActive(true);
        }
        hascast = true;
    }
    private void Fishing()
    {
        if (rng.Next(0, 10000) > 1 && !reel)
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
        if (!inventorypanel.activeInHierarchy)
        {
            inventorypanel.SetActive(true);
            for (int i = 0; i < caughtfish.Length; i++)
            {
                if ( i % 2 == 1)
                {
                    if (caughtfish[i] == Fish[0])
                    {
                        Instantiate(fishicons[0], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y + (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                    else if (caughtfish[i] == Fish[1])
                    {
                        Instantiate(fishicons[1], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y + (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                    else if (caughtfish[i] == Fish[2])
                    {
                        Instantiate(fishicons[2], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y + (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                }
                else
                {
                    if (caughtfish[i] == Fish[0])
                    {
                        Instantiate(fishicons[0], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y - (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                    else if (caughtfish[i] == Fish[1])
                    {
                        Instantiate(fishicons[1], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y - (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                    else if (caughtfish[i] == Fish[2])
                    {
                        Instantiate(fishicons[2], new Vector2(inventoryslots[i].transform.position.x + (192 / 2), inventoryslots[i].transform.position.y - (192 / 2)), Quaternion.identity, inventoryslots[i].transform);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < inventoryslots.Length; i++)
            {
                try { Destroy(inventoryslots[i].transform.GetChild(0).gameObject); }
                catch { }
            }
            inventorypanel.SetActive(false);
        }
    }
}
