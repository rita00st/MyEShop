
//< !-- ============================================
//    اسکریپت‌ها
//        ============================================ -->
// نمایش/مخفی کردن رمز عبور
function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(inputId + '-icon');
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bi-eye');
        icon.classList.add('bi-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('bi-eye-slash');
        icon.classList.add('bi-eye');
    }
}

// بررسی تطابق رمز عبور
document.addEventListener('DOMContentLoaded', function () {
    const password = document.getElementById('password');
    const repassword = document.getElementById('repassword');

    if (repassword) {
        repassword.addEventListener('input', function () {
            if (this.value !== password.value) {
                this.setCustomValidity('رمز عبور یکسان نیست');
            } else {
                this.setCustomValidity('');
            }
        });
    }

    // فرمت خودکار ایمیل (اختیاری)
    const emailInput = document.querySelector('input[name="User.Email"]');
    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            this.value = this.value.trim().toLowerCase();
        });
    }
});