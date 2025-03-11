document.addEventListener("DOMContentLoaded", function () {
    console.log("JavaScript Loaded!");

    const openModalButton = document.getElementById("openAddClientModal");
    const modal = document.getElementById("add-client-modal");
    const closeModalButton = document.getElementById("closeAddClientModal");

    console.log("Checking Elements:");
    console.log("openModalButton:", openModalButton);
    console.log("modal:", modal);
    console.log("closeModalButton:", closeModalButton);

    if (!openModalButton || !modal || !closeModalButton) {
        console.error("ERROR: Some modal elements are missing!");
        return;
    }

    openModalButton.addEventListener("click", function () {
        console.log("Add Client Button Clicked! Opening Modal...");
        modal.classList.add("active"); 
    });

    closeModalButton.addEventListener("click", function () {
        console.log("Close Button Clicked! Closing Modal...");
        modal.classList.remove("active");
    });

    window.addEventListener("click", function (event) {
        if (event.target === modal) {
            console.log("Clicked outside modal! Closing...");
            modal.classList.remove("active");
        }
    });
});
