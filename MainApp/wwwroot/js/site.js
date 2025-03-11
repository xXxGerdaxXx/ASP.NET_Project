document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ JavaScript Loaded!");

    // ✅ Open Modal
    document.querySelectorAll("[data-modal-target]").forEach(button => {
        button.addEventListener("click", function () {
            const modalId = this.getAttribute("data-modal-target");
            const modal = document.getElementById(modalId);
            if (modal) modal.classList.add("active");
        });
    });

    // ✅ Close Modal
    document.querySelectorAll("[data-close-modal]").forEach(button => {
        button.addEventListener("click", function () {
            this.closest(".modal-overlay").classList.remove("active");
        });
    });

    // ✅ Close Modal When Clicking Outside
    window.addEventListener("click", function (event) {
        if (event.target.classList.contains("modal-overlay")) {
            event.target.classList.remove("active");
        }
    });
});
