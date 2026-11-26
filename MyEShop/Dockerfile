# مرحله ۱: ساخت (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# کپی کردن فایل پروژه و بازگرداندن پکیج‌ها (برای استفاده از کش داکر)
COPY ["MyEShop.csproj", "./"]
RUN dotnet restore "MyEShop.csproj"

# کپی کردن بقیه فایل‌ها و ساخت پروژه
COPY . .
RUN dotnet publish "MyEShop.csproj" -c Release -o /app/publish

# مرحله ۲: اجرا (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyEShop.dll"]