using System;

namespace libXolTodoTxt
{
    public class IO
    {
        private static string path;
        public static string Path
        {
            get {
                if (path == null) {
                    bool isWindows = Environment.GetEnvironmentVariable("%HOMEPATH%") != null;
                    string basePath = "";
                    if(isWindows)
                    {
                        basePath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                    } else
                    {
                        basePath = Environment.GetEnvironmentVariable("HOME");
                    }
                    Path = System.IO.Path.Combine(basePath, "todo.txt");
                }
                return path;
            }
            set {
                if (!System.IO.File.Exists(value))
                {
                    System.IO.FileStream fileStream = System.IO.File.Create(value);
                    fileStream.Dispose();
                }
                path = value;
            }
        }

        public static string[] ReadFile()
        {
            return System.IO.File.ReadAllLines(Path);
        }

        public static void WriteFile(Todo todo)
        {
            System.IO.File.WriteAllText(Path, Serializer.serialize(todo));
        }


    }
}
