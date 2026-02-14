
namespace KhoaCNTT.Domain.Enums
{
    public enum FilePermission
    {
        Hidden = 0,             // Chỉ Admin thấy
        PublicRead = 1,         // Khách xem được, không tải được
        PublicDownload = 2,     // Khách tải được
        StudentRead = 3,        // Sinh viên xem được, không tải được
        StudentDownload = 4     // Sinh viên tải được
    }
}
