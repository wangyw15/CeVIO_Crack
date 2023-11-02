using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

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
        private static readonly Type _ProductLicense = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.ProductLicense");

        
        public static DateTime DescrambleDateTime(byte[] value)
        {
            var method = _ProductLicense.GetMethod("DescrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (DateTime)method.Invoke(null, new object[] { value });
        }

        public static byte[] ScrambleDateTime(DateTime value)
        {
            var method = _ProductLicense.GetMethod("ScrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (byte[])method.Invoke(null, new object[] { value });
        }
    }

    public static class Authorizer
    {
        private static readonly Type _Authorizer = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.Authorizer");
        
        public static string HDPrimaryVolumeSerialNo
        {
            get
            {
                return _Authorizer.GetProperty("HDPrimaryVolumeSerialNo").GetValue(null) as string;
            }
        }
    }

    public static class LicenseSummary
    {
        private static readonly Type _LicenseSummary = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.MissionAssistant.LicenseSummary");
        
        public static IEnumerable<object> Packages
        {
            get
            {
                var packages = _LicenseSummary.GetProperty("Packages");
                return packages.GetValue(null) as IEnumerable<object>;
            }
        }

        public static Encoding encoding
        {
            get
            {
                var e = _LicenseSummary.GetField("encoding", BindingFlags.NonPublic | BindingFlags.Static);
                return (Encoding)e.GetValue(null);
            }
        }
    }

    public static class Cipher
    {
        private static readonly Type _Cipher = CeVIOAssembly.CeVIO.GetType("CeVIO.Editor.Utilities.Cipher");

        public static byte[] Encrypt(byte[] value, byte[] keyToken)
        {
            var encrypt = _Cipher.GetMethod("Encrypt", new Type[] { typeof(byte[]), typeof(byte[]) });
            return (byte[])encrypt.Invoke(null, new object[] { value, keyToken });
        }

        public static byte[] Encrypt(byte[] value, string keyToken)
        {
            var encrypt = _Cipher.GetMethod("Encrypt", new Type[] { typeof(byte[]), typeof(string) });
            return (byte[])encrypt.Invoke(null, new object[] { value, keyToken });
        }
        
        public static byte[] Decrypt(byte[] value, byte[] keyToken)
        {
            var encrypt = _Cipher.GetMethod("Decrypt", new Type[] { typeof(byte[]), typeof(byte[]) });
            return (byte[])encrypt.Invoke(null, new object[] { value, keyToken });
        }

        public static byte[] Decrypt(byte[] value, string keyToken)
        {
            var encrypt = _Cipher.GetMethod("Decrypt", new Type[] { typeof(byte[]), typeof(string) });
            return (byte[])encrypt.Invoke(null, new object[] { value, keyToken });
        }
    }
}

