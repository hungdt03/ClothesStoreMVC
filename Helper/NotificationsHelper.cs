using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebBanQuanAo.Helpers
{
    public static class NotificationsHelper
    {
        private const string NotificationsSessionKey = "Notifications";

        public static void AddNotification(HttpContext httpContext, string message, string title, NotificationType notificationType)
        {
            var notifications = GetNotifications(httpContext);
            notifications.Add(new Notification { Message = message, Title = title, NotificationType = notificationType });
            SaveNotifications(httpContext, notifications);
        }

        public static HtmlString RenderNotifications(HttpContext httpContext)
        {
            var notifications = GetNotifications(httpContext);
            var script = string.Empty;

            foreach (var notification in notifications)
            {
                script += $"toastr.{notification.NotificationType.ToString().ToLower()}('{notification.Message}', '{notification.Title}');";
            }

            ClearNotifications(httpContext);
            return new HtmlString($"<script>{script}</script>");
        }

        private static List<Notification> GetNotifications(HttpContext httpContext)
        {
            var session = httpContext.Session;
            var notificationsJson = session.GetString(NotificationsSessionKey);
            if (notificationsJson == null)
            {
                return new List<Notification>();
            }

            return JsonConvert.DeserializeObject<List<Notification>>(notificationsJson) ?? new List<Notification>();
        }

        private static void SaveNotifications(HttpContext httpContext, List<Notification> notifications)
        {
            var session = httpContext.Session;
            var notificationsJson = JsonConvert.SerializeObject(notifications);
            session.SetString(NotificationsSessionKey, notificationsJson);
        }

        private static void ClearNotifications(HttpContext httpContext)
        {
            var session = httpContext.Session;
            session.Remove(NotificationsSessionKey);
        }
    }

    public enum NotificationType
    {
        Success,
        Info,
        Warning,
        Error
    }

    public class Notification
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
