//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.LogFacade
// 
//  Dapplo.LogFacade is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.LogFacade is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.LogFacade. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using NLog;
using Dapplo.LogFacade.Loggers;

#endregion

namespace Dapplo.LogFacade.Loggers
{
    /// <summary>
    ///     Wrapper for Dapplo.LogFacade.ILogger -> NLog.Logger
    /// </summary>
    public class NLogLogger : AbstractLogger
    {
        private static readonly NLog.Logger Log = LogManager.GetLogger("NLogLogger");

        public override void Write(LogInfo logInfo, string messageTemplate, params object[] logParameters)
        {
            LogManager.GetLogger(logInfo.Source.Source).Log(Convert(logInfo.Level), messageTemplate, logParameters);
        }

        public override void Write(LogInfo logInfo, Exception exception, string messageTemplate = null, params object[] logParameters)
        {
            LogManager.GetLogger(logInfo.Source.Source).Log(Convert(logInfo.Level), exception, messageTemplate, logParameters);
        }

        private NLog.LogLevel Convert(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
            }
            return NLog.LogLevel.Debug;
        }

        /// <summary>
        ///     Register the NLogLogger as the global LogFacade logger
        /// </summary>
        /// <param name="level">LogLevel, when none is given the LogSettings.DefaultLevel is used</param>
        public static void RegisterLogger(LogLevel level = default(LogLevel))
        {
            LogSettings.Logger = new NLogLogger { Level = level == LogLevel.None ? LogSettings.DefaultLevel : level };
        }
    }
}