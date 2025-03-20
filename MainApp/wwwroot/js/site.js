document.addEventListener("DOMContentLoaded", function () {
    console.log("site.js Loaded");

    // Attach Global Event Listeners
    attachGeneralEventListeners();

    function attachGeneralEventListeners() {
        console.log("Attaching general event listeners...");

        // Handle Open/Close Modals (General)
        document.querySelectorAll('[data-modal="true"]').forEach(button => {
            button.addEventListener("click", function () {
                const modalTarget = this.getAttribute("data-target");
                const modal = document.querySelector(modalTarget);
                if (modal) modal.style.display = "flex";
            });
        });

        document.querySelectorAll('[data-close="true"]').forEach(button => {
            button.addEventListener("click", function () {
                const modal = this.closest(".modal");
                if (modal) modal.style.display = "none";
            });
        });

        // Handle Global Notification Button
        const notificationButton = document.getElementById("notificationsButton");
        const notificationBadge = document.querySelector(".notification-badge");

        if (notificationButton && notificationBadge) {
            setTimeout(() => {
                notificationBadge.style.display = "block";
            }, 2000);

            notificationButton.addEventListener("click", function () {
                alert("You have new notifications!");
                notificationBadge.style.display = "none";
            });
        }
    }

    // Global Function to Show Success Notifications
    window.showSuccessMessage = function (message) {
        const notification = document.getElementById("createSuccessNotification");
        if (notification) {
            notification.querySelector("p").textContent = message;
            notification.style.display = "block";
            setTimeout(() => {
                notification.style.display = "none";
            }, 3000); // Auto-hide after 3 seconds
        }
    };

    // Global Function to Show Error Notifications
    window.showErrorMessage = function (message) {
        const notification = document.getElementById("deleteErrorNotification");
        if (notification) {
            notification.querySelector("p").textContent = message;
            notification.style.display = "block";
            setTimeout(() => {
                notification.style.display = "none";
            }, 3000);
        }
    };
});


//document.addEventListener('DOMContentLoaded', () => {
//    console.log("JavaScript Loaded");

//    const previewSize = 150;

//    //  Open Modal
//    document.querySelectorAll('[data-modal="true"]').forEach(button => {
//        button.addEventListener('click', () => {
//            const modalTarget = button.getAttribute('data-target');
//            const modal = document.querySelector(modalTarget);
//            if (modal) modal.style.display = 'flex';
//        });
//    });

//    //  Close Modal & Reset Form
//    document.querySelectorAll('[data-close="true"]').forEach(button => {
//        button.addEventListener('click', () => {
//            const modal = button.closest('.modal');
//            if (modal) {
//                modal.style.display = 'none';
//                modal.querySelectorAll('form').forEach(form => {
//                    form.reset();
//                    const imagePreview = form.querySelector('.image-preview');
//                    if (imagePreview) imagePreview.src = '';
//                    const imagePreviewer = form.querySelector('.image-previewer');
//                    if (imagePreviewer) imagePreviewer.classList.remove('selected');
//                });
//            }
//        });
//    });

//    // Image Previewer
//    document.querySelectorAll('.image-previewer').forEach(previewer => {
//        const fileInput = previewer.querySelector('input[type="file"]');
//        const imagePreview = previewer.querySelector('img');

//        if (!fileInput || !imagePreview) {
//            console.warn("⚠️ Missing file input or image preview element in:", previewer);
//            return;
//        }

//        previewer.addEventListener('click', () => fileInput.click());

//        fileInput.addEventListener('change', async (event) => {
//            const file = event.target.files[0];
//            if (file) {
//                try {
//                    await processImage(file, imagePreview, previewer, previewSize);
//                } catch (error) {
//                    console.error("❌ Error processing image:", error);
//                }
//            }
//        });
//    });

//    // Handle Form Submission (Prevent Page Reload & Update Table)
//    document.querySelectorAll('form').forEach(form => {
//        form.addEventListener('submit', async (e) => {
//            console.log("🚀 Form submit intercepted");
//            e.preventDefault();

//            clearErrorMessages(form);
//            const formData = new FormData(form);

//            try {
//                console.log("Sending form data to:", form.action);
//                const res = await fetch(form.action, {
//                    method: 'POST',
//                    body: formData
//                });

//                if (res.ok) {
//                    console.log("Form submitted successfully");

//                    //  Reload the client list after successful submission
//                    await reloadClientList();

//                    //  Close Modal & Reset Form
//                    const modal = form.closest('.modal');
//                    if (modal) {
//                        modal.style.display = 'none';
//                        form.reset();
//                    }
//                } else {
//                    console.error("Form submission failed", res.status);
//                }
//            } catch (error) {
//                console.error("Error submitting form:", error);
//            }
//        });
//    });

//    //  Reload Client List (Fetch Partial View)
//    async function reloadClientList() {
//        console.log("Reloading client list...");
//        try {
//            const res = await fetch("/admin/clients");
//            if (res.ok) {
//                const html = await res.text();
//                const clientListContainer = document.getElementById("clientListContainer");

//                if (clientListContainer) {
//                    clientListContainer.innerHTML = html;
//                    console.log("Client list updated successfully");
//                } else {
//                    console.error("Element #clientListContainer not found!");
//                }
//            } else {
//                console.error("Failed to reload client list", res.status);
//            }
//        } catch (error) {
//            console.error("Error reloading client list:", error);
//        }
//    }

//    //  Notification Button Simulation
//    const notificationButton = document.getElementById("notificationsButton");
//    const notificationBadge = document.querySelector(".notification-badge");

//    if (notificationButton && notificationBadge) {
//        setTimeout(() => {
//            notificationBadge.style.display = "block";
//        }, 2000);

//        notificationButton.addEventListener("click", function () {
//            alert("You have new notifications!");
//            notificationBadge.style.display = "none";
//        });
//    }

//    // Filtering Projects
//    document.querySelectorAll(".tab").forEach(tab => {
//        tab.addEventListener("click", function () {
//            const filter = this.getAttribute("data-filter");

//            document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));
//            this.classList.add("active");

//            document.querySelectorAll(".project-item").forEach(item => {
//                item.style.display = (filter === "all" || item.getAttribute("data-status") === filter) ? "block" : "none";
//            });
//        });
//    });

//    // Search Functionality for Team Members
//    const searchIcon = document.querySelector(".search-icon");
//    const searchInput = document.querySelector("#teamMemberSearch");
//    const teamMemberList = document.querySelector("#teamMemberList");

//    if (searchIcon && searchInput && teamMemberList) {
//        searchIcon.addEventListener("click", function () {
//            teamMemberList.style.display = (teamMemberList.style.display === "block") ? "none" : "block";
//        });

//        document.addEventListener("click", function (event) {
//            if (!searchIcon.contains(event.target) && !searchInput.contains(event.target) && !teamMemberList.contains(event.target)) {
//                teamMemberList.style.display = "none";
//            }
//        });

//        document.querySelectorAll(".team-member-list li").forEach(item => {
//            item.addEventListener("click", function () {
//                searchInput.value = this.textContent;
//                teamMemberList.style.display = "none";
//            });
//        });
//    }

//    // Date Formatting for Start and End Dates
//    function formatDate(dateString) {
//        let date = new Date(dateString);
//        return date.toLocaleDateString('en-US', {
//            month: 'long',
//            day: 'numeric',
//            year: 'numeric'
//        });
//    }

//    function setupDateField(formattedInputId, hiddenInputId) {
//        const formattedInput = document.getElementById(formattedInputId);
//        const hiddenInput = document.getElementById(hiddenInputId);

//        if (!formattedInput || !hiddenInput) return;

//        formattedInput.addEventListener("click", function () {
//            hiddenInput.showPicker();
//        });

//        hiddenInput.addEventListener("change", function () {
//            formattedInput.value = formatDate(hiddenInput.value);
//        });

//        if (hiddenInput.value) {
//            formattedInput.value = formatDate(hiddenInput.value);
//        }
//    }

//    setupDateField("formattedStartDate", "startDate");
//    setupDateField("formattedEndDate", "endDate");

//    // Function to Clear Error Messages
//    function clearErrorMessages(form) {
//        form.querySelectorAll('[data-val="true"]').forEach(input => {
//            input.classList.remove('input-validation-error');
//        });

//        form.querySelectorAll('[data-valmsg-for]').forEach(span => {
//            span.innerText = '';
//            span.classList.remove('field-validation-error');
//        });
//    }

//    // Function to Load an Image
//    async function loadImage(file) {
//        return new Promise((resolve, reject) => {
//            const reader = new FileReader();

//            reader.onerror = () => reject(new Error("Failed to load file."));
//            reader.onload = (e) => {
//                const img = new Image();
//                img.onerror = () => reject(new Error("Failed to load image"));
//                img.onload = () => resolve(img);
//                img.src = e.target.result;
//            };

//            reader.readAsDataURL(file);
//        });
//    }

//    // Function to Process and Resize Image
//    async function processImage(file, imagePreview, previewer, previewSize = 150) {
//        try {
//            const img = await loadImage(file);
//            const canvas = document.createElement('canvas');
//            canvas.width = previewSize;
//            canvas.height = previewSize;

//            const ctx = canvas.getContext('2d');
//            ctx.drawImage(img, 0, 0, previewSize, previewSize);

//            imagePreview.src = canvas.toDataURL('image/jpeg');
//            previewer.classList.add('selected');
//        } catch (error) {
//            console.error('Failed on image processing:', error);
//        }
//    }
//});
