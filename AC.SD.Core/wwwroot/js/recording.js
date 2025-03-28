let mediaRecorder;
let recordedChunks = [];

window.startRecording = async function () {
    recordedChunks = [];
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);

    mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) recordedChunks.push(event.data);
    };

    mediaRecorder.start();
};

window.stopRecording = async function () {
    return new Promise((resolve) => {
        if (mediaRecorder.state !== "inactive") {
            mediaRecorder.onstop = () => {
                let recordedBlob = new Blob(recordedChunks, { type: "audio/wav" });
                let url = URL.createObjectURL(recordedBlob);
                resolve({ url: url, size: recordedBlob.size });
            };
            mediaRecorder.stop();
        }
    });
};

//  Ensure this function exists in JavaScript
window.getRecordedFile = async function () {
    return new Promise((resolve) => {
        if (recordedChunks.length > 0) {
            let recordedBlob = new Blob(recordedChunks, { type: "audio/wav" });

            let reader = new FileReader();
            reader.readAsDataURL(recordedBlob);
            reader.onloadend = function () {
                let base64Data = reader.result.split(',')[1]; // Extract Base64 content
                resolve(base64Data);
            };
        } else {
            resolve(null);
        }
    });
};
