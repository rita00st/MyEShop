```markdown
# 🛒 MyEShop - فروشگاه اینترنتی کامل (ASP.NET Core + Bootstrap)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-8.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![GitHub Pages](https://img.shields.io/badge/GitHub%20Pages-Deployed-222222?style=for-the-badge&logo=githubpages&logoColor=white)](https://rita00st.github.io/MyEShop/)

---

## 📖 توضیحات کلی

**MyEShop** یک فروشگاه اینترنتی کامل و مدرن است که شامل دو بخش اصلی می‌شود:

| بخش | توضیح |
|-----|-------|
| **بک‌اند (Backend)** | یک برنامه وب با **ASP.NET Core 8** و **Entity Framework Core** که مدیریت محصولات، سبد خرید، پرداخت آنلاین و پنل مدیریت را فراهم می‌کند. |
| **فرانت‌اند (Frontend)** | صفحات ورود و ثبت‌نام با طراحی واکنش‌گرا و اعتبارسنجی پیشرفته که با **Bootstrap 5** و **JavaScript** ساخته شده‌اند. |

این پروژه به عنوان یک نمونه کار کامل برای نمایش مهارت‌های **توسعه وب با دات‌نت**، **طراحی پایگاه داده**، **مدیریت هویت** و **اتصال به درگاه پرداخت** طراحی شده است.

---

## 🛠️ تکنولوژی‌های استفاده شده

### بک‌اند (Backend)
- **ASP.NET Core 8** (MVC + Razor Pages)
- **Entity Framework Core 8** (Code-First)
- **SQL Server** (پایگاه داده)
- **AutoMapper** (نگاشت Entity ↔ ViewModel)
- **Cookie Authentication** (احراز هویت)
- **ZarinPal** (درگاه پرداخت آنلاین - قابل تعویض)
- **Dependency Injection** (IoC)

### فرانت‌اند (Frontend)
- **HTML5** + **CSS3**
- **Bootstrap 5.3** (طراحی واکنش‌گرا)
- **Bootstrap Icons** (آیکون‌ها)
- **JavaScript (Vanilla)** (اعتبارسنجی، پیش‌نمایش تصاویر، افزایش/کاهش تعداد)

---

## 🚀 نحوه اجرا (برای بک‌اند)

### ۱. کلون کردن مخزن
```bash
git clone https://github.com/rita00st/MyEShop.git
cd MyEShop
```

### ۲. تنظیم اتصال به دیتابیس
فایل `appsettings.json` را ویرایش کنید:
```json
"ConnectionStrings": {
  "MyConnection": "Server=YOUR_SERVER;Database=EshopCore_DB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### ۳. اعمال مایگریشن‌ها
```bash
dotnet ef database update
```

### ۴. اجرای پروژه
```bash
dotnet run
```
سپس به آدرس `https://localhost:7030` بروید.

### ۵. اجرای فرانت‌اند (صفحات لاگین و ثبت‌نام)
اگر فرانت‌اند را جداگانه کلون کرده‌اید:
```bash
git clone https://github.com/rita00st/MyEShop-Front.git
cd MyEShop-Front
```
سپس فایل `login.html` یا `register.html` را در مرورگر باز کنید.

---

## 📸 پیش‌نمایش

### صفحه اصلی (بک‌اند)
![Home Page](https://via.placeholder.com/800x400/6a11cb/ffffff?text=MyEShop+Home+Page)

### پنل مدیریت محصولات
![Admin Panel](https://via.placeholder.com/800x400/2575fc/ffffff?text=Admin+Panel)

### صفحه ورود (فرانت‌اند)
![Login Page](https://via.placeholder.com/400x300/6a11cb/ffffff?text=Login+Page)

### صفحه ثبت‌نام
![Register Page](https://via.placeholder.com/400x300/2575fc/ffffff?text=Register+Page)

---

## 📂 ساختار پروژه

```
MyEShop/                         # بک‌اند (ASP.NET Core)
├── Controllers/                 # کنترلرهای MVC
├── Pages/                       # Razor Pages (پنل ادمین)
│   └── Admin/                   # صفحات مدیریت
├── Models/                      # مدل‌ها و ViewModel‌ها
├── Services/                    # سرویس‌ها (IoC)
├── Migrations/                  # مایگریشن‌های EF Core
├── wwwroot/                     # فایل‌های استاتیک
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/
├── appsettings.json             # تنظیمات پروژه
└── Program.cs                   # نقطه شروع

MyEShop-Front/                   # فرانت‌اند (صفحات لاگین/ثبت‌نام)
├── login.html
├── register.html
├── style.css
└── assets/                      # منابع اضافی
```

---

## 🔧 تنظیمات درگاه پرداخت

برای فعال‌سازی پرداخت آنلاین، `MerchantId` خود را از زرین‌پال دریافت و در `appsettings.json` وارد کنید:

```json
"Zarinpal": {
  "MerchantId": "YOUR_MERCHANT_ID"
}
```

برای تست در محیط **Sandbox**، از `YOUR_MERCHANT_ID` استفاده کنید و آدرس بازگشت را در متد `Checkout` به `sandbox.zarinpal.com` تغییر دهید.

---

## 🤝 مشارکت

اگر پیشنهاد، ایده یا بهبودی دارید، خوشحال می‌شوم از آن مطلع شوم. لطفاً یک **Issue** باز کنید یا **Pull Request** ارسال کنید.

---

## 📄 لایسنس

این پروژه تحت لایسنس **MIT** منتشر شده است.

---

## 👨‍💻 توسعه‌دهنده

**ریتا اسدی**  
- [گیت‌هاب](https://github.com/rita00st)  
- [لینکدین](https://linkedin.com/in/rita-asadi) *(در صورت تمایل)*

---

## ⭐ حمایت

اگر این پروژه برای شما مفید بود، لطفاً یک **ستاره (⭐)** به مخزن‌های آن بدهید.

| مخزن | لینک |
|------|------|
| **بک‌اند (MyEShop)** | [https://github.com/rita00st/MyEShop](https://github.com/rita00st/MyEShop) |
| **فرانت‌اند (MyEShop-Front)** | [https://github.com/rita00st/MyEShop-Front](https://github.com/rita00st/MyEShop-Front) |
| **دموی زنده** | [https://rita00st.github.io/MyEShop/](https://rita00st.github.io/MyEShop/) |
```

---
