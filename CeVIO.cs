using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CeVIO
{
    public static class CeVIOAssembly
    {
        public static readonly Assembly CeVIO = Assembly.LoadFile("C:\\Program Files\\CeVIO\\CeVIO AI\\CeVIO AI.exe");

        private static readonly Type _EditorResource = CeVIO.GetType("CeVIO.Editor.Properties.Resources");

        public static object GetEditorResource(string name)
        {
            return _EditorResource.GetProperty(name, BindingFlags.Static | BindingFlags.Public).GetValue(null);
        }

        public static T GetEditorResource<T>(string name)
        {
            return (T)GetEditorResource(name);
        }
    }
    
    public static class ProductLicense
    {
        public static readonly Type Instance = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.ProductLicense");

        
        public static DateTime DescrambleDateTime(byte[] value)
        {
            var method = Instance.GetMethod("DescrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (DateTime)method.Invoke(null, new object[] { value });
        }

        public static byte[] ScrambleDateTime(DateTime value)
        {
            var method = Instance.GetMethod("ScrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (byte[])method.Invoke(null, new object[] { value });
        }
    }

    public static class Authorizer
    {
        public static readonly Type Instance = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.Authorizer");
        
        public static IEnumerable<object> ReadLicenses()
        {
            var read = Instance.GetMethod("ReadLicenses", BindingFlags.Static | BindingFlags.NonPublic);
            return read.Invoke(null, null) as IEnumerable<object>;
        }

        public static IEnumerable<object> Licenses
        {
            get
            {
                var licenses = Instance.GetProperty("Licenses");
                var a = licenses.GetValue(null);
                return licenses.GetValue(null) as IEnumerable<object>;
            }
        }
    }

    public static class LicenseSummary
    {
        public static readonly Type Instance = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.LicenseSummary");
        
        public static IEnumerable<object> Packages
        {
            get
            {
                var packages = Instance.GetProperty("Packages");
                return packages.GetValue(null) as IEnumerable<object>;
            }
        }

        public static Encoding encoding
        {
            get
            {
                var e = Instance.GetField("encoding", BindingFlags.NonPublic | BindingFlags.Static);
                return e.GetValue(null) as Encoding;
            }
        }

        public static XElement Load()
        {
            var load = Instance.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static);
            return load.Invoke(null, null) as XElement;
        }

        public static void Save()
        {
            var save = Instance.GetMethod("Save", BindingFlags.Static | BindingFlags.Public);
            save.Invoke(null, null);
        }

        public static void AddFeature(Feature feature)
        {
            var add = Instance.GetMethod("AddFeature", BindingFlags.NonPublic | BindingFlags.Static);
            add.Invoke(null, new object[] { feature });
        }

        public static void AddPackageCodes(IEnumerable<Guid> codes)
        {
            var add = Instance.GetMethod("AddPackageCodes", new Type[] { typeof(IEnumerable<Guid>) });
            add.Invoke(null, new object[] { codes });
        }

        public static string KeyPath
        {
            get
            {
                var keyPath = Instance.GetField("keyPath");
                return keyPath.GetValue(null) as string;
            }
        }

        public static string ValueName
        {
            get
            {
                var keyPath = Instance.GetField("valueName");
                return keyPath.GetValue(null) as string;
            }
        }

        public static Type PackageUnit
        {
            get
            {
                return Instance.GetNestedType("PackageUnit", BindingFlags.NonPublic);
            }
        }
    }

    public enum Feature : uint
    {
        Unknown = 0U,
        Talking = 1U,
        Singing = 2U,
        Full = 3U
    }
}
