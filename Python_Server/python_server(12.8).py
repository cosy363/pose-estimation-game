import os

import time

import Pose_Estimation as Pose

# ======================= 필요한 변수 생성 =======================
# 서버에서 받을 이미지 파일 저장 경로 입력
user_input_image_path = 'C:/Project_Image/Server_Image/'

# 본인 ip 주소 입력
host = '172.30.1.7'

# 안드로이드에 입력한 포트와 같은 포트 입력
port_android = 7002

# =============================================================================================
# 처음 실행때는 stage 1에서 시작(수정X)
stage = 1

while True:

    # stage 구분을 위해 일단 추가
    print(f'This is stage {stage}')

    # ======= 0. 안드로이드 서버에서 사진 받기 =======
    Pose.receive_from_android(host, port_android, user_input_image_path)

    # ======= 1. Image가 들어오기 전에 구동되지 않게 설정 =======
    while True:
        if os.path.exists(user_input_image_path):
            user_image_name_str = os.listdir(user_input_image_path)[0]
            user_image_name = user_image_name_str.rstrip('.jpg')
            break
        else:
            print('There is no image file')
            time.sleep(0.5)  # 구동환경에 따라 조절 가능

    # ======= 2. 오픈포즈 데모로 json 파일 생성 =======
    Pose.make_image_to_json(user_input_image_path)

    # ======= 3. json 파일에서 좌표값 추출 =======
    keypoint_list = Pose.make_json_to_dict(user_image_name, 0)

    # ======= 4. 자세 비교해 유사도로 저장 =======
    similarity = Pose.calculate_similarity(keypoint_list, stage)

    # ======= 5. 좌표값 + 유사도를 문자열로 저장 ======
    serversend = Pose.make_point_to_string(keypoint_list, similarity)
    os.remove(f'{user_input_image_path}/{user_image_name_str}')

    # ======= 6. 저장한 문자열 유니티 서버로 넘겨주기 + 난이도 받아오기 =======
    difficulty = Pose.send_string_to_unity(host, serversend)

    # ======= 7. stage 끝났으니까 stage 재설정 =======
    stage = Pose.set_stage(similarity, difficulty, stage)

