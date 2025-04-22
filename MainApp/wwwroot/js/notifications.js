document.addEventListener("DOMContentLoaded", () => {
    console.log("Notifications.js loaded and running");
    const button = document.getElementById("notification-dropdown-button");
    const dropdown = document.getElementById("notification-dropdown");

    if (button && dropdown) {
        button.addEventListener("click", () => {
            console.log(" Notification bell clicked");
            dropdown.classList.toggle("show");
        });
    }

    updateNotificationCount();
    updateRelativeTimes();  
    setInterval(updateRelativeTimes, 60 * 1000); 

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    connection.on("ReceiveNotification", function (notification) {
        const notifications = document.querySelector('.notifications');
        if (!notifications) return;

        const item = document.createElement('div');
        item.className = 'notification-item';
        item.setAttribute('data-id', notification.id);
        item.innerHTML = `
        <img class="image" src="${notification.icon}" />
        <div class="notification-content">
            <div class="message">${notification.message}</div>
            <div class="time" data-created="${new Date(notification.created).toISOString()}">Loading...</div>
            <button class="btn-close" onclick="dismissNotification('${notification.id}')">×</button>
        </div>
    `;

        notifications.insertBefore(item, notifications.firstChild);
        updateNotificationCount();
        updateRelativeTimes();
        updateLocalTimes(); 
    });

    function updateLocalTimes() {
        const timeElements = document.querySelectorAll(".notification-item .time");

        timeElements.forEach(el => {
            const iso = el.getAttribute("data-created");
            if (!iso) return;

            const localTime = new Date(iso).toLocaleTimeString("sv-SE", {
                timeZone: "Europe/Stockholm",
                hour: "2-digit",
                minute: "2-digit"
            });

            el.textContent = localTime;
        });
    }

    //connection.on("NotificationDismissed", function (notificationId) {
    //    const element = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
    //    if (element) {
    //        element.remove();
    //        updateNotificationCount();
    //    }
    //});

    connection.start().catch(error => console.error(error));




    function updateRelativeTimes() {


        const timeElements = document.querySelectorAll(".notification-item .time");

        timeElements.forEach(el => {
            const iso = el.getAttribute("data-created");
            if (!iso) return;

            const created = new Date(iso);
            const now = new Date();
            const secondsAgo = Math.floor((now - created) / 1000);

            let display = "";

            if (secondsAgo < 60) {
                display = "Just now";
            } else if (secondsAgo < 3600) {
                const minutes = Math.floor(secondsAgo / 60);
                display = `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
            } else if (secondsAgo < 86400) {
                const hours = Math.floor(secondsAgo / 3600);
                display = `${hours} hour${hours > 1 ? "s" : ""} ago`;
            } else {
                const days = Math.floor(secondsAgo / 86400);
                display = `${days} day${days > 1 ? "s" : ""} ago`;
            }

            el.textContent = display;
        });
    }

});
