﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace PointOfInterestSkill.Models.Foursquare
{
    public class VenuePage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
