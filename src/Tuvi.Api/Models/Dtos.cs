namespace Tuvi.Api.Models;

// ----- Hồ sơ / thần số học -----

public record ProfileRequest(DateOnly BirthDate);

public record NumerologyResult(int Number, string Title, string Description);

public record ProfileResult(
    DateOnly BirthDate,
    ZodiacInfo Zodiac,
    NumerologyResult Numerology);

// ----- Tử vi hằng ngày -----

public record DailyHoroscope(
    string SignKey,
    string SignNameVi,
    string Symbol,
    DateOnly Date,
    int Score,           // 1..5 sao
    string Headline,
    string Overall,
    string Love,
    string Career,
    string Money,
    string Mood,
    string Advice,
    int LuckyNumber,
    string LuckyColor);

// ----- Trắc nghiệm 16 nhóm tính cách -----

public record PersonalityOption(string Text, char Pole);

public record PersonalityQuestion(
    int Id,
    string Axis,
    string Text,
    PersonalityOption OptionA,
    PersonalityOption OptionB);

public record PersonalityAnswer(int QuestionId, char Pole);

public record PersonalityRequest(List<PersonalityAnswer> Answers);

public record PersonalityResult(string Type, string Nickname, string Description, string Strengths);

// ----- Độ hợp đôi -----

public record CompatibilityResult(
    string SignA,
    string SignB,
    int Percent,
    string Verdict,
    string Detail,
    string Advice);
