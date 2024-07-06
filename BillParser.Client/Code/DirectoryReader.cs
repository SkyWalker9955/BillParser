namespace BillParser.Client.Code
{
    public static class DirectoryReader
    {
        public static string GetBillName(string path)
        {
            var directoryContent = Read(path);
            if (!directoryContent.HasOnlyOneFile())
            {
                throw new Exception("Bill location directory has more than 1 file");
            }
            return directoryContent
                .First()
                .ParseFileName();
        }

        private static string[] Read(string path)
        {
            return Directory.GetFiles(Path.GetFullPath(path));
        }

        private static bool HasOnlyOneFile(this string[] files)
        {
            if (files.Length > 1) return false;
            return true;
        }

        private static string ParseFileName(this string str)
        {
            return str.Split(['\\', '/']).Last();
        }
    }
}
