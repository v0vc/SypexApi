// This file contains my intellectual property. Release of this file requires prior approval from me.
// 
// Copyright (c) 2015, v0v. All Rights Reserved

using System;
using System.Collections.Generic;

namespace SypexApi
{
    public static class StructConverter
    {
        #region Static Methods

        public static IEnumerable<object> Unpack(string fmt, byte[] bytes)
        {
            if (fmt.Length < 1)
            {
                throw new ArgumentException("Format string cannot be empty.");
            }

            fmt = fmt.Substring(1);

            var totalByteLength = 0;
            foreach (char c in fmt)
            {
                switch (c)
                {
                    case 'q':
                    case 'Q':
                    case 'd':
                        totalByteLength += 8;
                        break;
                    case 'i':
                    case 'I':
                    case 'l':
                    case 'L':
                        totalByteLength += 4;
                        break;
                    case 'h':
                    case 'H':
                        totalByteLength += 2;
                        break;
                    case 'b':
                    case 'B':
                    case 'x':
                        totalByteLength += 1;
                        break;
                    default:
                        throw new ArgumentException("Invalid character found in format string.");
                }
            }

            if (bytes.Length != totalByteLength)
            {
                throw new ArgumentException("The number of bytes provided does not match the total length of the format string.");
            }

            var byteArrayPosition = 0;
            var outputList = new List<object>();

            foreach (char c in fmt)
            {
                byte[] buf;
                switch (c)
                {
                    case 'q':
                        outputList.Add(BitConverter.ToInt64(bytes, byteArrayPosition));
                        byteArrayPosition += 8;
                        break;
                    case 'Q':
                        outputList.Add(BitConverter.ToUInt64(bytes, byteArrayPosition));
                        byteArrayPosition += 8;
                        break;
                    case 'l':
                        outputList.Add(BitConverter.ToInt32(bytes, byteArrayPosition));
                        byteArrayPosition += 4;
                        break;
                    case 'L':
                        outputList.Add(BitConverter.ToUInt32(bytes, byteArrayPosition));
                        byteArrayPosition += 4;
                        break;
                    case 'h':
                        outputList.Add(BitConverter.ToInt16(bytes, byteArrayPosition));
                        byteArrayPosition += 2;
                        break;
                    case 'H':
                        outputList.Add(BitConverter.ToUInt16(bytes, byteArrayPosition));
                        byteArrayPosition += 2;
                        break;
                    case 'b':
                        buf = new byte[1];
                        Array.Copy(bytes, byteArrayPosition, buf, 0, 1);
                        outputList.Add((sbyte)buf[0]);
                        byteArrayPosition++;
                        break;
                    case 'B':
                        buf = new byte[1];
                        Array.Copy(bytes, byteArrayPosition, buf, 0, 1);
                        outputList.Add(buf[0]);
                        byteArrayPosition++;
                        break;
                    case 'x':
                        byteArrayPosition++;
                        break;
                    default:
                        throw new ArgumentException("You should not be here.");
                }
            }
            return outputList.ToArray();
        }

        #endregion
    }
}
