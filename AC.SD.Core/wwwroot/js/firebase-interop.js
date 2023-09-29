// firebase-interop.js
window.initializeFirebase = function () {
    // Wait for Firebase SDK to load
    if (typeof firebase !== 'undefined') {
        firebase.initializeApp(firebaseConfig);
    } else {
        // Handle the case where Firebase SDK hasn't loaded
        console.error('Firebase SDK is not loaded.');
    }
};
