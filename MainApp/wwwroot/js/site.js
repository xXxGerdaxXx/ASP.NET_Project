document.addEventListener("DOMContentLoaded", function () {
    console.log("site.js Loaded");

    attachGeneralEventListeners();
    initializeThemeToggle();

    // ✅ General UI Events
    function attachGeneralEventListeners() {
        console.log("Attaching general event listeners...");

        document.querySelectorAll('[data-modal="true"]').forEach(button => {
            button.addEventListener("click", function () {
                const modalTarget = this.getAttribute("data-target");
                const modal = document.querySelector(modalTarget);
                if (modal) modal.style.display = "flex";
            });
        });

        document.querySelectorAll('[data-close="true"]').forEach(button => {
            button.addEventListener("click", function () {
                const modal = this.closest(".modal");
                if (modal) modal.style.display = "none";
            });
        });

        const notificationButton = document.getElementById("notificationsButton");
        const notificationBadge = document.querySelector(".notification-badge");

        if (notificationButton && notificationBadge) {
            setTimeout(() => {
                notificationBadge.style.display = "block";
            }, 2000);

            notificationButton.addEventListener("click", function () {
                alert("You have new notifications!");
                notificationBadge.style.display = "none";
            });
        }
    }

    // ✅ Theme Toggle
    function initializeThemeToggle() {
        const toggle = document.getElementById("themeToggle");
        const slider = toggle?.querySelector(".toggle-switch");

        const savedTheme = localStorage.getItem("theme");
        if (savedTheme) {
            document.documentElement.setAttribute("data-theme", savedTheme);
            if (savedTheme === "dark") {
                slider?.classList.add("active");
            }
        }

        toggle?.addEventListener("click", function () {
            const isDark = document.documentElement.getAttribute("data-theme") === "dark";
            const newTheme = isDark ? "light" : "dark";

            document.documentElement.setAttribute("data-theme", newTheme);
            localStorage.setItem("theme", newTheme);
            slider?.classList.toggle("active", newTheme === "dark");
        });
    }

    // ✅ Global Success Message
    window.showSuccessMessage = function (message) {
        const notification = document.getElementById("createSuccessNotification");
        if (notification) {
            notification.querySelector("p").textContent = message;
            notification.classList.add("show");
            notification.style.display = "block";
            setTimeout(() => {
                notification.classList.remove("show");
                notification.style.display = "none";
            }, 3000);
        }
    };

    // ✅ Global Error Message
    window.showErrorMessage = function (message) {
        const notification = document.getElementById("deleteErrorNotification");
        if (notification) {
            notification.querySelector("p").textContent = message;
            notification.classList.add("show");
            notification.style.display = "block";
            setTimeout(() => {
                notification.classList.remove("show");
                notification.style.display = "none";
            }, 3000);
        }
    };

    // ✅ Validation Helpers
    window.validateForm = function (form) {
        let isValid = true;

        form.querySelectorAll("[required]").forEach(input => {
            let errorSpan = input.nextElementSibling;

            if (!errorSpan || !errorSpan.classList.contains("field-validation-error")) {
                errorSpan = document.createElement("span");
                errorSpan.classList.add("field-validation-error");
                input.insertAdjacentElement("afterend", errorSpan);
            }

            if (input.value.trim() === "") {
                input.classList.add("input-validation-error");
                errorSpan.textContent = "This field is required";
                errorSpan.style.display = "inline";
                isValid = false;
            } else {
                input.classList.remove("input-validation-error");
                errorSpan.textContent = "";
                errorSpan.style.display = "none";
            }

            bindLiveValidation(input, errorSpan);
        });

        return isValid;
    };

    window.displayServerErrors = function (errors) {
        Object.keys(errors).forEach(key => {
            const inputField = document.querySelector(`[name="${key}"]`);
            if (inputField) {
                let errorSpan = inputField.nextElementSibling;
                if (!errorSpan || !errorSpan.classList.contains("field-validation-error")) {
                    errorSpan = document.createElement("span");
                    errorSpan.classList.add("field-validation-error");
                    inputField.insertAdjacentElement("afterend", errorSpan);
                }

                errorSpan.textContent = errors[key].join(", ");
                errorSpan.style.display = "inline";
                inputField.classList.add("input-validation-error");

                bindLiveValidation(inputField, errorSpan);
            }
        });
    };

    function bindLiveValidation(input, errorSpan) {
        if (!input.dataset.validationBound) {
            input.addEventListener("input", function () {
                if (input.value.trim() !== "") {
                    input.classList.remove("input-validation-error");
                    errorSpan.textContent = "";
                    errorSpan.style.display = "none";
                }
            });
            input.dataset.validationBound = "true";
        }
    }
});