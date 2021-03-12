using DotHome.Core.Tools;
using DotHome.RunningModel;
using DotHome.StandardBlocks.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace DotHome.Core.Services
{
    public class NotificationManager : INotificationSender
    {
        private IConfiguration configuration;
        private NotificationsStoreContext notificationsStoreContext;

        private string publicKey, privateKey, subject;

        public NotificationManager(IConfiguration configuration, NotificationsStoreContext notificationsStoreContext)
        {
            this.configuration = configuration;
            this.notificationsStoreContext = notificationsStoreContext;

            publicKey = configuration["VapidPublicKey"];
            privateKey = configuration["VapidPrivateKey"];
            subject = configuration["VapidSubject"];
        }

        public void Subscribe(NotificationSubscription subscription)
        {
            var oldSubscriptions = notificationsStoreContext.NotificationSubscriptions.Where(n => n.Url == subscription.Url && n.UserId == subscription.UserId);
            notificationsStoreContext.NotificationSubscriptions.RemoveRange(oldSubscriptions);
            notificationsStoreContext.NotificationSubscriptions.Add(subscription);
            notificationsStoreContext.SaveChanges();
        }

        public void Unsubscribe(NotificationSubscription subscription)
        {
            var oldSubscriptions = notificationsStoreContext.NotificationSubscriptions.Where(n => n.Url == subscription.Url && n.UserId == subscription.UserId);
            if (oldSubscriptions.Count() != 0)
            {
                notificationsStoreContext.NotificationSubscriptions.RemoveRange(oldSubscriptions);
                notificationsStoreContext.SaveChanges();
            }
        }

        public async void SendNotification(string message, string url, AuthenticatedBlock source)
        {
            foreach(NotificationSubscription subscription in notificationsStoreContext.NotificationSubscriptions.ToList().Where(n => source.AllowedUsers.Any(u => u.Name == n.UserId) ))
            {
                var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
                var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                var webPushClient = new WebPushClient(new System.Net.Http.HttpClientHandler());

                var options = new Dictionary<string, object>();
                options["vapidDetails"] = new VapidDetails(subject, publicKey, privateKey);
                options["headers"] = new Dictionary<string, object>() { ["Urgency"] = "high" };

                try
                {
                    var payload = JsonSerializer.Serialize(new{ message, url });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, options);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                }
            }
        }
    }
}
