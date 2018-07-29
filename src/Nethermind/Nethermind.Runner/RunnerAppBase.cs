﻿/*
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
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Nethermind.Config;
using Nethermind.Core;
using Nethermind.Core.Logging;
using Nethermind.Core.Specs.ChainSpec;
using Nethermind.JsonRpc.Config;
using Nethermind.KeyStore.Config;
using Nethermind.Network;
using Nethermind.Network.Config;
using Nethermind.Runner.Config;
using Nethermind.Runner.Runners;

namespace Nethermind.Runner
{
    public abstract class RunnerAppBase
    {
        protected ILogger Logger;
        protected readonly IPrivateKeyProvider PrivateKeyProvider;
        private IJsonRpcRunner _jsonRpcRunner = NullRunner.Instance;
        private IEthereumRunner _ethereumRunner = NullRunner.Instance;

        protected RunnerAppBase(ILogger logger, IPrivateKeyProvider privateKeyProvider)
        {
            Logger = logger;
            PrivateKeyProvider = privateKeyProvider;
        }

        public void Run(string[] args)
        {
            (var app, var buildConfigProvider, var getDbBasePath) = BuildCommandLineApp();
            ManualResetEvent appClosed = new ManualResetEvent(false);
            app.OnExecute(async () =>
            {
                var configProvider = buildConfigProvider();
                var initParams = configProvider.GetConfig<InitConfig>();
                
                if (initParams.RemovingLogFilesEnabled)
                {
                    RemoveLogFiles();
                }

                Logger = new NLogLogger(initParams.LogFileName);

                var pathDbPath = getDbBasePath();
                if (!string.IsNullOrWhiteSpace(pathDbPath))
                {
                    var newDbPath = Path.Combine(pathDbPath, initParams.BaseDbPath);
                    Logger.Info($"Adding prefix to baseDbPath, new value: {newDbPath}, old value: {initParams.BaseDbPath}");
                    initParams.BaseDbPath = newDbPath;
                }

                Console.Title = initParams.LogFileName;

                var serializer = new UnforgivingJsonSerializer();
                Logger.Info($"Running Nethermind Runner, parameters:\n{serializer.Serialize(initParams, true)}\n");

                Task userCancelTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Enter 'e' to exit");
                    while (true)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.KeyChar == 'e')
                        {
                            break;
                        }
                    }
                });

                await StartRunners(configProvider);
                await userCancelTask;

                Console.WriteLine("Closing, please wait until all functions are stopped properly...");
                StopAsync().Wait();
                Console.WriteLine("All done, goodbye!");
                appClosed.Set();

                return 0;
            });

            app.Execute(args);
            appClosed.WaitOne();
        }

        protected async Task StartRunners(IConfigProvider configProvider)
        {
            try
            {
                var initParams = configProvider.GetConfig<InitConfig>();
                var logManager = new NLogManager(initParams.LogFileName);

                //discovering and setting local, remote ips for client machine
                var networkHelper = new NetworkHelper(Logger);
                var localHost = networkHelper.GetLocalIp()?.ToString() ?? "127.0.0.1";
                var networkConfig = configProvider.GetConfig<NetworkConfig>();
                networkConfig.MasterExternalIp = localHost;
                networkConfig.MasterHost = localHost;
                
                ChainSpecLoader chainSpecLoader = new ChainSpecLoader(new UnforgivingJsonSerializer());

                string path = initParams.ChainSpecPath;
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                }

                byte[] chainSpecData = File.ReadAllBytes(path);
                ChainSpec chainSpec = chainSpecLoader.Load(chainSpecData);

                var nodes = chainSpec.NetworkNodes.Select(nn => GetNode(nn, localHost)).ToArray();
                networkConfig.BootNodes = nodes;
                networkConfig.DbBasePath = initParams.BaseDbPath;

                _ethereumRunner = new EthereumRunner(configProvider, networkHelper, logManager);
                await _ethereumRunner.Start();

                if (initParams.JsonRpcEnabled)
                {
                    Bootstrap.Instance.ConfigProvider = configProvider;
                    Bootstrap.Instance.LogManager = logManager;
                    Bootstrap.Instance.BlockchainBridge = _ethereumRunner.BlockchainBridge;
                    Bootstrap.Instance.EthereumSigner = _ethereumRunner.EthereumSigner;

                    _jsonRpcRunner = new JsonRpcRunner(configProvider, Logger);
                    await _jsonRpcRunner.Start();
                }
                else
                {
                    if (Logger.IsInfoEnabled)
                    {
                        Logger.Info("Json RPC is disabled");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error while starting Nethermind.Runner", e);
                throw;
            }
        }

        protected abstract (CommandLineApplication, Func<IConfigProvider>, Func<string>) BuildCommandLineApp();

        protected async Task StopAsync()
        {
            if (_jsonRpcRunner != null)
            {
                await _jsonRpcRunner.StopAsync();
            }

            if (_ethereumRunner != null)
            {
                await _ethereumRunner.StopAsync();
            }
        }

        private ConfigNode GetNode(NetworkNode networkNode, string localHost)
        {
            var node = new ConfigNode
            {
                NodeId = networkNode.NodeId.PublicKey.ToString(false),
                Host = networkNode.Host == "127.0.0.1" ? localHost : networkNode.Host,
                Port = networkNode.Port,
                Description = networkNode.Description
            };
            return node;
        }

        private void RemoveLogFiles()
        {
            Console.WriteLine("Removing log files.");
            var files = Directory.GetFiles("logs");
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}