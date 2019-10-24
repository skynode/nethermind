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
using Nethermind.JsonRpc.Modules.Trace;
using Utf8Json;

namespace Nethermind.JsonRpc.Modules.Eth
{
    public class SyncingResultFormatter : IJsonFormatter<SyncingResult>
    {
        public void Serialize(ref JsonWriter writer, SyncingResult value, IJsonFormatterResolver formatterResolver)
        {
            if (!value.IsSyncing)
            {
                writer.WriteBoolean(false);
                return;
            }

            writer.WriteBeginObject();
            writer.WriteProperty("startingBlock", value.StartingBlock, formatterResolver);
            writer.WriteProperty("currentBlock", value.CurrentBlock, formatterResolver);
            writer.WriteProperty("highestBlock", value.HighestBlock, formatterResolver);
            writer.WriteEndObject();
        }

        public SyncingResult Deserialize(ref Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}