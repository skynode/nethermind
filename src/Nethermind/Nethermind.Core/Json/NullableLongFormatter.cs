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

using Utf8Json;

namespace Nethermind.Core.Json
{
    public class NullableLongFormatter : IJsonFormatter<long?>
    {
        private readonly LongFormatter _longFormatter;
        
        public NullableLongFormatter() : this(NumberConversion.Hex)
        {
        }

        public NullableLongFormatter(NumberConversion conversion)
        {
            _longFormatter = new LongFormatter(conversion);
        }

        public void Serialize(ref JsonWriter writer, long? value, IJsonFormatterResolver formatterResolver)
        {
            if (!value.HasValue)
            {
                writer.WriteNull();
                return;
            }
            
            _longFormatter.Serialize(ref writer, value.Value, formatterResolver);
        }

        public long? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            return _longFormatter.Deserialize(ref reader, formatterResolver);
        }
    }
}