using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RPGTool.Save
{
    public class UnityTypeSaverLoader
    {
        public class Vector2IntLoader : IDataLoader
        {
            public object Load(BinaryReader stream)
            {
                var vec = new Vector2Int()
                {
                    x = stream.ReadInt32(),
                    y = stream.ReadInt32()
                };
                return vec;
            }
        }

        public class Vector2IntSaver : IDataSaver
        {
            public void Save(object data, BinaryWriter stream)
            {
                var vec = (Vector2Int)data;
                stream.Write(vec.x);
                stream.Write(vec.y);
            }
        }

        public class Vector3IntLoader : IDataLoader
        {
            public object Load(BinaryReader stream)
            {
                var vec = new Vector3Int()
                {
                    x = stream.ReadInt32(),
                    y = stream.ReadInt32(),
                    z = stream.ReadInt32()
                };
                return vec;
            }
        }

        public class Vector3IntSaver : IDataSaver
        {
            public void Save(object data, BinaryWriter stream)
            {
                var vec = (Vector3Int)data;
                stream.Write(vec.x);
                stream.Write(vec.y);
                stream.Write(vec.z);
            }
        }

        public class Vector2Loader : IDataLoader
        {
            public object Load(BinaryReader stream)
            {
                var vec = new Vector2()
                {
                    x = (float)stream.ReadDouble(),
                    y = (float)stream.ReadDouble()
                };
                return vec;
            }
        }

        public class Vector2Saver : IDataSaver
        {
            public void Save(object data, BinaryWriter stream)
            {
                var vec = (Vector2)data;
                stream.Write((double)vec.x);
                stream.Write((double)vec.y);
            }
        }

        public class Vector3Loader : IDataLoader
        {
            public object Load(BinaryReader stream)
            {
                var vec = new Vector3()
                {
                    x = (float)stream.ReadDouble(),
                    y = (float)stream.ReadDouble(),
                    z = (float)stream.ReadDouble()
                };
                return vec;
            }
        }

        public class Vector3Saver : IDataSaver
        {
            public void Save(object data, BinaryWriter stream)
            {
                var vec = (Vector3)data;
                stream.Write((double)vec.x);
                stream.Write((double)vec.y);
                stream.Write((double)vec.z);
            }
        }
    }
}
