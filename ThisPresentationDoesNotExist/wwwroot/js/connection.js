const connection = new signalR.HubConnectionBuilder()
    .withUrl("/presentationHub")
    .build();

await connection.start();
export { connection }