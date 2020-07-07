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
using Cast.Util.Log;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Cast.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathUtil
    {

        /// <summary>
        /// 
        /// </summary>
        public static string CreateTempCopy(string tempFolder, string templatePath)
        {
            if (!string.IsNullOrEmpty(tempFolder) && !Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string tempName = $"~{Path.GetFileNameWithoutExtension(templatePath)}_{DateTime.Now:MMdd_HHmmss}{Path.GetExtension(templatePath)}";

            // ReSharper disable once AssignNullToNotNullAttribute
            string tempFile = Path.Combine(tempFolder, tempName);
            File.Copy(templatePath, tempFile);

            return tempFile;
        }

        public static string CreateTempCopyFlexi(string tempFolder, string templatePath)
        {
            if (!string.IsNullOrEmpty(tempFolder) && !Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string tempName = $"~{Path.GetFileNameWithoutExtension(templatePath)}_{DateTime.Now:yyMMdd_HHmmss}{Path.GetExtension(templatePath)}";

            // ReSharper disable once AssignNullToNotNullAttribute
            string tempFile = Path.Combine(tempFolder, tempName);
            File.Copy(templatePath, tempFile);

            return tempFile;

        }

        public static string CreateTempCopy(string tempFolder, string templatePath, DirectoryInfo templatesRoot)
        {
            string subtemplatePath = GetTemplatePathInRoot(templatePath, templatesRoot);
            return CreateTempCopy(tempFolder, subtemplatePath);
        }

        public static string CreateTempCopyFlexi(string tempFolder, string templatePath, DirectoryInfo templatesRoot)
        {
            string subtemplatePath = GetTemplatePathInRoot(templatePath, templatesRoot);
            return CreateTempCopyFlexi(tempFolder, subtemplatePath);
        }

        private static string GetTemplatePathInRoot(string templatePath, DirectoryInfo templatesRoot)
        {
            FileInfo _templateFile = new FileInfo(templatePath);
            if (_templateFile.Exists) return templatePath;
            FileInfo _template = FindFileInFolderTree(_templateFile.Name, templatesRoot);
            if (_template != null && _template.Exists)
            {
                return _template.FullName;
            }
            return templatePath;
        }

        private static FileInfo FindFileInFolderTree(string fileName, DirectoryInfo rootFolder)
        {
            FileInfo _file = rootFolder.GetFiles().ToList().FirstOrDefault(f => f.Name == fileName);
            if (_file != null && _file.Exists) return _file;
            return rootFolder.GetDirectories().Select(directoryInfo => FindFileInFolderTree(fileName, directoryInfo)).FirstOrDefault(subFile => subFile != null && subFile.Exists);
        }

        public static void UnzipAndCopy(string archive, string dirname)
        {
            try
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), "RG-templates_" + DateTime.Today.ToString("yyyyMMdd"));
                if (Directory.Exists(tempDirectory))
                {
                    File.SetAttributes(tempDirectory, FileAttributes.Normal);
                    Directory.Delete(tempDirectory, true);
                }
                Directory.CreateDirectory(tempDirectory);
                ZipFile.ExtractToDirectory(archive, tempDirectory);
                File.SetAttributes(tempDirectory, FileAttributes.Normal);
                DirectoryCopy(tempDirectory, dirname, true, true);
                File.SetAttributes(dirname, FileAttributes.Normal);
            }
            catch (IOException e)
            {
                LogHelper.LogError(e.Message);
            }

        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true, overwrite);
            }
        }
    }
}
