﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// Config
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthConfig
    {
        /// <summary>
        /// AppInsightsInstrumentationKey
        /// </summary>
        public string AppInsightsInstrumentationKey { get; set; } = string.Empty;

        /// <summary>
        /// ArmsContainerSas
        /// </summary>
        public string ArmsContainerSas { get; set; } = string.Empty;

        /// <summary>
        /// ArmsContainerUri
        /// </summary>
        public Uri? ArmsContainerUri { get; set; }

        /// <summary>
        /// Auth section
        /// </summary>
        public IngrowthAuthConfig Auth { get; set; } = new IngrowthAuthConfig();

        /// <summary>
        /// Competency section
        /// </summary>
        public IngrowthBusConfig Competency { get; set; } = new IngrowthBusConfig();

        /// <summary>
        /// Identity section
        /// </summary>
        public IngrowthIdentityConfig Identity { get; set; } = new IngrowthIdentityConfig();

        /// <summary>
        /// JobProfile section
        /// </summary>
        public IngrowthBusConfig JobProfile { get; set; } = new IngrowthBusConfig();

        /// <summary>
        /// Spa section
        /// </summary>
        public IngrowthSpaConfig Spa { get; set; } = new IngrowthSpaConfig();

        /// <summary>
        /// Shared section
        /// </summary>
        public IngrowthSharedConfig Shared { get; set; } = new IngrowthSharedConfig();
    }
}
