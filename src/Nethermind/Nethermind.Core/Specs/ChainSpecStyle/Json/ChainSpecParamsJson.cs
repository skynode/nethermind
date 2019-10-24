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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Nethermind.Core.Crypto;
using Nethermind.Core.Json;
using Nethermind.Dirichlet.Numerics;
using Utf8Json;

namespace Nethermind.Core.Specs.ChainSpecStyle.Json
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ChainSpecParamsJson
    {
        [DataMember(Name= "networkID")]
        [JsonFormatter(typeof(LongFormatter))]
        public long NetworkId { get; set; }
        
        [DataMember(Name= "registrar")]
        public Address EnsRegistrar { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? GasLimitBoundDivisor { get; set; }
        
        public UInt256? AccountStartNonce { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? MaximumExtraDataSize { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? MinGasLimit { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? ForkBlock { get; set; }
        
        public Keccak ForkCanonHash { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip150Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip152Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip160Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip161abcTransition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip161dTransition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip155Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? MaxCodeSize { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? MaxCodeSizeTransition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip140Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip211Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip214Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip658Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip145Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1014Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1052Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1108Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1283Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1283DisableTransition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1344Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip1884Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip2028Transition { get; set; }
        
        [JsonFormatter(typeof(NullableLongFormatter))]
        public long? Eip2200Transition { get; set; }
    }
}