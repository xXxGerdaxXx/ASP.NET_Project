document.addEventListener("DOMContentLoaded", function () {
    console.log("site.js Loaded");

    attachGeneralEventListeners();
    initializeThemeToggle();
    initializeDropdowns();

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
    }

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

    function initializeDropdowns() {
        const dropdownTriggers = document.querySelectorAll('[data-type="dropdown"]');
        const dropdownElements = new Set();

        dropdownTriggers.forEach(trigger => {
            const targetSelector = trigger.getAttribute('data-target');
            if (!targetSelector) return;

            const dropdown = document.querySelector(targetSelector);
            if (dropdown) dropdownElements.add(dropdown);
        });

        dropdownTriggers.forEach(trigger => {
            trigger.addEventListener('click', e => {
                e.stopPropagation();

                const targetSelector = trigger.getAttribute('data-target');
                if (!targetSelector) return;

                const dropdown = document.querySelector(targetSelector);
                if (!dropdown) return;

                closeAllDropdowns(dropdown, dropdownElements);
                dropdown.classList.toggle('show');
            });
        });

        dropdownElements.forEach(dropdown => {
            dropdown.addEventListener('click', e => e.stopPropagation());
        });

        document.addEventListener('click', () => {
            closeAllDropdowns(null, dropdownElements);
        });

        function closeAllDropdowns(exceptDropdown, dropdownElements) {
            dropdownElements.forEach(dropdown => {
                if (dropdown !== exceptDropdown) {
                    dropdown.classList.remove('show');
                }
            });
        }
    }

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

function toggleDropdown(button) {
    const menu = button.nextElementSibling;
    document.querySelectorAll('.dropdown-menu').forEach(m => {
        if (m !== menu) m.style.display = 'none';
    });

    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
}

document.addEventListener("click", function (e) {
    if (!e.target.closest('.project-actions')) {
        document.querySelectorAll('.dropdown-menu').forEach(m => m.style.display = 'none');
    }
});

window.dismissNotification = async function (notificationId) {
    try {
        const response = await fetch(`/api/notification/dismiss/${notificationId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include' 
        });

        if (!response.ok) {
            console.error(`Failed to dismiss notification. Status: ${response.status}`);
            return;
        }

        const element = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
        if (element) {
            element.remove();
            updateNotificationCount();
        }

        console.log(`Notification ${notificationId} dismissed successfully`);
    } catch (error) {
        console.error('Error dismissing notification:', error);
    }
};
function updateNotificationCount() {
    const items = document.querySelectorAll(".notification-item");
    const numberDisplay = document.querySelector(".notification-number");
    const dot = document.querySelector(".dot-red");

    if (numberDisplay) numberDisplay.textContent = items.length;
    if (dot) dot.style.display = items.length > 0 ? "block" : "none";
}


