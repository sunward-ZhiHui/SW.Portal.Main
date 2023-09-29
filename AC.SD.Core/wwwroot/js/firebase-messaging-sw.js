importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/6.4.2/firebase-messaging.js');

firebase.initializeApp({
    'messagingSenderId': '701663341939'
});

const messaging = firebase.messaging();

messaging.setBackgroundMessageHandler(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    const notificationTitle = 'Background Message Title';
    const notificationOptions = {
        body: 'Background Message body.',
        icon: '/firebase-logo.png'
    };

    return self.registration.showNotification(notificationTitle, notificationOptions);
});
