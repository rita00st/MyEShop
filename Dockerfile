# مرحله ساخت (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# کپی و بازیابی وابستگی‌ها
COPY . .
RUN dotnet restore "MyEShop.csproj"
RUN dotnet publish "MyEShop.csproj" -c Release -o /app/publish

# مرحله اجرا (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

# کپی خروجی ساخت از مرحله قبلی
COPY --from=build /app/publish .

# نقطه ورود برنامه
ENTRYPOINT ["dotnet", "MyEShop.dll"]