/*
 *   Copyright (c) 2024 CAST
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


using System;
using System.Text;

namespace Cast.Util.Security
{
    public static class AuthHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CreateBasicAuthenticationCredentials(string userName, string password)
        {
            string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
            var returnValue = $"Basic {base64UsernamePassword}";
            return returnValue;
        }

    }
}
