document.addEventListener('click', async function (e) {
    const trigger = e.target.closest('[data-url]');
    if (!trigger) return;

    e.preventDefault();

    const url = trigger.getAttribute('data-url');
    const response = await fetch(url);
    const html = await response.text();

    let modal = document.getElementById("ajax-profile-modal");
    if (modal) modal.remove();

    const wrapper = document.createElement("div");
    wrapper.id = "ajax-profile-modal";
    wrapper.innerHTML = html;
    document.body.appendChild(wrapper);

    const modalElement = wrapper.querySelector("#edit-profile-modal");
    if (modalElement) modalElement.style.display = "flex";
});
document.addEventListener('submit', async function (e) {
    const form = e.target;
    if (form.id === "editProfileForm") {
        e.preventDefault();

        const formData = new FormData(form);

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                showSuccessMessage("Profile updated!");
                document.getElementById("edit-profile-modal").style.display = "none";
                location.reload();
            } else {
                const html = await response.text();
                document.querySelector("#edit-profile-modal .surface-modal").innerHTML = html;
            }
        } catch (err) {
            showErrorMessage("Something went wrong.");
        }
    }
});
document.addEventListener('change', function (e) {
    if (e.target.name === "Avatar") {
        const fileInput = e.target;
        const previewImg = fileInput.closest(".image-previewer").querySelector("img");
        if (fileInput.files && fileInput.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImg.src = e.target.result;
            };
            reader.readAsDataURL(fileInput.files[0]);
        }
    }
});