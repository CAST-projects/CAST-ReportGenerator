/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Core.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CastReporting.Reporting.Helper
{
    /// <summary>
    /// Common Helper Class
    /// </summary>
    public static class BlockHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="blockName"></param>
        /// <returns></returns>
        public static T GetAssociatedBlockInstance<T>(string blockName) where T : class {
            // ensure cache is ready
            EnsureBlockCacheIsLoaded();
            // lookup a list of types for blockName
            if (!_blocks.TryGetValue(blockName, out List<Type> candidates)) return null;
            // find the first type that derives from T
            var candidate = candidates.Where(t => t.IsSubclassOf(typeof(T))).FirstOrDefault();
            if (candidate == null) return null;
            // return a new instance of the requested type
            return Activator.CreateInstance(candidate) as T;
        }

        private static readonly ConcurrentDictionary<string, List<Type>> _blocks = new ConcurrentDictionary<string, List<Type>>();

        private static void EnsureBlockCacheIsLoaded() {
            if (_blocks.Count == 0) {
                // the cache is empty, acquire exclusive lock to the cache
                lock (_blocks) {
                    if (_blocks.Count > 0) {
                        // the cache has been initialized in the meantime
                        // it is ready
                        return;
                    }

                    // inspect all loaded assemblies decorated with a BlockLibraryAttribute
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<BlockLibraryAttribute>() != null)) {
                        // inspect non-abstract types
                        foreach (var blockType in asm.GetTypes().Where(t => !t.IsAbstract)) {
                            // handle types decorated with one or more BlockAttributes
                            foreach (var blockAttr in blockType.GetCustomAttributes<BlockAttribute>()) {
                                _blocks.AddOrUpdate(
                                    blockAttr.Name,
                                    // first time this name is mentioned: create a new list and add blockType
                                    name => { var list = new List<Type> { blockType }; return list; },
                                    // the name is also used by another type: append blockType to the list
                                    (name, list) => { list.Add(blockType); return list; }
                                 );
                            }
                        }
                    }

                }
            }
        }
    }
}
