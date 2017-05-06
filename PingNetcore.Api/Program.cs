using System.IO;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PingNetcore.Api
{
    public class Program
    {
        /// <summary>
        /// Call http(s)://base_url/ping with ip adress in body.
        /// return 1 if ping sucess, 0 if not.
        /// </summary>
        public static void Main()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(builder =>
                {
                    builder.Use(async (context, next) =>
                    {
                        // Handle requests on /ping only.
                        if (!context.Request.Path.HasValue || context.Request.Path.Value.ToLower() != "/ping")
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            return;
                        }

                        // Handle POST requesto only.
                        if (context.Request.Method != "POST")
                        {
                            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return;
                        }

                        using (StreamReader sr = new StreamReader(context.Request.Body))
                        {
                            PingReply reply =
                                await new System.Net.NetworkInformation.Ping().SendPingAsync(sr.ReadToEnd().Replace("\"", ""));
                            await context.Response.WriteAsync(reply.Status == IPStatus.Success ? "1" : "0");
                        }
                    });
                })
                .Build();

            host.Run();
        }
    }
}
