function initTagSelector(config) {
    console.log("initTagSelector loaded!");
    let activeIndex = -1;
    let selectedIds = [];

    const tagContainer = document.getElementById(config.containerId);
    const input = document.getElementById(config.inputId);
    const results = document.getElementById(config.resultsId);

    if (Array.isArray(config.preselected)) {
        config.preselected.forEach(item => addTag(item));
    }

    input.addEventListener('focus', () => {
        tagContainer.classList.add('focused');
        results.classList.add('focused');
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            tagContainer.classList.remove('focused');
            results.classList.remove('focused');
        }, 100);
    });

    input.addEventListener('input', () => {
        const query = input.value.trim();
        activeIndex = -1;

        if (query.length === 0) {
            results.style.display = 'none';
            results.innerHTML = '';
            return;
        }

        fetch(config.searchUrl(query))
            .then(r => r.json())
            .then(data => renderSearchResults(data));
    });

    function renderSearchResults(data) {
        results.innerHTML = '';

        if (data.length === 0) {
            const noResult = document.createElement('div');
            noResult.classList.add('search-item');
            noResult.textContent = config.emptyMessage || 'No results.';
            results.appendChild(noResult);
        } else {
            data.forEach(item => {
                if (!selectedIds.includes(item.id)) {
                    const resultItem = document.createElement('div');
                    resultItem.classList.add('search-item');
                    resultItem.dataset.id = item.id;
                    resultItem.innerHTML = `
                        <img class="member-avatar" src="${config.avatarFolder || ''}${item[config.imageProperty]}">
                        <span>${item[config.displayProperty]}</span>
                    `;
                    resultItem.addEventListener('click', () => addTag(item));
                    results.appendChild(resultItem);
                }
            });
        }
        results.style.display = 'block';
    }

    function addTag(item) {
        const id = parseInt(item.id);
        if (selectedIds.includes(id)) return;
        selectedIds.push(id);

        const tag = document.createElement('div');
        tag.classList.add(config.tagClass || 'tag');

        if (config.tagType === 'tag') {
            tag.innerHTML = `
                <span>${item[config.displayProperty]}</span>
            `;
        } else if (config.tagType === 'member') {
            tag.innerHTML = `
                <img class="member-avatar" src="${config.avatarFolder || ''}${item[config.imageProperty]}" alt="${item[config.displayProperty]}">
                <span>${item[config.displayProperty]}</span>
            `;
        }

        const removeBtn = document.createElement('span');
        removeBtn.textContent = '×';
        removeBtn.classList.add('btn-remove');
        removeBtn.dataset.id = id;

        removeBtn.addEventListener('click', (e) => {
            selectedIds = selectedIds.filter(i => i !== id);
            tag.remove();
            updateSelectedIdsInput();
            e.stopPropagation();
        });

        tag.appendChild(removeBtn);
        tagContainer.insertBefore(tag, input);

        input.value = '';
        results.innerHTML = '';
        results.style.display = 'none';
    }

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
        if (hiddenInput)
            hiddenInput.value = JSON.stringify(selectedIds);
    }
}
