using System;
using System.IO;
using System.Linq;

namespace BatchReadUserStringsSample
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Enter path to directory which contains files to convert and press ENTER:");
      string path = Console.ReadLine();

      string[] filePaths = null;

      try
      {
        filePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
          .Where(s => s.EndsWith(".3dm"))
          .ToArray();
      }
      catch (Exception e)
      {
        Console.WriteLine("{0}, Press any key to exit...", e.Message);
        Console.ReadKey();
        return;
      }

      if (filePaths == null || filePaths.Length == 0)
      {
        Console.WriteLine("Directory is empty. No files to process. Press any key to exit.");
        Console.ReadKey();
        return;
      }

      Console.WriteLine("Found {0} files!", filePaths.Length);

      foreach (string file in filePaths)
      {
        using (var doc = Rhino.FileIO.File3dm.Read(file))
        {

          Console.WriteLine("Nº of objects in file: {0}", doc.Objects.Count);

          foreach (var obj in doc.Objects)
          {
            Console.WriteLine("Found object with id: {0}", obj.Id);
            var us = obj.Attributes.GetUserStrings();
            foreach (string key in us.AllKeys)
            {
              Console.WriteLine("{0}: {1}", key, us[key]);
            }
          }
        }
      }

      Console.WriteLine("Finished converting. Press any key to exit...");
      Console.ReadKey();
    }
  }
}
