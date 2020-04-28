using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionAppConsumoAPI.Clients;

namespace FunctionAppConsumoAPI
{
    public class TimerTriggerConsumoAPI
    {
        public readonly APIContagemClient _apiContagemClient;

        public TimerTriggerConsumoAPI(APIContagemClient apiContagemClient)
        {
            _apiContagemClient = apiContagemClient;
        }

        [FunctionName("TimerTriggerConsumoAPI")]
        public void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            var resultado = _apiContagemClient.ObterDadosContagem();
            if (resultado != null)
                log.LogInformation($"Valor do contador: {resultado.ValorAtual}");

            log.LogInformation($"TimerTriggerConsumoAPI executada em: {DateTime.Now}");
        }
    }
}