using System;
using System.Collections.Generic;
using System.IO;

namespace RPGTool.Save
{
    public class DataSaver
    {
        private static readonly Dictionary<string, IDataSaver> _dataSavers = new Dictionary<string, IDataSaver>();

        static DataSaver()
        {
            foreach (var type in typeof(DataLoader).Assembly.DefinedTypes)
                if (!type.IsAbstract && typeof(IDataSaver).IsAssignableFrom(type))
                    _dataSavers[type.Name.Substring(0, type.Name.Length - "Saver".Length).ToLower()] =
                        (IDataSaver) Activator.CreateInstance(type);
        }

        public static void Save<T>(T data, BinaryWriter stream)
        {
            var typeName = typeof(T).Name;
            if (typeof(T).IsEnum)
                _dataSavers["int"].Save(data, stream);
            else
                _dataSavers[typeof(T).Name.ToLower()].Save(data, stream);
        }
    }
}