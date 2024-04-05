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

        public static string[] Read(string path)
        {
            return Directory.GetFiles(Path.GetFullPath(path));
        }

        public static bool HasOnlyOneFile(this string[] files)
        {
            if (files.Length > 1) return false;
            return true;
        }

        public static string ParseFileName(this string str)
        {
            return str.Split("\\").Last();
        }
    }
}
