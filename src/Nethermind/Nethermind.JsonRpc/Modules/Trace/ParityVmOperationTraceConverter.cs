/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core.Extensions;
using Nethermind.Evm.Tracing;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityVmOperationTraceConverter : JsonConverter<ParityVmOperationTrace>
    {
        private readonly ParityVmTraceConverter _vmTraceConverter = new ParityVmTraceConverter();
        
        //{
//  "cost": 0.0,
//            "ex": {
//                "mem": null,
//                "push": [],
//                "store": null,
//                "used": 16961.0
//            },
//            "pc": 526.0,
//            "sub": null
//        }
        public override ParityVmOperationTrace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ParityVmOperationTrace value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("cost");
            writer.WriteNumberValue(value.Cost);
            writer.WritePropertyName("ex");
            writer.WriteStartObject();
            writer.WritePropertyName("mem");
            if (value.Memory != null)
            {
                writer.WriteStartObject();
                writer.WriteString("data", value.Memory.Data.ToHexString(true, false));
                writer.WriteNumber("off", value.Memory.Offset);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WritePropertyName("push");
            if (value.Push != null)
            {
                writer.WriteStartArray();
                for (int i = 0; i < value.Push.Length; i++)
                {
                    writer.WriteStringValue(value.Push[i].ToHexString(true, true));
                }

                writer.WriteEndArray();
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WritePropertyName("store");
            if (value.Store != null)
            {
                writer.WriteStartObject();
                writer.WriteString("key", value.Store.Key.ToHexString(true, true));
                writer.WriteString("val", value.Store.Value.ToHexString(true, true));
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WriteNumber("used", value.Used);
            writer.WriteEndObject();

            writer.WriteNumber("pc", value.Pc);
            writer.WritePropertyName("sub");
            _vmTraceConverter.Write(writer, value.Sub, options);

            writer.WriteEndObject();
        }
    }
}