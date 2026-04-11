using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace QLSanBong.Application.Services;

public class VnPayService(IConfiguration configuration)
{
    // 1. Hàm tạo URL chuyển hướng sang VNPay
    public string CreatePaymentUrl(HttpContext context, Guid bookingId, decimal amountToPay)
    {
        var vnp_TmnCode = configuration["VnPay:TmnCode"];
        var vnp_HashSecret = configuration["VnPay:HashSecret"];
        var vnp_Url = configuration["VnPay:BaseUrl"];
        var vnp_Returnurl = configuration["VnPay:ReturnUrl"];

        var vnp_Params = new SortedList<string, string>(new VnPayCompare())
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((long)(amountToPay * 100)).ToString() }, // VNPay yêu cầu nhân 100
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", "Thanh toan coc san bong " + bookingId },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", vnp_Returnurl },
            { "vnp_TxnRef", bookingId.ToString() } // Dùng BookingId làm mã giao dịch
        };

        // Build chuỗi dữ liệu gốc
        var queryString = string.Join("&", vnp_Params.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        // Ký số bảo mật SHA512
        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, queryString);
        return $"{vnp_Url}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
    }

    // 2. Hàm kiểm tra tính hợp lệ khi VNPay trả về
    public bool ValidateSignature(IQueryCollection queryData)
    {
        var vnp_HashSecret = configuration["VnPay:HashSecret"];
        var vnp_SecureHash = queryData["vnp_SecureHash"].ToString();

        var vnp_Params = new SortedList<string, string>(new VnPayCompare());
        foreach (var key in queryData.Keys.Where(k => k.StartsWith("vnp_") && k != "vnp_SecureHash" && k != "vnp_SecureHashType"))
        {
            vnp_Params.Add(key, queryData[key].ToString());
        }

        var queryString = string.Join("&", vnp_Params.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        var checkSum = HmacSHA512(vnp_HashSecret, queryString);
        return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hashValue = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
    }

    private class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.CompareOrdinal(x, y);
        }
    }
}