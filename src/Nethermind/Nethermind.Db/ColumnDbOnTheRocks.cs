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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Nethermind.Db.Config;
using Nethermind.Logging;
using RocksDbSharp;

namespace Nethermind.Db
{
    public class ColumnDbOnTheRocks : DbOnTheRocks
    {
        private ConcurrentDictionary<DbParts.DbPart, ColumnFamilyHandle> ColumnFamilies { get; } = new ConcurrentDictionary<DbParts.DbPart, ColumnFamilyHandle>();
        private ConcurrentDictionary<ColumnFamilyHandle, DbParts.DbPart> Handles { get; } = new ConcurrentDictionary<ColumnFamilyHandle, DbParts.DbPart>();
        
        public ColumnDbOnTheRocks(string basePath, string name, DbPartConfig dbConfig, ILogManager logManager = null) 
            : base(basePath, name, dbConfig, logManager)
        {
        }

        public void CreateColumnFamily(DbParts.DbPart dbPart, DbConfig dbConfig)
        {
            ColumnFamilyOptions CreateColumnFamilyOptions(DbPartConfig dbPartConfig)
            {
                var options = new ColumnFamilyOptions();
                options.SetWriteBufferSize(dbPartConfig.WriteBufferSize);
                options.SetMaxWriteBufferNumber((int) dbPartConfig.WriteBufferNumber);
                return options;
            }
            
            var cf = ColumnFamilies.GetOrAdd(dbPart, part => Db.CreateColumnFamily(CreateColumnFamilyOptions(dbConfig.PartConfig), dbPart.ToString()));
            Handles.GetOrAdd(cf, dbPart);
        }
        
        protected override RocksDb OpenDb(DbOptions options, string fullPath) => RocksDb.Open(options, fullPath);
        
        protected override DbParts.DbPart GetDbPart(ColumnFamilyHandle cf) => Handles[cf];
        
        public byte[] this[byte[] key, DbParts.DbPart dbPart]
        {
            get => this[key, ColumnFamilies[dbPart]];
            set => this[key, ColumnFamilies[dbPart]] = value;
        }

        public byte[][] GetAll(DbParts.DbPart dbPart)
        {
            return GetAll(ColumnFamilies[dbPart]);
        }

        public void Remove(byte[] key, DbParts.DbPart dbPart)
        {
            Remove(key, ColumnFamilies[dbPart]);
        }

        public bool KeyExists(byte[] key, DbParts.DbPart dbPart)
        {
            return KeyExists(key, ColumnFamilies[dbPart]);
        }

        public Span<byte> GetSpan(byte[] key, DbParts.DbPart dbPart)
        {
            return GetSpan(key, ColumnFamilies[dbPart]);
        }

        public void DisposePart(DbParts.DbPart dbPart)
        {
            if (ColumnFamilies.TryGetValue(dbPart, out var cf))
            {
                ColumnFamilies.TryRemove(dbPart, out _);
                Handles.TryRemove(cf, out _);
            }

            if (ColumnFamilies.Count == 0)
            {
                Dispose();
            }
        }
    }
}