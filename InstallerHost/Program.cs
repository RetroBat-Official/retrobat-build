using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace InstallerHost
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                var requestedAssemblyName = new AssemblyName(resolveArgs.Name).Name;
                if (requestedAssemblyName == "ICSharpCode.SharpZipLib")
                {
                    var currentAssembly = Assembly.GetExecutingAssembly();
                    var resourceName = "InstallerHost.resources.ICSharpCode.SharpZipLib.dll";

                    using (var stream = currentAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                            return null;

                        byte[] assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }
                return null;
            };
            CultureInfo windowsCulture = CultureInfo.CurrentUICulture;

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("-lang", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        windowsCulture = new CultureInfo(args[i + 1]);
                        Thread.CurrentThread.CurrentCulture = windowsCulture;
                        Thread.CurrentThread.CurrentUICulture = windowsCulture;
                        Logger.Log($"Langue forcée : {windowsCulture.Name}");
                    }
                    catch (CultureNotFoundException)
                    {
                        Logger.Log($"Langue non reconnue : {args[i + 1]} — langue système conservée.");
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Logger.Log("Running RetroBat Installer.");
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Logger.Log("Fatal startup error: " + ex.ToString());
                MessageBox.Show(Texts.GetString("StartupError"), Texts.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
