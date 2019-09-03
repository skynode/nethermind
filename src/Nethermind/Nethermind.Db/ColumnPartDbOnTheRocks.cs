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
using Nethermind.Db.Config;
using Nethermind.Store;

namespace Nethermind.Db
{
    public class ColumnPartDbOnTheRocks : IDb, IDbWithSpan
    {
        private readonly ColumnDbOnTheRocks _db;
        private readonly DbParts.DbPart _dbPart;

        public ColumnPartDbOnTheRocks(ColumnDbOnTheRocks db, DbParts.DbPart dbPart, DbConfig config)
        {
            _db = db;
            db.CreateColumnFamily(dbPart, config);
            _dbPart = dbPart;
            Name = dbPart.ToString();
        }
        
        public void Dispose()
        {
            _db.DisposePart(_dbPart);
        }

        public string Name { get; }

        public byte[] this[byte[] key]
        {
            get => _db[key, _dbPart];
            set => _db[key, _dbPart] = value;
        }

        public byte[][] GetAll()
        {
            return _db.GetAll(_dbPart);
        }

        public void StartBatch()
        {
            _db.StartBatch();
        }

        public void CommitBatch()
        {
            _db.CommitBatch();
        }

        public void Remove(byte[] key)
        {
            _db.Remove(key, _dbPart);
        }

        public bool KeyExists(byte[] key)
        {
            return _db.KeyExists(key, _dbPart);
        }

        public Span<byte> GetSpan(byte[] key)
        {
            return _db.GetSpan(key, _dbPart);
        }

        public void DangerousReleaseMemory(in Span<byte> span)
        {
            _db.DangerousReleaseMemory(span);
        }
    }
}