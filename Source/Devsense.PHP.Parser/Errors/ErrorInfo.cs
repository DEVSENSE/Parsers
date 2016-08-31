// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Errors
{
    #region ErrorSeverity

    public enum ErrorSeverity
    {
        /// <summary>
        /// Informational message.
        /// </summary>
        Information,

        /// <summary>
        /// A warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// Error.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal error.
        /// </summary>
        FatalError,

        /// <summary>
        /// A warning that is reported as error.
        /// </summary>
        WarningAsError
    }

    #endregion

    #region ErrorInfo

    /// <summary>
    /// An error descriptor.
    /// </summary>
    public abstract class ErrorInfo
    {
        /// <summary>
        /// Unique error identifier.
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// Gets error message format string containing string formatter parameters (<c>{0}, {1}, ...</c>).
        /// </summary>
        public abstract string FormatString { get; }

        /// <summary>
        /// Formats the error message with given error arguments.
        /// </summary>
        /// <param name="args">Error message arguments.</param>
        /// <returns>Error message. Cannot be <c>null</c>.</returns>
        public virtual string ToString(string[] args) => string.Format(FormatString, args);

        /// <summary>
        /// Gets error severity.
        /// </summary>
        public abstract ErrorSeverity Severity { get; }
    }

    #endregion

    /// <summary>
    /// Wrapper providing generic error descriptor.
    /// </summary>
    internal sealed class ErrorInfo_ : ErrorInfo
    {
        readonly int _id;
        readonly string _messageid;
        readonly ErrorSeverity _severity;

        public ErrorInfo_(int id, string messageid, ErrorSeverity severity)
        {
            Debug.Assert(messageid != null);
            Debug.Assert(Strings.ResourceManager.GetString(messageid) != null, $"String '{messageid}' could not be found in resources!");

            _id = id;
            _messageid = messageid;
            _severity = severity;
        }

        public override string FormatString => Strings.ResourceManager.GetString(_messageid);

        public override int Id => _id;

        public override ErrorSeverity Severity => _severity;
    }

    /// <summary>
    /// A syntax error.
    /// </summary>
    public sealed class SyntaxError : ErrorInfo
    {
        public override int Id => 2014;
        public override string FormatString => Strings.syntax_error;
        public override ErrorSeverity Severity => ErrorSeverity.FatalError;
    }
}
