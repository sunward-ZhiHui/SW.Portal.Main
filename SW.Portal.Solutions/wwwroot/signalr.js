const connection = new signalR.HubConnectionBuilder()
    .withUrl("/myhub") // Replace with your hub URL
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (user, message) => {
    // Handle incoming messages from the hub
});

connection.start().then(() => {
    // Connection to the hub is established
}).catch(err => {
    console.error(err.toString());
});
