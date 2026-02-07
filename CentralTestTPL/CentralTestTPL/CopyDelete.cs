using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace CentralTestTPL
{
    public class CopyDelete
    {
        public void CopyFiles(string sourceDirectory, string destinationDirectory, string testProgramName)
        {
            string[] files = Directory.GetFiles(sourceDirectory);

            foreach (string file in files)
            {
                if (file.Contains(testProgramName))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destFile, true);
                    WaitForFile(destFile);
                }
            }
        }

        public void CopyFilesFormat(string sourceDirectory, string destinationDirectory, string fileFormat)
        {
            string[] files = Directory.GetFiles(sourceDirectory);

            foreach (string file in files)
            {
                if (file.EndsWith(fileFormat, StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDirectory, fileName);
                    File.Copy(file, destFile, true);
                    WaitForFile(destFile);

                    //DataAccess da = new DataAccess();
                    //da.insertMasterLogs(LotInfo.LotNumber,
                    //LotInfo.LotAlias,
                    //LotInfo.LotCode,
                    //LotInfo.CustomerCode,
                    //LotInfo.RecipeCode,
                    //destFile,
                    //fileName,
                    //LotInfo.StageID,
                    //User.Emp_No);
                }
            }
        }

        public void CopyFolder(string sourceDir, string destinationDir)
        {

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get all files and directories in the source directory
            string[] files = Directory.GetFiles(sourceDir);
            string[] directories = Directory.GetDirectories(sourceDir);

            // Copy files
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationFile = Path.Combine(destinationDir, fileName);
                File.Copy(file, destinationFile, true);
                Paths.datalogfile = fileName;
            }

            // Recursively copy subdirectories
            foreach (string directory in directories)
            {
                string dirName = Path.GetFileName(directory);
                string destinationSubDir = Path.Combine(destinationDir, dirName);
                CopyFolder(directory, destinationSubDir);
                Paths.datalog = dirName;
            }

        }

        //string[] files = Directory.GetFiles(sourceDirectory);

        //foreach (string file in files)
        //{
        //    string fileName = Path.GetFileName(file);
        //    string folderName = Path.GetFileName(Path.GetDirectoryName(fileName));
        //    Directory.CreateDirectory(destinationDirectory + folderName);
        //    string destFile = Path.Combine(destinationDirectory + folderName, fileName);

        //    File.Copy(file, destFile, true);
        //    WaitForFile(destFile);
        //}


        public void WaitForFile(string filePath)
        {
            while (true)
            {
                try
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        // File is now accessible, so break out of the loop
                        break;
                    }
                }
                catch (IOException)
                {

                    Thread.Sleep(1000);
                }
            }
        }


        public void DeleteOriginalFiles(string sourceDirectory)
        {
            string[] files = Directory.GetFiles(sourceDirectory);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public void DeleteFolder(string sourceDirectory)
        {
            string[] subfolders = Directory.GetDirectories(sourceDirectory);
            string[] tpFiles = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
         
            foreach (string tpFile in tpFiles)
            {
                File.Delete(tpFile);
            }
            foreach (string subfolder in subfolders)
            {
                // Delete files within the subfolder
                string[] files = Directory.GetFiles(subfolder);
                foreach (string file in files)
                {
                    File.Delete(file);
                }


                Directory.Delete(subfolder);
            }
        }
    }
}
