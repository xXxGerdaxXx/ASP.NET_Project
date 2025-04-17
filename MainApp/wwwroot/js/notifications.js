document.addEventListener("DOMContentLoaded", () => {
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
                    <div class="icon-box">
                        <i class="${notification.icon}"></i>
                    </div>
                    <div class="notification-content">
                        <div class="message">${notification.message}</div>
                        <div class="time" data-created="${new Date(notification.created).toISOString()}">
                            ${new Date(notification.created).toLocaleTimeString()}
                        </div>
                    </div>
                    <button class="btn-close" onclick="dismissNotification('${notification.id}')">×</button>
                `;

        notifications.insertBefore(item, notifications.firstChild);

        updateNotificationCount();
        updateRelativeTimes();
    });

    connection.on("NotificationDismissed", function (notificationId) {
        const element = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
        if (element) {
            element.remove();
            updateNotificationCount();
        }
    });

    connection.start().catch(error => console.error(error));




    window.dismissNotification = function (notificationId) {
        fetch(`/api/notification/dismiss/${notificationId}`, { method: 'POST' })
            .then(res => {
                if (res.ok) {
                    const element = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
                    if (element) {
                        element.remove();
                        updateNotificationCount();
                    }
                }
            })
            .catch(error => console.error('Error removing notification: ', error));
    }



    function updateNotificationCount() {
        const items = document.querySelectorAll(".notification-item");
        const numberDisplay = document.querySelector(".notification-number");
        const dot = document.querySelector(".dot-red");

        if (numberDisplay) numberDisplay.textContent = items.length;
        if (dot) dot.style.display = items.length > 0 ? "block" : "none";
    }

    function updateRelativeTimes() {
        setInterval(updateRelativeTimes, 60 * 1000); 

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
