let mediaRecorder;
let audioChunks = [];

window.startVoiceRecording = async () => {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    mediaRecorder = new MediaRecorder(stream);

    audioChunks = []; // Reset chunks for a new recording
    mediaRecorder.ondataavailable = event => {
        audioChunks.push(event.data);
    };

    mediaRecorder.start();
};

window.stopVoiceRecording = () => {
    return new Promise((resolve) => {
        mediaRecorder.onstop = () => {
            const blob = new Blob(audioChunks, { type: "audio/wav" });
            const audioUrl = URL.createObjectURL(blob);

            resolve(audioUrl); // Return the blob URL for playback
        };

        mediaRecorder.stop();
    });
};
