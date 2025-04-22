window.validateForm = function (form) {
    let isValid = true;

    form.querySelectorAll("[required]").forEach(input => {
        const name = input.getAttribute("name");
        let errorSpan = document.querySelector(`span[data-valmsg-for='${name}']`);

        if (!errorSpan) {
            errorSpan = document.createElement("span");
            errorSpan.classList.add("field-validation-error");
            input.insertAdjacentElement("afterend", errorSpan);
        }

        if (!input.value || input.value.trim() === "") {
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
