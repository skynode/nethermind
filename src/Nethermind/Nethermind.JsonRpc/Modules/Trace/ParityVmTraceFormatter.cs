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
    public class ParityVmTraceFormatter : IJsonFormatter<ParityVmTrace>
    {
        public void Serialize(ref JsonWriter writer, ParityVmTrace value, IJsonFormatterResolver formatterResolver)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteBeginObject();
            writer.WriteProperty("code", value.Code ?? Bytes.Empty, formatterResolver);
            writer.WriteProperty("ops", value.Operations, formatterResolver, false);
            writer.WriteEndObject();
        }

        public ParityVmTrace Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}