// wwwroot/js/pushNotification.js

var firebaseConfig = {
    apiKey: "AIzaSyBRv5-8jH-Oj8HIBJHNffhvp8HF5KQgAsU",
    authDomain: "sunwardportal-9e39c.firebaseapp.com",
    projectId: "sunwardportal-9e39c",
    storageBucket: "sunwardportal-9e39c.appspot.com",
    messagingSenderId: "701663341939",
    appId: "1:701663341939:web:bdf0c935c30ea01c826906",
    measurementId: "G-9HNG9SMNXF"
};

firebase.initializeApp(firebaseConfig);

const messaging = firebase.messaging();

messaging.onMessage((payload) => {
    const notification = payload.notification;
    const message = notification.body;
    invokePushNotificationReceived(message);
});

window.initializeFirebase = async (dotnetHelper) => {
    try {
        await messaging.requestPermission();
        const token = await messaging.getToken();
        console.log("Firebase Token:", token);
    } catch (error) {
        console.error("Error initializing Firebase:", error);
    }
};

function invokePushNotificationReceived(message) {
    DotNet.invokeMethodAsync("AC.SD.Core", "PushNotificationReceived", message)
        .then(() => { })
        .catch((error) => {
            console.error("Error invoking PushNotificationReceived:", error);
        });
}
