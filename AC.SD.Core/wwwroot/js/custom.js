// wwwroot/js/custom.js

window.registerServiceWorker = async function () {
    if ('serviceWorker' in navigator) {
        try {
            const registration = await navigator.serviceWorker.register('_content/AC.SD.Core/js/firebase-messaging-sw.js');
            console.log('Service Worker registered with scope:', registration.scope);
        } catch (error) {
            console.error('Service Worker registration failed:', error);
        }
    }
}