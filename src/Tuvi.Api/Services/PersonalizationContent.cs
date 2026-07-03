using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>
/// Nội dung cho lớp cá nhân hóa: lời chào theo tâm trạng và "lá số chuyên sâu" (premium).
/// Càng nhiều tín hiệu cá nhân (mood, streak) thì lời tiên tri càng nghe "đúng với mình".
/// </summary>
public static class PersonalizationContent
{
    public static readonly IReadOnlyDictionary<Mood, string[]> Greetings = new Dictionary<Mood, string[]>
    {
        [Mood.Happy] =
        [
            "Thấy bạn đang vui — giữ vibe này nhé, vũ trụ hôm nay hợp gu bạn lắm!",
            "Tâm trạng tốt là một loại may mắn. Hôm nay hãy lan nó cho một người khác.",
        ],
        [Mood.Normal] =
        [
            "Một ngày bình thường cũng là một ngày đáng sống. Cùng xem hôm nay có gì nhé.",
            "Không cần rực rỡ, chỉ cần đều đặn tiến lên. Đây là điều hôm nay dành cho bạn.",
        ],
        [Mood.Sad] =
        [
            "Hôm nay bạn hơi chùng xuống, và điều đó hoàn toàn ổn. Đọc chậm những dòng dưới đây nhé.",
            "Buồn một chút không sao cả — bạn không cô đơn đâu. Vũ trụ có vài lời cho bạn.",
        ],
        [Mood.Stressed] =
        [
            "Đang căng thẳng à? Hít một hơi thật sâu. Hôm nay mình nhắc bạn sống chậm lại một nhịp.",
            "Áp lực là dấu hiệu bạn đang quan tâm. Nhưng nhớ nghỉ, bạn không phải cỗ máy.",
        ],
        [Mood.Excited] =
        [
            "Năng lượng của bạn đang bùng nổ! Hôm nay hợp để bắt đầu điều bạn ấp ủ.",
            "Hào hứng thế này thì tiếc gì mà không thử điều mới. Vũ trụ ủng hộ bạn.",
        ],
        [Mood.Tired] =
        [
            "Bạn đang mệt, và bạn xứng đáng được nghỉ. Hôm nay ưu tiên chăm sóc bản thân.",
            "Mệt không có nghĩa là yếu. Cho phép mình chậm lại hôm nay nhé.",
        ],
    };

    /// <summary>"Lá số chuyên sâu" chỉ dành cho user premium.</summary>
    public static readonly string[] DeepInsights =
    [
        "Lá số chuyên sâu: giai đoạn này sao chiếu mệnh nghiêng về chuyển hóa nội tâm. Một quyết định bạn trì hoãn suốt tuần qua sắp đến lúc phải chốt — hãy tin vào lần 'gut feeling' đầu tiên, nó thường đúng với bạn hơn bạn nghĩ.",
        "Lá số chuyên sâu: trục sự nghiệp – tình cảm của bạn đang giao nhau. Đừng để một hiểu lầm nhỏ trong công việc ảnh hưởng tới người bạn thương. Tách bạch hai vùng năng lượng này, tuần tới sẽ nhẹ hơn nhiều.",
        "Lá số chuyên sâu: đây là chu kỳ 'gieo hạt', chưa phải 'gặt'. Những gì bạn xây âm thầm lúc này sẽ cho quả sau khoảng 6–8 tuần. Kiên nhẫn là siêu năng lực của bạn ở giai đoạn này.",
        "Lá số chuyên sâu: một mối quan hệ cũ có thể quay lại trong tầm ảnh hưởng. Trước khi phản ứng, hãy tự hỏi 'mình đã khác trước rồi'. Bạn có quyền lựa chọn lại, không nợ ai một câu trả lời vội vàng.",
    ];

    public static string StreakMessage(int streak) => streak switch
    {
        <= 0 => "Check-in hôm nay để bắt đầu chuỗi ngày của bạn nhé!",
        1 => "🔥 Chuỗi 1 ngày — khởi đầu rồi đó, mai nhớ quay lại!",
        < 7 => $"🔥 Chuỗi {streak} ngày liên tiếp — giữ nhịp nào!",
        < 30 => $"🔥 {streak} ngày liên tục! Bạn đang tạo một thói quen thật sự đấy.",
        _ => $"🏆 {streak} ngày không nghỉ — bạn thuộc nhóm hiếm hoi kiên trì nhất!",
    };
}
