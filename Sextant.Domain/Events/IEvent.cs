// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Sextant.Domain.Events
{
    public interface IEvent
    {
        string Event { get; }
        Dictionary<string, object> Payload { get; }
    }

    public static class PayloadExtensions
    {
        public static int? GetIntValue(this Dictionary<string, object> payload, string key)
        {
            object val;
            payload.TryGetValue(key, out val);
            int intVal;
            int? returnVal = null;

            if (val != null && Int32.TryParse(val.ToString(), out intVal)) {
                returnVal = intVal;
            }

            return returnVal;
        }
    }
}
