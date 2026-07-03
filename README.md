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

### User · cá nhân hóa · streak
| Method | Route | Mô tả |
|--------|-------|-------|
| POST | `/api/users` | Đăng ký user (lưu ngày/giờ/nơi sinh). Body: `{ "displayName":"Minh", "birthDate":"2001-08-01", "birthTime":"07:30:00", "birthPlace":"Hà Nội" }` |
| GET | `/api/users/{id}` | Hồ sơ user |
| POST | `/api/users/{id}/device-token` | Đăng ký device token nhận push. Body: `{ "token":"..." }` |
| POST | `/api/users/{id}/checkin` | Check-in tâm trạng → tử vi cá nhân hóa + streak. Body: `{ "mood":"Sad", "note":null }` |
| GET | `/api/users/{id}/horoscope?date=` | Tử vi cá nhân hóa (dùng mood đã check-in) |
| GET | `/api/users/{id}/streak` | Chuỗi check-in hiện tại & dài nhất |
| GET | `/api/users/{id}/referral` | Mã & link mời bạn + số người đã mời được |
| PUT | `/api/users/{id}/focus` | Đổi mối quan tâm chính. Body: `{ "focus":"Love" }` |

`mood`: `Happy, Normal, Sad, Stressed, Excited, Tired`. `focus`: `Love, Career, Money, Growth`.

**Auth**: mọi endpoint `/api/users/**` (trừ đăng ký) và `/api/payment/create` yêu cầu header
`Authorization: Bearer <token>` (token nhận từ `POST /api/users`); truy cập id người khác → 403.

**Referral (viral)**: khi đăng ký kèm `"referralCode"` hợp lệ, cả người mời lẫn người được mời
đều +3 ngày Premium. Link mời có dạng `/app.html?ref=CODE`.

### Thanh toán (nâng cấp Premium)
| Method | Route | Mô tả |
|--------|-------|-------|
| POST | `/api/payment/create` | Tạo đơn → trả `payUrl`. Body: `{ "userId":1, "provider":"MoMo", "plan":"Monthly" }` |
| GET | `/api/payment/status?orderId=` | Trạng thái đơn |
| POST | `/api/payment/simulate-success` | **[Mock]** Mô phỏng thanh toán thành công để test local |
| POST | `/api/payment/momo/ipn` · `/api/payment/zalopay/callback` | Callback thật (chạy ở `Live` mode) |

`provider`: `MoMo, ZaloPay`. Ở `Mock` mode (mặc định), `payUrl` trỏ tới trang thanh toán mô phỏng `pay-mock.html`.

### Push
| Method | Route | Mô tả |
|--------|-------|-------|
| POST | `/api/push/run-now` | **[Dev]** Gửi push tử vi ngay cho mọi user có device token |
| GET | `/api/push/log` | **[Dev]** Xem các push đã "gửi" (Log mode) |

Key của 12 cung: `aries, taurus, gemini, cancer, leo, virgo, libra, scorpio, sagittarius, capricorn, aquarius, pisces`.

Trang demo đầy đủ (đăng ký → check-in → tử vi cá nhân hóa → mời bạn → mua Premium): mở **`/app.html`**.
Trang chia sẻ công khai (không cần đăng nhập): **`/share.html?a=leo&b=aries`** (độ hợp) hoặc **`/share.html?sign=leo`** (tử vi ngày).

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

## Cấu hình

`appsettings.json`:
- **Payment.Mode**: `Mock` (mặc định, chạy local không cần merchant key) hoặc `Live` (điền `MoMo`/`ZaloPay` key để gọi cổng thật).
- **Payment.MonthlyPriceVnd**: giá gói tháng (mặc định `20000`).
- **Push.Mode**: `Log` (ghi log để test) hoặc `Fcm`. **Push.SendAtHour**: giờ gửi push mỗi sáng (mặc định `8`).
- Database: SQLite file `tuvi.db`, tự tạo khi chạy (EnsureCreated).

## Định hướng tiếp theo

- **Live thanh toán**: đăng ký merchant MoMo/ZaloPay, đổi `Payment.Mode = Live`, điền key. Chữ ký HMAC đã sẵn.
- **Push thật**: thay `LogPushSender` bằng bản gọi FCM/APNs (giữ nguyên interface `IPushSender`).
- **Migration**: thay `EnsureCreated` bằng EF Core migrations khi schema bắt đầu thay đổi.
- **AI viết nội dung**: đã tách sẵn interface `IReadingWriter` (bản `TemplateReadingWriter` hiện tại).
  Chỉ cần thêm `AiReadingWriter` gọi LLM và đăng ký thay thế — lớp cache `(cung + ngày)` giữ nguyên nên chi phí vẫn ~0.

> Nội dung mang tính giải trí & tham khảo.
