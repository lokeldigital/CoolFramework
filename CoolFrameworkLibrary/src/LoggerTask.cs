/*
 *  CoolFramework
 *  -------------
 *  A C#.NET library for Object Oriented containers and publish-subscribe
 *  inter-component middleware.  This has been built to allow a more abstract
 *  level of composability based on role that has a more loosely defined
 *  contract than interfaces or inheritance provide.
 *  
 *  Copyright (C) 2014, 2015 Lokel Digital Pty Ltd
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace Lokel.CoolFramework {

    public delegate void LoggerTask(string log);

    [Flags]
    public enum LogLevel : short {
        LL_None = 0,
        LL_Info = 1,
        LL_DebugFramework = 2,
        LL_DebugApp = 4,
        LL_Warning = 8,
        LL_Error = 16,

        LL_ALL = 0xFF
    }

    public class Logger {
        private LoggerTask _Task;
        private LogLevel _Level;

        public Logger(LogLevel level) {
            _Level = level;
            _Task = null;
        }

        public Logger(Logger aLogger) {
            _Level = aLogger._Level;
            _Task = aLogger._Task;
        }

        public Logger() : this(LogLevel.LL_Warning) {}

        public LogLevel Level {
            get { return _Level; }
            set { _Level = value; }
        }

        public void AddTask(LoggerTask task) {
            if (_Task == null) {
                _Task = task;
            } else {
                _Task += task;
            }
        }

        public void LogAt(LogLevel level, string log) {
            if (_Task != null && (_Level & level) != 0) {
                _Task(log);
            }
        }
    } //- Logger


} //-- namespace --
