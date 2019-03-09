﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Save
{
    public class DataLoader
    {
        private static readonly Dictionary<string, IDataLoader> _dataLoaders = new Dictionary<string, IDataLoader>();

        static DataLoader()
        {
            foreach (var type in typeof(DataLoader).Assembly.DefinedTypes)
                if (!type.IsAbstract && typeof(IDataLoader).IsAssignableFrom(type))
                    _dataLoaders[type.Name.Substring(0, type.Name.Length - "Loader".Length).ToLower()] = (IDataLoader) Activator.CreateInstance(type);
        }

        public static T Load<T>(BinaryReader stream)
        {
            if (typeof(T).IsEnum)
                return (T)_dataLoaders["int"].Load(stream);

            return (T) _dataLoaders[typeof(T).Name.ToLower()].Load(stream);
        }
    }
}