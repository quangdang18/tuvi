# Tuvi ✨ — Backend Tử Vi / Thần Số Học / 16 Nhóm Tính Cách

Backend (ASP.NET Core Web API, .NET 10) dùng chung cho **cả web và mobile**. Sinh nội dung
tử vi hằng ngày theo **template + hiệu ứng Barnum**, chọn deterministic theo `(cung + ngày)`
và **cache** lại — nên phục vụ hàng chục nghìn user với **chi phí gần như bằng 0** (không gọi AI).

## Chạy local

```bash
cd src/Tuvi.Api
dotnet run
```

Mặc định mở tại `http://localhost:5xxx` (xem cổng in ra ở console):

- **Trang demo**: `/` — nhập ngày sinh, xem cung hoàng đạo + vận trình hôm nay.
- **Swagger UI**: `/swagger` — thử toàn bộ API.

## API

| Method | Route | Mô tả |
|--------|-------|-------|
| POST | `/api/profile` | Ngày sinh → cung hoàng đạo + con số chủ đạo (thần số học). Body: `{ "birthDate": "2001-08-01" }` |
| GET | `/api/horoscope/signs` | Danh sách 12 cung hoàng đạo |
| GET | `/api/horoscope/daily/{sign}?date=` | Tử vi hằng ngày theo cung (vd `leo`). Bỏ trống `date` = hôm nay |
| GET | `/api/horoscope/daily-by-birth?birthDate=&date=` | Tử vi hằng ngày theo ngày sinh (tự suy ra cung) |
| GET | `/api/personality/questions` | Bộ câu hỏi trắc nghiệm 16 nhóm tính cách |
| POST | `/api/personality/result` | Chấm kết quả. Body: `{ "answers": [ { "questionId": 1, "pole": "E" }, ... ] }` |
| GET | `/api/compatibility?a=leo&b=aries` | Độ hợp giữa hai cung |

Key của 12 cung: `aries, taurus, gemini, cancer, leo, virgo, libra, scorpio, sagittarius, capricorn, aquarius, pisces`.

## Kiến trúc

```
src/Tuvi.Api/
├── Program.cs              # Cấu hình DI, CORS, Swagger, static files
├── Controllers/           # Profile, Horoscope, Personality, Compatibility
├── Services/
│   ├── ZodiacService       # 12 cung + suy cung từ ngày sinh
│   ├── NumerologyService   # con số chủ đạo (Life Path)
│   ├── HoroscopeContent    # kho câu chữ (thêm nội dung ở đây)
│   ├── HoroscopeService    # sinh + cache tử vi hằng ngày
│   ├── PersonalityService  # trắc nghiệm 16 nhóm tính cách
│   ├── CompatibilityService# độ hợp đôi theo nguyên tố
│   └── StableHash          # băm ổn định để chọn nội dung (deterministic)
├── Models/                # ZodiacInfo, DTOs
└── wwwroot/index.html     # trang demo
```

## Vì sao user thấy "đúng"

Cảm giác chính xác ≈ **20% engine tính toán + 80% cách viết theo hiệu ứng Barnum + cá nhân hóa**.
Engine (cung, thần số học) tạo *cấu trúc & độ tin cậy*; kho nội dung viết khéo tạo *cảm giác được
nói riêng cho mình*. Toàn bộ deterministic nên nội dung cố định trong ngày, không tốn tiền AI.

## Định hướng tiếp theo

- Cá nhân hóa sâu hơn: hỏi giờ/nơi sinh, tâm trạng hằng ngày → nội dung càng "đúng".
- Thanh toán MoMo/ZaloPay để mở khóa tính năng premium (lá số chi tiết, phân tích cặp đôi).
- Tầng lai + cache: dùng AI viết lời văn nhưng cache theo `(cung + ngày)` để giữ chi phí ~0.

> Nội dung mang tính giải trí & tham khảo.
