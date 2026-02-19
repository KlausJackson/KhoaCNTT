# Overview

Có vấn đề gì thì cứ hỏi nhé anh em. Ai code thì đọc kỹ cái file này.

# Project Architecture

Dự án sử dụng .NET 8.0, nhớ update Visual Studio 2022 lên phiên bản mới nhất để tránh lỗi khi chạy dự án. `dotnet --version` để kiểm tra phiên bản.

## Backend

Clean architecture là một kiến trúc phần mềm giúp tách biệt các phần của ứng dụng thành các layers khác nhau, mỗi layer có một trách nhiệm riêng. Cái này sẽ được học ở kỳ sau - môn phát triển ứng dụng di động.

Clean Architecture có 4 layers: API, Application, Domain, Infrastructure.
```bash
└── 📁backend
    └── 📁ConsoleApp1
        ├── Program.cs # file tôi dùng để test code linh tinh, không liên quan đến dự án chính
    └── 📁KhoaCNTT.API # tầng nhận API request từ client (presentation)
        └── 📁Controllers
            ├── AdminsController.cs
            ├── AuthController.cs
            ├── CommentsController.cs
            ├── FilesController.cs
            ├── LecturersController.cs
            ├── NewsController.cs
            ├── StudentsController.cs
            ├── SubjectController.cs
        └── 📁Extensions
            ├── ServiceCollection.cs # đăng ký các services, controllers vào DI container
        └── 📁Filters
            ├── ApiExceptionFilter.cs
        ├── Program.cs
    └── 📁KhoaCNTT.Application # tầng logic nghiệp vụ
        └── 📁Common
            └── 📁Constants
                ├── RoleConstant.cs
            └── 📁Exceptions
                ├── BusinessRule.cs
                ├── NotFound.cs
            └── 📁Utils
                ├── AdminMappingProfile.cs
                ├── AutoMapperProfile.cs
                ├── PassswordHasher.cs
        └── 📁DTOs # dùng để ghi rõ các trường thông tin sẽ gửi cho client
            └── 📁Admin
                ├── AdminResponse.cs
                ├── CreateAdminRequest.cs
                ├── UpdateAdminRequest.cs
            └── 📁File
                ├── UpdateFileRequest.cs
                ├── UploadFileRequest.cs
            └── 📁School
                ├── ScheduleResponse.cs
                ├── SchoolLoginResponse.cs
                ├── ScoreResponse.cs
            └── 📁Comment
                ├── CommentResponse.cs
                ├── CreateCommentRequest.cs
            └── 📁Lecturer
                ├── CreateLecturerRequest.cs
                ├── LecturerResponse.cs
                ├── UpdateLecturerRequest.cs
            └── 📁News
                ├── CreateNewsRequest.cs
                ├── NewsResponse.cs
                ├── UpdateNewsRequest.cs
            ├── CommentDto.cs
            ├── FileResourceDto.cs
            ├── LecturerDto.cs
            ├── NewsDto.cs
        └── 📁Interfaces # giao diện cho các class, chỉ chứa tên các hàm chức năng, không chứa logic
            └── 📁Repositories # 
                ├── IAdminRepository.cs
                ├── ICommentRepository.cs
                ├── IFileRespository.cs
                ├── ILecturerRepository.cs
                ├── INewRepository.cs
                ├── ISubjectRepository.cs
            └── 📁Services # 
                ├── IAdminService.cs
                ├── IAuthService.cs
                ├── ICommentService.cs
                ├── IFileService.cs
                ├── IFileStorageService.cs
                ├── IJwtTokenGenerator.cs
                ├── ILecturerService.cs
                ├── INewsService.cs
                ├── ISchoolApiService.cs
        └── 📁Services # chứa các logic nghiệp vụ chính
            ├── AdminService.cs
            ├── AuthService.cs
            ├── CommentService.cs
            ├── FileService.cs
            ├── LecturerService.cs
            ├── NewsService.cs
    └── 📁KhoaCNTT.Domain # tầng mô hình dữ liệu
        └── 📁Common # lớp cơ sở cho tất cả các entity
            ├── BaseEntity.cs # chứa các trường chung như Id, CreatedAt, UpdatedAt
        └── 📁Entities
            ├── AdminUser.cs
            ├── Comment.cs
            ├── FileResource.cs
            ├── Lecturer.cs
            ├── LecturerSubject.cs
            ├── News.cs
            ├── Subject.cs
        └── 📁Enums
            ├── DegreeType.cs
            ├── FilePermission.cs
            ├── FileStatus.cs
            ├── NewsType.cs
    └── 📁KhoaCNTT.Infrastructure # tầng kết nối ra bên ngoài (DB, API trường, lưu trữ file vật lý trên server)
        └── 📁ExternalServices # gọi API trường
            ├── SchoolApiClient.cs
        └── 📁Identity # tạo token
            ├── JwtTokenGenerator.cs
        └── 📁Persistence
            └── 📁Configurations # cấu hình mapping entity với database
                ├── FileResourceConfig.cs
                ├── LecturerConfig.cs
                ├── NewsConfig.cs
            └── 📁Migrations
            ├── AppDbContext.cs
        └── 📁Repositories # cài đặt thao tác với database
            ├── AdminRepository.cs
            ├── CommentRepository.cs
            ├── FileRepository.cs
            ├── LecturerRepository.cs
            ├── NewsRepository.cs
            ├── SubjectRepository.cs
        └── 📁Storage
            ├── LocalFileStorageService.cs # lưu file vật lý trên server
    └── README.md
```

**Tóm tắt lại các layer:**
* API → Nhận request từ client (Controller).
* Application → Xử lý logic nghiệp vụ.
* Domain → Định nghĩa các trường thông tin trong các bảng (Entity, Enum).
* Infrastructure → Làm việc với database, file vật lý trên máy, API ngoài.

**Luồng hoạt động của một API request:**
Client → Controller (API) → Service (Application) → Repository (Infrastructure) → Database
Sau đó dữ liệu trả lại theo chiều ngược lại.

API → Application → Domain
        ↓
    Infrastructure

## Frontend

MVC với 3 layers: Views, Controllers, Models.
```bash

```

# Project Setup

Tạo folder mới để lưu trữ file trên ổ D nếu muốn test các chức năng quản lý tài liệu: `D:\KhoaCNTT_data`.

## Database Setup

Tạo database trong SQL Server Management Studio (SSMS) với tên `khoacntt`.

Cách xem và chỉnh sửa dữ liệu trực tiếp trong database mà không cần mở SQL Server:
1. Chọn View trên tab trên cùng của Visual Studio, chọn SQL Server Object Explorer.
2. (localdb)\\MSSQLLocalDB -> Databases -> khoacntt.

Khả năng cần sửa connectionStrings đúng với máy cá nhân trong file `appsettings.json` của KhoaCNTT.API để trỏ đến database mới tạo:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=khoacntt;Trusted_Connection=True;"
}
```

### Tạo các bảng trong cơ sở dữ liệu

Cách chạy migration tạo bảng, thực hiện thay đổi trong cơ sở dữ liệu:
1.  Vào **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
2.  Ở ô **Default project** (trên cùng console), chọn: `KhoaCNTT.Infrastructure`.
3.  `Add-Migration InitialCreate -StartupProject KhoaCNTT.API`
    * Nếu đã có database cũ, xóa database trước khi chạy InitialCreate.
    * Nếu nó báo Build Failed, hãy sửa hết lỗi đỏ trong code trước.
4.  `Update-Database -StartupProject KhoaCNTT.API`

Mọi thay đổi với database sử dụng code đều dùng 2 lệnh trên.

## Nuget Packages

Tải toàn bộ các gói thư viện cần thiết:
1. Chạy lệnh cd tới KhoaCNTT.API để cùng thư mục với file .sln.
2. Chạy lệnh `dotnet restore` để tải các gói thư viện.

### Admin

Sử dụng tài khoản Admin cấp 1 được lập trình sẵn trong code (admin, abc123) để thực hiện các quyền của cấp 1, và tạo tài khoản mới với quyền cấp 2/3 trong database rồi sửa trực tiếp thành cấp 1. 

### Thêm dữ liệu vào database

Danh sách môn học: "KhoaCNTT\database\subjects.sql"

1. Chọn View trên tab trên cùng của Visual Studio, chọn SQL Server Object Explorer.
2. (localdb)\\MSSQLLocalDB -> Databases -> khoacntt.
3. Làm theo hướng dẫn trong ảnh và update lệnh SQL như sau:
- CreatedAt: từ not null thành null.
- isDeleted: thêm default 0.

![Image](/subjects.png)


## Chạy dự án

Chọn KhoaCNTT.API trên Visual Studio làm startup project, sau đó nhấn F5 để chạy dự án.

# Getting Started to Code

1. Fork repo về tài khoản cá nhân của mình trên github.
2. Pull code từ github về, mở solution bằng Visual Studio.
3. Tạo branch mới với tên theo chức năng mình làm. Ví dụ: `tintuc`, `tin_tuc`, `tin-tuc`. Miễn sao đọc là hiểu được branch đó làm gì.
4. Code.
5. Push branch lên repo cá nhân trên github.
6. Tạo pull request từ branch cá nhân lên main của repo gốc.

## **Đặc biệt lưu ý**

**Conflict Database:** Làm sai cái là đi luôn, lại phải restore version trước khi lỗi xảy ra.

**Tình huống:** A thay đổi một cái gì đấy trong database. B cũng thay đổi một cái gì đó trong database. Rồi cả 2 cùng chạy `Add-Migration`. EF Core sẽ tạo:
* File migration mới
* Cập nhật file ModelSnapshot
-> File Snapshot của EF Core bị conflict nặng.

Chỉ 1 người được tạo migration tại một thời điểm, làm lần lượt từng người một khi động đến Database.

Sau khi có thay đổi về database và đã push lên github, báo lại cho team. Những người khác sẽ pull về trước khi làm tiếp để tránh conflict database.

Ai lỡ conflict rồi thì `Remove-Migration` + xóa database khoacntt, tạo lại database, rồi `Add-Migration FixAfterMerge`.

**Nguyên tắc quan trọng khi code clean architecture:**

1. Không viết logic nghiệp vụ trong Controller, trừ check phân quyền. Nghiệp vụ viết trong Application.
2. Không để Domain phụ thuộc vào Infrastructure. Domain là tầng độc lập.
3. Chỉ Infrastructure được phép truy cập database.

*Các lưu ý khác:*
Khi tạo file mới thì chọn New Class, nhớ sử dụng `public` thay vì syntax mặc định `internal` để các lớp có thể truy cập qua lại giữa các layers.

Thứ tự code khuyên dùng: Domain → Application → Infrastructure → API.

## 1. Domain

Code từ đây trước, viết rõ các trường thông tin trong các bảng ở file trong folder Domain/Entities, viết các enum ở folder Domain/Enums (dành cho phân loại).

## 2. Application

DTOs: dùng để định nghĩa rõ ràng các trường thông tin sẽ gửi cho client, tránh gửi thừa thông tin nhạy cảm như password hash, hoặc các trường không cần thiết khác.

Code các DTOs trước, sau đó viết các interface trong Interfaces/ (I Repositories và I Services), cuối cùng mới viết logic nghiệp vụ trong Services/.

Thêm các mapping profile trong Common/Utils để AutoMapper biết cách map giữa Entity và DTO.

## 3. Infrastructure

Code các class trong folder Repositories, cuối cùng code các class trong Storage.
Code các cấu hình mapping giữa entity và database trong folder Configurations.
Đăng ký các service vừa code vào DI container trong API/Extensions/ServiceCollection.cs để có thể gọi được ở các layers khác.

## 4. API

Viết API Controller để nhận request từ client, gọi service trong Application để xử lý nghiệp vụ, trả về response cho client.

## Ví dụ chức năng quản lý tin tức

Đoạn này tôi copy AI.
1.  **Domain:** Vào `Entities`, tạo class `News.cs`.
2.  **Infrastructure:**
    *   Vào `AppDbContext.cs`, thêm `DbSet<News> News { get; set; }`.
    *   Tạo Migration: `Add-Migration AddNewsTable` -> `Update-Database`.
    *   Vào `Repositories`, tạo `NewsRepository.cs` và Interface tương ứng.
3.  **Application:**
    *   Vào `DTOs/News`, tạo `CreateNewsRequest.cs`, `NewsResponse.cs`.
    *   Vào `Interfaces/Services`, tạo `INewsService.cs`.
    *   Vào `Services`, tạo `NewsService.cs` (Viết logic check, map dữ liệu, gọi repo).
    *   Vào `Mappings/AutoMapperProfile.cs`, cấu hình map từ Entity sang DTO.
4.  **API:**
    *   Vào `Controllers`, tạo `NewsController.cs`.
    *   Inject `INewsService` và viết các API (GET, POST, PUT, DELETE).
5.  **DI:** Vào `Extensions/ServiceCollection.cs`, đăng ký Service và Repository vừa tạo.

*   Dùng **AutoMapper** để chuyển đổi dữ liệu, không gán tay từng dòng `dto.Name = entity.Name`.
*   Luôn bắt lỗi bằng `BusinessRuleException` hoặc `NotFoundException` (đã cấu hình sẵn filter xử lý lỗi).

# Test

Cách test sử dụng swagger có sẵn (có thể dùng postman, nhưng swagger tiện hơn):
1. Chạy dự án, nó sẽ tự động mở trang swagger ở trình duyệt.
2. Chọn API muốn test, nhấn Try it out, điền thông tin cần thiết, nhấn Execute để gửi request.
3. Xem phần Response để kiểm tra kết quả trả về từ API.
4. Sử dụng tài khoản admin/sinh viên:
    - Copy token trong response của 2 route API đăng nhập /Auth/login/admin, /Auth/login/student.
    - Lướt lên trên đàu, nhấn Authorize, dán "Bearer token_vừa_copy" vào ô giá trị, nhấn Authorize xong có thể sử dụng các route yêu cầu tài khoản.
    *Ví dụ:* `Bearer eyJhbGciOi ... (dài lắm)`

*Quy trình test sau khi code:*
Code -> test.
Có lỗi -> sửa -> test lại.
Không sửa được -> chatgpt.
Chatgpt không sửa được -> hỏi tôi.
Test thấy không có lỗi -> commit -> push.

Ai code xong phần của mình thì nhắn cho người test phần đấy (đã ghi rõ trong doc) để clone repo về và test luôn. Thấy lỗi thì báo lại cho dev để sửa luôn, không cần đợi cả nhóm xong hết mới sửa.

# How to Commit code

**Quy tắc:** main là branch chính, không được push trực tiếp lên main mà phải tạo branch mới. Tôi kiểm tra thấy code ok thì tôi merge vô main, nếu không được thì tôi sẽ comment những vấn đề cần sửa.

Tạo branch mới với tên theo chức năng mình làm. Ví dụ: `tintuc`, `tin_tuc`, `tin-tuc`. Miễn sao đọc là hiểu được branch đó làm gì.

# Note

Sử dụng các file tôi đã code sẵn làm mẫu, copy-paste rồi sửa lại cho phù hợp sẽ dễ hơn là code hoàn toàn mới từ đầu.
Khó khăn gì thì hỏi. Và đừng động vào các file code quản lý admin, tài liệu của tôi :DDD