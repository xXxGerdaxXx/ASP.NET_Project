document.addEventListener("DOMContentLoaded", function () {
    loadProjects();

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
                attachEventListeners();
            })
            .catch(error => console.error("Error loading projects:", error));
    }

    function attachEventListeners() {
        document.querySelectorAll(".edit-project-btn").forEach(button => {
            button.addEventListener("click", function () {
                const projectId = this.getAttribute("data-project-id");
                loadEditProjectModal(projectId);
            });
        });

        const addButton = document.getElementById("openAddProjectModal");
        if (addButton) {
            addButton.addEventListener("click", openAddProjectModal);
        }
    }

    function openAddProjectModal() {
        fetch('/projects/create')
            .then(response => response.text())
            .then(html => {
                const existingModal = document.getElementById("add-project-modal");
                if (existingModal) existingModal.remove();

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("add-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    modal.querySelector("#closeAddProjectModal")?.addEventListener("click", () => {
                        modal.remove();
                    });
                    console.log("Calling initTagSelector...");
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

                    if (typeof setupFileUploadPreview === 'function') {
                        setupFileUploadPreview("createProjectForm");
                    }

                    if (window.jQuery && $.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse("#createProjectForm");
                    }

                    const form = modal.querySelector("#createProjectForm");
                    if (form) {
                        form.addEventListener("submit", function (event) {
                            event.preventDefault();
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

    function loadEditProjectModal(projectId) {

        console.log("1")

        fetch(`/projects/edit/${projectId}`)
            .then(response => response.text())

            console.log(respone)
            .then(html => {
                removeExistingModal("edit-project-modal");

                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("edit-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    modal.querySelector("#closeEditProjectModal")?.addEventListener("click", function () {
                        modal.remove();
                    });

                    setupFileUploadPreview(projectId);

                    const preSelectedMembersJson = document.getElementById("editProjectForm").dataset.preselectedMembers;
                    const preSelectedMembers = JSON.parse(preSelectedMembersJson || "[]");
                    console.log("Preselected members:", preSelectedMembers);

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

                    const deleteButton = modal.querySelector("#deleteProjectBtn");
                    if (deleteButton) {
                        deleteButton.addEventListener("click", function () {
                            deleteProject(projectId);
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading edit project modal.", error));
    }


    function setupEditFormSubmission(projectId) {
        const editForm = document.getElementById("editProjectForm");
        if (!editForm) {
            console.error("Edit form not found!");
            return;
        }

        editForm.addEventListener("submit", function (event) {
            event.preventDefault();
            if (!validateForm(editForm)) return;

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
            headers: {
                "Content-Type": "application/json"
            }
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
