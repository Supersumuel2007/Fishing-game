using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class String : MonoBehaviour
{
    [SerializeField] GameObject bobber;
    [SerializeField] GameObject rod;
    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(bobber.transform.position, rod.transform.position);
        transform.localScale = new Vector3(transform.localScale.x, distance);
        Vector3 location = (bobber.transform.position + rod.transform.position) / 2;
        transform.SetPositionAndRotation(location, Quaternion.identity);
        Vector2 targetPos = rod.transform.position;
        Vector3 dir = targetPos - (Vector2)transform.position;
        transform.up = dir;
    }
}
