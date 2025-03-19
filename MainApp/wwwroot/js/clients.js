document.addEventListener("DOMContentLoaded", function () {
    loadClients(); // ✅ Load clients list on page load

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
                attachEventListeners(); // ✅ Attach event listeners after clients are loaded
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
                const selectedClientIds = Array.from(clientCheckboxes)
                    .filter(checkbox => checkbox.checked)
                    .map(checkbox => parseInt(checkbox.value));

                if (selectedClientIds.length === 0) {
                    alert("No clients selected for deletion.");
                    return;
                }

                showDeleteModal(selectedClientIds);
            });
        }
    }

    function loadEditClientModal(clientId) {
        fetch(`/admin/clients/editclient/${clientId}`)
            .then(response => response.text())
            .then(html => {
                let modal = document.getElementById("edit-client-modal");

                if (modal) {
                    modal.remove(); // ✅ Remove existing modal before adding a new one
                }

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

                    setupEditFormSubmission(clientId); // ✅ Pass `clientId`
                    setupFileUploadPreview("editClientForm"); // ✅ Ensure image preview works
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
            const formData = new FormData(editForm);

            fetch("/admin/clients/editclient", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        showSuccessNotification("Client updated successfully!");
                        document.getElementById("edit-client-modal").remove(); // ✅ Close modal
                        loadClients(); // ✅ Refresh client list
                    } else {
                        showErrorNotification("Error updating client: " + data.message);
                    }
                })
                .catch(error => console.error("Error updating client:", error));
        });
    }

    function openAddClientModal() {
        fetch('/admin/clients/create')
            .then(response => response.text())
            .then(html => {
                const existingModal = document.getElementById("add-client-modal");
                if (existingModal) {
                    existingModal.remove();
                }

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

                    modal.querySelector("#createClientForm").addEventListener("submit", function (event) {
                        event.preventDefault();
                        const formData = new FormData(this);

                        fetch('/admin/clients/create', {
                            method: 'POST',
                            body: formData
                        })
                            .then(response => response.json())
                            .then(data => {
                                if (data.success) {
                                    modal.remove();
                                    loadClients();
                                } else {
                                    alert("Error: " + data.message);
                                }
                            })
                            .catch(error => console.error("Error creating client:", error));
                    });
                }
            })
            .catch(error => console.error("Error loading create client modal:", error));
    }

    function showDeleteModal(selectedClientIds) {
        const modal = document.getElementById("deleteConfirmationModal");
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
                        showSuccessNotification(`Deleted ${data.deleted} client(s) successfully!`);
                        modal.style.display = "none";
                        loadClients();
                    } else {
                        showErrorNotification("Error: " + data.message);
                    }
                })
                .catch(error => console.error("Error deleting clients:", error));
        };

        cancelBtn.onclick = function () {
            modal.style.display = "none";
        };
    }

    function showSuccessNotification(message) {
        const notification = document.getElementById("deleteSuccessNotification");
        const messageBox = document.getElementById("deleteSuccessMessage");

        messageBox.textContent = message;
        notification.style.display = "block";

        setTimeout(() => {
            notification.style.display = "none";
        }, 3000);
    }

    function showErrorNotification(message) {
        const notification = document.getElementById("deleteErrorNotification");
        const messageBox = document.getElementById("deleteErrorMessage");

        messageBox.textContent = message;
        notification.style.display = "block";

        setTimeout(() => {
            notification.style.display = "none";
        }, 3000);
    }

    function setupFileUploadPreview(formId) {
        const fileInput = document.querySelector(`#${formId} input[type='file']`);
        const imagePreview = document.querySelector(`#${formId} .image-preview`);

        if (fileInput && imagePreview) {
            fileInput.addEventListener("change", function () {
                const file = this.files[0];
                if (file) {
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        imagePreview.src = event.target.result;
                    };
                    reader.readAsDataURL(file);
                }
            });
        }
    }

    function toggleDeleteButton() {
        const clientCheckboxes = document.querySelectorAll(".client-checkbox");
        const deleteSelectedBtn = document.getElementById("deleteSelectedClients");

        const selectedClients = Array.from(clientCheckboxes).some(checkbox => checkbox.checked);
        deleteSelectedBtn.disabled = !selectedClients;
    }
});


//document.getElementById("avatarInput").addEventListener("change", function (event) {
//    const file = event.target.files[0];
//    if (file) {
//        const reader = new FileReader();
//        reader.onload = function (e) {
//            document.getElementById("avatarPreview").src = e.target.result;
//        };
//        reader.readAsDataURL(file);

//        // ✅ Upload file via AJAX
//        const formData = new FormData();
//        formData.append("File", file);
//        formData.append("Folder", "clients"); // Change this dynamically for different models

//        fetch("/fileupload/upload", {
//            method: "POST",
//            body: formData
//        })
//            .then(response => response.json())
//            .then(data => {
//                if (data.success) {
//                    document.getElementById("avatarUrl").value = data.filePath;
//                } else {
//                    alert("Error uploading file: " + data.message);
//                }
//            })
//            .catch(error => console.error("Error uploading file:", error));
//    }
//});