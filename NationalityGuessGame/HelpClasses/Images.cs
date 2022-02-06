using System;
using System.Collections.Generic;
using System.IO;

namespace NationalityGuessGame.HelpClasses
{
    /// <summary>
    /// Relate to Images Directory
    /// </summary>
    public static class Images
    {
        private static List<string> imagesPathList;
        static Images()
        {
            GetDirectoryNames();
        }

        public static List<string> ImagesPath
        {
            get
            {
                if (imagesPathList == null)
                {
                    imagesPathList = new List<string>();
                }
                return imagesPathList;
            }
        }

        /// <summary>
        /// Get all filename of Images and putting them with directory name in ImagesPath
        /// </summary>
        private static void GetDirectoryNames()
        {
            string folderProjectPath = new DirectoryInfo(
                AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string folderImagesPath = Path.Combine(folderProjectPath, "Images");
            if (Directory.Exists(folderImagesPath))
            {
                foreach (var item in Directory.EnumerateDirectories(folderImagesPath))
                {
                    foreach (string file in Directory.EnumerateFiles(item, "*.jpg"))
                    {
                        ImagesPath.Add(Path.GetFileName(item) + "/" + Path.GetFileName(file));
                    }
                }
            }
        }
    }
}
