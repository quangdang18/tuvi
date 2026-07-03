using Tuvi.Api.Models;

namespace Tuvi.Api.Services;

/// <summary>
/// Trắc nghiệm 16 nhóm tính cách (dựa trên 4 cặp đối lập E/I, S/N, T/F, J/P).
/// Đặt tên "16 nhóm tính cách" thay vì "MBTI" để tránh vấn đề nhãn hiệu.
/// </summary>
public class PersonalityService
{
    private static readonly PersonalityQuestion[] Questions =
    [
        new(1, "EI", "Cuối tuần lý tưởng của bạn là?",
            new("Ra ngoài gặp gỡ bạn bè, càng đông càng vui", 'E'),
            new("Ở nhà thư giãn một mình hoặc với vài người thân", 'I')),
        new(2, "EI", "Sau một ngày dài, bạn nạp lại năng lượng bằng cách?",
            new("Trò chuyện, đi chơi cùng mọi người", 'E'),
            new("Ở yên một mình làm điều mình thích", 'I')),
        new(3, "SN", "Bạn chú ý tới điều gì hơn?",
            new("Chi tiết cụ thể, thực tế trước mắt", 'S'),
            new("Ý nghĩa ẩn sau và khả năng ở tương lai", 'N')),
        new(4, "SN", "Khi học điều mới, bạn thích?",
            new("Ví dụ thực tế, hướng dẫn từng bước", 'S'),
            new("Bức tranh tổng thể và ý tưởng lớn", 'N')),
        new(5, "TF", "Khi ra quyết định, bạn dựa vào?",
            new("Logic và sự công bằng", 'T'),
            new("Cảm xúc và ảnh hưởng tới mọi người", 'F')),
        new(6, "TF", "Bạn bè thường tìm đến bạn để?",
            new("Nghe lời khuyên thẳng thắn, khách quan", 'T'),
            new("Được lắng nghe và đồng cảm", 'F')),
        new(7, "JP", "Bạn thấy thoải mái hơn khi?",
            new("Có kế hoạch rõ ràng, mọi thứ đâu vào đó", 'J'),
            new("Linh hoạt, tùy hứng theo tình huống", 'P')),
        new(8, "JP", "Với deadline, bạn thường?",
            new("Làm sớm cho chắc ăn", 'J'),
            new("Nước đến chân mới nhảy nhưng vẫn xong", 'P')),
    ];

    private static readonly Dictionary<string, (string Nick, string Desc, string Strengths)> Types = new()
    {
        ["INTJ"] = ("Kiến trúc sư", "Chiến lược gia thầm lặng, tư duy dài hạn và độc lập. Bạn thích hiểu cả hệ thống và tối ưu mọi thứ.", "Tầm nhìn, quyết đoán, tự chủ"),
        ["INTP"] = ("Nhà tư duy", "Bộ óc tò mò, mê phân tích và ý tưởng. Bạn có thể nghĩ về một vấn đề hàng giờ mà không chán.", "Logic, sáng tạo, khách quan"),
        ["ENTJ"] = ("Nhà chỉ huy", "Sinh ra để dẫn dắt, quyết đoán và giỏi tổ chức. Bạn nhìn ra mục tiêu và con đường đạt tới nó.", "Lãnh đạo, tự tin, hiệu quả"),
        ["ENTP"] = ("Người tranh luận", "Nhanh trí, thích thử thách lối mòn và tranh luận. Ý tưởng mới làm bạn phấn khích.", "Sáng tạo, linh hoạt, hoạt ngôn"),
        ["INFJ"] = ("Người che chở", "Lý tưởng và sâu sắc, quan tâm tới ý nghĩa cuộc sống. Bạn thấu hiểu người khác một cách hiếm có.", "Đồng cảm, tận tâm, sâu sắc"),
        ["INFP"] = ("Người hòa giải", "Mơ mộng, giàu giá trị nội tâm và lòng trắc ẩn. Bạn sống theo điều mình tin là đúng.", "Chân thành, sáng tạo, giàu cảm xúc"),
        ["ENFJ"] = ("Người dẫn dắt", "Ấm áp và truyền cảm hứng, giỏi nâng đỡ người khác. Bạn khiến mọi người muốn tốt hơn.", "Truyền cảm hứng, đồng cảm, kết nối"),
        ["ENFP"] = ("Người truyền lửa", "Nhiệt huyết, tự do và tràn đầy ý tưởng. Bạn nhìn cuộc sống như chuỗi khả năng thú vị.", "Nhiệt tình, sáng tạo, thân thiện"),
        ["ISTJ"] = ("Người trách nhiệm", "Đáng tin, thực tế và kỷ luật. Đã nhận việc là bạn làm tới nơi tới chốn.", "Chỉn chu, bền bỉ, đáng tin"),
        ["ISFJ"] = ("Người bảo vệ", "Tận tụy và chu đáo, luôn để ý nhu cầu người khác. Bạn là chỗ dựa thầm lặng.", "Chu đáo, trung thành, kiên nhẫn"),
        ["ESTJ"] = ("Nhà điều hành", "Thực tế, quyết đoán, giỏi sắp xếp con người và công việc. Bạn coi trọng trật tự và kết quả.", "Tổ chức, trách nhiệm, thẳng thắn"),
        ["ESFJ"] = ("Người chăm sóc", "Nồng hậu, hòa đồng và có trách nhiệm với tập thể. Bạn thích thấy mọi người quanh mình vui vẻ.", "Hòa đồng, tận tâm, chu đáo"),
        ["ISTP"] = ("Nghệ nhân", "Điềm tĩnh, khéo tay và giỏi xử lý vấn đề thực tế. Bạn học bằng cách bắt tay vào làm.", "Thực tế, bình tĩnh, linh hoạt"),
        ["ISFP"] = ("Nghệ sĩ", "Nhẹ nhàng, tinh tế và yêu cái đẹp. Bạn sống trọn khoảnh khắc và theo cảm nhận riêng.", "Tinh tế, chân thành, sáng tạo"),
        ["ESTP"] = ("Người hành động", "Năng động, gan dạ và thích trải nghiệm. Bạn giỏi ứng biến trong tình huống thực tế.", "Nhanh nhẹn, thực tế, dạn dĩ"),
        ["ESFP"] = ("Người trình diễn", "Vui tươi, phóng khoáng và lan tỏa năng lượng. Bạn khiến mọi cuộc vui rộn ràng hơn.", "Hoạt náo, thân thiện, tự nhiên"),
    };

    public IReadOnlyList<PersonalityQuestion> GetQuestions() => Questions;

    public PersonalityResult Evaluate(IEnumerable<PersonalityAnswer> answers)
    {
        var counts = new Dictionary<char, int>();
        foreach (var a in answers)
            counts[a.Pole] = counts.GetValueOrDefault(a.Pole) + 1;

        char Axis(char first, char second, char tieDefault)
        {
            int f = counts.GetValueOrDefault(first);
            int s = counts.GetValueOrDefault(second);
            return f > s ? first : s > f ? second : tieDefault;
        }

        string type = string.Concat(
            Axis('E', 'I', 'I'),
            Axis('S', 'N', 'N'),
            Axis('T', 'F', 'F'),
            Axis('J', 'P', 'P'));

        var (nick, desc, str) = Types[type];
        return new PersonalityResult(type, nick, desc, str);
    }
}
