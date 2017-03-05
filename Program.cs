using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace ClrMdExample
{
    partial class Program
    {
        static void Main()
        {
            var exePath = CreateSampleApplication();

            var target = DataTarget.AttachToProcess(Process.Start(exePath).Id, uint.MaxValue);

            var clr = target.ClrVersions.First().CreateRuntime();

            Console.WriteLine("*** Threads");

            clr.Threads
               .Select(thread => thread.StackTrace.Select(frame => frame.DisplayString))
               .ToList()
               .ForEach
               (
                   frame =>
                   {
                       Console.WriteLine("*** Stack trace for thread");

                       frame.ToList().ForEach(Console.WriteLine);
                   }
               );

            Console.WriteLine("*** Strings in the heap");

            var objects = clr.GetHeap()
                             .EnumerateObjectAddresses()
                             .Select(address => new { Type = clr.GetHeap().GetObjectType(address), Address = address })
                             .ToList();
            objects.ForEach
                    (
                        o => o.Type
                              .Fields
                              .Where(f => f.Type.IsString)
                              .Select(field => field.GetValue(o.Address)?.ToString().Trim())
                              .Where(s => !string.IsNullOrEmpty(s))
                              .OrderByDescending(s => s.Length)
                              .ToList()
                              .ForEach(Console.WriteLine)
                    );

            Console.ReadKey();

            Console.WriteLine("*** Biggest 10 objects in the heap");

            objects.OrderByDescending(o => o.Type.GetSize(o.Address))
                   .Take(10)
                   .ToList()
                   .ForEach
                    (
                        o =>
                        {
                            Console.WriteLine($"*** Type: {o.Type.Name}");
                        
                            o.Type
                             .Fields
                             .ToList()
                             .ForEach
                              (
                                  f =>
                                  {
                                      if (!f.HasSimpleValue)
                                          return;
                                  
                                      var value = f.GetValue(o.Address, false, true)?.ToString().Trim();
                                  
                                      Console.WriteLine($"{value}");
                                  }
                              );
                        }
                    );

            Console.ReadKey();

            

            File.Delete(exePath);
        }
    }
}
