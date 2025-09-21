using BudgetManagement.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BudgetManagement.Infrastructure.Services;

public class ClientInfoProvider : IClientInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string GetBrowserInfo()
    {
        return _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "";
    }

    public string GetClientIpAddress()
    {
        var ips = new List<string>();

        // گرفتن IPهای لوکال سرور (فقط IPv4 و غیر لوپ‌بک)
        // Get only local IPv4 addresses
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up) continue;
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue; // حذف لوپ‌بک مثل 127.0.0.1

            foreach (var ua in ni.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork) // فقط IPv4
                    //ua.Address.AddressFamily == AddressFamily.InterNetworkV6) //IPv6
                {
                    var ip = ua.Address;
                    if (!IPAddress.IsLoopback(ip)) // حذف ::1 و 127.0.0.1
                    {
                        ips.Add(ip.ToString());
                    }
                }
            }
        }


        // گرفتن IP کلاینت از HttpContext
        if (_httpContextAccessor.HttpContext != null)
        {

            // ۱) Remote IP مستقیم
            var remoteIp = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(remoteIp))
                ips.Add(remoteIp);

            // ۲) بررسی X-Forwarded-For (ممکنه چند تا IP داشته باشه)
            var forwardedFor = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var forwardedIps = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(ip => ip.Trim())
                                               .ToList();
                ips.AddRange(forwardedIps);
            }

            // ۳) بررسی X-Real-IP (بعضی سرورها این رو میدن)
            var realIp = _httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"].ToString();
            if (!string.IsNullOrEmpty(realIp))
                ips.Add(realIp);

        }

        var ipList = ips.Distinct().ToList();  // خروجی لیست
        var ipString = string.Join(" - ", ipList); // همه آی‌پی‌ها در یک رشته جدا شده با -

        return ipString;
    }

}

