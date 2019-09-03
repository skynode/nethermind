/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General License for more details.
 *
 * You should have received a copy of the GNU Lesser General License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using Nethermind.Config;
using Nethermind.Logging;
using Nethermind.Store;
using RocksDbSharp;

namespace Nethermind.Db.Config
{
    public interface IDbsConfig : IConfig
    {
        IDictionary<DbParts.DbPart, DbConfig> Parts { get; set; }
        IEnumerable<DbConfig> Dbs { get; set; }
        DbPartConfig Default { get; set; }
        DbConfig GetPartConfig(DbParts.DbPart dbPart);
        (DbConfig Config, bool MultiPartDb) GetDbConfig(DbConfig partConfig);
        void SetNewDefaultBasePath(string defaultBasePath, ILogger logger);
    }

//    public interface IDbConfig
//    {
//        IDbPartConfig PartConfig { get; }
//        string Key { get; }
//    }

    public class DbConfig
    {
        public DbConfig(DbParts.DbPart dbPart, DbPartConfig partConfig) : this(dbPart.ToString(), partConfig)
        {
            DbPart = dbPart;
        }
        
        public DbConfig(string key, DbPartConfig partConfig)
        {
            Key = key;
            PartConfig = partConfig;
        }

        public DbPartConfig PartConfig { get; }
        public string Key { get; }
        public DbParts.DbPart DbPart { get; }
    }
}