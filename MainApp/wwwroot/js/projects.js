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
        // Edit buttons
        document.querySelectorAll(".edit-project-btn").forEach(button => {
            button.addEventListener("click", function () {
                const projectId = this.getAttribute("data-project-id");
                loadEditProjectModal(projectId);
            });
        });

        // Open modal
        const addButton = document.getElementById("openAddProjectModal");
        if (addButton) {
            addButton.addEventListener("click", openAddProjectModal);
        }
    }

    function openAddProjectModal() {
        fetch('/projects/create')
            .then(response => response.text())
            .then(html => {
                // Remove any existing modal to prevent duplicates
                const existingModal = document.getElementById("add-project-modal");
                if (existingModal) existingModal.remove();

                // Add the new modal HTML
                const modalContainer = document.createElement("div");
                modalContainer.innerHTML = html;
                document.body.appendChild(modalContainer);

                // Open modal
                const modal = document.getElementById("add-project-modal");
                if (modal) {
                    modal.classList.add("active");
                    modal.style.display = "flex";

                    // Close button
                    modal.querySelector("#closeAddProjectModal")?.addEventListener("click", () => {
                        modal.remove();
                    });

                    // Setup file preview (if used)
                    if (typeof setupFileUploadPreview === 'function') {
                        setupFileUploadPreview("createProjectForm");
                    }

                    // Handle form submit
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
                                        alert("Project created successfully!");
                                        location.reload();
                                    } else {
                                        if (typeof displayServerErrors === 'function') {
                                            displayServerErrors(data.errors);
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
});
