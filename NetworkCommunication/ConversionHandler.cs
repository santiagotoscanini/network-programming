﻿using System;
using System.Text;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class ConversionHandler: IConversionHandler
    {
        public byte[] ConvertStringToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public string ConvertBytesToString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public byte[] ConvertIntToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public int ConvertBytesToInt(byte[] value)
        {
            return BitConverter.ToInt32(value);
        }

        public byte[] ConvertLongToBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public long ConvertBytesToLong(byte[] value)
        {
            return BitConverter.ToInt64(value);
        }
    }
}
