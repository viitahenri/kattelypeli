﻿using UnityEngine;
using System.Collections;

public class GenerateLine : MonoBehaviour
{
    public enum LineState { MovingIn, ShakingHands, MovingOut }

    private LineState currentState = LineState.MovingIn;

    [SerializeField]
    private GameObject handShakeBehaviourPrefab;

    private HandShakeBehaviour handShakeBehaviour;

    [SerializeField]
    private GameObject[] guests;
    [SerializeField]
    private GameObject guestInstatiate;
    private GameObject guest;

    [SerializeField]
    private float speed = 5f;
    private float xPos = 1f;
    private float zPos = 0;
    private float scale = 1;
    private int x;
    private int y;

    private bool moving;

    bool isGuestMoving = false;
    float delayTimer = 0f;

    void ChangeState(LineState _newState)
    {
        currentState = _newState;

        switch (currentState)
        {
            case LineState.MovingIn:
                MoveTheLine();
                break;
            case LineState.ShakingHands:
                delayTimer = 0f;
                break;
            case LineState.MovingOut:
                MoveOut();
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        x = 0;
        moving = false;
        for (int i = 0; i < guests.Length; i++)
        {
            guest = (Instantiate(guestInstatiate, new Vector3(xPos, -6.5f, zPos), Quaternion.identity) as GameObject);
            guest.transform.position = new Vector3(guest.transform.position.x, guest.transform.position.y + guest.GetComponent<AnimateGuest>().yOffset, guest.transform.position.z);
            guests[i] = guest;
            xPos -= 2f;
            zPos = 5;
            guests[i].GetComponent<AnimateGuest>().moving = false;
        }

        GameObject hsbObj = Instantiate(handShakeBehaviourPrefab);
        handShakeBehaviour = hsbObj.GetComponent<HandShakeBehaviour>();
        handShakeBehaviour.gameObject.SetActive(false);

        ChangeState(LineState.MovingIn);
    }

    void Update()
    {
        //if (moving && Input.GetButtonDown("Fire2"))
        //{
        //    MoveOut();
        //}
        //if (Input.GetButtonDown("Fire2") && !moving)
        //{
        //    moving = true;
        //    print("I'm gonna see the president!!");
        //    MoveTheLine();
        //}

        switch (currentState)
        {
            case LineState.MovingIn:
                isGuestMoving = false;

                foreach(var guest in guests)
                {
                    if (guest.GetComponent<AnimateGuest>().moving)
                        isGuestMoving = true;
                }

                if (!isGuestMoving)
                {
                    // Start shaking with delay
                    if (delayTimer > 1f)
                    {
                        handShakeBehaviour.gameObject.SetActive(true);
                        handShakeBehaviour.StartHandShakeSequence(ShakingHandsFinished);
                        ChangeState(LineState.ShakingHands);
                    }
                    else
                        delayTimer += Time.deltaTime;
                }
                break;
            case LineState.ShakingHands:
                
                break;
            case LineState.MovingOut:
                isGuestMoving = false;

                foreach (var guest in guests)
                {
                    if (guest.GetComponent<AnimateGuest>().moving)
                        isGuestMoving = true;
                }
                if (!isGuestMoving)
                {
                    ChangeState(LineState.MovingIn);
                }
                break;
            default:
                break;
        }
    }

    void ShakingHandsFinished()
    {
        handShakeBehaviour.gameObject.SetActive(false);
        ChangeState(LineState.MovingOut);
    }

    void MoveTheLine()
    {
        guests[x].GetComponent<AnimateGuest>().target = new Vector3(5, guests[x].transform.position.y, guests[x].transform.position.z);

        xPos = 1;
        for (y = x + 1; y < guests.Length; y++)
        {
            guests[y].GetComponent<AnimateGuest>().target = new Vector3(xPos, guests[y].transform.position.y, guests[y].transform.position.z);
            xPos -= 2f;
        }
        if(x > 1 && x < guests.Length)
        {
            for (y = 0; y < x; y++)
            {
                guests[y].GetComponent<AnimateGuest>().target = new Vector3(xPos, guests[y].transform.position.y, guests[y].transform.position.z);
                xPos -= 2f;
            }
        }
    }

    void MoveOut()
    {
        guests[x].GetComponent<AnimateGuest>().target = new Vector3(15, guests[x].transform.position.y, guests[x].transform.position.z);

        print(guests[x].GetComponent<AnimateGuest>().target);

        if (guests[x].transform.position.x == guests[x].GetComponent<AnimateGuest>().target.x)
        {
            guests[x].transform.position = new Vector3(xPos, guests[x].transform.position.y, guests[x].transform.position.z);
            guests[x].GetComponent<AnimateGuest>().target = new Vector3(xPos, guests[x].transform.position.y, guests[x].transform.position.z);
            guests[x].GetComponentInChildren<GenerateCharacter>().Reroll();
            x++;
            moving = false;
            if(x >= guests.Length)
            {
                x = 0;
            }
        }
    }
}

