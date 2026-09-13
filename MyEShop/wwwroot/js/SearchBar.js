// ============================================
// جستجوی زنده محصولات
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    const clearBtn = document.getElementById('clearBtn');
    const resultCount = document.getElementById('resultCount');

    let debounceTimer;

    if (!searchInput) return;

    // ✅ رویداد تایپ کردن با تاخیر (Debounce)
    searchInput.addEventListener('input', function () {
        const term = this.value.trim();

        // نمایش دکمه پاک کردن
        if (clearBtn) {
            clearBtn.style.display = term.length > 0 ? 'block' : 'none';
        }

        clearTimeout(debounceTimer);

        if (term.length < 1) {
            searchResults.innerHTML = '';
            if (resultCount) resultCount.textContent = '0 نتیجه';
            return;
        }

        // نمایش لودینگ
        searchResults.innerHTML = `
            <div class="text-center py-4">
                <div class="spinner-border text-purple" role="status">
                    <span class="visually-hidden">در حال جستجو...</span>
                </div>
                <p class="text-muted mt-2 small">در حال جستجو...</p>
            </div>
        `;

        // ✅ تاخیر ۳۰۰ میلی‌ثانیه برای جلوگیری از درخواست‌های اضافی
        debounceTimer = setTimeout(() => {
            performSearch(term);
        }, 300);
    });

    // ✅ پاک کردن جستجو
    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            searchInput.value = '';
            searchResults.innerHTML = '';
            if (resultCount) resultCount.textContent = '0 نتیجه';
            this.style.display = 'none';
            searchInput.focus();
        });
    }

    // ✅ تابع اصلی جستجو
    function performSearch(term) {
        fetch(`/Product/LiveSearch?term=${encodeURIComponent(term)}`)
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    searchResults.innerHTML = `
                        <div class="alert alert-warning text-center">
                            <i class="bi bi-exclamation-triangle"></i>
                            ${data.message}
                        </div>
                    `;
                    return;
                }

                if (data.count === 0) {
                    searchResults.innerHTML = `
                        <div class="text-center py-5">
                            <i class="bi bi-search fs-1 text-muted opacity-25"></i>
                            <p class="text-muted mt-3">محصولی با این نام پیدا نشد!</p>
                            <small class="text-muted">لطفاً کلمه دیگری را امتحان کنید.</small>
                        </div>
                    `;
                    if (resultCount) resultCount.textContent = '0 نتیجه';
                    return;
                }

                // ✅ نمایش نتایج
                let html = '<div class="row g-3">';
                data.data.forEach(product => {
                    const isInStock = product.quantity > 0;
                    const stockBadge = isInStock
                        ? '<span class="badge bg-success">موجود</span>'
                        : '<span class="badge bg-danger">ناموجود</span>';
                    const imagepath = product.imagePath ?? "/images/no-image.jpg";

                    html += `
                        <div class="col-12">
                            <a href="/Home/Details/${product.id}" 
                               class="search-result-item d-flex align-items-center gap-3 p-3 rounded-3 border text-decoration-none text-dark">
                                <img src="${imagepath}" 
                                     alt="${product.name}" 
                                     style="width: 60px; height: 60px; object-fit: cover; border-radius: 12px;">
                                    
                                <div class="flex-grow-1">
                                    <h6 class="mb-1 fw-bold">${product.name}</h6>
                                    <div class="d-flex justify-content-between align-items-center">
                                        <span class="text-purple fw-bold">
                                            ${product.price.toLocaleString('fa-IR')} تومان
                                        </span>
                                        ${stockBadge}
                                    </div>
                                </div>
                                <i class="bi bi-chevron-left text-muted"></i>
                            </a>
                        </div>
                    `;
                });
                html += '</div>';

                searchResults.innerHTML = html;
                if (resultCount) resultCount.textContent = `${data.count} نتیجه`;
            })
            .catch(error => {
                console.error('خطا در جستجو:', error);
                searchResults.innerHTML = `
                    <div class="alert alert-danger text-center">
                        <i class="bi bi-x-circle"></i>
                        خطا در ارتباط با سرور. لطفاً دوباره تلاش کنید.
                    </div>
                `;
            });
    }
});