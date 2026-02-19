# KhoaCNTT

Backend: [README.md](/backend/README.md)


### Cách cập nhật code mới khi nhánh main có thay đổi

Bước 1: Lấy code mới từ main
```bash
git fetch origin
```

Bước 2: Merge main vào branch của mình
```bash
git merge origin/main
```

Hoặc vừa fetch vừa merge trong một lệnh
```bash
git pull origin main
```

**Trường hợp đặc biệt:**
- Có người tạo migration mới
- Sửa Entity
- Thay đổi DbContext
- Update database structure

Lệnh để cập nhật database sau khi đã merge code mới từ main về:
```bash
dotnet ef database update
```