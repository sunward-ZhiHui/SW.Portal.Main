// firebase-functions.js

// Initialize Firebase Messaging
const messaging = firebase.messaging();

// Function to request permission for push notifications
function requestNotificationPermission() {
    return messaging.requestPermission()
        .then(() => {
            console.log('Notification permission granted.');
            return messaging.getToken();
        })
        .catch((error) => {
            console.error('Error requesting notification permission:', error);
        });
}

// Function to listen for incoming messages
messaging.onMessage((payload) => {
    console.log('Message received:', payload);
    // Handle the incoming message as needed
});
