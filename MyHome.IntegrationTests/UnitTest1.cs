using HaKafkaNet;
using HaKafkaNet.Testing;
using Moq;
using System.Text.Json;

namespace MyHome.IntegrationTests
{
[Collection("HomeAutomation")]
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
            await _fixture.Helpers.SendState(_fixture.Helpers.Make<OnOff>(Binary_Sensor.BackHallCoatClosetContactOpening, OnOff.On));

            _fixture.API.Verify(api => api.TurnOn(Switch.BackHallLight, default), Times.Exactly(1));
        }
    }
}