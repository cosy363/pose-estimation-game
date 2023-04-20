package project.eleven.mylibrary;


import android.annotation.SuppressLint;
import android.content.Context;
import android.view.Display;
import android.view.WindowManager;
import android.widget.Toast;
import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.graphics.ImageFormat;
import android.graphics.SurfaceTexture;
import android.hardware.camera2.CameraAccessException;
import android.hardware.camera2.CameraCaptureSession;
import android.hardware.camera2.CameraCharacteristics;
import android.hardware.camera2.CameraDevice;
import android.hardware.camera2.CameraManager;
import android.hardware.camera2.CameraMetadata;
import android.hardware.camera2.CaptureRequest;
import android.hardware.camera2.TotalCaptureResult;
import android.hardware.camera2.params.StreamConfigurationMap;
import android.media.Image;
import android.media.ImageReader;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Environment;
import android.os.Handler;
import android.os.HandlerThread;
import androidx.annotation.NonNull;
import androidx.core.app.ActivityCompat;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import android.os.Bundle;
import android.util.Log;
import android.util.Size;
import android.util.SparseIntArray;
import android.view.Surface;
import android.view.TextureView;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;


public class test_String {
    TCP_Client tc;
    private int portvalue;
    private String servervalue;

    //TCP-IP 서버 클래스
    private class TCP_Client extends AsyncTask {
        String SERV_IP = servervalue; // 서버 IP 주소
        int PORT = portvalue; //서버 PORT

        @Override
        protected Object doInBackground(Object... params) {
            try {
                //Socket 선언
                InetAddress AddforServer = InetAddress.getByName(SERV_IP);
                Socket socket = new Socket(AddforServer, PORT);

                try{
                    //파일 경로 지정
                    String m = "/data/user/0/com.eleven.Unity1/cache/IMG_camera.jpg";
                    System.out.println(m);
                    File file = new File("/data/user/0/com.eleven.Unity1/cache/IMG_camera.jpg"); //읽을 파일 경로 적어 주시면 됩니다.

                    //Data Stream 선언
                    DataInputStream dis = new DataInputStream(new FileInputStream(file));
                    DataOutputStream dos = new DataOutputStream(socket.getOutputStream());

                    long fileSize = file.length();
                    byte[] buffer = new byte[1024];

                    long ReadTotal = 0;
                    int Read;

                    //버퍼가 꽉 찰때마다 다시 Read를 열어 전송
                    while ((Read = dis.read(buffer)) > 0) {
                        dos.write(buffer, 0, Read);
                        ReadTotal += Read;
                    }

                    dos.close();

                } catch(IOException e){
                    e.printStackTrace();
                }

            } catch (UnknownHostException e) {
                e.printStackTrace();
            } catch(IOException    e){
                e.printStackTrace();
            }
            return null;
        }
    }



    ////Unity-AndroidStudio 연동 부분
    private static test_String m_instance;
    private static Context context;

    // 싱글턴 메소드로 안드로이드 플러그인이 정의되는 인스턴스 선언
    public static test_String instance(Context ct) {
        if (m_instance == null) {
            context = ct;
            m_instance = new test_String();
        }
        return m_instance;
    }

    // Unity에서 직접 호출되는 클래스로 TCP_Client를 새로 호출
    void ButtonforCameraTakePicture(String server, int port){
        this.portvalue = port;
        this.servervalue = server;
        tc = new TCP_Client();
        tc.execute(this);

    }

    //전역변수로 Unity에서 가져온 server 및 port 정보 TCP_Client 클래스와 공유
    public int getGlobalValue(){
        return portvalue;
    }

    public String getGlobalValue2(){
        return servervalue;
    }

}
