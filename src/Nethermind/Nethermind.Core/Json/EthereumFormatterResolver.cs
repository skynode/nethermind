//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Reflection;
using Nethermind.Core.Specs.ChainSpecStyle.Json;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Nethermind.Core.Json
{
    public class EthereumFormatterResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new EthereumFormatterResolver();

        private static readonly IJsonFormatterResolver[] Resolvers = {
            StandardResolver.AllowPrivateCamelCase
        };

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }
        
        public static IList<IJsonFormatter> Formatters { get; } = new List<IJsonFormatter>
        {
            new AddressFormatter(),
            new BigIntegerFormatter(),
            new BloomFormatter(),
            new ByteArrayFormatter(),
            new Bytes32Formatter(),
            new KeccakFormatter(),
            new LongFormatter(),
            new NullableBigIntegerFormatter(),
            new NullableLongFormatter(),
            new NullableUInt256Formatter(),
            new PublicKeyFormatter(),
            new UInt256Formatter(),
            new Utf8Json.Formatters.GenericDictionaryFormatter<long, ChainSpecJson.AuRaValidatorJson,
                Dictionary<long, ChainSpecJson.AuRaValidatorJson>>()
        };


        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> Formatter;

            static FormatterCache()
            {
                foreach (var item in Formatters)
                {
                    foreach (var implInterface in item.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        var ti = implInterface.GetTypeInfo();
                        if (ti.IsGenericType && ti.GenericTypeArguments[0] == typeof(T))
                        {
                            Formatter = (IJsonFormatter<T>) item;
                            return;
                        }
                    }
                }

                foreach (var item in Resolvers)
                {
                    var formatter = item.GetFormatter<T>();
                    if (formatter is null)
                    {
                        continue;
                    }

                    Formatter = formatter;
                    return;
                }
            }
        }

        public static void AddFormatter(IJsonFormatter formatter)
        {
            Formatters.Add(formatter);
        }
    }
}