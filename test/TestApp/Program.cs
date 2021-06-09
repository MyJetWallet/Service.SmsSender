using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.SmsSender.Client;
using Service.SmsSender.Domain.Models.Enums;
using Service.SmsSender.Grpc.Models.Requests;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new SmsSenderClientFactory("http://localhost:5001");
            var client = factory.GetSmsService();

            var resp1 = await  client.SendLogInSuccessAsync(new SendLogInSuccessRequest
            {
                Phone = "+380974593496",
                Brand = "BlahBlahBrand",
                Lang = LangEnum.En,
                Ip = "127.0.0.1",
                Date = DateTime.Now
            });

            Console.WriteLine($"Status: {resp1?.Result}, Error message: {resp1?.ErrorMessage}");

            var resp2 = await client.SendTradeMadeAsync(new SendTradeMadeRequest
            {
                Phone = "+380974593496",
                Brand = "BlahBlahBrand",
                Lang = LangEnum.En,
                Symbol = "BTC/USD",
                Volume = 1,
                Price = 38000
            });

            Console.WriteLine($"Status: {resp2?.Result}, Error message: {resp2?.ErrorMessage}");

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
