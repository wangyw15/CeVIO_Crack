﻿using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CeVIO
{
    public class CeVIOAssembly
    {
        public Assembly Instance { get; }

        private Type _EditorResource;

        public CeVIOAssembly(string cevioExecutablePath)
        {
            Instance = Assembly.LoadFile(cevioExecutablePath);
            _EditorResource = Instance.GetType("CeVIO.Editor.Properties.Resources");
        }

        public object GetEditorResource(string name)
        {
            return _EditorResource.GetProperty(name, BindingFlags.Static | BindingFlags.Public).GetValue(null);
        }

        public T GetEditorResource<T>(string name)
        {
            return (T)GetEditorResource(name);
        }
    }

    public class App
    {
        private CeVIOAssembly _Assembly;
        
        public Type Instance { get; }

        public App(CeVIOAssembly assembly)
        {
            _Assembly = assembly;
            Instance = _Assembly.Instance.GetType("CeVIO.Editor.App");
        }
        
        public string CommonName
        {
            get
            {
                var name = Instance.GetField("CommonName");
                return name.GetValue(null) as string;
            }
        }
    }
    
    public class ProductLicense
    {
        private CeVIOAssembly _Assembly;

        public Type Instance { get; }

        public ProductLicense(CeVIOAssembly assembly)
        {
            _Assembly = assembly;
            Instance = _Assembly.Instance.GetType("CeVIO.Editor.MissionAssistant.ProductLicense");
        }

        
        public DateTime DescrambleDateTime(byte[] value)
        {
            var method = Instance.GetMethod("DescrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (DateTime)method.Invoke(null, new object[] { value });
        }

        public byte[] ScrambleDateTime(DateTime value)
        {
            var method = Instance.GetMethod("ScrambleDateTime", BindingFlags.Static | BindingFlags.NonPublic);
            return (byte[])method.Invoke(null, new object[] { value });
        }
    }

    public class Authorizer
    {
        private CeVIOAssembly _Assembly;

        public Type Instance { get; }

        public Authorizer(CeVIOAssembly assembly)
        {
            _Assembly = assembly;
            Instance = _Assembly.Instance.GetType("CeVIO.Editor.MissionAssistant.Authorizer");
        }
        
        public IEnumerable<object> ReadLicenses()
        {
            var read = Instance.GetMethod("ReadLicenses", BindingFlags.Static | BindingFlags.NonPublic);
            return read.Invoke(null, null) as IEnumerable<object>;
        }

        public IEnumerable<object> Licenses
        {
            get
            {
                var licenses = Instance.GetProperty("Licenses");
                return licenses.GetValue(null) as IEnumerable<object>;
            }
        }
    }

    public class LicenseSummary
    {
        private CeVIOAssembly _Assembly;

        public Type Instance { get; }

        public LicenseSummary(CeVIOAssembly assembly)
        {
            _Assembly = assembly;
            Instance = _Assembly.Instance.GetType("CeVIO.Editor.MissionAssistant.LicenseSummary");
        }
        
        public IEnumerable<object> Packages
        {
            get
            {
                var packages = Instance.GetProperty("Packages");
                return packages.GetValue(null) as IEnumerable<object>;
            }
        }

        public Encoding encoding
        {
            get
            {
                var e = Instance.GetField("encoding", BindingFlags.NonPublic | BindingFlags.Static);
                return e.GetValue(null) as Encoding;
            }
        }

        public XElement Load()
        {
            var load = Instance.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static);
            return load.Invoke(null, null) as XElement;
        }

        public void Save()
        {
            var save = Instance.GetMethod("Save", BindingFlags.Static | BindingFlags.Public);
            save.Invoke(null, null);
        }

        public void AddFeature(Feature feature)
        {
            var add = Instance.GetMethod("AddFeature", BindingFlags.NonPublic | BindingFlags.Static);
            add.Invoke(null, new object[] { feature });
        }

        public void AddPackageCodes(IEnumerable<Guid> codes)
        {
            var add = Instance.GetMethod("AddPackageCodes", new Type[] { typeof(IEnumerable<Guid>) });
            add.Invoke(null, new object[] { codes });
        }

        public string KeyPath
        {
            get
            {
                var keyPath = Instance.GetField("keyPath");
                return keyPath.GetValue(null) as string;
            }
        }

        public string ValueName
        {
            get
            {
                var keyPath = Instance.GetField("valueName");
                return keyPath.GetValue(null) as string;
            }
        }

        public Type PackageUnit
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
