document.addEventListener("DOMContentLoaded", function () {
    loadProjects();
    attachEventListeners();

    // --- FUNCTIONS ---

    function loadProjects() {
        fetch('/projects/list')
            .then(response => response.text())
            .then(html => {
                const projectsList = document.getElementById("projectsList");
                if (!projectsList) {
                    console.error("Error: #projectsList element not found.");
                    return;
                }
                projectsList.innerHTML = html;
                // Reattach listeners since the content is dynamic.
                attachEventListeners();
            })
            .catch(error => console.error("Error loading projects:", error));
    }

    function attachEventListeners() {
        // Attach listener for edit project buttons using a common class.
        document.querySelectorAll(".edit-project-btn").forEach(button => {
            button.addEventListener("click", function () {
                const projectId = this.getAttribute("data-project-id");
                loadEditProjectModal(projectId);
            });
        });

        // Attach listener for the "add project" button if it exists.
        const addButton = document.getElementById("openAddProjectModal");
        if (addButton) {
            addButton.addEventListener("click", openAddProjectModal);
        }
    }

    function openAddProjectModal() {
        fetch('/projects/create')
            .then(response => response.text())
            .then(html => {
                // Remove any existing add-project modal to avoid duplicates.
                const existingModal = document.getElementById("add-project-modal");
                if (existingModal) existingModal.remove();

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("add-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";
                    initQuillEditor();
                    // Close button listener.
                    modal.querySelector("#closeAddProjectModal")?.addEventListener("click", () => {
                        modal.remove();
                    });

                    // Initialize the tag selector.
                    initTagSelector({
                        containerId: 'tagged-members',
                        inputId: 'member-search',
                        resultsId: 'member-search-results',
                        searchUrl: (query) => `/admin/members/search?term=${encodeURIComponent(query)}`,
                        displayProperty: 'tagName',
                        imageProperty: 'avatar',
                        tagClass: 'tag',
                        tagType: 'member',
                        avatarFolder: '',
                        emptyMessage: 'No members found.',
                        preselected: [],
                        hiddenInputId: 'SelectedTeamMemberIds'
                    });

                    // Set up file upload preview.
                    setupFileUploadPreview("createProjectForm");

                    // If using jQuery unobtrusive validation, reparse the form.
                    if (window.jQuery && $.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse("#createProjectForm");
                    }

                    // Handle the form submission.
                    const form = modal.querySelector("#createProjectForm");
                    if (form) {
                        form.addEventListener("submit", function (event) {
                            event.preventDefault();

                            // Optionally validate form if you have a validateForm function.
                            if (typeof validateForm === 'function' && !validateForm(form)) return;

                            const formData = new FormData(form);
                            fetch('/projects/create', {
                                method: 'POST',
                                body: formData
                            })
                                .then(response => response.json())
                                .then(data => {
                                    if (data.success) {
                                        showSuccessMessage("Project created successfully!");
                                        location.reload();
                                    } else {
                                        if (data.errors) {
                                            displayServerErrors(data.errors);
                                        } else {
                                            showErrorMessage(data.message || "An unknown error occurred.");
                                        }
                                    }
                                })
                                .catch(error => console.error("Error creating project:", error));
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading create project modal:", error));
    }

    // Expose loadEditProjectModal to the global scope for inline calls if necessary.
    window.loadEditProjectModal = function (projectId) {
        fetch(`/projects/edit/${projectId}`)
            .then(response => response.text())
            .then(html => {
                removeExistingModal("edit-project-modal");

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("edit-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";
                    initQuillEditor();


                    modal.querySelector("#closeEditProjectModal")?.addEventListener("click", function () {
                        modal.remove();
                    });

                    setupFileUploadPreview("editProjectForm");

                    const editProjectForm = document.getElementById("editProjectForm");
                    if (editProjectForm) {
                        const preSelectedMembersJson = editProjectForm.dataset.preselectedMembers;
                        const preSelectedMembers = JSON.parse(preSelectedMembersJson || "[]");

                        initTagSelector({
                            containerId: 'edit-tags',
                            inputId: 'edit-tag-search',
                            resultsId: 'edit-tag-search-results',
                            searchUrl: (query) => `/admin/members/search?term=${encodeURIComponent(query)}`,
                            displayProperty: 'tagName',
                            imageProperty: 'avatar',
                            tagClass: 'tag',
                            tagType: 'member',
                            avatarFolder: '',
                            emptyMessage: 'No members found.',
                            preselected: preSelectedMembers,
                            hiddenInputId: 'SelectedTeamMemberIds'
                        });

                        if (window.jQuery && $.validator && $.validator.unobtrusive) {
                            $.validator.unobtrusive.parse("#editProjectForm");
                        }

                        setupEditFormSubmission(projectId);
                    }

                    const deleteButton = modal.querySelector("#deleteProjectBtn");
                    if (deleteButton) {
                        deleteButton.addEventListener("click", function () {
                            deleteProject(projectId);
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading edit project modal.", error));
    };

    function setupEditFormSubmission(projectId) {
        const editForm = document.getElementById("editProjectForm");
        if (!editForm) {
            console.error("Edit form not found!");
            return;
        }

        editForm.addEventListener("submit", function (event) {
            event.preventDefault();

            if (typeof validateForm === 'function' && !validateForm(editForm)) return;

            const formData = new FormData(editForm);
            fetch("/projects/editproject", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        showSuccessMessage("Project updated successfully!");
                        document.getElementById("edit-project-modal").remove();
                        loadProjects();
                    } else {
                        displayServerErrors(data.errors);
                    }
                })
                .catch(error => showErrorMessage("Error updating project."));
        });
    }

    function deleteProject(projectId) {
        if (!confirm("Are you sure you want to delete this project?")) return;

        fetch(`/projects/delete/${projectId}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showSuccessMessage("Project deleted successfully.");
                    document.getElementById("edit-project-modal")?.remove();
                    loadProjects();
                } else {
                    showErrorMessage("Error: " + data.message);
                }
            })
            .catch(error => console.error("Error deleting project:", error));
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

    function displayServerErrors(errors) {
        for (const [field, messages] of Object.entries(errors)) {
            const span = document.querySelector(`[data-valmsg-for="${field}"]`);
            if (span) {
                span.innerText = messages.join(", ");
                span.classList.add("text-danger");
            }
        }
    }
});

