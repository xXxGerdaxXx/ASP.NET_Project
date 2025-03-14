document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ JavaScript Loaded!");

    // ✅ Open Modal
    document.querySelectorAll("[data-modal-target]").forEach(button => {
        button.addEventListener("click", function () {
            const modalId = this.getAttribute("data-modal-target");
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.classList.add("active");
                console.log(`✅ Opened modal: ${modalId}`);
            }
        });
    });

    // ✅ Close Modal
    const closeButtons = document.querySelectorAll('[data-close="true"]');
    closeButtons.forEach(button => {
        button.addEventListener("click", () => {
            const modal = button.closest('.modal-overlay');
            if (modal) {
                modal.style.display = 'none'; // Hide the modal

                modal.querySelectorAll('form').forEach(form => {
                    form.reset(); // Reset the form inputs

                    const imagePreview = form.querySelector('.image-preview img');
                    if (imagePreview) {
                        imagePreview.src = ''; // Clear the image preview
                    }

                    const imagePreviewer = form.querySelector('.image-previewer'); // Fixed class selector
                    if (imagePreviewer) {
                        imagePreviewer.classList.remove('selected'); // Remove 'selected' class
                    }
                });
            }
        });
    });




    // ✅ Notification Button (Simulating notifications)
    const notificationButton = document.getElementById("notificationsButton");
    const notificationBadge = document.querySelector(".notification-badge");

    // Simulate new notifications (Replace this with real API data)
    setTimeout(() => {
        notificationBadge.style.display = "block"; // Show badge
    }, 2000);

    notificationButton.addEventListener("click", function () {
        alert("You have new notifications!");
        notificationBadge.style.display = "none"; // Hide badge after click
    });

    // ✅ Filtering Projects
    document.querySelectorAll(".tab").forEach(tab => {
        tab.addEventListener("click", function () {
            const filter = this.getAttribute("data-filter");

            // ✅ Remove "active" from all tabs
            document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));
            this.classList.add("active");

            // ✅ Show/Hide Projects Based on Filter
            document.querySelectorAll(".project-item").forEach(item => {
                if (filter === "all" || item.getAttribute("data-status") === filter) {
                    item.style.display = "block";
                } else {
                    item.style.display = "none";
                }
            });
        });
    });

    // ✅ Search Functionality for Team Members
    const searchIcon = document.querySelector(".search-icon");
    const searchInput = document.querySelector("#teamMemberSearch");
    const teamMemberList = document.querySelector("#teamMemberList");

    // Toggle visibility of team member list
    searchIcon.addEventListener("click", function () {
        teamMemberList.style.display = (teamMemberList.style.display === "block") ? "none" : "block";
    });

    // Hide the list when clicking outside
    document.addEventListener("click", function (event) {
        if (!searchIcon.contains(event.target) && !searchInput.contains(event.target) && !teamMemberList.contains(event.target)) {
            teamMemberList.style.display = "none";
        }
    });

    // Select team member from the list
    document.querySelectorAll(".team-member-list li").forEach(item => {
        item.addEventListener("click", function () {
            searchInput.value = this.textContent;
            teamMemberList.style.display = "none";
        });
    });

    // ✅ Quill Editor Initialization
    var quill = new Quill('#quill-editor', {
        theme: 'snow',
        placeholder: 'Type something...',
        modules: {
            toolbar: [
                ['bold', 'italic', 'underline'],
                [{ 'align': '' }, { 'align': 'center' }, { 'align': 'right' }],
                [{ 'list': 'bullet' }, { 'list': 'ordered' }],
                ['link']
            ]
        }
    });

    // Move toolbar outside, placing it after the Quill container
    var quillContainer = document.querySelector('.ql-container');
    var quillToolbar = document.querySelector('.ql-toolbar');
    quillContainer.parentNode.insertBefore(quillToolbar, quillContainer.nextSibling);

    // Sync Quill content with the hidden textarea before form submission
    document.querySelector("form").onsubmit = function () {
        document.querySelector("#description").value = quill.root.innerHTML;
    };

    // ✅ Date Formatting for Start and End Dates
    function formatDate(dateString) {
        let date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            month: 'long',
            day: 'numeric',
            year: 'numeric'
        });
    }

    function setupDateField(formattedInputId, hiddenInputId) {
        const formattedInput = document.getElementById(formattedInputId);
        const hiddenInput = document.getElementById(hiddenInputId);

        formattedInput.addEventListener("click", function () {
            hiddenInput.showPicker(); // Open native date picker
        });

        hiddenInput.addEventListener("change", function () {
            formattedInput.value = formatDate(hiddenInput.value); // Format date for display
        });

        // Initialize input if a value exists
        if (hiddenInput.value) {
            formattedInput.value = formatDate(hiddenInput.value);
        }
    }

    // Apply to both start and end date fields
    setupDateField("formattedStartDate", "startDate");
    setupDateField("formattedEndDate", "endDate");
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.image-preview').forEach(previewer => {
        const fileInput = previewer.querySelector('input[type="file"]');
        const imagePreview = previewer.querySelector('img');

        // Check if fileInput and imagePreview are valid
        if (!fileInput || !imagePreview) {
            console.error("Missing file input or image preview element.");
            return;
        }

        // Trigger the file input click when previewer is clicked
        previewer.addEventListener('click', () => fileInput.click());

        // Handle file selection and update the preview image
        fileInput.addEventListener('change', ({ target: { files } }) => {
            const file = files[0];
            if (file) {
                processImage(file, imagePreview, previewer, previewSize);
            }
        });

        /* Loading image function */
        async function loadImage(file) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();

                reader.onerror = () => reject(new Error("Failed to load file."));
                reader.onload = (e) => {
                    const img = new Image();
                    img.onerror = () => reject(new Error("Failed to load image"));
                    img.onload = () => resolve(img);
                    img.src = e.target.result;
                };

                reader.readAsDataURL(file);
            });
        }

        // Function to process and resize the image
        async function processImage(file, imagePreview, previewer, previewSize = 150) {
            try {
                const img = await loadImage(file);
                const canvas = document.createElement("canvas");
                canvas.width = previewSize;
                canvas.height = previewSize;

                const ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0, previewSize, previewSize);

                // Update imagePreview to show the resized image
                imagePreview.src = canvas.toDataURL();  // Convert canvas content to a data URL and set it as the image source
            } catch (error) {
                console.error(error);
            }
        }
    });
});
