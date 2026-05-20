using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Utils;

namespace TopEng.Utils
{
    public static class IOFunctionUtil
    {
        public static bool IsDiskSpaceAvailable(string path, string input)
        {
            string driveRoot = Path.GetPathRoot(path);
            DriveInfo drive = new DriveInfo(path);
            if (drive.AvailableFreeSpace <= input.Length)
            {
                MessageBox.Show($"{drive.Name} drive Disk is full. Please clear before save data!", "SYSTEM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static void CreateFileAndDirectory(string path)
        {
            string directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(path))
            {
                using (File.Create(path))
                {

                }
            }
            else
            {
                MessageBox.Show("Error: Already exist");
            }
        }

        public static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    MessageBox.Show("Error: Can not find file");
                }
            }
            catch (Exception ex) { }
        }

        public static void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    MessageBox.Show("Error: Directory not found.");
                }
            }
            catch (Exception ex) { }
        }

        public static void CopyFile(string sourcePath, string targetPath)
        {
            string directoryPath = Path.GetDirectoryName(targetPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath);
            }
            else
            {
                MessageBox.Show("Error: Can not find source file");
            }
        }

        public static void RenameFile(string sourcePath, string targetPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Move(sourcePath, targetPath);
            }
            else
            {
                MessageBox.Show("Error: Can not find source file");
            }
        }

        public static void RenameDirectory(string sourceDirectory, string targetDirectory)
        {
            if (Directory.Exists(sourceDirectory))
            {
                Directory.Move(sourceDirectory, targetDirectory);
            }
            else
            {
                MessageBox.Show("Error: Can not find source Directory");
            }
        }

        public static string GetDirectoryOfFile(string strFilePath)
        {
            return Path.GetDirectoryName(strFilePath);
        }

        public static bool CheckFileExist(string sourcePath)
        {
            return File.Exists(sourcePath);
        }

        public static bool CheckDirectoryExist(string sourcePath)
        {
            return Directory.Exists(sourcePath);
        }
    }
}
