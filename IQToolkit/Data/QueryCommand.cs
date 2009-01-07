// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace IQ.Data
{
    public class QueryCommand<T>
    {
        string commandText;
        ReadOnlyCollection<string> paramNames;
        Func<DbDataReader, T> projector;

        public QueryCommand(string commandText, IEnumerable<string> paramNames, Func<DbDataReader, T> projector)
        {
            this.commandText = commandText;
            this.paramNames = new List<string>(paramNames).AsReadOnly();
            this.projector = projector;
        }

        public string CommandText
        {
            get { return this.commandText; }
        }

        public ReadOnlyCollection<string> ParameterNames
        {
            get { return this.paramNames; }
        }

        public Func<DbDataReader, T> Projector
        {
            get { return this.projector; }
        }
    }
}
