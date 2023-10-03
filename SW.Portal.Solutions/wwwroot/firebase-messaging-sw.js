importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-messaging.js');


const firebaseConfig = {
    apiKey: 'AIzaSyBRv5-8jH-Oj8HIBJHNffhvp8HF5KQgAsU',
    authDomain: 'https://portal.sunwardpharma.com',
    projectId: 'sunwardportal-9e39c',
    storageBucket: 'sunwardportal-9e39c.appspot.com',
    messagingSenderId: '701663341939',
    appId: '1:701663341939:web:bdf0c935c30ea01c826906',
    measurementId: 'G-9HNG9SMNXF'
};

firebase.initializeApp(firebaseConfig);
const messaging = firebase.messaging();

console.log('sw', messaging);

//messaging.setBackgroundMessageHandler(async remoteMessage => {
//    console.log('Message handled in the background!', remoteMessage);
//});
 
 
messaging.setBackgroundMessageHandler(function (payload) {   
    console.log('Received background message ', payload);
    // Customize notification here
    const notificationTitle = 'Background Message Title';
    const notificationOptions = {
        body: 'Background Message body.',
        icon: '/firebase-logo.png'
    };

    return self.registration.showNotification(notificationTitle,
        notificationOptions);
});


 