using HaKafkaNet;
using HaKafkaNet.Testing;
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
            await _fixture.Services.SendState(TestHelper.Make<OnOff>(Binary_Sensor.BackHallCoatClosetContactOpening, OnOff.On));
            //await _fixture.Services.SendState(state);

            await Task.Delay(300);

            _fixture.API.Verify(api => api.TurnOn(Switch.BackHallLight, default));
        }
    }
}