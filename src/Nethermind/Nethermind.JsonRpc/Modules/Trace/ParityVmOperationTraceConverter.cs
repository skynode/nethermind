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
using Nethermind.Core.Extensions;
using Nethermind.Evm.Tracing;
using Utf8Json;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityVmOperationTraceConverter : IJsonFormatter<ParityVmOperationTrace>
    {
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
        public void Serialize(ref JsonWriter writer, ParityVmOperationTrace value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();

            writer.WriteProperty("cost", value.Cost, formatterResolver);
            writer.WritePropertyName("ex");
            writer.WriteBeginObject();
            writer.WritePropertyName("mem");
            if (value.Memory != null)
            {
                writer.WriteBeginObject();
                writer.WritePropertyName("data");
                writer.WriteString(value.Memory.Data.ToHexString(true, false));
                writer.WritePropertyName("off");
                writer.WriteInt64(value.Memory.Offset);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }

            writer.WritePropertyName("push");
            if (value.Push != null)
            {
                writer.WriteBeginArray();
                for (int i = 0; i < value.Push.Length; i++)
                {
                    writer.WriteString(value.Push[i].ToHexString(true, true));
                }

                writer.WriteEndArray();
            }
            else
            {
                writer.WriteNull();
            }

            writer.WritePropertyName("store");
            if (value.Store != null)
            {
                writer.WriteBeginObject();
                writer.WritePropertyName("key");
                writer.WriteString(value.Store.Key.ToHexString(true, true));
                writer.WritePropertyName("val");
                writer.WriteString(value.Store.Value.ToHexString(true, true));
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }

            writer.WriteProperty("used", value.Used, formatterResolver);
            writer.WriteEndObject();

            writer.WriteProperty("pc", value.Pc, formatterResolver);
            writer.WriteProperty("sub", value.Sub, formatterResolver);
            writer.WriteEndObject();
        }

        public ParityVmOperationTrace Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}