# Hướng dẫn test API Quản lý Giảng viên (Lecturers)

## 1. Chạy API

```bash
cd d:\learning\KhoaCNTT\backend\KhoaCNTT.API
dotnet run
```

Hoặc mở solution trong Visual Studio / Cursor và nhấn **F5** (hoặc chọn profile **https** / **http**).

- **HTTPS:** https://localhost:7108  
- **HTTP:** http://localhost:5249  

Swagger sẽ mở tại: **https://localhost:7108/swagger** (hoặc http://localhost:5249/swagger tùy profile).

---

## 2. Test bằng Swagger UI

### Bước 1: Test API không cần đăng nhập (Sinh viên / Khách)

1. Vào **GET /api/Lecturers** → **Try it out**.
2. Có thể nhập:
   - `name`: tên giảng viên
   - `degree`: 0=Cử nhân, 1=Thạc sĩ, 2=Tiến sĩ, 3=PGS, 4=GS
   - `position`: chức vụ
   - `subjectCodeOrName`: mã hoặc tên môn
   - `page`, `pageSize`
3. **Execute** → xem danh sách (có thể rỗng nếu chưa có dữ liệu).

4. **GET /api/Lecturers/{id}** → nhập `id` (ví dụ 1) → **Execute** (sẽ 404 nếu chưa có giảng viên).

---

### Bước 2: Lấy token Admin (để test POST / PUT / DELETE)

1. Trong Swagger, tìm **Auth** → **POST /api/Auth/login/admin**.
2. **Try it out**.
3. Body mẫu (theo tài khoản admin trong DB của bạn, ví dụ):

```json
{
  "username": "admin",
  "password": "abc123"
}
```

4. **Execute**.
5. Trong response, copy giá trị **token** (chuỗi dài, không có chữ "Bearer" phía trước).

---

### Bước 3: Gắn token vào Swagger (Authorize)

1. Ở đầu trang Swagger, nhấn nút **Authorize** (ổ khóa).
2. Ở ô **Value** nhập: `Bearer <dán_token_vừa_copy>`  
   Ví dụ: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. **Authorize** → **Close**.

Sau bước này mọi request từ Swagger sẽ tự gửi kèm header `Authorization: Bearer <token>`.

---

### Bước 4: Test tạo giảng viên (Admin)

1. **POST /api/Lecturers** → **Try it out**.
2. Body mẫu (chỉnh Email, tên, môn học cho phù hợp DB của bạn):

```json
{
  "fullName": "Nguyễn Văn A",
  "imageUrl": "/images/avatar.jpg",
  "degree": 1,
  "position": "Trưởng bộ môn",
  "birthdate": "1985-06-15T00:00:00",
  "email": "nva@khoa.edu.vn",
  "phoneNumber": "0901234567",
  "subjectCodes": ["CSE481"]
}
```

**Lưu ý:** `subjectCodes` phải là mã môn đã có trong bảng **Subjects**. Nếu chưa có môn nào, bỏ trống `[]` hoặc tạo môn trước qua API/DB.

3. **Execute** → 200 và message "Tạo giảng viên thành công."

---

### Bước 5: Test cập nhật và xóa

- **GET /api/Lecturers** → lấy `id` của giảng viên vừa tạo.
- **PUT /api/Lecturers/{id}** → sửa thông tin (cùng format body như Create) → **Execute**.
- **DELETE /api/Lecturers/{id}** → **Execute** (chỉ khi muốn xóa).

---

## 3. Test bằng curl (command line)

**Danh sách (không cần token):**
```bash
curl "http://localhost:5249/api/Lecturers?page=1&pageSize=10"
```

**Đăng nhập Admin (thay username/password đúng):**
```bash
curl -X POST "http://localhost:5249/api/Auth/login/admin" -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"abc123\"}"
```

**Tạo giảng viên (thay YOUR_TOKEN bằng token trả về từ login):**
```bash
curl -X POST "http://localhost:5249/api/Lecturers" -H "Content-Type: application/json" -H "Authorization: Bearer YOUR_TOKEN" -d "{\"fullName\":\"Nguyễn Văn A\",\"imageUrl\":\"\",\"degree\":1,\"position\":\"GV\",\"email\":\"nva@khoa.edu.vn\",\"phoneNumber\":\"0901234567\",\"subjectCodes\":[]}"
```

---

## 4. Test bằng Postman / Insomnia

1. Import base URL: `http://localhost:5249` (hoặc https://localhost:7108).
2. Tạo request **POST** `{{baseUrl}}/api/Auth/login/admin` với body JSON `{"username":"admin","password":"abc123"}` → lấy token từ response.
3. Tạo collection cho Lecturers:
   - GET `{{baseUrl}}/api/Lecturers` (không cần auth).
   - GET `{{baseUrl}}/api/Lecturers/1` (không cần auth).
   - POST `{{baseUrl}}/api/Lecturers` → tab **Authorization** chọn **Bearer Token** và dán token.
   - PUT `{{baseUrl}}/api/Lecturers/1` → Bearer Token.
   - DELETE `{{baseUrl}}/api/Lecturers/1` → Bearer Token.

---

## 5. Lỗi thường gặp

| Lỗi | Nguyên nhân | Cách xử lý |
|-----|-------------|------------|
| 401 Unauthorized khi POST/PUT/DELETE | Chưa đăng nhập hoặc token hết hạn | Gọi lại POST /api/Auth/login/admin và dùng token mới. |
| 404 Not Found GET /api/Lecturers/1 | Chưa có giảng viên id=1 | Tạo giảng viên bằng POST trước, hoặc dùng id có trong GET /api/Lecturers. |
| 400 / 500 khi tạo giảng viên | Email trùng hoặc dữ liệu không hợp lệ | Kiểm tra body (email unique, subjectCodes tồn tại trong bảng Subjects). |
| CORS / không gọi được từ frontend | Chưa bật CORS | Trong `Program.cs` thêm `app.UseCors(...)` nếu bạn gọi từ web app khác domain. |

Nếu bạn cần thêm CORS hoặc biến môi trường cho URL, có thể cấu hình trong `Program.cs` và `appsettings.json`.
