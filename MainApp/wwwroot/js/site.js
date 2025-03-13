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
    document.querySelectorAll("[data-close-modal]").forEach(button => {
        button.addEventListener("click", function () {
            this.closest(".modal-overlay").classList.remove("active");
            console.log("❌ Modal closed!");
        });
    });

    // ✅ Close Modal When Clicking Outside
    window.addEventListener("click", function (event) {
        if (event.target.classList.contains("modal-overlay")) {
            event.target.classList.remove("active");
            console.log("❌ Clicked outside modal, closing...");
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
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
});

document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ JavaScript Loaded!");

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
});

document.addEventListener("DOMContentLoaded", function () {
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
});
document.addEventListener("DOMContentLoaded", function () {
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
});

document.addEventListener("DOMContentLoaded", function () {
    function formatDate(dateString) {
        let date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            month: 'long', // Display full month name
            day: 'numeric', // Display day number
            year: 'numeric' // Display full year
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
