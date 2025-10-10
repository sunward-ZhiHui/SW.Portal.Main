window.speechRecognitionInterop = {
    recognition: null,
    startRecognition: function (dotNetRef) {
        if (!('webkitSpeechRecognition' in window)) {
            alert("Speech recognition not supported in this browser.");
            return;
        }

        this.recognition = new webkitSpeechRecognition();
        this.recognition.continuous = true;
        this.recognition.interimResults = true;
        this.recognition.lang = 'en-US';

        this.recognition.onresult = function (event) {
            let transcript = '';
            for (let i = event.resultIndex; i < event.results.length; ++i) {
                transcript += event.results[i][0].transcript;
            }
            dotNetRef.invokeMethodAsync("UpdateSpeechText", transcript);
        };

        this.recognition.onerror = function (event) {
            console.error("Speech recognition error: ", event.error);
        };

        this.recognition.start();
    },
    stopRecognition: function () {
        if (this.recognition) {
            this.recognition.stop();
        }
    }
};
