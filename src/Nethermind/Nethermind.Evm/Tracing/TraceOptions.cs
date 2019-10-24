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


using System.Runtime.Serialization;

namespace Nethermind.Evm.Tracing
{
    public class GethTraceOptions
    {
        [DataMember(Name = "disableStorage")]
        public bool DisableStorage { get; set; }
        
        [DataMember(Name = "disableMemory")]
        public bool DisableMemory { get; set; }
        
        [DataMember(Name = "disableStack")]
        public bool DisableStack { get; set; }
        
        [DataMember(Name = "tracer")]
        public string Tracer { get; set; }
        
        [DataMember(Name = "timeout")]
        public string Timeout { get; set; }
        
        public static readonly GethTraceOptions Default = new GethTraceOptions();
    }
}