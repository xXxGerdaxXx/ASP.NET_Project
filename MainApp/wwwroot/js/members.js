document.addEventListener("DOMContentLoaded", function () {
    const pageSizeSelector = document.getElementById("pageSize");

    function loadMembers(page = 1) {
        const pageSize = document.getElementById("pageSize")?.value || 6;

        fetch(`/admin/members/list?page=${page}&pageSize=${pageSize}`)

            .then(response => response.text())
            .then(html => {
                const membersList = document.getElementById("membersList");
                if (!membersList) {
                    console.error("Error: #membersList element not found.");
                    return;
                }

                membersList.innerHTML = html;
                attachEventListeners();
                attachPaginationEvents(); 
            })
            .catch(error => console.error("Error loading members:", error));
    }
    function attachPaginationEvents() {
        document.querySelectorAll(".pagination-link").forEach(link => {
            link.addEventListener("click", function (e) {
                e.preventDefault();
                const page = this.getAttribute("data-page");
                loadMembers(parseInt(page));
            });
        });
    }

    if (pageSizeSelector) {
        pageSizeSelector.addEventListener("change", () => loadMembers(1));
    }

    loadMembers();

    function attachEventListeners() {

        document.querySelectorAll(".edit-member-btn").forEach(button => {
            button.addEventListener("click", function () {
                const memberId = this.getAttribute("data-member-id");
                if (!memberId) {
                    console.error("Error: Missing member ID!");
                    return;
                }
                loadEditMemberModal(memberId);
            });
        });
        document.querySelectorAll(".pagination-link").forEach(link => {
            link.addEventListener("click", e => {
                e.preventDefault();
                const page = parseInt(link.dataset.page);
                loadMembers(page);
            });
        });
        const addMemberModalButton = document.getElementById("openAddMemberModal");
        if (addMemberModalButton) {
            addMemberModalButton.addEventListener("click", openAddMemberModal);
        }
    }




    function openAddMemberModal() {
        fetch('/admin/members/create')
            .then(response => response.text())
            .then(html => {
                removeExistingModal("add-member-modal");

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("add-member-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    modal.querySelector("#closeAddMemberModal").addEventListener("click", function () {
                        modal.remove();
                    });

                    setupFileUploadPreview("createMemberForm");

                    const form = modal.querySelector("#createMemberForm");
                    if (form) {
                        form.addEventListener("submit", function (event) {
                            event.preventDefault();
                            if (!validateForm(form)) return;

                            const formData = new FormData(form);
                            fetch('/admin/members/create', {
                                method: 'POST',
                                body: formData
                            })
                                .then(response => response.json())
                                .then(data => {
                                    if (data.success) {
                                        showSuccessMessage("Member created successfully!");
                                        modal.remove();
                                        loadMembers();
                                    } else {
                                        displayServerErrors(data.errors);
                                    }
                                })
                                .catch(error => showErrorMessage("Error creating member."));
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading create member modal:", error));
    }


    function loadEditMemberModal(memberId) {
        fetch(`/admin/members/edit/${memberId}`)
            .then(response => response.text())
            .then(html => {
                removeExistingModal("edit-member-modal");

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("edit-member-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    modal.querySelector("#closeEditMemberModal").addEventListener("click", function () {
                        modal.remove();
                    });

                    setupFileUploadPreview("editMemberForm");
                    setupEditFormSubmission(memberId);

                    const deleteButton = modal.querySelector("#deleteMemberBtn");
                    if (deleteButton) {
                        deleteButton.addEventListener("click", function () {
                            deleteMember(memberId); 
                        });
                    }
                }
            })
            .catch(error => showErrorMessage("Error loading edit member modal."));
    }

    function setupEditFormSubmission(memberId) {
        const editForm = document.getElementById("editMemberForm");
        if (!editForm) {
            console.error("Edit form not found!");
            return;
        }

        editForm.addEventListener("submit", function (event) {
            event.preventDefault();
            if (!validateForm(editForm)) return;

            const formData = new FormData(editForm);

            fetch("/admin/members/editmember", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        showSuccessMessage("Member updated successfully!");
                        document.getElementById("edit-member-modal").remove();
                        loadMembers();
                    } else {
                        displayServerErrors(data.errors);
                    }
                })
                .catch(error => showErrorMessage("Error updating member."));
        });
    }

    function deleteMember(memberId) {
        const modal = document.getElementById("deleteConfirmationModal");
        const confirmBtn = document.getElementById("confirmDelete");
        const cancelBtn = document.getElementById("cancelDelete");
        const message = document.getElementById("deleteModalMessage");

        if (!modal || !confirmBtn || !cancelBtn || !message) {
            console.error("Delete modal elements not found.");
            return;
        }
        const editModal = document.getElementById("edit-member-modal");
        if (editModal) editModal.remove(); 

        message.textContent = "Are you sure you want to delete this member?";

        modal.style.display = "flex";

        confirmBtn.onclick = function () {
            fetch(`/admin/members/delete/${memberId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        showSuccessMessage("Member deleted successfully.");
                        modal.style.display = "none";
                        loadMembers();
                    } else {
                        showErrorMessage("Error: " + data.message);
                    }
                })
                .catch(error => {
                    console.error("Error deleting member:", error);
                    showErrorMessage("An error occurred while deleting the member.");
                });
        };

        // Cancel delete
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
                if (file) {
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        imagePreview.src = event.target.result;
                        previewer.classList.add("selected");
                    };
                    reader.readAsDataURL(file);
                }
            });

            previewer.addEventListener("click", function () {
                fileInput.click();
            });
        }
    }

    function removeExistingModal(modalId) {
        const existingModal = document.getElementById(modalId);
        if (existingModal) existingModal.remove();
    }

});
