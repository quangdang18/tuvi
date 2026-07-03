namespace Tuvi.Api.Push;

/// <summary>Cấu hình push (đọc từ appsettings "Push").</summary>
public class PushOptions
{
    /// <summary>"Log" (mặc định, ghi log để test local) hoặc "Fcm" (gửi thật qua Firebase).</summary>
    public string Mode { get; set; } = "Log";

    /// <summary>Giờ (địa phương) gửi push tử vi mỗi sáng.</summary>
    public int SendAtHour { get; set; } = 8;

    public string FcmServerKey { get; set; } = "";
}
