namespace Tuvi.Api.Services;

/// <summary>
/// Băm ổn định (deterministic) để chọn nội dung theo khóa (cung + ngày + mục).
/// Cùng input luôn cho cùng kết quả → nội dung cố định trong ngày, cache được, chi phí = 0.
/// Dùng FNV-1a để không phụ thuộc GetHashCode (vốn có thể đổi giữa các phiên chạy).
/// </summary>
public static class StableHash
{
    public static int Compute(string s)
    {
        unchecked
        {
            const uint fnvPrime = 16777619;
            uint hash = 2166136261;
            foreach (char c in s)
            {
                hash ^= c;
                hash *= fnvPrime;
            }
            return (int)(hash & 0x7fffffff); // luôn không âm
        }
    }

    /// <summary>Chọn một chỉ số trong [0, count) theo khóa ghép từ các phần.</summary>
    public static int Pick(int count, params string[] parts) =>
        Compute(string.Join('|', parts)) % count;

    /// <summary>Chọn một phần tử trong pool theo khóa ghép từ các phần.</summary>
    public static T Pick<T>(IReadOnlyList<T> pool, params string[] parts) =>
        pool[Compute(string.Join('|', parts)) % pool.Count];
}
