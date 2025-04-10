function initQuillEditor(editorId = "quill-editor", hiddenInputId = "Description") {
    const editorContainer = document.getElementById(editorId);
    const hiddenInput = document.getElementById(hiddenInputId);

    if (!editorContainer || !hiddenInput) return;

    const quill = new Quill(editorContainer, {
        theme: 'snow',
        placeholder: 'Write your description here...',
        modules: {
            toolbar: [
                ['bold', 'italic', 'underline'],
                [
                    { 'align': '' },        
                    { 'align': 'center' }, 
                    { 'align': 'right' }    
                ],
                [{ 'list': 'bullet' }, { 'list': 'ordered' }],
                ['link']
            ]
        }
    });

    const form = hiddenInput.closest('form');
    form.addEventListener('submit', function () {
        hiddenInput.value = quill.root.innerHTML;
    });

    if (hiddenInput.value) {
        quill.root.innerHTML = hiddenInput.value;
    }
}
