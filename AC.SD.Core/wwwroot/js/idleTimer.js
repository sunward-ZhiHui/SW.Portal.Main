window.idleTimer = {
    idleTimeSec: 30000,
    start: function (dotnetHelper, idleMinutes) {
        let idleTime = 0;
        let lastSent = 0; // last time notified .NET (ms)
        let interval = this.idleTimeSec;

        function sendLastActivityIfNeeded() {
            const now = Date.now();
            // notify .NET once every 30 seconds max
            if (now - lastSent > interval) {
                lastSent = now;
                dotnetHelper.invokeMethodAsync("UpdateLastActivity");
            }
        }

        function resetTimer() {
            idleTime = 0;
            sendLastActivityIfNeeded();
        }

        // reset on activity
        window.onload = resetTimer;
        window.onmousemove = resetTimer;
        window.onmousedown = resetTimer;
        window.ontouchstart = resetTimer;
        window.onclick = resetTimer;
        window.onkeypress = resetTimer;
        window.addEventListener('scroll', resetTimer, true);

        // Check idle every 30 seconds
        setInterval(function () {
            idleTime += 0.5; // 0.5 minute = 30 seconds
            if (idleTime >= idleMinutes) {
                dotnetHelper.invokeMethodAsync("AutoLogout");
            }
        }, interval);
    }
};
