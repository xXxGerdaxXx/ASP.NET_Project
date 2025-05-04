window.validateForm = function (form) {
    let isValid = true;

    form.querySelectorAll("input, textarea, select").forEach(input => {
        const name = input.getAttribute("name");
        if (!name) return;

        const isCheckbox = input.type === "checkbox";
        const hasRequiredAttr = input.hasAttribute("required");
        if (!hasRequiredAttr) return;

        let errorSpan = document.querySelector(`span[data-valmsg-for='${name}']`);
        if (!errorSpan) {
            errorSpan = document.createElement("span");
            errorSpan.classList.add("field-validation-error");
            input.insertAdjacentElement("afterend", errorSpan);
        }

        const isEmpty = isCheckbox ? !input.checked : !input.value.trim();

        if (isEmpty) {
            const customMessage = input.getAttribute("data-val-required") || "This field is required";
            input.classList.add("input-validation-error");
            errorSpan.textContent = customMessage;
            errorSpan.style.display = "inline";
            isValid = false;
        } else {
            if (input.type === "email" && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.value.trim())) {
                input.classList.add("input-validation-error");
                errorSpan.textContent = "Please enter a valid email address.";
                errorSpan.style.display = "inline";
                isValid = false;
            } else {
                input.classList.remove("input-validation-error");
                errorSpan.textContent = "";
                errorSpan.style.display = "none";
            }
        }

        bindLiveValidation(input, errorSpan, isCheckbox);
    }); // 🛠 <--- Correct closing bracket for forEach

    // Confirm password validation AFTER the forEach loop
    const password = form.querySelector("[name='Password']");
    const confirm = form.querySelector("[name='ConfirmPassword']");
    if (password && confirm && password.value !== confirm.value) {
        const errorSpan = document.querySelector(`span[data-valmsg-for='ConfirmPassword']`);
        confirm.classList.add("input-validation-error");

        const mismatchMsg = confirm.dataset.valEqualto || "Passwords do not match.";
        if (errorSpan) {
            errorSpan.textContent = mismatchMsg;
            errorSpan.style.display = "inline";
        }

        isValid = false;
    }

    return isValid;
};

function bindLiveValidation(input, errorSpan, isCheckbox = false) {
    if (!input.dataset.validationBound) {
        const eventType = isCheckbox ? "change" : "input";

        input.addEventListener(eventType, function () {
            const isValid = isCheckbox ? input.checked : input.value.trim() !== "";
            if (isValid) {
                input.classList.remove("input-validation-error");
                errorSpan.textContent = "";
                errorSpan.style.display = "none";
            }
        });

        input.dataset.validationBound = "true";
    }
}

window.togglePasswordByData = function (icon) {
    const inputId = icon.dataset.target;
    const input = document.getElementById(inputId);
    if (!input) return;

    if (input.type === "password") {
        input.type = "text";
        icon.classList.replace("fa-eye", "fa-eye-slash");
    } else {
        input.type = "password";
        icon.classList.replace("fa-eye-slash", "fa-eye");
    }
};