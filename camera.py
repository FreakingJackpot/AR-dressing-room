
import cv2
import os
import numpy as np
import json
import socket
import base64
import sys


cap = cv2.VideoCapture(0,cv2.CAP_DSHOW)
ret, frame = cap.read()

HOST = "127.0.0.1"
PORT=9000

sock=socket.socket(socket.AF_INET,socket.SOCK_STREAM)
sock.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR,1)
sock.bind((HOST,PORT))
sock.listen(1)
client,addr=sock.accept()

data =json.dumps({'width':frame.shape[1],'height':frame.shape[0]})
client.sendall(data.encode())

while(True):
    # Capture frame-by-frame
    ret, frame = cap.read()
    # Our operations on the frame come here
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # Display the resulting frame
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    """
    тут выполняем расчеты точек для кольца и тд
    """
    #отправка размера кадра приложению
    result, encimg = cv2.imencode('.jpg', frame)
    client.sendall(str(len(encimg.flatten())).encode('utf-8'))

    #отправка изображения
    client.sendall(encimg)

    #отправка массивов npy в json
    lpoints = np.load('joint_location.npy').tolist()
    rpoints = np.load('joint_rotation.npy').tolist()
    data = json.dumps({"positions": lpoints,
                       "rotations": rpoints})

    client.sendall(data.encode())

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()