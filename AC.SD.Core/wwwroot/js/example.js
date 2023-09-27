// wwwroot/firebase-init.js

// Initialize Firebase with your Firebase project configuration
var firebaseConfig = {
    apiKey: 'AIzaSyBRv5-8jH-Oj8HIBJHNffhvp8HF5KQgAsU',
    authDomain: 'sunwardportal-9e39c.firebaseapp.com',
    projectId: 'sunwardportal-9e39c',
    storageBucket: 'sunwardportal-9e39c.appspot.com',
    messagingSenderId: '701663341939',
    appId: '1:701663341939:web:bdf0c935c30ea01c826906',
    measurementId: 'G-9HNG9SMNXF'
};

firebase.initializeApp(firebaseConfig);

// Initialize Firebase Messaging
const messaging = firebase.messaging();

// Define a function to request permission and get the FCM token
window.initializeFirebaseMessaging = async () => {
    try {
        await messaging.requestPermission();
        const token = await messaging.getToken();
        console.log("FCM Token:", token);
    } catch (error) {
        console.error("Error requesting permission:", error);
    }
};
