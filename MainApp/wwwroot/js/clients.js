
    document.addEventListener("DOMContentLoaded", function () {
        loadClients(); // ✅ Load clients list on page load

    function loadClients() {
        fetch('/admin/clients/list')
            .then(response => response.text())
            .then(html => {
                document.getElementById("clientsList").innerHTML = html;
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

    document.getElementById("openAddClientModal").addEventListener("click", openAddClientModal);

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

                    setupEditFormSubmission(); // ✅ Attach form submission handler
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

                    setupFileUploadPreview();

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

    function setupFileUploadPreview() {
            const fileInput = document.querySelector("#createClientForm input[type='file']");
    const imagePreview = document.querySelector(".image-preview");

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
