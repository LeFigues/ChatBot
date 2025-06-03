using System.Text.Json;
using System.Text.RegularExpressions;

namespace chatbot_api.Services
{
    public class PromptExecutorService
    {
        private readonly PromptService _promptService;
        private readonly CompanyApiService _companyApiService;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public PromptExecutorService(
            PromptService promptService,
            CompanyApiService companyApiService,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _promptService = promptService;
            _companyApiService = companyApiService;
            _httpFactory = httpClientFactory;
            _config = config;
        }

        public async Task<string> EjecutarDesdeTexto(string mensajeUsuario)
        {
            var systemPrompt = await _promptService.GetByKeyAsync("detectar_intencion");
            if (systemPrompt == null) return "❌ Error: Prompt 'detectar_intencion' no está definido.";

            var openAiIntentClient = _httpFactory.CreateClient("OpenAI");
            var intentPayload = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt.PromptText },
                    new { role = "user", content = mensajeUsuario }
                },
                temperature = 0.2
            };

//            var intentResponse = await openAiIntentClient.PostAsJsonAsync("chat/completions", intentPayload);
            var intentResponse = await PostWithRetryAsync(openAiIntentClient, "chat/completions", intentPayload);
            if (!intentResponse.IsSuccessStatusCode)
                return "❌ Error al detectar intención del mensaje.";

            var intentJson = await intentResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(intentJson);
            var intent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?
                .ToLower() ?? "";

            if (intent.Contains("consultar_stock"))
                return await EjecutarConsultarStock(mensajeUsuario);

            if (intent.Contains("comparar_productos"))
                return await EjecutarCompararProductos(mensajeUsuario);

            if (intent.Contains("listar_productos_disponibles"))
                return await EjecutarListarProductos();

            if (intent.Contains("ayuda"))
            {
                var prompts = await _promptService.GetAllAsync();
                if (prompts.Count == 0) return "No hay prompts registrados.";
                var lista = prompts.Select(p => $"- {p.Key}: {p.Description}");
                return "🧠 Comandos disponibles:\n" + string.Join("\n", lista);
            }

            return "🤖 No entendí tu solicitud. Escribe *ayuda* para ver los comandos disponibles.";
        }
        private async Task<HttpResponseMessage> PostWithRetryAsync(HttpClient client, string url, object payload)
        {
            const int maxRetries = 3;
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                var response = await client.PostAsJsonAsync(url, payload);
                if (response.IsSuccessStatusCode || response.StatusCode != System.Net.HttpStatusCode.TooManyRequests || attempt == maxRetries)
                    return response;

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                await Task.Delay(delay);
            }

            // Should not reach here, but return generic error
            return new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
        }
        private async Task<string> EjecutarConsultarStock(string mensajeUsuario)
        {
            var prompt = await _promptService.GetByKeyAsync("consultar_stock");
            if (prompt == null) return "No se encontró el prompt para stock.";

            var nombre = ExtraerNombreProducto(mensajeUsuario);
            var rawJson = await _companyApiService.GetJsonFromEndpoint(prompt.Endpoint!.Url);
            using var doc = JsonDocument.Parse(rawJson);

            var coincidencias = doc.RootElement.EnumerateArray()
                .Where(p =>
                    p.TryGetProperty("product", out var prod) &&
                    prod.TryGetProperty("name", out var nameProp) &&
                    nameProp.GetString()?.ToLower().Contains(nombre.ToLower()) == true
                )
                .ToList();

            if (!coincidencias.Any())
                return $"❌ El producto \"{nombre}\" no se encuentra disponible.";

            var filtered = JsonSerializer.Serialize(coincidencias, new JsonSerializerOptions { WriteIndented = true });
            var entrada = prompt.PromptText.Replace("{nombre}", nombre) + "\n\n" + filtered;
            return await LlamarOpenAIAsync(entrada);
        }

        private async Task<string> EjecutarCompararProductos(string mensajeUsuario)
        {
            var prompt = await _promptService.GetByKeyAsync("comparar_productos");
            if (prompt == null) return "No se encontró el prompt de comparación.";

            var (n1, n2) = ExtraerDosProductos(mensajeUsuario);
            var jsonData = await _companyApiService.GetJsonFromEndpoint(prompt.Endpoint!.Url);
            var entrada = prompt.PromptText.Replace("{nombre1}", n1).Replace("{nombre2}", n2) + "\n\n" + jsonData;
            return await LlamarOpenAIAsync(entrada);
        }

        private async Task<string> EjecutarListarProductos()
        {
            var prompt = await _promptService.GetByKeyAsync("listar_productos_disponibles");
            if (prompt == null) return "No se encontró el prompt para listar productos.";

            var jsonData = await _companyApiService.GetJsonFromEndpoint(prompt.Endpoint!.Url);
            using var doc = JsonDocument.Parse(jsonData);
            var disponibles = doc.RootElement.EnumerateArray()
                .Where(p =>
                    p.TryGetProperty("quantity", out var qty) &&
                    qty.GetInt32() > 0
                )
                .ToList();

            if (!disponibles.Any())
                return "📦 Actualmente no hay productos disponibles.";

            var filteredJson = JsonSerializer.Serialize(disponibles, new JsonSerializerOptions { WriteIndented = true });
            var entrada = prompt.PromptText + "\n\n" + filteredJson;
            return await LlamarOpenAIAsync(entrada);
        }

        private async Task<string> LlamarOpenAIAsync(string input)
        {
            var client = _httpFactory.CreateClient("OpenAI");
            var payload = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "user", content = input }
                }
            };

//            var response = await client.PostAsJsonAsync("chat/completions", payload);
            var response = await PostWithRetryAsync(client, "chat/completions", payload);
            if (!response.IsSuccessStatusCode)
                return $"❌ Error al llamar a GPT. Código: {response.StatusCode}";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "Sin respuesta.";
        }

        private string ExtraerNombreProducto(string mensaje)
        {
            var match = Regex.Match(mensaje, @"stock (?:de|del|disponible de)?\s*(.+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "producto";
        }

        private (string, string) ExtraerDosProductos(string mensaje)
        {
            var sinSimbolos = Regex.Replace(mensaje, @"[^\w\s]", "");
            var palabras = sinSimbolos.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var posibles = palabras
                .SkipWhile(p => !p.Equals("comparar", StringComparison.OrdinalIgnoreCase))
                .Skip(1)
                .Take(10)
                .ToArray();

            if (posibles.Length < 2)
                return ("producto A", "producto B");

            var mitad = posibles.Length / 2;
            return (
                string.Join(" ", posibles.Take(mitad)),
                string.Join(" ", posibles.Skip(mitad))
            );
        }
    }
}