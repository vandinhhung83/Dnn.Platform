﻿#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using DotNetNuke.Common;

using NPoco;

namespace DotNetNuke.Data.NPoco
{
    public class NPocoDataContext : IDataContext
    {
        #region Private Members

        private readonly Database _database;

        #endregion

        #region Constructors

        public NPocoDataContext()
            : this(ConfigurationManager.ConnectionStrings[0].Name, String.Empty)
        {
        }

        public NPocoDataContext(string connectionStringName)
            : this(connectionStringName, String.Empty)
        {
        }

        public NPocoDataContext(string connectionStringName, string tablePrefix)
        {
            Requires.NotNullOrEmpty("connectionStringName", connectionStringName);

            _database = new Database(connectionStringName) {Mapper = new NPocoMapper(tablePrefix)};
        }

        #endregion

        #region Implementation of IDataContext

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void Commit()
        {
            _database.CompleteTransaction();
        }

        public void Execute(CommandType type, string sql, params object[] args)
        {
            if (type == CommandType.StoredProcedure)
            {
                sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
            }

            _database.Execute(DataUtil.ReplaceTokens(sql), args);
        }

        public IEnumerable<T> ExecuteQuery<T>(CommandType type, string sql, params object[] args)
        {
            if (type == CommandType.StoredProcedure)
            {
                sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
            }

            return _database.Fetch<T>(DataUtil.ReplaceTokens(sql), args);
        }

        public T ExecuteScalar<T>(CommandType type, string sql, params object[] args)
        {
            if (type == CommandType.StoredProcedure)
            {
                sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
            }

            return _database.ExecuteScalar<T>(DataUtil.ReplaceTokens(sql), args);
        }

        public T ExecuteSingleOrDefault<T>(CommandType type, string sql, params object[] args)
        {
            if (type == CommandType.StoredProcedure)
            {
                sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
            }

            return _database.SingleOrDefault<T>(DataUtil.ReplaceTokens(sql), args);
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new NPocoRepository<T>(_database);
        }

        public void RollbackTransaction()
        {
            _database.AbortTransaction();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _database.Dispose();
        }

        #endregion
    }
}