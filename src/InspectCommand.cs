#region Copyright (c) 2016 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace LinqPadless
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    #endregion

    partial class Program
    {
        enum Inspection
        {
            None,
            Hash,
            HashSource,
            Meta,
            Code,
            Kind,
            Namespaces,
            DefaultNamespaces,
            Packages,
            Loads,
            RemovedNamespaces,
            ActualNamespaces,
            ActualPackages
        }

        static int InspectCommand(IEnumerable<string> args)
        {
            var help = Ref.Create(false);
            var verbose = Ref.Create(false);
            var template = (string) null;

            var options = new OptionSet(CreateStrictOptionSetArgumentParser())
            {
                Options.Help(help),
                Options.Verbose(verbose),
                Options.Debug,
                { "t|template=", "template", v => template = v },
            };

            var tail = options.Parse(args);

            var log = verbose ? Console.Error : null;
            if (log != null)
                Trace.Listeners.Add(new TextWriterTraceListener(log));

            if (help)
            {
                Help("inspect", options);
                return 0;
            }

            if (tail.Count == 0)
                throw new Exception("Missing inspection query.");

            var inspection = tail[0] switch
            {
                "hash"               => Inspection.Hash,
                "hash-source"        => Inspection.HashSource,
                "meta"               => Inspection.Meta,
                "code"               => Inspection.Code,
                "kind"               => Inspection.Kind,
                "loads"              => Inspection.Loads,
                "default-namespaces" => Inspection.DefaultNamespaces,
                "removed-namespaces" => Inspection.RemovedNamespaces,
                "namespaces"         => Inspection.Namespaces,
                "actual-namespaces"  => Inspection.ActualNamespaces,
                "packages"           => Inspection.Packages,
                "actual-packages"    => Inspection.ActualPackages,
                _ => throw new Exception("Unknown inspection query.")
            };

            if (inspection == Inspection.DefaultNamespaces)
            {
                foreach (var ns in LinqPad.DefaultNamespaces)
                    Console.WriteLine(ns);
                return 0;
            }

            if (tail.Count == 1)
                throw new Exception("Missing LINQPad query file path.");

            var queryPath = tail[1];

            return DefaultCommand(queryPath,
                                  args: Enumerable.Empty<string>(),
                                  template: template,
                                  outDirPath: null,
                                  inspection: inspection,
                                  uncached: false,
                                  dontExecute: true,
                                  force: true,
                                  publishIdleTimeout: TimeSpan.Zero,
                                  publishTimeout: TimeSpan.Zero,
                                  log);
        }
    }
}
