using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>Tính "con số chủ đạo" (Life Path) từ ngày sinh và ý nghĩa của nó.</summary>
public class NumerologyService
{
    private static readonly Dictionary<int, (string Title, string Desc)> Meanings = new()
    {
        [1]  = ("Người tiên phong", "Độc lập, quyết đoán, sinh ra để dẫn đầu. Bạn hợp khởi xướng cái mới hơn là đi theo lối mòn."),
        [2]  = ("Người kết nối", "Nhạy cảm, tinh tế, giỏi làm hòa. Bạn tỏa sáng khi hợp tác và giữ cân bằng cho mọi người."),
        [3]  = ("Người truyền cảm hứng", "Sáng tạo, vui vẻ, có duyên ăn nói. Năng lượng của bạn khiến người khác thấy tích cực hơn."),
        [4]  = ("Người xây nền", "Chăm chỉ, đáng tin, thực tế. Bạn biến ý tưởng thành kết quả nhờ kỷ luật và sự bền bỉ."),
        [5]  = ("Người tự do", "Thích khám phá, linh hoạt, ghét gò bó. Thay đổi và trải nghiệm mới là nhiên liệu của bạn."),
        [6]  = ("Người chăm sóc", "Ấm áp, trách nhiệm, coi trọng gia đình và bạn bè. Bạn là chỗ dựa mà người khác tìm đến."),
        [7]  = ("Người tìm tòi", "Sâu sắc, thích phân tích, giàu trực giác. Bạn cần không gian riêng để suy ngẫm và hiểu bản chất."),
        [8]  = ("Người kiến tạo", "Tham vọng, giỏi tổ chức, hướng tới thành tựu và sự sung túc. Bạn có tố chất lãnh đạo và làm chủ."),
        [9]  = ("Người lý tưởng", "Rộng lượng, giàu lòng trắc ẩn, sống vì điều lớn lao. Bạn quan tâm tới cộng đồng hơn lợi ích riêng."),
        [11] = ("Bậc thầy trực giác", "Số bậc thầy: nhạy bén, giàu cảm hứng, có sức ảnh hưởng tinh thần mạnh mẽ tới người xung quanh."),
        [22] = ("Bậc thầy kiến tạo", "Số bậc thầy: tầm nhìn lớn đi cùng khả năng hiện thực hóa — bạn có thể tạo ra thứ để đời."),
        [33] = ("Bậc thầy dẫn dắt", "Số bậc thầy: tình yêu thương và sự tận tụy nâng đỡ người khác ở mức hiếm có."),
    };

    public NumerologyResult LifePath(DateOnly birth)
    {
        static int DigitSum(int n)
        {
            int s = 0;
            while (n > 0) { s += n % 10; n /= 10; }
            return s;
        }

        int total = DigitSum(birth.Year) + DigitSum(birth.Month) + DigitSum(birth.Day);
        while (total > 9 && total != 11 && total != 22 && total != 33)
            total = DigitSum(total);

        var (title, desc) = Meanings.TryGetValue(total, out var m) ? m : ("Con số chủ đạo", "");
        return new NumerologyResult(total, title, desc);
    }
}
