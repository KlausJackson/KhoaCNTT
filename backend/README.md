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
            ├── FilesController.cs
            ├── LecturersController.cs
            ├── NewsController.cs
            ├── StudentsController.cs
            ├── SubjectController.cs
        └── 📁Utils
            ├── ServiceCollection.cs # đăng ký các services, controllers vào DI container
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
                ├── AutoMapperProfile.cs
                ├── PassswordHasher.cs
        └── 📁DTOs
            └── 📁Admin
                ├── AdminResponse.cs
                ├── CreateAdminRequest.cs
                ├── UpdateAdminRequest.cs
            └── 📁Comment
                ├── CommentResponse.cs
                ├── CreateCommentRequest.cs
            └── 📁File
                ├── FileDto.cs
                ├── FileRequestDto.cs
                ├── UpdateFileRequest.cs
                ├── UploadFileRequest.cs
            └── 📁Lecturer
                ├── CreateLecturerRequest.cs
                ├── LecturerResponse.cs
                ├── UpdateLecturerRequest.cs
                ├── LecturerSearchParams.cs
            └── 📁News
                ├── CreateNewsRequest.cs
                ├── NewsResponse.cs
                ├── UpdateNewsRequest.cs
            └── 📁School
                ├── ScheduleResponse.cs
                ├── SchoolLoginResponse.cs
                ├── ScoreResponse.cs
            ├── CommentDto.cs
            ├── LecturerDto.cs
            ├── PagedResult.cs
            └── NewsDto.cs
        └── 📁Interfaces
            └── 📁Repositories
                └── 📁IFileRepositories
                    ├── IFileApprovalRepository.cs
                    ├── IFileRepository.cs
                    ├── IFileRequestRepository.cs
                    ├── IFileResourceRepository.cs
                └── 📁INewsRepositories
                    ├── INewsApprovalRepository.cs
                    ├── INewsRepository.cs
                    ├── INewsRequestRepository.cs
                    ├── INewsResourceRepository.cs
                ├── IAdminRepository.cs
                ├── ICommentRepository.cs
                ├── ILecturerRepository.cs
                ├── IRepository.cs
                ├── ISubjectRepository.cs
            └── 📁Services
                ├── IAdminService.cs
                ├── IAuthService.cs
                ├── IFileService.cs
                ├── IFileStorageService.cs
                ├── IJwtTokenGenerator.cs
                ├── ILecturerService.cs
                ├── INewsService.cs
                ├── ISchoolApiService.cs
                └── semesterId.json
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
            └── 📁FileEntities
                ├── FileApproval.cs
                ├── FileEntity.cs
                ├── FileRequest.cs
                └── FileResource.cs
            └── 📁NewsEntities
                ├── NewsApproval.cs
                ├── Comment.cs                
                ├── News.cs
                ├── NewsRequest.cs
                └── NewsResource.cs
            ├── Admin.cs
            ├── Lecturer.cs
            ├── LecturerSubject.cs
            ├── Subject.cs
        └── 📁Enums
            ├── ApprovalDecision.cs
            ├── DegreeType.cs
            ├── FilePermission.cs
            ├── FileType.cs
            ├── NewsType.cs
            └── RequestType.cs
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
            └── File
                ├── FileApprovalRepository.cs
                ├── FileRepository.cs
                ├── FileRequestRepository.cs
                ├── FileResourceRepository.cs
            └── News
                ├── NewsApprovalRepository.cs
                ├── NewsRepository.cs
                ├── NewsRequestRepository.cs
                ├── NewsResourceRepository.cs
            ├── AdminRepository.cs
            ├── LecturerRepository.cs
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

# Project Setup

Tạo folder mới để lưu trữ file trên ổ D nếu muốn test các chức năng quản lý tài liệu: `D:\KhoaCNTT_data`.
Máy không có ổ D thì tạo folder `KhoaCNTT_data` ở bất kỳ đâu trên máy, sau đó chỉnh sửa đường dẫn trong file `appsettings.json` của KhoaCNTT.API:

```json
  "FileStorage": {
    "UploadFolder": "D:\\KhoaCNTT_data"
  },
```


## Database Setup

Tạo database trong SQL Server Management Studio (SSMS) với tên `khoacntt`.

Cách xem và chỉnh sửa dữ liệu trực tiếp trong database mà không cần mở SQL Server:
1. Chọn View trên tab trên cùng của Visual Studio, chọn SQL Server Object Explorer.
2. (localdb)\\MSSQLLocalDB -> Databases -> khoacntt.

Đa phần là không cần, nhưng tùy máy sẽ phải sửa connectionStrings đúng với máy cá nhân trong file `appsettings.json` của KhoaCNTT.API để trỏ đến database mới tạo:
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

### Thêm dữ liệu vào database

Danh sách môn học: "KhoaCNTT\database\subjects.sql"

1. Chọn View trên tab trên cùng của Visual Studio, chọn SQL Server Object Explorer.
2. (localdb)\\MSSQLLocalDB -> Databases -> khoacntt.
3. Chuột phải vô khoacntt, chọn "New Query".
4. Dán lệnh SQL từ file `subjects.sql` vào ô query.
5. Nhấn Execute để chạy lệnh, dữ liệu sẽ được thêm vào database.

### Admin

Sử dụng tài khoản Admin cấp 1 được lập trình sẵn trong code (admin, abc123) để thực hiện các quyền của cấp 1, và tạo tài khoản mới với quyền cấp 2/3 trong database rồi sửa trực tiếp thành cấp 1. 

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

# Test

Cách test sử dụng swagger có sẵn (có thể dùng postman, nhưng swagger tiện hơn):
1. Chạy dự án, nó sẽ tự động mở trang swagger ở trình duyệt.
2. Chọn API muốn test, nhấn Try it out, điền thông tin cần thiết, nhấn Execute để gửi request.
3. Xem phần Response để kiểm tra kết quả trả về từ API.
4. Sử dụng tài khoản admin/sinh viên:
    - Copy token trong response của 2 route API đăng nhập /Auth/login/admin, /Auth/login/student.
    - Lướt lên trên đàu, nhấn Authorize, dán "Bearer token_vừa_copy" vào ô giá trị, nhấn Authorize xong có thể sử dụng các route yêu cầu tài khoản.
    *Ví dụ:* `Bearer eyJhbGciOi ... (dài lắm)`

Ai code xong phần của mình thì nhắn cho người test phần đấy (đã ghi rõ trong doc) để clone repo về và test luôn. Thấy lỗi thì báo lại cho dev để sửa luôn, không cần đợi cả nhóm xong hết mới sửa.

## Chạy các file tạo data mẫu

Mục đích là cho tiện việc kiểm thử.

Yêu cầu: máy có python, và đã cài thư viện requests (`pip install requests`).

Chạy dự án trước, rồi sử dụng Swagger để tạo token cho admin cấp 1, 2, 3. Copy & paste 3 token vô 3 file: 
- `upfiles.py` - it's me =))
- `uplecturers.py` - Le Dinh Minh
- `upnews.py` - Cao Duc Dao 

*Những file này chạy được hay không đều do dev nhé.*

**Lưu ý**: đặc biệt với `upfiles.py`, vì nó có liên quan đến file vật lý trên server, nên cần chỉnh sửa đường dẫn lưu file cho phù hợp với máy cá nhân trước khi chạy. Tìm một thư mục chứa nhiều tài liệu trên máy, copy đường dẫn, dán vào dòng này:

```bash
ROOT_FOLDER = r"E:\PDF\Programming"
```

# How to Commit code

**Quy tắc:** main là branch chính, không được push trực tiếp lên main mà phải tạo branch mới. Tôi kiểm tra thấy code ok thì tôi merge vô main, nếu không được thì tôi sẽ comment những vấn đề cần sửa.

Tạo branch mới với tên theo chức năng mình làm. Ví dụ: `tintuc`, `tin_tuc`, `tin-tuc`. Miễn sao đọc là hiểu được branch đó làm gì.

# Note

Sử dụng các file tôi đã code sẵn làm mẫu, copy-paste rồi sửa lại cho phù hợp sẽ dễ hơn là code hoàn toàn mới từ đầu.
Khó khăn gì thì hỏi. Và đừng động vào các file code quản lý admin, tài liệu của tôi :DDD Đm thằng Đạo.
