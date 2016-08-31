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

namespace Devsense.PHP.Errors
{
    /// <summary>
    /// Provides error sink to report errors into.
    /// </summary>
    /// <typeparam name="TSpan">Type of position.</typeparam>
    public interface IErrorSink<TSpan>
    {
        /// <summary>
        /// Reports an error to the sink.
        /// </summary>
        /// <param name="span">Error position.</param>
        /// <param name="info">Error descriptor.</param>
        /// <param name="argsOpt">Error message arguments.</param>
        void Error(TSpan span, ErrorInfo info, params string[] argsOpt);
    }
}
