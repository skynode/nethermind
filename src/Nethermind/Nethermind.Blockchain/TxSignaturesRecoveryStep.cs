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
using Nethermind.Core;
using Nethermind.Core.Crypto;

namespace Nethermind.Blockchain
{
    public class TxSignaturesRecoveryStep : IBlockDataRecoveryStep
    {
        private readonly IEthereumSigner _signer;

        public TxSignaturesRecoveryStep(IEthereumSigner signer)
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }
        
        public void RecoverData(Block block)
        {
            if (block.Transactions.Length == 0 || block.Transactions[0].SenderAddress != null)
            {
                return;
            }
            
            for (int i = 0; i < block.Transactions.Length; i++)
            {
                block.Transactions[i].SenderAddress = _signer.RecoverAddress(block.Transactions[i], block.Number);
            }
        }
    }
}