// منوی موبایل
const menuToggle = document.getElementById('menuToggle');
const sidebar = document.getElementById('sidebar');
const mainContent = document.getElementById('mainContent');

menuToggle.addEventListener('click', () => {
    sidebar.classList.toggle('show');
});

// بستن منو با کلیک بیرون
document.addEventListener('click', (event) => {
    if (!sidebar.contains(event.target) && !menuToggle.contains(event.target)) {
        sidebar.classList.remove('show');
    }
});


// پیش نمایش عکس
function previewImage(input) {
    const wrapper = document.getElementById('previewWrapper');
    const placeholder = document.getElementById('previewPlaceholder');
    const preview = document.getElementById('imagePreview');

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            wrapper.style.display = 'inline-block';
            placeholder.style.display = 'none';
        }

        reader.readAsDataURL(input.files[0]);
    } else {
        wrapper.style.display = 'none';
        placeholder.style.display = 'block';
        preview.src = '#';
    }
}

// دکمه حذف عکس
const removeBtn = document.getElementById('removeImage');
if (removeBtn) {
    removeBtn.addEventListener('click', function () {
        const fileInput = document.querySelector('input[type="file"]');
        if (fileInput) {
            fileInput.value = '';
        }

        const wrapper = document.getElementById('previewWrapper');
        const placeholder = document.getElementById('previewPlaceholder');
        const preview = document.getElementById('imagePreview');

        wrapper.style.display = 'none';
        placeholder.style.display = 'block';
        preview.src = '#';
    });
}