let mediaRecorder;
let audioChunks = [];

window.startVoiceRecording = async () => {
    audioChunks = [];

    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);

    mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
            audioChunks.push(event.data);
        }
    };

    mediaRecorder.start();
};

window.stopVoiceRecording = async () => {
    return new Promise((resolve, reject) => {
        if (!mediaRecorder) {
            reject("No recording found.");
            return;
        }

        mediaRecorder.onstop = async () => {
            const audioBlob = new Blob(audioChunks, { type: "audio/wav" });

            // Convert Blob to Object URL and resolve
            const audioUrl = URL.createObjectURL(audioBlob);
            resolve(audioUrl);
        };

        mediaRecorder.stop();
    });
};
