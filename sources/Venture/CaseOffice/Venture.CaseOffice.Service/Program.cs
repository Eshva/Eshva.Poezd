#region Usings

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

#endregion


namespace Venture.CaseOffice.Service
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
  }
}
