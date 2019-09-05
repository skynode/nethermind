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
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using Nethermind.Core.Extensions;

namespace Nethermind.Benchmarks.Evm
{
    [MemoryDiagnoser]
    [CoreJob(baseline: true)]
    public class Endianness
    {
        [GlobalSetup]
        public void Setup()
        {
            a = (byte[])a_template.Clone();
            if (Current().ToHexString() != Improved().ToHexString())
            {
                throw new Exception($"{Current().ToHexString()} != {Improved().ToHexString()}");
            }
        }

        private byte[] a_template = new byte[]  {31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0};
        private byte[] a = new byte[]  {31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0};
        private byte[] mask = new byte[] {31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0};

        [Benchmark(Baseline = true)]
        public byte[] Current()
        {
            return Bytes.Reverse(a);
        }
        
        [Benchmark]
        public unsafe byte[] Improved()
        {
            fixed (byte* ptr_a = a, ptr_mask = mask)
            {
                Vector256<byte> aVec = Avx2.LoadVector256(ptr_a);
                Vector256<byte> maskVec = Avx2.LoadVector256(ptr_mask);
                Vector256<byte> resVec = Avx2.Shuffle(aVec, maskVec);
                resVec = Avx2.Permute4x64(resVec.As<byte, ulong>(), 0b01001110).As<ulong, byte>();
                Avx2.Store(ptr_a, resVec);
            }

            return a;
        }
    }
}