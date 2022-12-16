//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.IO;
using System.Threading;

namespace nanoFramework.Serialization.Helper
{
    internal class BitStream
    {
        public class Buffer
        {
            const int c_BufferSize = 512;

            internal Buffer _next;
            internal byte[] _data;
            internal int _length;
            //Bits available at last position.
            internal int _avail;

            internal Buffer()
            {
                _data = new byte[c_BufferSize];
                _length = 0;
                _next = null;
            }

            internal Buffer(
                byte[] data,
                int pos,
                int len)
            {
                _data = new byte[len];
                _length = len;
                _next = null;

                Array.Copy(data, pos, _data, 0, len);
            }

            internal Buffer(
                byte[] data,
                int pos,
                int len,
                int bitsInLastPos)
            {
                if (bitsInLastPos < 1 || bitsInLastPos > 8)
                {
                    throw new ArgumentException("bits");
                }

                _data = new byte[len];
                _length = len;
                _avail = bitsInLastPos;
                _next = null;

                Array.Copy(data, pos, _data, 0, len);
            }
        }

        private Buffer _first;
        private Buffer _current;
        private Buffer _last;
        private int _pos;
        private int _avail;

        private readonly bool _blockingRead;
        private bool _streamEnded;

        private readonly object _lock;

        public BitStream()
        {
            _first = new Buffer();
            _last = _first;
            _blockingRead = false;
            _streamEnded = false;
            _lock = new object();
            Rewind();
        }

        public BitStream(bool blockingRead) : this() => _blockingRead = blockingRead;

        public BitStream(
            byte[] data,
            int pos,
            int len) : this()
        {
            AppendChunk(data, pos, len * 8);
        }

        public void MarkStreamEnd()
        {
            lock (_lock)
            {
                _streamEnded = true;
                Monitor.Pulse(_lock);
            }
        }

        public void AppendChunk(
            byte[] data,
            int pos,
            int bitlen)
        {
            lock (_lock)
            {
                if (bitlen > 0)
                {
                    int len = bitlen / 8;
                    int bitsInLast = bitlen % 8;

                    if (bitsInLast == 0)
                    {
                        bitsInLast = 8;
                    }
                    else
                    {
                        len++;
                    }

                    Buffer next = new(
                        data,
                        pos,
                        len,
                        bitsInLast);

                    if (_last == null)
                    {
                        _first = _last = _current = next;
                        Rewind();
                    }
                    else
                    {
                        _last._next = next;
                        _last = next;
                    }

                    Monitor.Pulse(_lock);
                }
            }
        }

        public void Rewind()
        {
            lock (_lock)
            {
                _current = _first;
                _pos = -1;
                _avail = 0;
            }
        }

        public byte[] ToArray()
        {
            /*
             * WARNING: Buffer._avail is the number of bits written in the last byte of the buffer.
             * This function doesn't account for buffers whose contents don't end on a byte-boundary.
             * Under normal circumstances this isn't an issue because such a situation only occurs by
             * calling AppendChunk where (bitLen % 8 != 0) on a stream before calling ToArray().
             * AppendChunk is currently exclusively used by the profiling stream, which doesn't use ToArray()
             * so currently there is no problem.
             */

            byte[] res = null;

            lock (_lock)
            {
                for (int pass = 0; pass < 2; pass++)
                {
                    int tot = 0;
                    Buffer ptr = _first;

                    while (ptr != null)
                    {
                        if (pass == 1)
                        {
                            Array.Copy(ptr._data, 0, res, tot, ptr._length);
                        }

                        tot += ptr._length;
                        ptr = ptr._next;
                    }

                    if (pass == 0)
                    {
                        res = new byte[tot];
                    }
                }
            }

            return res;
        }

        public int BitsAvailable
        {
            get
            {
                int val;

                lock (_lock)
                {
                    Buffer ptr = _current;
                    val = 8 * (ptr._length - _pos) + _avail - 8;

                    while (ptr._next != null)
                    {
                        ptr = ptr._next;

                        val += 8 * (ptr._length - 1) + ptr._avail;
                    }
                }

                return val;
            }
        }

        public void WriteBits(
            uint val,
            int bits)
        {
            if (bits > 32)
            {
                throw new ArgumentException("Max number of bits per write is 32");
            }

            BinaryFormatter.WriteLine("OUTPUT: {0:X8} {1}", val & (0xFFFFFFFF >> (32 - bits)), bits);

            int pos = bits;

            lock (_lock)
            {
                while (bits > 0)
                {
                    while (_avail == 0)
                    {
                        _pos++;
                        _avail = 8;

                        if (_pos >= _current._data.Length)
                        {
                            //WriteBits will always try to fill the last bits of a buffer.
                            _current._avail = 8;
                            _current._next = new Buffer();

                            _current = _current._next;
                            _pos = 0;
                        }

                        if (_pos >= _current._length)
                        {
                            _current._length = _pos + 1;
                        }
                    }

                    int insert = Math.Min(bits, _avail);
                    uint mask = ((1U << insert) - 1U);

                    pos -= insert; _current._data[_pos] |= (byte)(((val >> pos) & mask) << (_avail - insert));
                    bits -= insert;
                    _avail -= insert;
                }

                if (_pos == _current._length - 1)
                {
                    _current._avail = 8 - _avail;
                }

                Monitor.Pulse(_lock);
            }
        }

        public uint ReadBits(int bits)
        {
            if (bits > 32)
            {
                throw new ArgumentException("Max number of bits per read is 32");
            }

            uint val = 0;
            int pos = bits;
            int bitsOrig = bits;

            lock (_lock)
            {
                while (bits > 0)
                {
                    while (_avail == 0)
                    {
                        _pos++;

                        while (_pos >= _current._length)
                        {
                            if (_current._next == null)
                            {
                                if (_blockingRead && !_streamEnded)   //Don't wait if stream has ended.
                                {
                                    Monitor.Wait(_lock);
                                }
                                else
                                {
                                    throw new EndOfStreamException();
                                }
                            }
                            else
                            {
                                _current = _current._next;
                                _pos = 0;
                            }
                        }

                        if (_pos < _current._length - 1)
                        {
                            _avail = 8;
                        }
                        else
                        {
                            _avail = _current._avail;
                        }
                    }

                    int insert = Math.Min(bits, _avail);
                    uint mask = ((1U << insert) - 1U);
                    int shift = _avail - insert;

                    if (_pos == _current._length - 1)
                    {
                        shift += 8 - _current._avail;
                    }

                    pos -= insert; val |= (((uint)_current._data[_pos] >> shift) & mask) << pos;
                    bits -= insert;
                    _avail -= insert;
                }
            }

            BinaryFormatter.WriteLine("INPUT: {0:X8}, {1}", val, bitsOrig);

            return val;
        }

        public void WriteArray(
            byte[] data,
            int pos,
            int len)
        {
            lock (_lock)
            {
                while (len-- > 0)
                {
                    WriteBits(data[pos++], 8);
                }
            }
        }

        public void ReadArray(
            byte[] data,
            int pos,
            int len)
        {
            lock (_lock)
            {
                while (len-- > 0)
                {
                    data[pos++] = (byte)ReadBits(8);
                }
            }
        }
    }
}
