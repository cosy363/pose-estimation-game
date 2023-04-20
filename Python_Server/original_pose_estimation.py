import os

# 필요한 변수 생성

# 오픈포즈 저장된 주소(각자 폴더에 맞게 수정)
# 모든 주소의 \를 /로 바꿔주세요 \를 이스케이프 코드로 인식해서 폴더를 제대로 못찾아요
openpose_path = 'C:/Users/jwcho/Downloads/openpose/'

# 아래 3개는 수정X, original output의 경우 Output_Images와 Original_Output_Json 폴더를 만들어 저장하게함
model_path = openpose_path + 'models/'
output_image_path = openpose_path + 'Output_Images/'
original_output_json_path = openpose_path + 'Original_Output_Json/'

# original image file을 저장해둔 폴더 위치 입력
original_input_image_path = 'C:/Project_Image/Original_Image'

# ======= 1. 오픈포즈 데모로 json 파일 생성 =======

os.system(
    # Openpose 폴더로 가서 데모 실행
    openpose_path + 'bin/OpenPoseDemo.exe' +

    # 이미지가 저장된 폴더위치
    ' --image_dir ' + original_input_image_path +

    # 크기가 다르기 때문에 좌표를 모두 0에서 1 사이로 normalization
    ' --keypoint_scale 3' +

    # Out of Memory 방지로 화질 줄임
    ' --net_resolution -1x128 ' +

    # json으로 출력 + 저장할 파일 위치 설정
    ' --write_json ' + original_output_json_path +

    # 원본이미지에 스켈레톤 입혀서 출력 + 저장할 파일 위치 설정
    ' --write_images ' + output_image_path +

    # 실행 중에 이미지 출력안하도록 설정
    ' --display 0' +

    # 사용할 모델의 폴더 위치
    ' --model_folder ' + model_path
    )





