// داده‌های نمونه محصولات
const products = [
    {
        id: 1,
        name: "لپ تاپ اپل مک‌بوک پرو",
        category: "لپ تاپ",
        price: "85,000,000 تومان",
        image: "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=150&h=150&fit=crop"

    },
    {
        id: 2,
        name: "گوشی سامسونگ گلکسی S24",
        category: "گوشی موبایل",
        price: "45,000,000 تومان",
        image: "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=150&h=150&fit=crop"
    },
    {
        id: 3,
        name: "هدفون سونی WH-1000XM5",
        category: "هدفون",
        price: "12,500,000 تومان",
        image: "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=150&h=150&fit=crop"
    },
    {
        id: 4,
        name: "ساعت هوشمند اپل واچ",
        category: "ساعت هوشمند",
        price: "18,000,000 تومان",
        image: "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=150&h=150&fit=crop"
    },
    {
        id: 5,
        name: "تبلت آیپد پرو",
        category: "تبلت",
        price: "32,000,000 تومان",
        image: "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=150&h=150&fit=crop"
    },
    {
        id: 6,
        name: "دوربین کانن EOS R5",
        category: "دوربین",
        price: "95,000,000 تومان",
        image: "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=150&h=150&fit=crop"
    },
    {
        id: 7,
        name: "کنسول بازی پلی استیشن 5",
        category: "کنسول بازی",
        price: "28,000,000 تومان",
        image: "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=150&h=150&fit=crop"
    },
    {
        id: 8,
        name: "اسپیکر بلوتوث JBL",
        category: "اسپیکر",
        price: "3,500,000 تومان",
        image: "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=150&h=150&fit=crop"
    },
    {
        id: 9,
        name: "کیبورد مکانیکال Razer",
        category: "کیبورد",
        price: "8,500,000 تومان",
        image: "https://images.unsplash.com/photo-1541140532154-b024d705b90a?w=150&h=150&fit=crop"
    },
    {
        id: 10,
        name: "ماوس گیمینگ Logitech",
        category: "ماوس",
        price: "2,800,000 تومان",
        image: "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=150&h=150&fit=crop"
    },
    {
        id: 11,
        name: "مانیتور سامسونگ 32 اینچ",
        category: "مانیتور",
        price: "15,000,000 تومان",
        image: "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=150&h=150&fit=crop"
    },
    {
        id: 12,
        name: "پاوربانک Anker",
        category: "پاوربانک",
        price: "1,200,000 تومان",
        image: "https://images.unsplash.com/photo-1609592806598-04c4d7e5c1a8?w=150&h=150&fit=crop"
    }
];

// عناصر DOM
const searchInput = document.getElementById('searchInput');
const searchResults = document.getElementById('searchResults');
const resultCount = document.getElementById('resultCount');
const clearBtn = document.getElementById('clearBtn');

let searchTimeout;

// تابع جستجو
function performSearch(query) {
    if (!query.trim()) {
        showAllProducts();
        return;
    }

    // نمایش لودینگ
    showLoading();

    // شبیه‌سازی تاخیر شبکه
    setTimeout(() => {
        const filteredProducts = products.filter(product => 
            product.name.toLowerCase().includes(query.toLowerCase()) ||
            product.category.toLowerCase().includes(query.toLowerCase())
        );

        displayResults(filteredProducts);
    }, 500);
}

// نمایش لودینگ
function showLoading() {
    searchResults.innerHTML = `
        <div class="loading">
            <i class="fas fa-spinner"></i>
            <p>در حال جستجو...</p>
        </div>
    `;
    resultCount.textContent = 'جستجو...';
}

// نمایش همه محصولات
function showAllProducts() {
    displayResults(products);
}

// نمایش نتایج
function displayResults(results) {
    resultCount.textContent = `${results.length} نتیجه`;
    
    if (results.length === 0) {
        searchResults.innerHTML = `
            <div class="no-results">
                <i class="fas fa-search"></i>
                <p>نتیجه‌ای یافت نشد</p>
                <small>لطفاً کلمات کلیدی دیگری امتحان کنید</small>
            </div>
        `;
        return;
    }

    const resultsHTML = results.map(product => `
        <div class="result-item animate" onclick="selectProduct(${product.id})">
            <img src="${product.image}" alt="${product.name}" onerror="this.src='/images/no-image.jpg'">
            <div class="result-info">
                <div class="result-title">${highlightMatch(product.name, searchInput.value)}</div>
                <div class="result-category">${product.category}</div>
            </div>
            <div class="result-price">${product.price}</div>
        </div>
    `).join('');

    searchResults.innerHTML = resultsHTML;
}

// هایلایت کردن کلمات مطابق
function highlightMatch(text, query) {
    if (!query) return text;
    
    const regex = new RegExp(`(${query})`, 'gi');
    return text.replace(regex, '<mark style="background: #667eea; color: white; padding: 2px 4px; border-radius: 3px;">$1</mark>');
}

// انتخاب محصول
function selectProduct(productId) {
    const product = products.find(p => p.id === productId);
    if (product) {
        alert(`محصول انتخاب شده: ${product.name}\nقیمت: ${product.price}`);
    }
}

// رویدادهای input
searchInput.addEventListener('input', (e) => {
    const query = e.target.value;
    
    // نمایش/مخفی کردن دکمه پاک کردن
    if (query) {
        clearBtn.classList.add('show');
    } else {
        clearBtn.classList.remove('show');
    }
    
    // تاخیر در جستجو برای عملکرد بهتر
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        performSearch(query);
    }, 200);
});

// رویداد focus
searchInput.addEventListener('focus', () => {
    if (searchInput.value) {
        clearBtn.classList.add('show');
    }
});

// رویداد blur
searchInput.addEventListener('blur', () => {
    setTimeout(() => {
        clearBtn.classList.remove('show');
    }, 200);
});

// دکمه پاک کردن
clearBtn.addEventListener('click', () => {
    searchInput.value = '';
    searchInput.focus();
    clearBtn.classList.remove('show');
    showAllProducts();
});

// کلیدهای کیبورد
searchInput.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        searchInput.value = '';
        clearBtn.classList.remove('show');
        showAllProducts();
    }
});

// نمایش همه محصولات در ابتدا
document.addEventListener('DOMContentLoaded', () => {
    showAllProducts();
}); 