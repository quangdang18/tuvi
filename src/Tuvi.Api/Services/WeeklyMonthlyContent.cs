namespace Tuvi.Api.Services;

/// <summary>Kho nội dung cho tử vi tuần và tháng (giọng "dự báo giai đoạn", rộng hơn tử vi ngày).</summary>
public static class WeeklyMonthlyContent
{
    // ----- TUẦN -----

    public static readonly string[] WeekHeadlines =
    [
        "Một tuần để gieo hạt cho những mục tiêu dài hơi",
        "Tuần này nhịp sống của bạn dần vào guồng",
        "Tuần của những kết nối và cơ hội mới",
        "Chậm mà chắc — tuần này hợp để củng cố nền tảng",
        "Tuần bạn học được nhiều điều từ chính thử thách của mình",
        "Một tuần cân bằng giữa nỗ lực và nghỉ ngơi",
        "Tuần này bản lĩnh của bạn được tôi luyện",
        "Tuần mở ra vài cánh cửa bạn từng nghĩ đã khép",
    ];

    public static readonly string[] WeekOverall =
    [
        "Tuần này mở đầu có thể hơi chậm, nhưng càng về cuối tuần năng lượng của bạn càng lên. Đừng nản ở những ngày đầu — đà tốt đang tích lũy dần.",
        "Đây là tuần thích hợp để nhìn lại mục tiêu và điều chỉnh cho thực tế hơn. Bớt ôm đồm, tập trung vào 1-2 việc quan trọng sẽ cho kết quả rõ rệt.",
        "Một tuần nhiều tương tác: bạn sẽ gặp gỡ, trao đổi và học từ người khác nhiều hơn thường lệ. Giữ tinh thần cởi mở, cơ hội thường đến qua các mối quan hệ.",
        "Tuần này nhắc bạn chăm sóc bản thân song song với công việc. Ngủ đủ và vận động nhẹ sẽ giúp bạn giữ phong độ tới cuối tuần.",
        "Có thể giữa tuần xuất hiện một vài xáo trộn nhỏ, nhưng chúng là dịp để bạn rèn sự linh hoạt. Bình tĩnh xử lý, mọi thứ sẽ ổn định lại nhanh.",
        "Tuần của sự bền bỉ: kết quả chưa đến ngay, nhưng những gì bạn làm đều đặn tuần này sẽ tạo nền cho bước nhảy ở tuần sau.",
    ];

    public static readonly string[] WeekLove =
    [
        "Chuyện tình cảm tuần này ấm dần lên nếu bạn chịu chủ động hơn một chút. Một lời quan tâm đúng lúc có sức nặng hơn nhiều món quà.",
        "Tuần này hợp để lắng nghe nhiều hơn nói. Người ấy cần cảm giác được thấu hiểu, và đó là điều bạn làm tốt khi thật sự để tâm.",
        "Nếu độc thân, tuần này bạn có cơ duyên gặp người thú vị qua bạn bè hoặc hoạt động chung. Cứ tự nhiên, đừng cố gồng.",
        "Giữa tuần có thể có chút hiểu lầm, nhưng cuối tuần là thời điểm tốt để làm hòa và xích lại gần nhau.",
    ];

    public static readonly string[] WeekCareer =
    [
        "Công việc/học tập tuần này thưởng cho sự đều đặn. Chia nhỏ mục tiêu theo ngày, cuối tuần nhìn lại bạn sẽ bất ngờ với tiến độ.",
        "Một cơ hội thể hiện năng lực có thể đến giữa tuần. Chuẩn bị kỹ và tự tin — bạn sẵn sàng hơn bạn nghĩ.",
        "Tuần này hợp để học kỹ năng mới hoặc hoàn thiện thứ còn dang dở. Đầu tư vào bản thân lúc này sẽ sinh lời về sau.",
        "Đừng ngại phối hợp với người khác tuần này. Một góc nhìn mới có thể tháo gỡ đúng nút thắt bạn đang mắc.",
    ];

    public static readonly string[] WeekMoney =
    [
        "Tài chính tuần này ổn định nếu bạn bám kế hoạch chi tiêu. Tránh những quyết định mua sắm bốc đồng vào giữa tuần.",
        "Tuần này hợp để rà soát và cắt bớt các khoản không cần thiết. Một chút kỷ luật giờ, cuối tháng bạn sẽ cảm ơn chính mình.",
        "Có thể có khoản thu ngoài dự kiến. Thay vì tiêu ngay, hãy để một phần vào quỹ tiết kiệm mục tiêu của bạn.",
        "Tuần thích hợp để lên kế hoạch tài chính cho tháng: đặt hạn mức, chia nhóm chi tiêu. Rõ ràng từ đầu giúp bạn an tâm hơn.",
    ];

    // ----- THÁNG -----

    public static readonly string[] MonthThemes =
    [
        "Tháng của sự khởi đầu mới",
        "Tháng để củng cố và ổn định",
        "Tháng của kết nối và mở rộng",
        "Tháng nhìn lại và trưởng thành",
        "Tháng của nỗ lực được đền đáp",
        "Tháng cân bằng và chăm sóc bản thân",
        "Tháng bứt phá khỏi vùng an toàn",
        "Tháng gieo hạt cho mục tiêu dài hạn",
    ];

    public static readonly string[] MonthOverall =
    [
        "Tháng này là một chu kỳ 'gieo hạt' hơn là 'gặt hái'. Những gì bạn xây âm thầm lúc này sẽ cho quả rõ rệt trong 1-2 tháng tới. Kiên nhẫn là chìa khóa.",
        "Một tháng nhiều chuyển động: bạn sẽ phải ra vài quyết định quan trọng. Tin vào giá trị cốt lõi của mình, đừng để áp lực bên ngoài cuốn đi.",
        "Tháng này nhắc bạn cân bằng giữa tham vọng và sức khỏe. Đi đường dài cần giữ sức — đừng đốt cháy bản thân vì những mục tiêu ngắn hạn.",
        "Nửa đầu tháng thích hợp để lên kế hoạch và khởi động; nửa sau là lúc tăng tốc và hoàn thiện. Chia tháng làm hai nhịp sẽ giúp bạn đỡ quá tải.",
        "Tháng của những bài học từ quan hệ xung quanh. Bạn sẽ hiểu hơn ai thật lòng đồng hành, và điều đó định hình các lựa chọn sắp tới.",
        "Đây là tháng bạn có thể tạo bước ngoặt nhỏ nếu dám thử điều mới. Một quyết định can đảm đầu tháng có thể thay đổi cả quỹ đạo cuối năm.",
    ];

    public static readonly string[] MonthLove =
    [
        "Tình cảm tháng này đi vào chiều sâu. Đây là lúc để vun đắp sự tin tưởng thay vì chạy theo cảm xúc nhất thời.",
        "Nếu độc thân, tháng này có nhiều cơ hội gặp gỡ. Đừng vội, hãy để thời gian sàng lọc người thật sự phù hợp với bạn.",
        "Tháng thích hợp để nói ra điều bạn ấp ủ trong lòng với người quan trọng. Sự chân thành sẽ được đáp lại xứng đáng.",
        "Có thể tháng này bạn cần khoảng riêng để hiểu mình muốn gì trong tình cảm. Rõ với bản thân trước, mọi thứ khác sẽ sáng ra.",
    ];

    public static readonly string[] MonthCareer =
    [
        "Sự nghiệp/học tập tháng này thưởng cho tầm nhìn dài hạn. Đặt mục tiêu tháng cụ thể và bám sát, bạn sẽ thấy tiến bộ rõ.",
        "Một cơ hội lớn hơn có thể xuất hiện giữa tháng. Hãy chuẩn bị từ sớm để khi thời điểm đến, bạn ở tư thế sẵn sàng nắm bắt.",
        "Tháng này hợp để đầu tư vào kỹ năng nền. Những gì bạn học lúc này sẽ là lợi thế cạnh tranh trong các tháng bận rộn sắp tới.",
        "Mở rộng mạng lưới quan hệ tháng này rất có lợi. Một kết nối đúng người có thể mở ra cánh cửa bạn tìm bấy lâu.",
    ];

    public static readonly string[] MonthMoney =
    [
        "Tài chính tháng này cần một kế hoạch rõ ràng. Chia thu nhập theo nhóm ngay đầu tháng sẽ giúp bạn chủ động và bớt lo cuối tháng.",
        "Tháng thích hợp để xây thói quen tiết kiệm tự động: trích một khoản nhỏ ngay khi có tiền, trước khi tiêu.",
        "Cẩn trọng với các khoản đầu tư 'nóng' trong tháng. Ưu tiên hiểu rõ trước khi xuống tiền, đừng chạy theo đám đông.",
        "Có thể tháng này có cơ hội tăng thu nhập từ một việc phụ hoặc kỹ năng riêng. Đáng để bạn thử nghiêm túc.",
    ];

    public static readonly string[] Weekdays =
    [
        "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật",
    ];
}
