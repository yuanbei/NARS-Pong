﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NARSPong_PongPaddle : MonoBehaviour
{
    public static string TAG = "Paddle";
    public bool goLeft, goRight;

    protected NARSSensorimotor _sensorimotor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (goLeft)
        {
            MoveLeft();
        }
        else if (goRight)
        {
            MoveRight();
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    public void SetSensorimotor(NARSSensorimotor sensorimotor)
    {
        _sensorimotor = sensorimotor;
    }

    public NARSSensorimotor GetSensorimotor()
    {
        return _sensorimotor;
    }

    public void MoveLeft()
    {
        GetComponent<Rigidbody>().velocity = -20 * transform.right;
    }

    public void MoveRight()
    {
        GetComponent<Rigidbody>().velocity = 20 * transform.right;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == PongBall.TAG)
        {
            NARSPong_GameManager.GetInstance().AddBlock();
            if(GetSensorimotor() != null)
            {
                GetSensorimotor().Praise();
            }
          
        }
    }

}
