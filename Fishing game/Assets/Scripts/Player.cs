using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI caughttext;
    [SerializeField] TextMeshProUGUI timetext;
    [SerializeField] TextMeshProUGUI daytext;
    [SerializeField] TextMeshProUGUI qoutatext;
    [SerializeField] TextMeshProUGUI moneytext;
    [SerializeField] GameObject[] caughtfishicons;
    [SerializeField] GameObject[] inventoryslots;
    [SerializeField] GameObject[] fishicons;
    [SerializeField] GameObject fishingrod;
    [SerializeField] GameObject bobber;
    [SerializeField] GameObject bobberstart;
    [SerializeField] GameObject reelSign;
    [SerializeField] GameObject reelSign2;
    [SerializeField] GameObject inventorypanel;
    [SerializeField] GameObject caughtpanel;
    [SerializeField] GameObject settingspanel;
    [SerializeField] GameObject shoppanel;
    [SerializeField] GameObject maincamera;
    [SerializeField] GameObject secondarycamera;
    public string[] caughtfish;
    public string[] Fish;
    public string progressfish = null;
    public float chargetime;
    float allowedtime = 0;
    float reelsigntimer;
    float reeltimer;
    float fishstrength;
    float days = 1;
    float hours = 0;
    float minutes = 0;
    float money = 0;
    float qouta;
    public bool enableextracam;
    bool reel;
    bool charging;
    bool hascast;
    System.Random rng;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < caughtfishicons.Length; i++)
        {
            caughtfishicons[i].SetActive(false);
        }
        rng = new System.Random();
        bobberstart.transform.position = bobber.transform.position;
        progressfish = null;
        for (int i = 0; i < caughtfish.Length; i++)
        {
            caughtfish[i] = null;
        }
        qouta = 57.12f;
    }
    private void Update()
    {
        if (hascast && bobber.transform.position == bobberstart.transform.position)
        {
            hascast = false;
        }
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
                    if (enableextracam)
                    {
                        maincamera.SetActive(true);
                        secondarycamera.SetActive(false);
                    }
                    caughtfish[i] = progressfish;
                    caughttext.text = progressfish;
                    caughtpanel.SetActive(true);
                    if (progressfish == Fish[0]) { caughtfishicons[0].SetActive(true); }
                    else if (progressfish == Fish[1]) { caughtfishicons[1].SetActive(true); }
                    else if (progressfish == Fish[2]) { caughtfishicons[2].SetActive(true); }
                    i = caughtfish.Length;
                    reelsigntimer = 0;
                }
            }
            progressfish = null;
        }
        if (reel && bobber.GetComponent<Bobber>().inwater)
        {
            if (enableextracam)
            {
                if (reelSign2.activeInHierarchy)
                {
                    reelsigntimer += Time.deltaTime;
                    if (reelsigntimer >= 2) { reelSign2.SetActive(false); reelsigntimer = 0; }
                }
                else
                {
                    reelsigntimer += Time.deltaTime;
                    if (reelsigntimer >= 1) { reelSign2.SetActive(true); reelsigntimer = 0; }
                }
            }
            else
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
            bobber.transform.position = bobberstart.transform.position;
            reelSign.SetActive(false);
            reelSign2.SetActive(false);
            reel = false;
        }
        if (chargetime >= 3) { chargetime = 3; reel = false; }
        if (charging)
        {
            reel = false;
            if (hascast) 
            {
                bobber.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
        moneytext.text = "Money: " + money;
        qoutatext.text = "Qouta: " + qouta;
    }
    private void FixedUpdate()
    {
        minutes += Time.deltaTime;
        if (hours >= 24)
        {
            if (money > qouta)
            {
                Exit();
            }
            days++;
            qouta *= days / 2;
            hours = 0;
        }
        if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }
        if (hours > 0)
        {
            if (minutes < 10 && hours < 10) { timetext.text = "Time: 0" + (int)hours + ":0" + (int)minutes; }
            else if (hours < 10) { timetext.text = "Time: 0" + (int)hours + ":" + (int)minutes; }
            else if (minutes < 10) { timetext.text = "Time: " + (int)hours + ":0" + (int)minutes; }
            else { timetext.text = "Time: " + (int)hours + ":" + (int)minutes; }
        }
        else
        {
            if (minutes < 10) { timetext.text = "Time: 00:0" + (int)minutes; }
            else { timetext.text = "Time: 00:" + (int)minutes; }
        }
        daytext.text = "Day: " + days;
    }
    public void ChargeandReel(InputAction.CallbackContext context)
    {

        if (caughtpanel.activeInHierarchy) 
        {
            for (int i = 0; i < caughtfishicons.Length; i++)
            {
                caughtfishicons[i].SetActive(false);
            }
            caughtpanel.SetActive(false); 
        }
        int temp = 0;
        for (int i = 0; i < caughtfish.Length; i++)
        {
            if (caughtfish[i] == null) temp++;
        }
        if (temp <= caughtfish.Length)
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
                charging = context.performed;
            }
        }
    }
    private void Cast()
    {
        if (enableextracam)
        {
            maincamera.SetActive(false);
            secondarycamera.SetActive(true);
        }
        Vector2 targetPos = -transform.position;
        Vector3 dir = targetPos - (Vector2)bobber.transform.position;
        bobber.transform.up = dir;
        bobber.GetComponent<Rigidbody2D>().AddForce(bobber.transform.up * (chargetime * 20));
        bobber.GetComponent<Rigidbody2D>().gravityScale = 1;
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
            if (enableextracam) reelSign2.SetActive(true);
            else reelSign.SetActive(true);
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
    #region settingsmenu
    public void Settings()
    {
        if (settingspanel.activeInHierarchy) settingspanel.SetActive(false);
        else settingspanel.SetActive(true);
    }
    public void Resetdata()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void extracameratoggle()
    {
        enableextracam = !enableextracam;
    }
    #endregion
    #region shop
    public void Shop()
    {
        if (shoppanel.activeInHierarchy) shoppanel.SetActive(false);
        else shoppanel.SetActive(true);
    }
    public void Sellfish0()
    {
        for (int i = 0; i < caughtfish.Length; i++)
        {
            if (caughtfish[i] == Fish[0])
            {
                caughtfish[i] = "";
                money += 6;
                i = caughtfish.Length;
            }
        }
    }
    public void Sellfish1()
    {
        for (int i = 0; i < caughtfish.Length; i++)
        {
            if (caughtfish[i] == Fish[1])
            {
                caughtfish[i] = "";
                money += 7.5f;
                i = caughtfish.Length;
            }
        }
    }
    public void Sellfish2()
    {
        for (int i = 0; i < caughtfish.Length; i++)
        {
            if (caughtfish[i] == Fish[2])
            {
                caughtfish[i] = "";
                money += 9;
                i = caughtfish.Length;
            }
        }
    }
    #endregion
}