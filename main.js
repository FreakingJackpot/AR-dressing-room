import * as THREE from './node_modules/three/src/Three.js';
import {OBJLoader} from './node_modules/three/examples/jsm/loaders/OBJLoader.js';
import {GUI} from './node_modules/three/examples/jsm/libs/dat.gui.module.js'

navigator.mediaDevices.getUserMedia({video: true, audio: false}).then(function (stream) {
    let rawVideoStream = stream;
    let videoSettings = stream.getVideoTracks()[0].getSettings();
    let video = document.createElement("video");
    Object.assign(video, {
        srcObject: stream,
        width: videoSettings.width,
        height: videoSettings.height,
        autoplay: true,
    })
    let videoTexture = new THREE.VideoTexture(video);
    var WIDTH = window.innerWidth;
    var HEIGHT = window.innerHeight;

    var renderer = new THREE.WebGLRenderer({antialias: true});
    renderer.setSize(WIDTH, HEIGHT);
    renderer.setClearColor(0xDDDDDD, 1);
    document.body.appendChild(renderer.domElement);

    var scene = new THREE.Scene();

    scene.background = videoTexture;

    var camera = new THREE.PerspectiveCamera(70, WIDTH / HEIGHT);
    camera.position.z = 50;
    scene.add(camera);

    var frontSpot = new THREE.SpotLight(0xeeeece);
    frontSpot.position.set(1000, 1000, 1000);
    scene.add(frontSpot);
    var frontSpot2 = new THREE.SpotLight(0xddddce);
    frontSpot2.position.set(-300, -300, -300);
    scene.add(frontSpot2);


    const loader = new OBJLoader();

// load a resource
    loader.load(
        // resource URL
        'ring.obj',
        // called when resource is loaded
        function (object) {
            object.name="lol";
            object.scale.set(10, 10, 10);
            object.rotation.x = Math.PI / 2;
            object.position.set(20, 1, 1);
            scene.add(object);

        },
        // called when loading is in progresses
        function (xhr) {

            console.log((xhr.loaded / xhr.total * 100) + '% loaded');

        },
        // called when loading has errors
        function (error) {

            console.log('An error happened');

        }


    );
        let i = {};
        i.value = 0;
        const gui = new GUI()
        const RingFolder = gui.addFolder("Ring");
        RingFolder.add(i, "value", 0, 20, 1);
        RingFolder.open();

    function setrotationandposition (response) {
        let object=scene.getObjectByName('lol');
        object.position.set(response['positions'][i.value][0]*20,response['positions'][i.value][1]*20,response['positions'][i.value][2]*20);
        const quaternion = new THREE.Quaternion().fromArray(response['rotations'][i.value]);
        object.rotation.setFromQuaternion(quaternion);

    }

    function render() {
        $.ajax({
            url: 'http://127.0.0.1:5000/s',             // указываем URL и
            async : true,
            dataType : "json",                     // тип загружаемых данных
            success : setrotationandposition,
        });

        requestAnimationFrame(render);
        renderer.render(scene, camera);
    }

    render();

});

