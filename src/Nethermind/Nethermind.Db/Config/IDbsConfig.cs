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
using Nethermind.Store;
using RocksDbSharp;

namespace Nethermind.Db.Config
{
    public interface IDbsConfig : IConfig
    {
        IDbConfig GetPartConfig(DbParts.DbPart dbPart = null);
    }

    public interface IDbConfig
    {
        IDbPartConfig PartConfig { get; }
        string Key { get; }
    }

    public class DbConfig : IDbConfig
    {
        public DbConfig(DbParts.DbPart dbPart, IDbPartConfig partConfig) : this(dbPart.ToString(), partConfig)
        {
            DbPart = dbPart;
        }
        
        public DbConfig(string key, IDbPartConfig partConfig)
        {
            Key = key;
            PartConfig = partConfig;
        }

        public IDbPartConfig PartConfig { get; }
        public string Key { get; }
        public DbParts.DbPart DbPart { get; }
    }
}