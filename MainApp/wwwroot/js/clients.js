document.addEventListener("DOMContentLoaded", function () {
    setTimeout(() => { attachEventListeners(); }, 100); // Delay event attachment
    loadClients(); // Load clients list on page load

    function loadClients() {
        fetch('/admin/clients/list')
            .then(response => response.text())
            .then(html => {
                const clientsList = document.getElementById("clientsList");
                if (!clientsList) {
                    console.error("Error: #clientsList element not found.");
                    return;
                }
                clientsList.innerHTML = html;
                attachEventListeners();
            })
            .catch(error => console.error("Error loading clients:", error));
    }

    function attachEventListeners() {
        document.querySelectorAll(".edit-client-btn").forEach(button => {
            button.addEventListener("click", function () {
                const clientId = this.getAttribute("data-client-id");
                loadEditClientModal(clientId);
            });
        });

        const addClientModalButton = document.getElementById("openAddClientModal");
        if (addClientModalButton) {
            addClientModalButton.addEventListener("click", openAddClientModal);
        }

        const selectAllCheckbox = document.getElementById("selectAllClients");
        const clientCheckboxes = document.querySelectorAll(".client-checkbox");
        const deleteSelectedBtn = document.getElementById("deleteSelectedClients");

        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener("change", function () {
                clientCheckboxes.forEach(checkbox => {
                    checkbox.checked = selectAllCheckbox.checked;
                });
                toggleDeleteButton();
            });
        }

        clientCheckboxes.forEach(checkbox => {
            checkbox.addEventListener("change", toggleDeleteButton);
        });

        if (deleteSelectedBtn) {
            deleteSelectedBtn.addEventListener("click", function () {
                const selectedClientIds = Array.from(document.querySelectorAll(".client-checkbox:checked"))
                    .map(checkbox => parseInt(checkbox.value));

                if (selectedClientIds.length === 0) {
                    alert("No clients selected for deletion.");
                    return;
                }

                showDeleteModal(selectedClientIds);
            });
        }
    }

    function openAddClientModal() {
        fetch('/admin/clients/create')
            .then(response => response.text())
            .then(html => {
                const existingModal = document.getElementById("add-client-modal");
                if (existingModal) existingModal.remove();

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("add-client-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    setupFileUploadPreview("createClientForm");

                    modal.querySelector("#closeAddClientModal").addEventListener("click", function () {
                        modal.remove();
                    });

                    const form = modal.querySelector("#createClientForm");
                    if (form) {
                        form.addEventListener("submit", function (event) {
                            event.preventDefault();
                            if (!validateForm(form)) return; 

                            const formData = new FormData(form);
                            fetch('/admin/clients/create', {
                                method: 'POST',
                                body: formData
                            })
                                .then(response => response.json())
                                .then(data => {
                                    if (data.success) {
                                        alert("Client created successfully!");
                                        location.reload(); // Refresh or update the UI
                                    } else {
                        
                                        displayServerErrors(data.errors);
                                    }
                                })
                                .catch(error => console.error("Error creating client:", error));
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading create client modal:", error));
    }

    function validateForm(form) {
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
        });

        return isValid;
    }
    function displayServerErrors(errors) {
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
            }
        });
    }
    function loadEditClientModal(clientId) {
        fetch(`/admin/clients/editclient/${clientId}`)
            .then(response => response.text())
            .then(html => {
                let modal = document.getElementById("edit-client-modal");
                if (modal) modal.remove();

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                modal = document.getElementById("edit-client-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    modal.querySelector("#closeEditClientModal").addEventListener("click", function () {
                        modal.remove();
                    });

                    setupEditFormSubmission(clientId);
                    setupFileUploadPreview("editClientForm");
                }
            })
            .catch(error => console.error("Error loading edit client modal:", error));

    }

    function setupEditFormSubmission(clientId) {
        const editForm = document.getElementById("editClientForm");
        if (!editForm) {
            console.error("Edit form not found!");
            return;
        }

        editForm.addEventListener("submit", function (event) {
            event.preventDefault();
            if (!validateForm(editForm)) return; // Validate before submitting
            const formData = new FormData(editForm);

            fetch("/admin/clients/editclient", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert("Client updated successfully!");
                        document.getElementById("edit-client-modal").remove();
                        loadClients();
                    } else {
                displayServerErrors(data.errors); // Show server-side validation errors
                    }
                })
                .catch(error => console.error("Error updating client:", error));
        });
    }

    function showDeleteModal(selectedClientIds) {
        const modal = document.getElementById("deleteConfirmationModal");
        if (!modal) {
            console.error("Error: Delete confirmation modal not found.");
            return;
        }

        const confirmBtn = document.getElementById("confirmDeleteClients");
        const cancelBtn = document.getElementById("cancelDeleteClients");

        modal.style.display = "flex";
        document.getElementById("deleteClientCount").textContent = selectedClientIds.length;

        confirmBtn.onclick = function () {
            fetch('/admin/clients/delete-multiple', {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(selectedClientIds)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert(`Deleted ${data.deleted} client(s) successfully!`);
                        modal.style.display = "none";
                        loadClients();
                    } else {
                        alert("Error: " + data.message);
                    }
                })
                .catch(error => console.error("Error deleting clients:", error));
        };

        cancelBtn.onclick = function () {
            modal.style.display = "none";
        };
    }

    function setupFileUploadPreview(formId) {
        const fileInput = document.querySelector(`#${formId} input[type='file']`);
        const imagePreview = document.querySelector(`#${formId} .image-preview`);
        const previewer = document.querySelector(`#${formId} .image-previewer`);

        if (fileInput && imagePreview && previewer) {
            fileInput.addEventListener("change", function () {
                const file = this.files[0];
                if (file) processImage(file, imagePreview, previewer);
            });

            previewer.addEventListener("click", function () {
                fileInput.click();
            });
        }
    }

    function processImage(file, imagePreview, previewer) {
        const reader = new FileReader();
        reader.onload = function (event) {
            imagePreview.src = event.target.result;
            previewer.classList.add("selected");
        };
        reader.readAsDataURL(file);
    }

    function toggleDeleteButton() {
        const deleteSelectedBtn = document.getElementById("deleteSelectedClients");
        deleteSelectedBtn.disabled = document.querySelectorAll(".client-checkbox:checked").length === 0;
    }
});
