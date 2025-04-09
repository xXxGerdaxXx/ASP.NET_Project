function initTagSelector(config) {
    console.log("initTagSelector loaded!");

    // Track which search result is currently focused (not implemented visually here)
    let activeIndex = -1;

    // Store selected item IDs to avoid duplicates
    let selectedIds = [];

    // Grab DOM elements by their IDs passed via the config object
    const tagContainer = document.getElementById(config.containerId);
    const input = document.getElementById(config.inputId);
    const results = document.getElementById(config.resultsId);

    // If there are preselected items, add them as tags on load
    if (Array.isArray(config.preselected)) {
        config.preselected.forEach(item => addTag(item));
    }

    // When input is focused, show results container styling
    input.addEventListener('focus', () => {
        tagContainer.classList.add('focused');
        results.classList.add('focused');
    });

    // When input loses focus, hide the results dropdown shortly after
    input.addEventListener('blur', () => {
        setTimeout(() => {
            tagContainer.classList.remove('focused');
            results.classList.remove('focused');
        }, 100);
    });

    // Handle typing in the input
    input.addEventListener('input', () => {
        const query = input.value.trim();
        activeIndex = -1;

        // If nothing is typed, hide and clear the results
        if (query.length === 0) {
            results.style.display = 'none';
            results.innerHTML = '';
            return;
        }

        // Fetch matching items from the server based on the query
        fetch(config.searchUrl(query))
            .then(r => r.json())
            .then(data => renderSearchResults(data));
    });

    // Renders the fetched search results
    function renderSearchResults(data) {
        results.innerHTML = ''; // Clear previous results

        if (data.length === 0) {
            // Show a 'no results' message if nothing matches
            const noResult = document.createElement('div');
            noResult.classList.add('search-item');
            noResult.textContent = config.emptyMessage || 'No results.';
            results.appendChild(noResult);
        } else {
            // For each search result, create a clickable item
            data.forEach(item => {
                if (!selectedIds.includes(item.id)) {
                    const resultItem = document.createElement('div');
                    resultItem.classList.add('search-item');
                    resultItem.dataset.id = item.id;

                    // Customize result content with avatar and name
                    resultItem.innerHTML = `
                        <div class="search-item-content">
                            <img class="member-avatar" src="${config.avatarFolder || ''}${item[config.imageProperty]}" alt="${item[config.displayProperty]}">
                            <span class="member-name">${item[config.displayProperty]}</span>
                        </div>
                    `;

                    // Add tag on click
                    resultItem.addEventListener('click', () => addTag(item));
                    results.appendChild(resultItem);
                }
            });
        }

        // Show the results dropdown
        results.style.display = 'block';
    }

    // Adds a tag for the selected item
    function addTag(item) {
        const id = parseInt(item.id);

        // Prevent adding the same item twice
        if (selectedIds.includes(id)) return;

        selectedIds.push(id); // Track the selected ID

        // Create a new tag element
        const tag = document.createElement('div');
        tag.classList.add(config.tagClass || 'tag');

        // Different HTML structure depending on tag type
        if (config.tagType === 'tag') {
            tag.innerHTML = `<span>${item[config.displayProperty]}</span>`;
        } else if (config.tagType === 'member') {
            tag.innerHTML = `
                <img class="member-avatar" src="${config.avatarFolder || ''}${item[config.imageProperty]}" alt="${item[config.displayProperty]}">
                <span>${item[config.displayProperty]}</span>
            `;
        }

        // Create and attach a remove button (×)
        const removeBtn = document.createElement('span');
        removeBtn.textContent = '×';
        removeBtn.classList.add('btn-remove');
        removeBtn.dataset.id = id;

        // Remove the tag when the button is clicked
        removeBtn.addEventListener('click', (e) => {
            selectedIds = selectedIds.filter(i => i !== id);
            tag.remove();
            updateSelectedIdsInput();
            e.stopPropagation();
        });

        tag.appendChild(removeBtn);

        // Insert the tag before the input field inside the container
        tagContainer.insertBefore(tag, input);

        // Clear the input and hide the results dropdown
        input.value = '';
        results.innerHTML = '';
        results.style.display = 'none';

        updateSelectedIdsInput(); // Save the updated selection list to a hidden field
    }

    // Removes the last tag from the list (can be triggered by Backspace, if you want)
    function removeLastTag() {
        const tags = tagContainer.querySelectorAll(`.${config.tagClass}`);
        if (tags.length === 0) return;

        const lastTag = tags[tags.length - 1];
        const lastId = parseInt(lastTag.querySelector('.btn-remove').dataset.id);

        selectedIds = selectedIds.filter(id => id !== lastId);
        lastTag.remove();
        updateSelectedIdsInput();
    }

function updateSelectedIdsInput() {
    const hiddenInput = document.getElementById(config.hiddenInputId);
    if (hiddenInput) {
        // Convert the array of numbers into a comma-separated string.
        hiddenInput.value = selectedIds.join(",");
        console.log("Hidden input updated:", hiddenInput.value);
    }
}

}
