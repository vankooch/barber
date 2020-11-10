namespace Barber.IoT.Api.Swagger
{
    using System.Collections.Generic;
    using Barber.OpenApi.Extensions.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Add vendor x-tagGroups
    /// </summary>
    public static class TagGroupExtensions
    {
        private const string AREA_ADMIN = "Administration";
        private const string AREA_MQTT = "MQTT";

        /// <summary>
        /// Add x-tagGroups
        /// </summary>
        public static void AddGroups(OpenApiDocument document, HttpRequest request)
        {
            var groupAdmin = new OpenApiXTagGroup(AREA_ADMIN)
            {
                Tags = new List<string>()
                    {
                        ContollerToTag(nameof(Controllers.Administration.DeviceController)),
                        ContollerToTag(nameof(Controllers.Administration.DeviceLockoutController)),
                        ContollerToTag(nameof(Controllers.Administration.DevicePasswordController)),
                        ContollerToTag(nameof(Controllers.Administration.DeviceActivityController)),
                    },
            };

            var groupMqtt = new OpenApiXTagGroup(AREA_MQTT)
            {
                Tags = new List<string>()
                    {
                        ContollerToTag(nameof(Controllers.Mqtt.ClientsController)),
                        ContollerToTag(nameof(Controllers.Mqtt.MessagesController)),
                        ContollerToTag(nameof(Controllers.Mqtt.SessionsController)),
                    },
            };

            var groups = new OpenApiXTagGroups();
            groups.List.Add(groupAdmin);
            groups.List.Add(groupMqtt);

            document.Extensions.Add("x-tagGroups", groups);
        }

        /// <summary>
        /// Convert Controller name to Tag by removing the 'Controller' Part
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string ContollerToTag(string name) => name.Replace("Controller", string.Empty);
    }
}
