﻿using System.IO;

namespace RPGTool.Save
{
    public class UShortLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadUInt16();
        }
    }

    public class UShortSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((ushort) data);
        }
    }

    public class UInt32Loader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadUInt32();
        }
    }

    public class UInt32Saver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((uint) data);
        }
    }

    public class ULongLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadUInt64();
        }
    }

    public class ULongSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((ulong) data);
        }
    }

    public class ShortLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadInt16();
        }
    }

    public class ShortSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((short) data);
        }
    }

    public class Int32Loader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadInt32();
        }
    }

    public class Int32Saver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((int) data);
        }
    }

    public class LongLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadInt64();
        }
    }

    public class LongSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((long) data);
        }
    }

    public class FloatLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadDouble();
        }
    }

    public class FloatSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((double) data);
        }
    }

    public class StringLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadString();
        }
    }

    public class StringSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((string) data);
        }
    }

    public class BooleanLoader : IDataLoader
    {
        public object Load(BinaryReader stream)
        {
            return stream.ReadBoolean();
        }
    }

    public class BooleanSaver : IDataSaver
    {
        public void Save(object data, BinaryWriter stream)
        {
            stream.Write((bool) data);
        }
    }
}