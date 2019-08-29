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

using Nethermind.Core.Extensions;

namespace Nethermind.Db.Config
{
    public class DbPartConfig : IDbPartConfig
    {
        public static readonly DbPartConfig Default8 = new DbPartConfig(8.MB(), 32.MB());
        public static readonly DbPartConfig Default16 = new DbPartConfig(16.MB(), 64.MB());
        public static readonly DbPartConfig Default = Default16;
        
        public DbPartConfig(
            ulong writeBufferSize,
            ulong blockCacheSize,
            bool cacheIndexAndFilterBlock = true,
            uint writeBufferNumber = 4)
        {
            BlockCacheSize = blockCacheSize;
            CacheIndexAndFilterBlocks = cacheIndexAndFilterBlock;
            WriteBufferSize = writeBufferSize;
            WriteBufferNumber = writeBufferNumber;
        }

        public ulong WriteBufferSize { get; }
        public uint WriteBufferNumber { get; }
        public ulong BlockCacheSize { get; }
        public bool CacheIndexAndFilterBlocks { get; }
    }
    
    // public class DbPart
}