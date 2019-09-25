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

namespace Nethermind.JsonRpc.Modules.Eth
{
    public class SyncingResultConverter : JsonConverter<SyncingResult>
    {
        public override SyncingResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, SyncingResult value, JsonSerializerOptions options)
        {
            if (!value.IsSyncing)
            {
                writer.WriteBooleanValue(false);
                return;
            }

            writer.WriteStartObject();
            writer.WriteNumber("startingBlock", value.StartingBlock);
            writer.WriteNumber("currentBlock", value.CurrentBlock);
            writer.WriteNumber("highestBlock", value.HighestBlock);
            writer.WriteEndObject();
        }
    }
}