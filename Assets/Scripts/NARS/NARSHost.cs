﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.UI;

public class NARSHost : MonoBehaviour
{
    public enum NARSType : int
    {
        NARS, ONA
    }

    public NARSType type;
    NARSSensorimotor _sensorimotor;
    Process process = null;
    StreamWriter messageStream;

    //UI output text
    public OperationOutputGUI UIOutput;
    string lastOperationTextForUI = "";
    bool operationUpdated = false;

    //Babbling
    float babbleTimer = 0;
    [SerializeField] int babblesRemaining;
  
    private void Start()
    {
        Application.targetFrameRate = 60;
        babblesRemaining = 90;
        switch (type)
        {
            case NARSType.NARS:
                LaunchNARS();
                break;
            case NARSType.ONA:
                LaunchONA();
                break;
            default:
                break;
        }

        _sensorimotor = GetComponent<NARSSensorimotor>();
        _sensorimotor.SetNARSHost(this);
    }

    private NARSSensorimotor GetSensorimotor()
    {
        return _sensorimotor;
    }

    private void Update()
    {
        if (operationUpdated)
        {
            string text = "";
            switch (type)
            {
                case NARSType.NARS:
                    text = "NARS Operation:\n";
                    break;
                case NARSType.ONA:
                    text = "ONA Operation:\n";
                    break;
                default:
                    break;
            }
            text += lastOperationTextForUI;
            UIOutput.SetOutputText(text);
            operationUpdated = false;
        }

        if (type == NARSType.NARS)
        {
            babbleTimer -= Time.deltaTime;
            if (babblesRemaining > 0 && babbleTimer <= 0f)
            {
                Babble();
                babbleTimer = 1.0f;
                babblesRemaining--;
            }
        }
    }

    void Babble()
    {
        int randInt = Random.Range(1, 4);
        string input = "";

        if (randInt == 1)
        {
            GetSensorimotor().MoveRight();

            lastOperationTextForUI = "(babble) ^right";
            operationUpdated = true;

            input = "<(*,{SELF}) --> ^right>. :|:";
        }
        else if (randInt == 2)
        {
            GetSensorimotor().MoveLeft();

            lastOperationTextForUI = "(babble) ^left";
            operationUpdated = true;

            input = "<(*,{SELF}) --> ^left>. :|:";
        }
        else if (randInt == 3)
        {
            GetSensorimotor().DontMove();

            lastOperationTextForUI = "(babble) ^deactivate";
            operationUpdated = true;

            input = "<(*,{SELF}) --> ^deactivate>. :|:";
        }

        if (input != "")
        {
            this.AddInput(input);
        }
    }

    public void LaunchONA()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(@"cmd.exe");
        startInfo.WorkingDirectory = Application.dataPath + @"\NARS";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += new DataReceivedEventHandler(ONAOutputReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.StandardInput.WriteLine("NAR shell");
        process.StandardInput.Flush();

        messageStream = process.StandardInput;
        AddInput("*volume=0");
    }

    public void LaunchNARS()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
        startInfo.WorkingDirectory = Application.dataPath + @"\NARS";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += new DataReceivedEventHandler(NARSOutputReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.StandardInput.WriteLine("java -Xmx1024m -jar opennars.jar");
        process.StandardInput.Flush();

        messageStream = process.StandardInput;
        AddInput("*volume=0");
    }

    public void AddInferenceCycles(int cycles)
    {
        AddInput("" + cycles);
    }

    public void AddInput(string message)
    {
        //UnityEngine.Debug.Log("SENDING INPUT: " + message);
        messageStream.WriteLine(message);
    }
    void NARSOutputReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.Log(eventArgs.Data);
        if (eventArgs.Data.Contains("EXE:")) //operation executed
        {
            //UnityEngine.Debug.Log("RECEIVED OUTPUT: " + eventArgs.Data);
            int length = eventArgs.Data.IndexOf("(") - eventArgs.Data.IndexOf("^");
            string operation = eventArgs.Data.Substring(eventArgs.Data.IndexOf("^"), length);
            //UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);

            if (operation == "^left")
            {
                GetSensorimotor().MoveLeft();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
            else if (operation == "^right")
            {
                GetSensorimotor().MoveRight();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
            else if (operation == "^deactivate")
            {
                GetSensorimotor().DontMove();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
        }

    }

    void ONAOutputReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        //UnityEngine.Debug.Log(eventArgs.Data);
        if (eventArgs.Data.Contains("executed with args")) //operation executed
        {
            string operation = eventArgs.Data.Split(' ')[0];
          
            if (operation == "^left")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);

                GetSensorimotor().MoveLeft();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
            else if(operation == "^right")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);

                GetSensorimotor().MoveRight();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
            else if (operation == "^deactivate")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);

                GetSensorimotor().DontMove();

                lastOperationTextForUI = operation;
                operationUpdated = true;
            }
        }
        
    }

    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    void OnApplicationQuit()
    {
        if (process != null || !process.HasExited )
        {
            process.CloseMainWindow();
        }
    }

}



