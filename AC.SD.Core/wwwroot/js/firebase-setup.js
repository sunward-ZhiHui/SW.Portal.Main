// firebase-setup.js
var messaging;

function initializeFirebaseMessaging() {
    var config = {
        apiKey: 'AIzaSyBRv5-8jH-Oj8HIBJHNffhvp8HF5KQgAsU',
        authDomain: 'sunwardportal-9e39c.firebaseapp.com',
        projectId: 'sunwardportal-9e39c',
        storageBucket: 'sunwardportal-9e39c.appspot.com',
        messagingSenderId: '701663341939',
        appId: '1:701663341939:web:bdf0c935c30ea01c826906',
        measurementId: 'G-9HNG9SMNXF'
    };

    firebase.initializeApp(config);
    messaging = firebase.messaging();
}

function requestNotificationPermission() {
    messaging.requestPermission()
        .then(function () {
            console.log("Permission granted");
            if (isTokenSentToServer()) {
                console.log("Already granted");
            } else {
                getRegtoken();
            }
        })
        .catch(function (error) {
            console.error("Permission denied:", error);
        });
}

function getRegtoken() {
    messaging.getToken()
        .then(function (currentToken) {
            if (currentToken) {
                console.log("Token:", currentToken);
                setTokenSentToServer(true);
                saveToken(currentToken);
            } else {
                console.log("No Instance ID token available. Request permission to generate one.");
                setTokenSentToServer(false);
            }
        })
        .catch(function (error) {
            console.error("An error occurred while retrieving token:", error);
            setTokenSentToServer(false);
        });
}

function setTokenSentToServer(sent) {
    window.localStorage.setItem("sentToServer", sent ? 1 : 0);
}

function isTokenSentToServer() {
    return window.localStorage.getItem("sentToServer") === "1";
}

function saveToken(token) {
    // Implement your logic to save the token to the server or local storage.
}
