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
using System.Collections.Concurrent;
using Nethermind.Db.Config;
using Nethermind.Logging;
using Nethermind.Store;

using RocksDbSharp;

namespace Nethermind.Db
{   
    public class SeparatePartDbOnTheRocks : DbOnTheRocks, IDb, IDbWithSpan
    {
        private readonly DbParts.DbPart _dbPart;
        private static readonly ConcurrentDictionary<string, RocksDb> DbsByPath = new ConcurrentDictionary<string, RocksDb>();

        public SeparatePartDbOnTheRocks(string basePath, DbParts.DbPart dbPart, DbPartConfig dbConfig, ILogManager logManager = null)
            : base(basePath, dbPart.ToString(), dbConfig, logManager)
        {
            _dbPart = dbPart;
        }

        protected override RocksDb OpenDb(DbOptions options, string fullPath)
            => DbsByPath.GetOrAdd(fullPath, path => RocksDb.Open(options, path));

        protected override DbParts.DbPart GetDbPart(ColumnFamilyHandle cf) => _dbPart;

        public byte[] this[byte[] key]
        {
            get => base[key];
            set => base[key] = value;
        }

        public Span<byte> GetSpan(byte[] key) => base.GetSpan(key);

        public void Remove(byte[] key)
        {
            base.Remove(key);
        }

        public byte[][] GetAll() => base.GetAll();

        public bool KeyExists(byte[] key) => base.KeyExists(key);
    }
}