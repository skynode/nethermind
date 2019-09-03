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
using Nethermind.Db.Config;
using Nethermind.Logging;
using Nethermind.Store;

namespace Nethermind.Db
{
    public abstract class RocksDbProviderBase : IDisposable
    {
        private readonly IDbsConfig _dbsConfig;
        private readonly ILogManager _logManager;
        private readonly ConcurrentDictionary<string, DbOnTheRocks> _rocksDbs = new ConcurrentDictionary<string, DbOnTheRocks>();

        protected RocksDbProviderBase(IDbsConfig dbsConfig, ILogManager logManager)
        {
            _dbsConfig = dbsConfig;
            _logManager = logManager;
        }
        
        private IDb CreateColumnDb(DbParts.DbPart dbPart, DbConfig config)
        {
            var db = _rocksDbs.GetOrAdd(config.Key, key => new ColumnDbOnTheRocks(GetBasePath(config), config.Key, config.PartConfig, _logManager));
            return new ColumnPartDbOnTheRocks((ColumnDbOnTheRocks) db,  dbPart, config);
        }

        private IDb CreateSeparateDb(DbParts.DbPart dbPart, DbConfig config) => 
            (IDb) _rocksDbs.GetOrAdd(config.Key, key => new SeparatePartDbOnTheRocks(GetBasePath(config), dbPart, config.PartConfig, _logManager));

        private string GetBasePath(DbConfig config)
        {
            return config.PartConfig.BasePath ?? _dbsConfig.Default.BasePath;
        }

        protected IDb CreatePartDb(DbParts.DbPart dbPart)
        {
            var partConfig = _dbsConfig.GetPartConfig(dbPart);
            var (config, multiPart) = _dbsConfig.GetDbConfig(partConfig);
            return multiPart ? CreateColumnDb(dbPart, config) : CreateSeparateDb(dbPart, config);
        }

        public virtual void Dispose()
        {
            _rocksDbs.Clear();
        }
    }
}