﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void LoadNARSPong()
    {
        SceneManager.LoadScene("NARS_Pong");
    }

    public void LoadPlayerVsNARS()
    {
        SceneManager.LoadScene("Player_Vs_NARS");
    }
}
