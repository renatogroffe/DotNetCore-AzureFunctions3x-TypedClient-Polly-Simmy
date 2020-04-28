using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using FunctionAppConsumoAPI.Clients;

[assembly: FunctionsStartup(typeof(FunctionAppConsumoAPI.Startup))]
namespace FunctionAppConsumoAPI
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Geração de uma mensagem simulado erro HTTP do tipo 500
            // (Internal Server Error)
            var resultInternalServerError = new HttpResponseMessage(
                HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(
                    "Simulacao de caos com Simmy...")
            };

            // Criação da Chaos Policy com uma probabilidade
            // de 60% de erro
            var chaosPolicy = MonkeyPolicy
                .InjectResultAsync<HttpResponseMessage>(with =>
                    with.Result(resultInternalServerError)
                        .InjectionRate(0.6)
                        .Enabled()
                );


            // Configuração da Policy para Retry
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(2, onRetry: (message, retryCount) =>
                {
                    var backColor = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.Yellow;

                    var foreColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.Out.WriteLine($"Content: {message.Result.Content.ReadAsStringAsync().Result}");
                    Console.Out.WriteLine($"ReasonPhrase: {message.Result.ReasonPhrase}");
                    Console.Out.WriteLine($"Retentativa: {retryCount}");

                    Console.BackgroundColor = backColor;
                    Console.ForegroundColor = foreColor;
                });

            // Criação de um PolicyWrap agrupando as 2 Policies
            var policyWrap = Policy.WrapAsync(retryPolicy, chaosPolicy);

            builder.Services.AddHttpClient<APIContagemClient>()
                .AddPolicyHandler(policyWrap);
        }
    }
}