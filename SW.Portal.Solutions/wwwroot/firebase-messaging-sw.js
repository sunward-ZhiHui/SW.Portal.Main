importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-messaging.js');


const firebaseConfig = {
    apiKey: 'AIzaSyARCH_URyG6nyoq0J88CkWhVqhQLy14EJc',
    authDomain: 'https://portal.sunwardpharma.com',
    projectId: 'sunwardportal-9e39c',
    storageBucket: 'sunwardportal-9e39c.appspot.com',
    messagingSenderId: '701663341939',
    appId: '1:701663341939:web:bdf0c935c30ea01c826906:android:43c2dc3302ffbbe4826906',
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

// Handle background push notifications
self.addEventListener('push', function (event) {
    const payload = event.data.json();

    // Extract the URL from the payload data
    const url = payload.data.url || 'https://example.com';

    // Customize notification here
    const notificationTitle = payload.notification.title || 'Default Title';
    const notificationOptions = {
        body: payload.notification.body || 'Default Body',
        icon: '/firebase-logo.png'
    };

    event.waitUntil(
        self.registration.showNotification(notificationTitle, notificationOptions)
    );  
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    // Extract the URL or action from the notification payload
    const url = event;
    console.log(url);
    // Perform custom actions based on the URL or other data
    if (url) {
        // Open the specified URL
        //clients.openWindow(url);
    } else {
        // Handle other actions or navigate within your app
    }
});





 