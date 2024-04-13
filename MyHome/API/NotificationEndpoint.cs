// using FastEndpoints;
// using HaKafkaNet;

// namespace MyHome;

// public class PostNotificationRequest
// {
//     public string? Id { get; set; }
//     public string? Title { get; set; }
//     public string? Message { get; set; }
//     public string? UpdateType { get; set; }
// }


// public class NotificationEndpoint : Endpoint<PostNotificationRequest, EmptyResponse>
// {
//     readonly IHaServices _services;
//     readonly INotificationService _notificationService;

//     public NotificationEndpoint(IHaServices haServices, INotificationService notificationService)
//     {
//         _services = haServices;
//         _notificationService = notificationService;
//     }

//     public override void Configure()
//     {
//         Post("api/notification");
//         AllowAnonymous();
//     }

//     public override  Task<EmptyResponse> ExecuteAsync(PostNotificationRequest req, CancellationToken ct)
//     {
//         //await _services.Api.NotifyGroupOrDevice(NotificationGroups.LeonardPhone, $"Title: {req.Title}, Message: {req.Message}, ID: {req.Id}, Update Type: {req.UpdateType}");
//         if (req.UpdateType == "removed" && req.Id is not null)
//         {
//             _notificationService.Clear(new NotificationId(req.Id));
//         }
//         return Task.FromResult(Response);
//     }
// }
