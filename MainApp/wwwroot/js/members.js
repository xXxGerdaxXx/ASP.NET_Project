document.addEventListener("DOMContentLoaded", function () {
    loadMembers(); 

    function loadMembers() {
        fetch('/admin/members/list')
            .then(response => response.text())
            .then(html => {
                const membersList = document.getElementById("membersList");
                if (!membersList) {
                    console.error("Error: #membersList element not found.");
                    return;
                }
                membersList.innerHTML = html;
                attachEventListeners();
            })
            .catch(error => console.error("Error loading members:", error));
    }

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
            .catch(error => showErrorMessage("Error loading create member modal."));
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
                    // ✅ FIX: Attach delete button event listener inside modal
                    const deleteButton = modal.querySelector("#deleteMemberBtn");
                    if (deleteButton) {
                        deleteButton.addEventListener("click", function () {
                            deleteMember(memberId); // ✅ Delete only the selected member
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
        if (!confirm("Are you sure you want to delete this member?")) return;

        fetch(`/admin/members/delete/${memberId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert("Member deleted successfully.");
                    document.getElementById("edit-member-modal")?.remove(); // Close modal if open
                    loadMembers(); // Refresh members list
                } else {
                    alert("Error: " + data.message);
                }
            })
            .catch(error => console.error("Error deleting member:", error));
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
