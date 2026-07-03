namespace Tuvi.Api.Services;

/// <summary>
/// Kho nội dung viết sẵn cho tử vi hằng ngày.
/// "Cảm giác đúng" đến từ cách viết theo hiệu ứng Barnum (câu chung nhưng nghe rất cá nhân),
/// chứ không phải từ dữ liệu — nên toàn bộ là template, chọn theo băm ổn định (cung + ngày).
/// Muốn nội dung phong phú hơn, chỉ cần thêm phần tử vào các mảng dưới đây.
/// </summary>
public static class HoroscopeContent
{
    public static readonly string[] Headlines =
    [
        "Một ngày để bạn tin vào trực giác của mình",
        "Vũ trụ đang đứng về phía bạn hôm nay",
        "Ngày thích hợp để bắt đầu lại điều còn dang dở",
        "Giữ bình tĩnh, mọi thứ đang dần vào guồng",
        "Năng lượng hôm nay nhắc bạn sống chậm lại một chút",
        "Một cơ hội nhỏ có thể xuất hiện đúng lúc bạn cần",
        "Hôm nay hợp để nói ra điều bạn giữ trong lòng",
        "Ngày của những kết nối bất ngờ và tin vui nho nhỏ",
    ];

    public static readonly string[] Overall =
    [
        "Hôm nay bạn mang một nguồn năng lượng khá đặc biệt: bên ngoài có vẻ bình thản nhưng bên trong lại suy nghĩ rất nhiều. Đừng ép mình phải hoàn hảo — chỉ cần nhích lên một bước nhỏ là đủ.",
        "Có thể gần đây bạn thấy mình cố gắng nhiều mà kết quả chưa rõ ràng. Tin vui là hôm nay những nỗ lực âm thầm bắt đầu được ghi nhận. Kiên nhẫn thêm chút nữa.",
        "Bạn là kiểu người dễ để tâm tới cảm xúc người khác mà đôi khi quên mất chính mình. Hôm nay vũ trụ nhắc bạn: chăm sóc bản thân không phải là ích kỷ.",
        "Một ngày nhiều luồng suy nghĩ trái chiều. Bạn vừa muốn an toàn, vừa khao khát thay đổi. Cứ cho phép mình được lưỡng lự — câu trả lời sẽ đến khi bạn bớt căng thẳng.",
        "Hôm nay thích hợp để 'dọn dẹp': dọn bàn học, dọn điện thoại và dọn cả những mối bận tâm không đáng. Bỏ bớt đi, bạn sẽ thấy nhẹ tênh.",
        "Bạn có một sức mạnh mà chính mình hay xem nhẹ: khả năng đứng dậy sau những lần hụt hẫng. Hôm nay là dịp để bạn nhớ lại điều đó.",
    ];

    public static readonly string[] Love =
    [
        "Trong chuyện tình cảm, đôi khi bạn tỏ ra ổn nhưng thật ra để tâm nhiều hơn mình thừa nhận. Một tin nhắn chân thành hôm nay có thể thay đổi cả bầu không khí.",
        "Nếu đang độc thân, một người quen cũ hoặc một kết nối tình cờ có thể khiến bạn mỉm cười. Đừng vội định nghĩa, cứ để mọi thứ tự nhiên.",
        "Người ấy có thể không giỏi nói lời ngọt ngào, nhưng để ý kỹ bạn sẽ thấy quan tâm nằm ở hành động. Hôm nay hãy nhìn bằng con tim bao dung hơn.",
        "Đừng ngại thể hiện. Bạn hay sợ mình 'quá nhiều', nhưng người thật lòng sẽ trân trọng đúng con người thật của bạn.",
        "Một hiểu lầm nhỏ có thể xuất hiện, nhưng cũng dễ hóa giải nếu bạn chịu nói thẳng thay vì im lặng chờ đối phương tự đoán.",
        "Tình cảm hôm nay cần khoảng thở. Yêu thương không có nghĩa lúc nào cũng phải dính lấy nhau — đôi khi tin tưởng mới là món quà lớn nhất.",
    ];

    public static readonly string[] Career =
    [
        "Việc học/công việc hôm nay đòi hỏi sự tập trung. Bạn dễ bị phân tâm bởi điện thoại — thử úp máy 25 phút, bạn sẽ ngạc nhiên với thứ mình làm được.",
        "Một ý tưởng bạn từng cho là 'hơi điên' thật ra rất đáng thử. Đừng để nỗi sợ bị chê cười ngăn bạn cất lời.",
        "Hôm nay hợp để hỏi khi chưa hiểu. Người khác không đánh giá bạn kém đâu — họ nể người dám hỏi để làm cho đúng.",
        "Bạn đang gánh nhiều thứ cùng lúc. Hãy chọn MỘT việc quan trọng nhất và hoàn thành nó trước — cảm giác 'xong' sẽ tiếp thêm động lực.",
        "Một cơ hội nhỏ (một lời rủ hợp tác, một bài tập được giao) có thể mở ra điều lớn hơn về sau. Hãy nhận lấy với thái độ cầu thị.",
        "Đừng so chương 1 của bạn với chương 20 của người khác. Hôm nay chỉ cần bạn hơn chính mình hôm qua là đã thắng.",
    ];

    public static readonly string[] Money =
    [
        "Tài chính hôm nay nhắc bạn: những khoản chi nhỏ lặt vặt mới là thứ âm thầm bào mòn ví. Thử ghi lại 3 khoản bạn tiêu hôm nay xem sao.",
        "Có thể bạn đang muốn 'thưởng cho bản thân'. Hợp lý thôi, nhưng hãy để 24 tiếng trước khi bấm nút thanh toán món đắt tiền.",
        "Một khoản thu nhỏ hoặc cơ hội kiếm thêm có thể xuất hiện. Đừng chê ít — dòng suối nhỏ góp lại thành sông.",
        "Hôm nay hợp để đặt một mục tiêu tiết kiệm cụ thể: một con số, một cái tên (quỹ đi chơi, quỹ mua đồ bạn thích). Có tên gọi, bạn sẽ giữ được lâu hơn.",
        "Cẩn thận với những lời mời 'lãi cao, nhanh giàu'. Trực giác của bạn hôm nay khá nhạy — thấy sai sai thì thường là sai thật.",
        "Chia sẻ một bữa ăn hay giúp ai đó một chút hôm nay sẽ quay lại với bạn theo cách bạn không ngờ.",
    ];

    public static readonly string[] Mood =
    [
        "Tâm trạng lên xuống thất thường, nhưng đó là dấu hiệu bạn đang cảm nhận cuộc sống một cách sâu sắc, không phải điểm yếu.",
        "Bạn có thể thấy hơi uể oải. Một ly nước, vài phút hít thở sâu và bước ra ngoài nhìn trời sẽ giúp bạn 'khởi động lại'.",
        "Năng lượng hôm nay khá tích cực. Tận dụng lúc tinh thần tốt để làm điều bạn hay trì hoãn.",
        "Nếu thấy quá tải, cho phép mình nói 'không'. Bảo vệ năng lượng của bạn cũng là một dạng trưởng thành.",
        "Một bản nhạc cũ hay một món ăn quen có thể kéo tâm trạng bạn lên bất ngờ. Hãy tự tạo niềm vui nhỏ cho mình.",
        "Bạn đang mạnh mẽ hơn mình nghĩ. Việc bạn vẫn bước tiếp dù mệt đã là điều đáng tự hào rồi.",
    ];

    public static readonly string[] Advice =
    [
        "Nhắn tin hỏi thăm một người bạn lâu chưa liên lạc.",
        "Uống đủ nước và đi ngủ sớm hơn 30 phút so với hôm qua.",
        "Viết ra 3 điều bạn biết ơn trước khi ngủ.",
        "Làm xong việc bạn ngại nhất trước 12 giờ trưa.",
        "Dành 10 phút không điện thoại, chỉ ngồi yên với chính mình.",
        "Nói lời cảm ơn với một người đã giúp bạn gần đây.",
        "Dọn gọn một góc nhỏ trong phòng — không gian sạch, đầu óc nhẹ.",
    ];

    public static readonly string[] LuckyColors =
    [
        "Đỏ", "Cam", "Vàng", "Xanh lá", "Xanh dương", "Xanh ngọc",
        "Tím", "Hồng pastel", "Trắng", "Đen", "Nâu đất", "Xám khói",
    ];
}
