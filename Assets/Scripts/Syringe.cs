using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.Networking.NetworkSystem;

public class Syringe : MonoBehaviour
{

    public GameObject SyringePositionsParent;
    List<GameObject> syringePositions;
    bool moving = false;
    bool plunging = false;
    bool plunge = false;
    GameObject parent;
    public GameObject Plunger;

    // rotation and position movement
    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;
    private float moveLerpTime = 1f;
    private float moveCurrentLerpTime = 0f;

    // plunger details
    private Vector3 plungerStartPosition = new Vector3(0, 0, 0.00831f);
    private Vector3 plungerEndPosition = new Vector3(0, 0, -0.005948212f);
    private float plungeLerpTime = 1f;
    private float plungeCurrentLerpTime = 0f;

    // Use this for initialization
    void Start()
    {
        
        if (SyringePositionsParent == null)
        {
            SyringePositionsParent = GameObject.Find("SyringePositions");
        }

        syringePositions = new List<GameObject>();

        for (int i = 0; i < SyringePositionsParent.transform.childCount; i++)
        {
            syringePositions.Add(SyringePositionsParent.transform.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    //bool debug = true;
    //bool ranOnce = false;
    void Update()
    {
        //if (Time.realtimeSinceStartup > 10 && debug && !ranOnce)
        //{
        //    TogglePlunger(true);
        //    debug = false;
        //    ranOnce = true;
        //}
        //if (Time.realtimeSinceStartup > 20 && !debug)
        //{
        //    TogglePlunger(false);
        //    debug = true;
        //}

        if (moving)
        {
            moveCurrentLerpTime += Time.deltaTime;
            if (moveCurrentLerpTime > moveLerpTime)
            {
                moveCurrentLerpTime = moveLerpTime;
            }
            float t = moveCurrentLerpTime / moveLerpTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, t);

            if (t >= 0.98f)
            {
                moving = false;
                moveCurrentLerpTime = 0;
            }

        }


        if (plunging)
        {
            plungeCurrentLerpTime += Time.deltaTime;
            if (plungeCurrentLerpTime > plungeLerpTime)
            {
                plungeCurrentLerpTime = plungeLerpTime;
            }
            float t = plungeCurrentLerpTime / plungeLerpTime;
            if (plunge)
            {
                Plunger.transform.localPosition = Vector3.Lerp(plungerStartPosition, plungerEndPosition, t);
            }
            else
            {
                Plunger.transform.localPosition = Vector3.Lerp(plungerEndPosition, plungerStartPosition, t);
            }

            if (t >= 0.98f)
            {
                plunging = false;
                plungeCurrentLerpTime = 0;
            }
        }
    }

    public void MoveToPositionIndex(int index)
    {
        GameObject target = syringePositions[index];

        if (parent == null)
        {
            parent = target;
        }
        else
        {
            if (target == parent)
            {
                return;
            }

            parent = target;
        }

        transform.SetParent(parent.transform, true);

        moving = true;


    }

    public void TogglePlunger(bool toggle)
    {
        if (plunge != toggle && !plunging)
        {
            plunge = toggle;
            plunging = true;
        }

    }
}
