using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

          foreach(var layer in doc.AllLayers)
          {
            Console.WriteLine("Layer \"{0}\", with index: {1}", layer.Name, layer.Index);
          }

          Console.WriteLine("Nº of objects in file: {0}", doc.Objects.Count);

          var old = new List<Tuple<Rhino.Geometry.GeometryBase, Rhino.DocObjects.ObjectAttributes>>();

          foreach (var obj in doc.Objects)
          {
            Console.WriteLine("Found object with id: {0}, on layer: {1}", obj.Id, obj.Attributes.LayerIndex);
            var us = obj.Attributes.GetUserStrings();
            foreach (string key in us.AllKeys)
            {
              Console.WriteLine("{0}: {1}", key, us[key]);
            }

            old.Add(Tuple.Create(obj.Geometry.Duplicate(), obj.Attributes.Duplicate()));
          }

          var new_layer_index = 1;
          Console.WriteLine("Updating layer indices to: {0}", new_layer_index);

          doc.Objects.Clear();

          foreach (var pair in old)
          {
            var attr = pair.Item2;
            attr.LayerIndex = new_layer_index;

            var geom = pair.Item1;

            switch (geom)
            {
              case Rhino.Geometry.Mesh mesh:
                doc.Objects.AddMesh(mesh, attr);
                break;
              case Rhino.Geometry.Brep brep:
                doc.Objects.AddBrep(brep, attr);
                break;
              case Rhino.Geometry.Curve curve:
                doc.Objects.AddCurve(curve, attr);
                break;
              default:
                break;
            }
          }

          doc.Write(file + ".3dm", 6);
        }
      }

      Console.WriteLine("Finished converting. Press any key to exit...");
      Console.ReadKey();
    }
  }
}
