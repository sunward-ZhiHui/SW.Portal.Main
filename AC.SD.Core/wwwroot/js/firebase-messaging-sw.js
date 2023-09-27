// firebase-messaging-sw.js
importScripts("https://www.gstatic.com/firebasejs/10.4.0/firebase-app.js");
importScripts("https://www.gstatic.com/firebasejs/10.4.0/firebase-messaging.js");

const firebaseConfig = {
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
    console.log("Message received:", payload);
    const { title, body } = payload.notification;
    self.registration.showNotification(title, {
        body,
    });
});
