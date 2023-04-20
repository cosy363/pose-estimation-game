====== 12.8 추가 ======
1. 변수 설정
 1) Pose_Estimation.py
    - Line 13 : openpose_path
    - Line 22 : port_unity_easy    -> easy 모드에서 사용할 port번호 지정 (유니티와 같은 값 지정)
    - Line 22 : port_unity_hard    -> hard 모드에서 사용할 port번호 지정 (유니티와 같은 값 지정)
    - Line 249, 251 : threshold    -> easy 모드와 hard 모드의 threshold 값을 설정
 
2) python_server.py
    - Line 9 : user_input_image_path
    - Line 12 : host
    - Line 15 : port_android