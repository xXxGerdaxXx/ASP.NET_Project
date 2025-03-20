//document.addEventListener("DOMContentLoaded", function () {
//    attachGlobalEditEventListeners(); // Attach event listeners on page load
//});

//// ✅ Attach Click Events to All Edit Buttons
//function attachGlobalEditEventListeners() {
//    document.querySelectorAll(".edit-button").forEach(button => {
//        button.addEventListener("click", function () {
//            const entityType = this.dataset.entity; // "client", "team-member", "project"
//            const entityId = this.dataset.id;
//            const modalSelector = this.dataset.modal;

//            if (!entityType || !entityId || !modalSelector) {
//                console.error("❌ Missing required attributes on edit button:", this);
//                return;
//            }

//            console.log(`🔍 Fetching ${entityType} data for ID ${entityId}`);

//            // ✅ Generate API URL based on entity type
//            const apiUrl = `/admin/${entityType}s/get/${entityId}`;

//            fetch(apiUrl)
//                .then(response => {
//                    if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
//                    return response.json();
//                })
//                .then(data => {
//                    if (data.success) {
//                        populateEditModal(modalSelector, data[entityType]); // Pass entity data to function
//                        document.querySelector(modalSelector).style.display = "flex"; // ✅ Open modal
//                    } else {
//                        alert("Error: " + data.message);
//                    }
//                })
//                .catch(error => console.error(`❌ Error loading ${entityType}:`, error));
//        });
//    });
//}

//// ✅ Function to Populate the Correct Edit Modal
//function populateEditModal(modalSelector, data) {
//    const modal = document.querySelector(modalSelector);
//    if (!modal) {
//        console.error("❌ Modal not found:", modalSelector);
//        return;
//    }

//    console.log("✅ Populating modal with data:", data);

//    // Loop through all inputs inside the modal and populate them with data
//    modal.querySelectorAll("input, textarea, select").forEach(input => {
//        const fieldName = input.name || input.getAttribute("asp-for");
//        if (fieldName && data[fieldName] !== undefined) {
//            input.value = data[fieldName];
//        }
//    });

//    // Handle image preview if applicable
//    const previewImage = modal.querySelector(".image-preview");
//    if (previewImage && data.avatarUrl) {
//        previewImage.src = data.avatarUrl || "/images/default-avatar.png";
//    }
//}
