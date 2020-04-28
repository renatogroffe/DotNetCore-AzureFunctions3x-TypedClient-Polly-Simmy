using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using FunctionAppConsumoAPI.Models;

namespace FunctionAppConsumoAPI.Clients
{
    public class APIContagemClient
    {
        private HttpClient _client;
        private ILogger<APIContagemClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public APIContagemClient(
             HttpClient client,
             ILogger<APIContagemClient> logger)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _logger = logger;

            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public ResultadoContador ObterDadosContagem()
        {
            var response = _client.GetAsync(
                Environment.GetEnvironmentVariable("UrlAPIContagem")).Result;

            ResultadoContador resultado = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string conteudo =
                    response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation($"Retorno JSON: {conteudo}");

                resultado = JsonSerializer
                    .Deserialize<ResultadoContador>(conteudo, _jsonOptions);
            }

            response.EnsureSuccessStatusCode();
            return resultado;
        }
    }
}