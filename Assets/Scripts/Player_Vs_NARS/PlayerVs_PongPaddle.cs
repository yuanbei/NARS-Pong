﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVs_PongPaddle : NARSPong_PongPaddle
{
    public bool isPlayer = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isPlayer)
        {
            GetPlayerInput();
        }

        base.Update();
    }

    void GetPlayerInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            goLeft = true;
            goRight = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            goLeft = false;
            goRight = true;
        }
        else
        {
            goLeft = false;
            goRight = false;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!isPlayer && collision.collider.tag == PongBall.TAG)
        {
            PlayerVs_GameManager.GetInstance().AddNARSBlock(GetSensorimotor().GetNARSHost().type.ToString());

            if (GetSensorimotor() != null)
            {
                GetSensorimotor().Praise();
            }
        }
    }
}
