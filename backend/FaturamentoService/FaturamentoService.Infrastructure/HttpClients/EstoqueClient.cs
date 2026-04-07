using System.Net;
using System.Text;
using System.Text.Json;
using FaturamentoService.Application.Interfaces;
using FaturamentoService.Domain.Entities;

public sealed class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;

    public EstoqueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool Sucesso, string? MensagemErro)> AbaterEstoqueAsync(NotaFiscal nota, CancellationToken cancellationToken)
    {
        var payload = new
        {
            itens = nota.Itens.Select(i => new
            {
                codigoProduto = i.CodigoProduto,
                quantidade = i.Quantidade
            })
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.PostAsync("api/produtos/abater-estoque", content, cancellationToken);

            if (response.IsSuccessStatusCode)
                return (true, null);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("mensagem", out var msg))
                        return (false, msg.GetString());
                }
                catch { }

                return (false, body);
            }

            return (false, "Serviço de estoque temporariamente indisponível. A nota não pode ser impressa.");
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            return (false, "Serviço de estoque temporariamente indisponível. A nota não pode ser impressa.");
        }
    }
}
