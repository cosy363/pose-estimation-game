using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.Net.Sockets; 
using UnityEngine; 
using System.Text; 
using System; 
using System.IO; 
using System.Runtime.InteropServices;


public class Servermanager : MonoBehaviour
{
    TcpClient client; 
    // 1. serverIp를 pose_estimation에 있는 ip와 동일하게 설정해주세요
    string serverIP = "172.30.1.10"; 
    int port = 8000; byte[] receivedBuffer; 
    StreamReader reader; 
    bool socketReady = false; 
    NetworkStream stream;
    public static string serverdata;    // 전역 변수 선언(pose.cs와 연동되게끔)

    // Start is called before the first frame update
    void Start()
    {
        CheckReceive();
    }

    // Update is called once per frame
    void Update()
    {
        if(socketReady) 
        { 
            if (stream.DataAvailable) 
            { 
                receivedBuffer = new byte[10000]; // 2. 버퍼의 값을 조절하여 속도를 조절할 수 있습니다
                stream.Read(receivedBuffer, 0, receivedBuffer.Length); 
                string data = Encoding.UTF8.GetString(receivedBuffer, 0, receivedBuffer.Length);
                Debug.Log(data); //Unity Log에 data의 값이 출력되게 됩니다
                serverdata = data; //전역 변수 serverdata에 data값을 입력하여 pose.cs와 연동 가능하게끔 합니다
            } 
        }

    }

    void CheckReceive() 
    { 
    if (socketReady) return; 
    try 
        { client = new TcpClient(serverIP, port); 
        if (client.Connected) 
            { stream = client.GetStream(); 
            Debug.Log("Connect Success"); 
            socketReady = true; } 
        } 
    catch (Exception e) 
        { Debug.Log("On client connect exception " + e); } 
    } 
    
    void OnApplicationQuit() 
    { 
        CloseSocket(); 
    } 
        
    void CloseSocket() 
    { 
        if (!socketReady) return; 
        reader.Close(); 
        client.Close(); 
        socketReady = false; 
    }






}
