document.addEventListener("DOMContentLoaded", function () {
    loadProjects();
    attachEventListeners();

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

        const filterButtons = document.querySelectorAll(".project-tabs .tab");
        filterButtons.forEach(button => {
            button.addEventListener("click", function () {
                const filter = this.dataset.filter;

                filterButtons.forEach(btn => btn.classList.remove("active"));
                this.classList.add("active");

                fetch(`/projects/filter?status=${filter}`)
                    .then(response => response.json())
                    .then(data => {
                        const projectsList = document.querySelector("#projectsList");
                        if (projectsList) {
                            projectsList.innerHTML = data.html;
                            attachEventListeners();
                        }

                        document.querySelector('[data-filter="all"] span').textContent = `[${data.counts.all}]`;
                        document.querySelector('[data-filter="Not Started"] span').textContent = `[${data.counts.notStarted}]`;
                        document.querySelector('[data-filter="In Progress"] span').textContent = `[${data.counts.started}]`;
                        document.querySelector('[data-filter="Completed"] span').textContent = `[${data.counts.completed}]`;
                    })
                    .catch(error => console.error("Error loading projects:", error));
            });
        });
    }
/*
* This function is triggered when the "Add Project" button is pressed. It opens a modal for creating a new project and 
* loads the HTML from partial view. 
* 
* I used ChatGPT to help me generate and understand this function. I asked for advice on how to dynamically load a modal
* with Razor content, initialize form components like Quill and a custom tag selector, and handle AJAX form submissions
* using `fetch`. Then I customized it fit the structure of my app.
*/

    function openAddProjectModal() {
/* `fetch('/projects/create')` sends a GET request to load the partial view that contains the create form */
        fetch('/projects/create')
 /* converts the HTML response into a string so it can be injected into the DOM */
            .then(response => response.text())
            .then(html => {
/* removes any existing "add-project-modal" */
                const existingModal = document.getElementById("add-project-modal");
                if (existingModal) existingModal.remove();
/* Creates a new container for the modal and injects HTML */
                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                const modal = document.getElementById("add-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";
/* initializes the Quill rich text editor*/
                    initQuillEditor();
/* adds close button functionality*/
                    modal.querySelector("#closeAddProjectModal")?.addEventListener("click", () => {

                        modal.remove();
                    });

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
/* sets up image file preview */
                    setupFileUploadPreview("createProjectForm");
/* applies jQuery validation rules if available */
                    if (window.jQuery && $.validator && $.validator.unobtrusive) {
                        $.validator.unobtrusive.parse("#createProjectForm");
                    }
/* handle form submission */
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
/* Reload page to reflect new project */
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

                        const tagSelector = initTagSelector({
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

    function setupEditFormSubmission(projectId, tagSelector) {
        const editForm = document.getElementById("editProjectForm");
        const modal = document.getElementById("edit-project-modal");
        if (!editForm || !modal) return;

        editForm.addEventListener("submit", async function handleEditSubmit(event) {
            event.preventDefault();

            if (typeof validateForm === 'function' && !validateForm(editForm)) return;

            if (tagSelector && typeof tagSelector.updateHiddenInput === 'function') {
                tagSelector.updateHiddenInput();
            }

            const formData = new FormData(editForm);
            try {
                const response = await fetch("/projects/editproject", {
                    method: 'POST',
                    body: formData
                });

                const data = await response.json();

                if (data.success) {
                    showSuccessMessage("Project updated successfully!");
                    modal.remove();
                    loadProjects();
                } else {
                    displayServerErrors(data.errors || {});
                }
            } catch (error) {
                console.error("Error updating project:", error);
                showErrorMessage("Error updating project.");
            }
        }, { once: true });
    }

    window.openAddMemberModal = function (projectId) {
        fetch(`/projects/addmembermodal/${projectId}`)
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

                    modal.querySelector("#closeAddMemberModal")?.addEventListener("click", function () {
                        modal.remove();
                    });

                    const form = modal.querySelector("#addMemberForm");

                    let preSelectedMembers = [];
                    if (form) {
                        const preSelectedMembersJson = form.dataset.preselectedMembers;
                        if (preSelectedMembersJson) {
                            try {
                                preSelectedMembers = JSON.parse(preSelectedMembersJson);
                            } catch (error) {
                                console.error("Error parsing preselected members JSON", error);
                            }
                        }
                    }

                    initTagSelector({
                        containerId: 'add-member-tags',
                        inputId: 'add-member-search',
                        resultsId: 'add-member-search-results',
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

                    if (form) {
                        form.addEventListener("submit", function (event) {
                            event.preventDefault();

                            const formData = new FormData(form);
                            fetch(`/projects/addmembers`, {
                                method: 'POST',
                                body: formData
                            })
                                .then(response => response.json())
                                .then(data => {
                                    if (data.success) {
                                        showSuccessMessage("Member(s) added successfully!");
                                        modal.remove();
                                        loadProjects();
                                    } else {
                                        showErrorMessage(data.message || "Failed to add members.");
                                    }
                                })
                                .catch(error => console.error("Error adding members:", error));
                        });
                    }
                }
            })
            .catch(error => console.error("Error loading add member modal:", error));
    };

    window.showProjectDeleteModal = function (projectId) {
        const modal = document.getElementById("deleteConfirmationModal");
        const confirmBtn = document.getElementById("confirmDelete");
        const cancelBtn = document.getElementById("cancelDelete");
        const message = document.getElementById("deleteModalMessage");

        modal.dataset.projectId = projectId;

        modal.style.display = "flex";
        message.textContent = "Are you sure you want to delete this project?";

        confirmBtn.onclick = function () {
            const id = modal.dataset.projectId;
            fetch(`/projects/delete/${id}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" }
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        showSuccessMessage("Project deleted successfully.");
                        modal.style.display = "none";
                        loadProjects();
                    } else {
                        showErrorMessage("Error: " + data.message);
                    }
                });
        };

        cancelBtn.onclick = function () {
            modal.style.display = "none";
        };
    };

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