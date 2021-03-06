﻿namespace Barber.OpenApi.Settings.Types
{
    public class BaseModel
    {
        public BaseModel()
        {
        }

        /// <summary>
        /// Default value
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Typescript type used
        /// </summary>
        public string Type { get; set; }
    }
}
