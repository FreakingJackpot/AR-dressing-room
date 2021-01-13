import numpy
import os
import io
from flask_cors import CORS, cross_origin
from flask import Flask,request
from flask import jsonify
from PIL import Image
app = Flask(__name__)
cors = CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'
@app.route("/s",methods=['GET', 'POST'])
@cross_origin()
def main():
    data=request.get_json()
    image=Image.open(io.BytesIO(bytearray(data['image'])))
    lpoints=numpy.load('joint_location.npy')
    rpoints=numpy.load('joint_rotation.npy')

    return jsonify({
        "positions":lpoints.tolist(),
        "rotations":rpoints.tolist()
    })


if __name__ == "__main__":
    app.run(debug = True)