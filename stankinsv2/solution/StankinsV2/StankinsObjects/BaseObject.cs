﻿using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public abstract class BaseObject : IBaseObject
    {
        //TODO: maybe reflection to get properties values from dataNeeded?
        public BaseObject(CtorDictionary dataNeeded)
        {
            Version = new Version(1, 0, 0, 0);
            //TODO: read this from somewhere
            StoringDataBetweenCalls = new Dictionary<string, object>();
            this.dataNeeded = dataNeeded;
        }
        protected readonly CtorDictionary dataNeeded;
        public string Name { get ; set ; }
        public IDictionary<string, object> StoringDataBetweenCalls { get ; set ; }
        protected void FastAddTables(IDataToSent receiveData , params DataTable[] items)
        {
            foreach (var dt in items)
            {
                var id = receiveData.AddNewTable(dt);
                receiveData.Metadata.AddTable(dt, id);
            }
            
        }
        //todo : this should stay into IDataToSent
        public IEnumerable<KeyValuePair<int,DataTable>> FindTableAfterColumnName(string nameColumn, IDataToSent receiveData)
        {
            
            var cols = receiveData.Metadata.Columns
                .Where(it => string.Equals(nameColumn, it.Name))
                .Select(it=>it.IDTable)
                .ToArray();
            var tables = receiveData.DataToBeSentFurther;
            foreach(var i in tables.Keys)
            {
                if (!cols.Contains(i))
                    continue;

                yield return new KeyValuePair<int, DataTable>(i, tables[i]);
            }
            
        }
        public Version Version { get; }

        protected T GetMyDataOrDefault<T>(string name, T def)
        {
            if (dataNeeded == null)
                return def;
            name = name?.ToLowerInvariant();
            if (!dataNeeded.ContainsKey(name))
                return def;

            var ret= (T)dataNeeded[name];
            if (object.Equals(ret , default(T)))
                return def;

            return ret;



        }
        protected T GetMyDataOrThrow<T>(string name)
        {
            if (dataNeeded == null)
                throw new ArgumentException($"{nameof(dataNeeded)} is null");
            name = name?.ToLowerInvariant();
            if (!dataNeeded.ContainsKey(name))
                throw new ArgumentException($"{nameof(dataNeeded)} does not contain {name}");

            return (T)dataNeeded[name];
        }

        public abstract Task<IDataToSent> TransformData(IDataToSent receiveData);
        public abstract Task<IMetadata> TryLoadMetadata();

    }
}
