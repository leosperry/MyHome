using HaKafkaNet;
using KafkaFlow;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Text.Json;

namespace MyHome.IntegrationTests
{
    public class HaKafkaNetFixture : WebApplicationFactory<Program>
    {
        public Mock<IHaApiProvider> API { get; } = new Mock<IHaApiProvider>();

        private Mock<IMessageContext> _context = new();

        MemoryDistributedCache _cache;

        public HaKafkaNetFixture()
        {
            Mock<IConsumerContext> consumerContext = new();
            _context.SetupGet(context => context.ConsumerContext).Returns(consumerContext.Object);

            IOptions<MemoryDistributedCacheOptions> options = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new MemoryDistributedCache(options);

            API.Setup(api => api.GetEntity(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                //(HttpResponseMessage response, HaEntityState? entityState)
                .ReturnsAsync((new HttpResponseMessage()
                { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.OK}, new HaEntityState()
                { EntityId = "", State = "unknown", Attributes = JsonSerializer.SerializeToElement("{}")}));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => { 
                services.RemoveAll<IHaApiProvider>();
                services.TryAddSingleton<IHaApiProvider>(API.Object);
                services.RemoveAll<IDistributedCache>();
                services.AddSingleton<IDistributedCache>(_cache);
                services.AddSingleton<IMessageHandler<HaEntityState>, HaStateHandler>();
            });
        }

        public async Task SendState(HaEntityState state)
        {
            var handler = Services.GetRequiredService<IMessageHandler<HaEntityState>>();
            await handler.Handle(_context.Object, state);
        }

        public async Task SendState<Tstate>(HaEntityState<Tstate, JsonElement> state)
        {
            var handler = Services.GetRequiredService<IMessageHandler<HaEntityState>>();
            await handler.Handle(_context.Object, Convert(state));
        }

        public async Task SendState<Tstate, Tatt>(HaEntityState<Tstate, Tatt> state)
        {
            if (state.Attributes is null)
            {
                throw new ArgumentException("state.Attribute cannot be null", nameof(state));
            }
            var handler = Services.GetRequiredService<IMessageHandler<HaEntityState>>();
            await handler.Handle(_context.Object, Convert(state));
        }

        private HaEntityState Convert(object state)
        {
            return JsonSerializer.Deserialize<HaEntityState>(JsonSerializer.Serialize(state))!;
        }

    }
}
