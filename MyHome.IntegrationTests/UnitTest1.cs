using HaKafkaNet;
using Moq;
using System.Text.Json;

namespace MyHome.IntegrationTests
{
    public class UnitTest1 : IClassFixture<HaKafkaNetFixture>
    {
        private readonly HaKafkaNetFixture _fixture;

        public UnitTest1(HaKafkaNetFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task Test1()
        {
            await _fixture.SendState(new HaEntityState<OnOff, object>()
            {
                EntityId = Binary_Sensor.BackHallCoatClosetContactOpening,
                State = OnOff.On,
                Attributes = new { },
            });

            _fixture.API.Verify(api => api.CallService("light", "turn_on", It.IsAny<object>(), It.IsAny<CancellationToken>()));
        }
    }
}