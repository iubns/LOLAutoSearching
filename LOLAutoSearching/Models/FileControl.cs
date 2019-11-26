using System.IO;
using System.Text;

namespace LOLAutoSearching.Models
{
    public static class FileControl
    {
        public static bool AutoSearching
        {
            get
            {
                FileStream file = File.Open(@"C:\ProgramData\Iubns\AutoSearching.ini", FileMode.OpenOrCreate);
                if (file.Length == 0)
                {
                    return false;
                }
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);
                file.Close();
                return bool.Parse(Encoding.UTF8.GetString(buffer));
            }
            set
            {
                FileStream file = File.Open(@"C:\ProgramData\Iubns\AutoSearching.ini", FileMode.Create);
                byte[] buffer = Encoding.UTF8.GetBytes(value.ToString());
                file.Write(buffer, 0, buffer.Length);
                file.Close();
            }
        }

        public static string Session
        {
            get
            {
                var file = File.Open(@"C:\ProgramData\Iubns\login.ini", FileMode.OpenOrCreate);
                if (file.Length == 0)
                {
                    return string.Empty;
                }
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);
                file.Close();
                return Encoding.UTF8.GetString(buffer);
            }
            set
            {
                FileStream file = File.Open(@"C:\ProgramData\Iubns\login.ini", FileMode.Create);
                byte[] buffer = Encoding.UTF8.GetBytes(value); 
                file.Write(buffer, 0, buffer.Length);
                file.Close();
            }
        }
    }
}
