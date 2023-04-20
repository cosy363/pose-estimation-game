import os
import json
import numpy as np
from numpy.linalg import norm
import socket
import time
import sys
import select

# 필요한 변수 생성

# 오픈포즈 저장된 주소(각자 폴더에 맞게 수정)
openpose_path = 'C:/Users/jwcho/Downloads/openpose/'

# 수정X
model_path = openpose_path + 'models/'
output_image_path = openpose_path + 'Output_Images/'
user_output_json_path = openpose_path + 'User_Output_Json/'
original_output_json_path = openpose_path + 'Original_Output_Json/'

# 각자 원하는 값으로 설정 (단, 유니티와 맞춰야함)
port_unity_easy = 8000
port_unity_hard = 8001

# 필요한 함수 생성

# ======= json 파일에서 좌표+확률이 있는 list 꺼내는 함수 =======
def make_json_to_dict(image, judge):

    # json 파일에 여러 정보다 딕셔너리 형태로 저장되는데 그 중 2d keypoints 정보의 key 값
    keypoints = 'pose_keypoints_2d'

    if judge == 0:
        output_json_path = user_output_json_path
    else:
        output_json_path = original_output_json_path

    # json 파일을 열어 파일 전체를 딕셔너리로 저장
    with open(f'{output_json_path}{image}_keypoints.json') as f:
        json_object = json.load(f)

    # 파일 전체가 담긴 딕셔너리에서 keypoint 정보가 담긴 부분만 list로 저장
    arr = json_object['people'][0][keypoints]

    # keypoint가 담긴 list에 좌표의 확률정보도 담겨있는데 이를 걸러내고 좌표만 저장
    keypoint_list = []

    for i in range(25):
        point_vector = [arr[3 * i], arr[3 * i + 1]]
        keypoint_list.append(point_vector)

    f.close()
    return keypoint_list


# ======= 유사도 검사하는 함수 =======
def calc_sim(user, original):
    # 확률까지 있으니까 좌표만 뽑아냄
    user_keypoint = [user[0], user[1]]
    original_keypoint = [original[0], original[1]]

    # 제대로 측정이 안되서 (x, y, 확률)이 (0,0,0)인 점이 있는데 이러면 그냥 유사도 0으로 처리함
    # 임의로 정하긴 했는데 혹시 해결책이 있을까요...
    if 0 in user_keypoint or 0 in original_keypoint:
        return 0
    else:
        # 두 벡터의 크기 구함
        user_norm = norm(user_keypoint)
        original_norm = norm(original_keypoint)

        # 코사인 유사도를 구함
        cos_sim = np.dot(user_keypoint, original_keypoint) / (user_norm * original_norm)

        # 길이의 유사도를 구함
        # original의 길이에서 user와 original의 차이의 절댓값을 뺌
        # 이러면 user가 original보다 길어도 결과값이 0~1의 양수가 나옴
        len_sim = abs(original_norm - abs(user_norm - original_norm)) / original_norm

        # 코사인 유사도와 길이의 유사도를 곱한 값을 리턴
        return cos_sim * len_sim


# ======= 안드로이드 서버에서 사진 받는 함수 =======
def receive_from_android(host, port_android, user_input_image_path):

    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port_android))
    server.listen(5)

    count = 0
    print("Waiting for Android connection...")

    while True:

        #   클라이언트 요청 대기
        client_socket, address = server.accept()
        print("Connection success from ", str(address))

        data = None

        #   데이터 수신
        while True:
            img_data = client_socket.recv(1024)
            data = img_data

            if img_data:
                print("recving Img...", end='')
                while img_data:
                    print(".", end='')
                    img_data = client_socket.recv(1024)
                    data += img_data
                else:
                    print('')
                    break

        # 받은 데이터 저장
        time1 = time.time()
        myfile = user_input_image_path + str(time1).split('.')[0]
        print(myfile)

        myfile = myfile + ".jpg"

        img_file = open(myfile, "wb")

        print("finish img recv")
        print(f'size is {sys.getsizeof(data)}')

        img_file.write(data)
        img_file.close()
        client_socket.close()

        print("Finish\n")
        break


# ======= 오픈포즈 데모로 json 파일 생성 =======
def make_image_to_json(user_input_image_path):
    os.system(
        # Openpose 폴더로 가서 데모 실행
        openpose_path + 'bin/OpenPoseDemo.exe' +

        # 이미지가 저장된 폴더위치
        ' --image_dir ' + user_input_image_path +

        # 크기가 다르기 때문에 좌표를 모두 0에서 1 사이로 normalization
        ' --keypoint_scale 3' +

        # Out of Memory 방지로 화질 줄임
        ' --net_resolution -1x128 ' +

        # json으로 출력 + 저장할 파일 위치 설정
        ' --write_json ' + user_output_json_path +

        # 원본이미지에 스켈레톤 입혀서 출력 + 저장할 파일 위치 설정
        ' --write_images ' + output_image_path +

        # 실행 중에 이미지 출력안하도록 설정
        ' --display 0' +

        # 사용할 모델의 폴더 위치
        ' --model_folder ' + model_path
    )


# ======= 유사도 계산하는 함수 =======
def calculate_similarity(keypoint_list, stage):
    # original image의 keypoint list 만들기
    original_image_name = f'original{stage}'
    original_keypoint_list = make_json_to_dict(original_image_name, 1)

    similarity_list = []

    # 각 좌표마다 similarity를 구함
    for i in range(15):
        similarity_list.append(calc_sim(keypoint_list[i], original_keypoint_list[i]))

    # 구한 좌표마다의 similarity의 평균을 구함
    # 해보니까 같은 동작이면 0.95이상은 나오는것 같더라고요
    similarity = sum(similarity_list) / 15
    print(f'similarity : {similarity}\n')
    return similarity


# ======= 좌표값과 유사도를 문자열로 저장하는 함수 ======
def make_point_to_string(keypoint_list, similarity):
    # x와 y는 ','로 구분하고 좌표끼리는 띄어쓰기로 구분해서 저장(유니티 스크립트에서 .split() 함수 사용하기 편하도록 설정함)
    serversend = ""

    for i in range(25):
        x = str(keypoint_list[i][0])
        y = str(keypoint_list[i][1])
        # f.write(f'{x},{y} ')
        serversend += f'{x},{y} '

    # 계산한 similiraty 저장, '\n'로 좌표와 구분
    # f.write(f'\n{similarity}')
    serversend += f'\n{similarity}'
    return serversend


# ======= 저장한 문자열을 유니티 서버로 넘기는 함수 =======
def send_string_to_unity(host, serversend, stage):
    print('Waiting for Unity connection...')

    # 소켓 두개를 열어줌
    server_socket1 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket1.bind((host, port_unity_easy))
    server_socket1.listen()

    server_socket2 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket2.bind((host, port_unity_hard))
    server_socket2.listen()

    # 소켓 list 생성
    socket_list = [server_socket1, server_socket2]

    while True:
        # select 함수를 통해 어떤 소켓에 클라이언트가 접속하는지 확인
        readable_socket_list, writeables, exeptions = select.select(socket_list, [], [])

        for connected_socket in readable_socket_list:
            # 접속한 소켓에 따라 easy와 hard모드 나눔
            if connected_socket == server_socket1:
                print('hello easy mode')
                client_socket, client_addr = server_socket1.accept()
                print('Connected by', client_addr)

                msg = str(serversend)
                client_socket.sendall(msg.encode())
                print(f'send 완료\n{str(serversend)}\n')
                return port_unity_hard

            elif connected_socket == server_socket2:
                print('hello hard mode')
                client_socket, client_addr = server_socket2.accept()
                print('Connected by', client_addr)

                msg = str(serversend)
                client_socket.sendall(msg.encode())
                print(f'send 완료\n{str(serversend)}\n')
                return port_unity_easy


# ======= 스테이지 설정 함수 =======
def set_stage(similarity, difficulty, stage):

    # difficulty에 따라 threshold 변화
    if difficulty == port_unity_hard:
        threshold = 0.9
    elif difficulty == port_unity_easy:
        threshold = 0.8

    if similarity > threshold:
        stage += 1
    else:
        stage = 1

    return stage