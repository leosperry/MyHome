﻿using HaKafkaNet;
using HaKafkaNet.Tests;
using Moq;
using System.Text.Json;

namespace MyHome.Tests;
public class KazulAlertsTests
{
    Mock<IUpdatingEntityProvider> startup = new();

    [Fact]
    public async Task WhenBatteryReports_andBatteryLevelLow_ShouldReport()
    {
        // Given

        
        Mock<IHaEntityProvider> entities = new();
        Mock<INotificationService> notify = new();
        var called = false;
        NotificationSender sender = (message, title, id) =>
        {
            called = true;
            return Task.FromResult<NotificationId>(new(""));
        };
        notify.Setup(n => n.CreateNotificationSender(It.IsAny<IEnumerable<INotificationChannel>>(), It.IsAny<IEnumerable<INoTextNotificationChannel>>()))
            .Returns(sender);
        
        notify.Setup(n => n.CreateInformationalSender()).Returns(sender);
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP_BATTERY, "19.9");
        await sut.Execute(fakeState, default);
    
        // Then
        Assert.True(called);
    }

    [Fact]
    public async Task WhenBatteryReports_andBatteryLevelOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP_BATTERY, "50.0");
        await sut.Execute(fakeState, default);
    
        // Then
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }

    [Fact]
    public async Task WhenTempReports_andTempLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();

        var called = false;
        NotificationSender sender = (message, title, id) =>
        {
            called = true;
            return Task.FromResult<NotificationId>(new(""));
        };
        notify.Setup(n => n.CreateNotificationSender(It.IsAny<IEnumerable<INotificationChannel>>(), It.IsAny<IEnumerable<INoTextNotificationChannel>>()))
            .Returns(sender);
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP, "64.9");
        await sut.Execute(fakeState, default);
    
        // Then
        Assert.True(called);
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenTempReports_andTempOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();

        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.TEMP, "66.0");
        await sut.Execute(fakeState, default);
    
        // Then
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOn_andPowerLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.CERAMIC_SWITCH, OnOff.On));

        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();

        var called = false;
        NotificationSender sender = (message, title, id) =>
        {
            called = true;
            return Task.FromResult<NotificationId>(new(""));
        };

        notify.Setup(n => n.CreateNotificationSender(It.IsAny<IEnumerable<INotificationChannel>>(), It.IsAny<IEnumerable<INoTextNotificationChannel>>()))
            .Returns(sender);
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, default));   
        await Task.Delay(500);
        Assert.True(called);
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), default));
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOff_ShouldNotNotify()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.CERAMIC_SWITCH, OnOff.Off));
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, default));
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }

    [Fact]
    public async Task WhenCeramicReports_andSwitchOn_andPowerOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.CERAMIC_SWITCH, OnOff.On));
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.CERAMIC_POWER, "75.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.CERAMIC_SWITCH, It.IsAny<CancellationToken>()));
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOn_andPowerLow_ShouldReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.HALOGEN_SWITCH, OnOff.On));
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();

        var called = false;
        NotificationSender sender = (message, title, id) =>
        {
            called = true;
            return Task.FromResult<NotificationId>(new(""));
        };
        notify.Setup(n => n.CreateNotificationSender(It.IsAny<IEnumerable<INotificationChannel>>(), It.IsAny<IEnumerable<INoTextNotificationChannel>>()))
            .Returns(sender);

    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.HALOGEN_SWITCH, It.IsAny<CancellationToken>()));
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), default));
        Assert.True(called);
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOn_andPowerOk_ShouldNotReport()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.HALOGEN_SWITCH, OnOff.On));
        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "75.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, default));
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }

    [Fact]
    public async Task WhenHalogenReports_andSwitchOff_ShouldNotNotify()
    {
        // Given
        Mock<IHaEntityProvider> entities = new();
        entities.Setup(e => e.GetEntity<HaEntityState<OnOff,JsonElement>>(KazulAlerts.HALOGEN_SWITCH, default))
            .ReturnsAsync(TestHelpers.GetEntity<OnOff,JsonElement>(KazulAlerts.HALOGEN_SWITCH, OnOff.Off));

        Mock<IHaApiProvider> api = new();
        Mock<INotificationService> notify = new();
    
        KazulAlerts sut = new KazulAlerts(entities.Object, notify.Object, startup.Object);

        // When
        var fakeState = TestHelpers.GetStateChange(KazulAlerts.HALOGEN_POWER, "65.0");
        await sut.Execute(fakeState, default);
    
        // Then
        entities.Verify(e => e.GetEntity<HaEntityState<OnOff, JsonElement>>(KazulAlerts.HALOGEN_SWITCH, default));
        //api.Verify(a => a.NotifyGroupOrDevice(NotificationGroups.Critical, It.IsAny<string>(), null, default), Times.Never);
    }
}
