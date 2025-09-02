// Configuration de la connexion SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notifications")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Gestion des notifications
connection.on("ReceiveNotification", (message) => {
    showToast(message);  // Votre fonction d'affichage
    console.log("Notification reçue:", message);
});

// Démarrer la connexion
async function startConnection() {
    try {
        await connection.start();
        console.log("Connecté au hub SignalR");

        // Souscrire aux notifications pour la demande actuelle
        if (typeof currentDemandeId !== 'undefined') {
            await connection.invoke("SubscribeToDemande", currentDemandeId);
        }
    } catch (err) {
        console.error("Erreur de connexion:", err);
        setTimeout(startConnection, 5000);
    }
}

// Gestion des reconnexions
connection.onclose(startConnection);

// Démarrer
startConnection();

function showToast(message) {
    // Implémentez votre système de notification UI
    Toastify({
        text: message,
        duration: 3000
    }).showToast();
}