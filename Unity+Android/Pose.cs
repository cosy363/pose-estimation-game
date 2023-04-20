using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class Pose : MonoBehaviour
{
    public GameObject spine2;
    public GameObject model;

    float[,] arr = new float[25, 2];   // txt파일로 받은 original 관절좌표
    float similarity;                  // txt파일로 받은 유사도값
    GameObject[] obj = new GameObject[25];


    void Start()
    {


    }

    void GetData()
    {

        string txt = Servermanager.serverdata;
        string[] point = txt.Split(' ');

        for (int i = 0; i < 25; i++)
        {
            string[] a = point[i].Split(',');
            arr[i, 0] = float.Parse(a[0]);
            arr[i, 1] = float.Parse(a[1]);
        }

        similarity = float.Parse(txt.Split('\n')[1]);
    }

    void SetBallPosition()
    {
        Vector3 Hip = new Vector3(0, (float)0.95, -3);
        spine2 = GameObject.Find("LowManSpine2");

        float x = arr[8, 0];
        float y = (-1) * arr[8, 1];

        float x_offset = Hip.x - x; //인체의 중심인 Hip을 기준으로 좌표를 재설정
        float y_offset = Hip.y - y;


        for (int i = 0; i < 25; i++)
        {
            x = arr[i, 0] + x_offset;
            y = (-1 * arr[i, 1]) + y_offset;

            string num = i.ToString();
            if (obj[i] = GameObject.Find(num))
            {
                obj[i].transform.position = new Vector3(x, y, -3);
            }
            else continue;

        }


        //////////// ************** Hip to Head normalization *********************************************

        Vector3 startToTarget_Vector = obj[1].transform.position - obj[8].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        float startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        Vector3 startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        float avatar_Distance = (new Vector3(0, (float)0.5, 0)).sqrMagnitude; // Avatar관절사이의 거리(거리조절의 목표거리)
        float difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        Vector3 adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터


        List<int> hipToHead = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 0, 17, 15, 16, 18 };
        foreach (int i in hipToHead)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }

        hipToHead.RemoveRange(0,7);

        startToTarget_Vector = obj[0].transform.position - obj[1].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.15; // Avatar관절사이의 거리(거리조절의 목표거리)
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in hipToHead)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }


        //////////// ************ Left Arm noramlization ******************************************

        List<int> NectToHand_L = new List<int>() { 2, 3, 4};

        startToTarget_Vector = obj[2].transform.position - obj[1].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.05; // Spine To shoulder
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        NectToHand_L.Remove(2);

        startToTarget_Vector = obj[3].transform.position - obj[2].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.12; // Shoulder To elbow
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        NectToHand_L.Remove(3);

        startToTarget_Vector = obj[4].transform.position - obj[3].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.26; //  elbow to hand
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }

        ////////////// ************ Right Arm noramlization *************************************************

        List<int> NectToHand_R = new List<int>() { 5, 6, 7 };

        startToTarget_Vector = obj[5].transform.position - obj[1].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.05; // Spine To shoulder
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        NectToHand_R.Remove(5);

        startToTarget_Vector = obj[6].transform.position - obj[5].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.12; // Shoulder To elbow
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        NectToHand_R.Remove(6);

        startToTarget_Vector = obj[7].transform.position - obj[6].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.26; //  elbow to hand
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in NectToHand_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }

        /////////////// ************ Left Leg noramlization *********************************

        List<int> HipToFoot_L = new List<int>() { 9, 10, 11, 22, 23, 24 };

        startToTarget_Vector = obj[9].transform.position - obj[8].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.05; // Hip To Left_hip
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        HipToFoot_L.Remove(9);

        startToTarget_Vector = obj[10].transform.position - obj[9].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.3; // Shoulder To elbow
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        HipToFoot_L.Remove(10);

        startToTarget_Vector = obj[11].transform.position - obj[10].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.26; //  knee to foot
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_L)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }

        /////////////// ************ Rigt Leg noramlization *********************************

        List<int> HipToFoot_R = new List<int>() { 12, 13, 14, 19, 20, 21 };

        startToTarget_Vector = obj[12].transform.position - obj[8].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.05; // Hip To Left_hip
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        HipToFoot_R.Remove(12);

        startToTarget_Vector = obj[13].transform.position - obj[12].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.3; // Shoulder To elbow
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }
        HipToFoot_R.Remove(13);

        startToTarget_Vector = obj[14].transform.position - obj[13].transform.position; // 기준의 볼에서 거리조절되어야할 볼까지 가리키는 벡터
        startToTarget_Distance = startToTarget_Vector.sqrMagnitude; // 기준의 볼에서 거리조절되어야할 볼까지의 현재거리
        startToTarget_Direction = startToTarget_Vector.normalized; // 기준의 볼에서 거리조절되어야할 볼까지의 방향벡터
        avatar_Distance = (float)0.26; //   knee to foot
        difference = avatar_Distance - startToTarget_Distance; // 조절되어야할 거리의 길이
        adjusting = startToTarget_Direction * difference; // 방향벡터방향*조절되어야할 크기로 이루어진 벡터

        foreach (int i in HipToFoot_R)
        {
            obj[i].transform.position = obj[i].transform.position + adjusting;
        }

    }

    // Start is called before the first frame update
    void Update()
    {
        GetData();
        SetBallPosition();

        
        if (similarity > 0.75)
        {
            GameManager.instance.isSimilar = true;
        }
    }
}
