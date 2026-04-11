using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using QLSanBong.Application.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace QLSanBong.Application.Services;

public class VnPayService(IConfiguration configuration) : IVnPayService
{
    public string CreatePaymentUrl(HttpContext context, Guid bookingId, decimal amountToPay)
    {
        var vnp_Params = new SortedList<string, string>(new VnPayCompare())
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", configuration["VnPay:TmnCode"] },
            { "vnp_Amount", ((long)(amountToPay * 100)).ToString() },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"Thanh toan dat san: {bookingId}" },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", configuration["VnPay:ReturnUrl"] },
            { "vnp_TxnRef", bookingId.ToString() }
        };

        var queryString = string.Join("&", vnp_Params.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        var vnp_SecureHash = HmacSHA512(configuration["VnPay:HashSecret"], queryString);
        return $"{configuration["VnPay:BaseUrl"]}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
    }

    public bool ValidateSignature(IQueryCollection queryData)
    {
        var vnp_SecureHash = queryData["vnp_SecureHash"].ToString();
        var vnp_Params = new SortedList<string, string>(new VnPayCompare());

        foreach (var key in queryData.Keys.Where(k => k.StartsWith("vnp_") && k != "vnp_SecureHash" && k != "vnp_SecureHashType"))
        {
            vnp_Params.Add(key, queryData[key].ToString());
        }

        var queryString = string.Join("&", vnp_Params.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        return HmacSHA512(configuration["VnPay:HashSecret"], queryString)
               .Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hashValue = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
    }

    private class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y) => string.CompareOrdinal(x, y);
    }
}