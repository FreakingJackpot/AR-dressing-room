import numpy
from flask_cors import CORS, cross_origin
from flask import Flask
from flask import jsonify

app = Flask(__name__)
cors = CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'

@app.route("/s" )
@cross_origin()
def main():
    lpoints=numpy.load('joint_location.npy')
    rpoints=numpy.load('joint_rotation.npy')

    return jsonify({
        "positions":lpoints.tolist(),
        "rotations":rpoints.tolist()
    })


if __name__ == "__main__":
    app.run(debug = True)