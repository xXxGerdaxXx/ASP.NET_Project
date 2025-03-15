document.addEventListener('DOMContentLoaded', () => {
    const previewSize = 150;

    // ✅ Open Modal
    document.querySelectorAll('[data-modal="true"]').forEach(button => {
        button.addEventListener('click', () => {
            const modalTarget = button.getAttribute('data-target');
            const modal = document.querySelector(modalTarget);
            if (modal) modal.style.display = 'flex';
        });
    });

// Close modal
const closeButtons = document.querySelectorAll('[data-close="true"]');
closeButtons.forEach(button => {
    button.addEventListener('click', () => {
        const modal = button.closest('.modal');
        if (modal) {
            modal.style.display = 'none';

            modal.querySelectorAll('form').forEach(form => {
                form.reset();

                const imagePreview = form.querySelector('.image-preview');
                if (imagePreview) 
                    imagePreview.src = '';

                const imagePreviewer = form.querySelector('.image-previewer');
                if (imagePreviewer) 
                    imagePreviewer.classList.remove('selected');
            });
        }
    });
});


    // ✅ Image Previewer
    document.querySelectorAll('.image-previewer, .image-preview').forEach(previewer => {
        const fileInput = previewer.querySelector('input[type="file"]');
        const imagePreview = previewer.querySelector('img');

        if (!fileInput || !imagePreview) {
            console.error("Missing file input or image preview element.");
            return;
        }

        previewer.addEventListener('click', () => fileInput.click());

        fileInput.addEventListener('change', async ({ target: { files } }) => {
            const file = files[0];
            if (file) {
                try {
                    await processImage(file, imagePreview, previewer, previewSize);
                } catch (error) {
                    console.error("Error processing image:", error);
                }
            }
        });
    });

    // handle submit forms
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault(); // Stop full page reload

            clearErrorMessages(form);
            const formData = new FormData(form);

            try {
                const res = await fetch(form.action, {
                    method: 'POST',
                    body: formData
                });

                if (res.ok) {
                    const html = await res.text();
                    document.getElementById("clientTableBody").innerHTML = html; // ✅ Update table
                    const modal = form.closest('.modal');
                    if (modal) modal.style.display = 'none'; // Close modal
                } else if (res.status === 400) {
                    const data = await res.json();
                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
                            let input = form.querySelector(`[name="${key}"]`);
                            if (input) input.classList.add('input-validation-error');
                            let span = form.querySelector(`[data-valmsg-for="${key}"]`);
                            if (span) {
                                span.innerText = data.errors[key].join('\n');
                                span.classList.add('field-validation-error');
                            }
                        });
                    }
                }
            } catch (error) {
                console.error("❌ Error submitting form:", error);
                alert("Failed to create client. Check console for details.");
            }
        });
    });
});



function clearErrorMessages(form) {
    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove('input-validation-error');
    })

    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.innerText = '';
        span.classList.remove('field-validation-error');
    })
}

function addErrorMessage(key, errormessage) {

}
                            



    // ✅ Function to load an image
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

    // ✅ Function to process and resize the image
    async function processImage(file, imagePreview, previewer, previewSize = 150) {
        try {
            const img = await loadImage(file);
            const canvas = document.createElement('canvas');
            canvas.width = previewSize;
            canvas.height = previewSize;

            const ctx = canvas.getContext('2d');
            ctx.drawImage(img, 0, 0, previewSize, previewSize);

            imagePreview.src = canvas.toDataURL('image/jpeg');
            previewer.classList.add('selected');
        } catch (error) {
            console.error('Failed on image processing:', error);
        }
    }

    // ✅ Notification Button Simulation
    const notificationButton = document.getElementById("notificationsButton");
    const notificationBadge = document.querySelector(".notification-badge");

    setTimeout(() => {
        notificationBadge.style.display = "block";
    }, 2000);

    notificationButton.addEventListener("click", function () {
        alert("You have new notifications!");
        notificationBadge.style.display = "none";
    });

    // ✅ Filtering Projects
    document.querySelectorAll(".tab").forEach(tab => {
        tab.addEventListener("click", function () {
            const filter = this.getAttribute("data-filter");

            document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));
            this.classList.add("active");

            document.querySelectorAll(".project-item").forEach(item => {
                item.style.display = (filter === "all" || item.getAttribute("data-status") === filter) ? "block" : "none";
            });
        });
    });

    // ✅ Search Functionality for Team Members
    const searchIcon = document.querySelector(".search-icon");
    const searchInput = document.querySelector("#teamMemberSearch");
    const teamMemberList = document.querySelector("#teamMemberList");

    searchIcon.addEventListener("click", function () {
        teamMemberList.style.display = (teamMemberList.style.display === "block") ? "none" : "block";
    });

    document.addEventListener("click", function (event) {
        if (!searchIcon.contains(event.target) && !searchInput.contains(event.target) && !teamMemberList.contains(event.target)) {
            teamMemberList.style.display = "none";
        }
    });

    document.querySelectorAll(".team-member-list li").forEach(item => {
        item.addEventListener("click", function () {
            searchInput.value = this.textContent;
            teamMemberList.style.display = "none";
        });
    });

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
            hiddenInput.showPicker();
        });

        hiddenInput.addEventListener("change", function () {
            formattedInput.value = formatDate(hiddenInput.value);
        });

        if (hiddenInput.value) {
            formattedInput.value = formatDate(hiddenInput.value);
        }
    }

    setupDateField("formattedStartDate", "startDate");
    setupDateField("formattedEndDate", "endDate");

