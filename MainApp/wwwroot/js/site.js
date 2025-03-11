document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ JavaScript Loaded!");

    // ✅ Open Modal
    document.querySelectorAll("[data-modal-target]").forEach(button => {
        button.addEventListener("click", function () {
            const modalId = this.getAttribute("data-modal-target");
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.classList.add("active");
                console.log(`✅ Opened modal: ${modalId}`);
            }
        });
    });

    // ✅ Close Modal
    document.querySelectorAll("[data-close-modal]").forEach(button => {
        button.addEventListener("click", function () {
            this.closest(".modal-overlay").classList.remove("active");
            console.log("❌ Modal closed!");
        });
    });

    // ✅ Close Modal When Clicking Outside
    window.addEventListener("click", function (event) {
        if (event.target.classList.contains("modal-overlay")) {
            event.target.classList.remove("active");
            console.log("❌ Clicked outside modal, closing...");
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const notificationButton = document.getElementById("notificationsButton");
    const notificationBadge = document.querySelector(".notification-badge");

    // Simulate new notifications (Replace this with real API data)
    setTimeout(() => {
        notificationBadge.style.display = "block"; // Show badge
    }, 2000);

    notificationButton.addEventListener("click", function () {
        alert("You have new notifications!");
        notificationBadge.style.display = "none"; // Hide badge after click
    });
});

document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ JavaScript Loaded!");

    // ✅ Filtering Projects
    document.querySelectorAll(".tab").forEach(tab => {
        tab.addEventListener("click", function () {
            const filter = this.getAttribute("data-filter");

            // ✅ Remove "active" from all tabs
            document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));
            this.classList.add("active");

            // ✅ Show/Hide Projects Based on Filter
            document.querySelectorAll(".project-item").forEach(item => {
                if (filter === "all" || item.getAttribute("data-status") === filter) {
                    item.style.display = "block";
                } else {
                    item.style.display = "none";
                }
            });
        });
    });
});
