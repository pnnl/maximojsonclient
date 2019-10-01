using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pnnl.Data.Maximo.JsonClient.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pnnl.Data.Maximo.JsonClient.Exceptions;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// A client capable of retrieving information from Maximo using JSON HTTP services.
    /// </summary>
    /// <seealso cref="Pnnl.Data.Maximo.JsonClient.IMaximoJsonClient" />
    public class MaximoJsonClient : IMaximoJsonClient
    {
        private JsonClientConfig _configuration;

        private static SemaphoreSlim _semaphoreSlim;
        private readonly IMemoryCache _cache;

        private JsonSerializerSettings _serializerSettings;
        private JsonSerializerSettings _deserializerSettings;

        /// <summary>
        /// Gets or sets the serializer settings.
        /// </summary>
        public JsonSerializerSettings SerializerSettings
        {
            get
            {
                return _serializerSettings;
            }
            set
            {
                _serializerSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the deserializer settings.
        /// </summary>
        public JsonSerializerSettings DeserializerSettings
        {
            get
            {
                return _deserializerSettings;
            }
            set
            {
                _deserializerSettings = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximoJsonClient"/> class.
        /// </summary>
        /// <param name="memoryCache">The cache to use for storing and retrieving items.</param>
        /// <param name="configuration">The configuration.</param>
        public MaximoJsonClient(IMemoryCache memoryCache, JsonClientConfig configuration)
        {
            _configuration = configuration;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _cache = memoryCache;

            _serializerSettings = new JsonSerializerSettings();
            _deserializerSettings = new JsonSerializerSettings();

            if (_configuration.SerializerContractResolverTypeName != null)
            {
                IContractResolver resolver;
                try
                {
                    var type = Type.GetType(_configuration.SerializerContractResolverTypeName);
                    resolver = Activator.CreateInstance(type) as IContractResolver;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Unable to activate serializer contract resolver.", ex);
                }

                _serializerSettings.ContractResolver = resolver ?? throw new ArgumentException($"Invalid value for {nameof(_configuration.SerializerContractResolverTypeName)}");
            }

            if (_configuration.DeserializerContractResolverTypeName != null)
            {
                IContractResolver resolver;
                try
                {
                    var type = Type.GetType(_configuration.DeserializerContractResolverTypeName);
                    resolver = Activator.CreateInstance(type) as IContractResolver;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Unable to activate deserializer contract resolver.", ex);
                }

                _deserializerSettings.ContractResolver = resolver ?? throw new ArgumentException($"Invalid value for {nameof(_configuration.DeserializerContractResolverTypeName)}");
            }
        }

        private string Serialize(object body)
        {
            return JsonConvert.SerializeObject(body, _serializerSettings);
        }

        private object Deserialize(string response)
        {
            return JsonConvert.DeserializeObject(response, _deserializerSettings);
        }

        private T Deserialize<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response, _deserializerSettings);
        }

        private bool TryParseJson<T>(string response, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(response, _deserializerSettings);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously sets the session token.
        /// </summary>
        /// <param name="action">An action that returns the token value to set.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the value of the session token.</returns>
        private async Task<string> SetSessionTokenAsync(Func<Task<string>> action, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Wait for the semaphore to become available
            await _semaphoreSlim.WaitAsync(cancellationToken);

            try
            {
                var key = GetCacheKey(identity);

                var value = await action();

                _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(_configuration.SessionLifetime));

                return value;
            }
            finally
            {
                // use Finally so that the semaphore is always released even if the code path faults
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Asynchronously gets a session token from the token cache or creates it if it does not exist.
        /// </summary>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the value of the session token.</returns>
        private async Task<string> GetOrCreateSessionTokenAsync(WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SetSessionTokenAsync(async () => {
                var key = GetCacheKey(identity);

                if (_cache.TryGetValue(key, out string cacheEntry))
                {
                    return cacheEntry;
                }

                return await CreateNewSession(identity, cancellationToken);
            }, identity, cancellationToken);
        }

        /// <summary>
        /// Gets the cache key name.
        /// </summary>
        /// <param name="identity">The identity of the user.</param>
        /// <returns>The name of the key.</returns>
        private string GetCacheKey(WindowsIdentity identity = null)
        {
            var cachePrefix = $"{this.GetType().FullName}-{_configuration.Name}";

            if (identity != null)
            {
                return $"{cachePrefix}-IDENTITY-{identity.Name}";
            }
            else if (!string.IsNullOrEmpty(_configuration.User))
            {
                return $"{cachePrefix}-USER-{_configuration.User}";
            }

            return $"{cachePrefix}-DEFAULT";
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        /// <param name="identity">The identity of the user.</param>
        /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
        private HttpClient CreateHttpClient(WindowsIdentity identity = null)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                PreAuthenticate = true,
                UseDefaultCredentials = true,
                UseCookies = false
            };

            if (identity == null && !string.IsNullOrEmpty(_configuration.User))
            {
                handler.UseDefaultCredentials = false;

                handler.Credentials = new NetworkCredential(_configuration.User, _configuration.Password, _configuration.Domain);
            }

            return new HttpClient(handler, false)
            {
                BaseAddress = new Uri(_configuration.BaseUri),
                Timeout = TimeSpan.FromSeconds(_configuration.RequestTimeSpan)
            };
        }

        /// <summary>
        /// Asynchronously retrieves the users session token from Maximo.
        /// </summary>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the value of the session token.</returns>
        /// <exception cref="AuthenticationException">Thrown when the session could not be created or the session can not be read from the response.</exception>
        private async Task<string> CreateNewSession(WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var loginRoute = "login";

            if (_configuration.UseLean)
            {
                loginRoute += "?lean=1";
            }

            var requestUri = new Uri(new Uri(_configuration.BaseUri), loginRoute);

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                using (var client = CreateHttpClient(identity))
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await response.Content.ReadAsStringAsync();

                        if (TryParseJson<MaximoException>(message, out var maximoException))
                        {
                            ThrowNewMaximoException(maximoException);
                        }

                        throw new AuthenticationException($"Unable to create a new session. HTTP Code {response.StatusCode}: {message}");
                    }

                    if (TryGetSessionToken(response.Headers, out var sessionId))
                    {
                        return sessionId;
                    }
                    else
                    {
                        throw new AuthenticationException($"Unable to read session Id from response headers.");
                    }
                }
            }
        }

        /// <summary>
        /// Tries the get session token.
        /// </summary>
        /// <param name="headers">The headers from the server response.</param>
        /// <param name="value">The value of the session token.</param>
        /// <returns><see langword="true" /> if a token was found, <see langword="false" /> otherwise.</returns>
        private bool TryGetSessionToken(HttpResponseHeaders headers, out string value)
        {
            value = null;

            if (headers.Any(x => x.Key == "Set-Cookie"))
            {
                var sessionId = headers
                    .First(x => x.Key == "Set-Cookie").Value
                    .ToList()
                    .FirstOrDefault(x => x.Contains("JSESSION"));

                if (sessionId != null)
                {
                    value = sessionId.Split(';')[0];

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        public async Task<string> SendAndGetStringResponseAsync(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (identity != null)
            {
                return await WindowsIdentity.RunImpersonated(identity.AccessToken,
                    async () => await ExecuteRequest(verb, uri, body, headers, identity, cancellationToken));
            }

            return await ExecuteRequest(verb, uri, body, headers, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        public async Task<object> SendAndGetObjectResponseAsync(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await SendAndGetStringResponseAsync(verb, uri, body, headers, identity, cancellationToken);

            return Deserialize(response);
        }

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        public async Task<T> SendAndGetTypedObjectResponseAsync<T>(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await SendAndGetStringResponseAsync(verb, uri, body, headers, identity, cancellationToken);

            return Deserialize<T>(response);
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the response from the server.</returns>
        /// <exception cref="AuthenticationException">Thrown when the server requires authorization to process the request.</exception>
        /// <exception cref="UnauthorizedException">Thrown when the user making the request is forbidden from making the request.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the server responds with an unexpected response type.</exception>
        private async Task<string> ExecuteRequest(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sessionToken = await GetOrCreateSessionTokenAsync(identity, cancellationToken);

            using (var request = new HttpRequestMessage(verb, uri))
            {
                request.Headers.Add("Cookie", sessionToken);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (body != null)
                {
                    var bodyString = body is string ? body.ToString() : Serialize(body);

                    request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");
                }

                using (var client = CreateHttpClient(identity))
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await response.Content.ReadAsStringAsync();

                        if (TryParseJson<MaximoException>(message, out var maximoException))
                        {
                            ThrowNewMaximoException(maximoException);
                        }

                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.Unauthorized:
                                throw new AuthenticationException($"HTTP Code {response.StatusCode}: {message}");
                            case HttpStatusCode.Forbidden:
                                throw new UnauthorizedException($"HTTP Code {response.StatusCode}: {message}");
                            case HttpStatusCode.InternalServerError:
                                throw new MaximoOfflineException($"HTTP Code {response.StatusCode}: {message}");
                            default:
                                throw new InvalidOperationException($"HTTP Code {response.StatusCode}: {message}");
                        }
                    }

                    // Check for new session header
                    if (TryGetSessionToken(response.Headers, out var value))
                    {
                        await SetSessionTokenAsync(() => { return Task.FromResult(value); }, identity, cancellationToken);
                    }

                    var data = await response.Content.ReadAsStringAsync();

                    return data;
                }
            }
        }

        /// <summary>
        /// Evaluates a MaximoException object for specific reason codes and throws the associated Exception
        /// </summary>
        /// <param name="maximoException"></param>
        private static void ThrowNewMaximoException(MaximoException maximoException)
        {
            switch (maximoException.Error.reasonCode)
            {
                case "BMXAA3851E":
                    throw new MaximoOfflineException($"{maximoException.Error.message}");
                default:
                    throw new InvalidOperationException($"{maximoException.Error.message}");
            }
        }
    }
}
