﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using CeVIO;

namespace CeVIO_crack
{
    public class Activator
    {
        public string ActivationKey { get; set; } = "00000-00000-00000-00000";
        
        private readonly byte[] _EmptyData = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private CeVIOAssembly _Assembly;
        private App _App;
        private LicenseSummary _LicenseSummary;
        private ProductLicense _ProductLicense;

        public Activator(string cevioPath)
        {
            _Assembly = new CeVIOAssembly(cevioPath);
            _App = new App(_Assembly);
            _LicenseSummary = new LicenseSummary(_Assembly);
            _ProductLicense = new ProductLicense(_Assembly);
        }

        public void ActivateProducts(TimeSpan? validateTime = null)
        {
            var packageCodes = GetPackageCodes();
            var mainPackageCode = GetCeVIOProductCode();

            foreach (var code in packageCodes)
            {
                var keyPath = "";
                if (code.ToString() == mainPackageCode)
                {
                    keyPath = $"{_LicenseSummary.KeyPath}\\Creative Studio\\Product";
                }
                else
                {
                    keyPath = _LicenseSummary.KeyPath + "\\Product\\{" + code.ToString().ToUpper() + "}";
                }

                using (var registryKey = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    var expire = DateTime.Now + (validateTime ?? TimeSpan.FromDays(365));
                    var data = _ProductLicense.ScrambleDateTime(expire);
                    registryKey.SetValue(null, _EmptyData);
                    registryKey.SetValue("ProductKey", ActivationKey);
                    registryKey.SetValue("License", data);
                    registryKey.SetValue("Registration", data);
                }
            }
        }

        public IEnumerable<Guid> GetPackageCodes()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"{_LicenseSummary.KeyPath}\\Product"))
            {
                if (registry == null)
                {
                    yield break;
                }
                foreach (var x in registry.GetSubKeyNames())
                {
                    yield return new Guid(x.Split(' ')[0]);
                }
            }
        }

        public void GenerateLicenseSummary()
        {
            _LicenseSummary.AddFeature(Feature.Full);
            _LicenseSummary.AddPackageCodes(GetPackageCodes());
            _LicenseSummary.Save();
        }

        public string GetCeVIOProductCode()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"{_LicenseSummary.KeyPath}\\Product"))
            {
                foreach(var subKey in registry.GetSubKeyNames())
                {
                    var subRegistry = registry.OpenSubKey(subKey);
                    if (subRegistry.GetValue("ProductName") as string == _App.CommonName)
                    {
                        return new Guid(subKey.Split(' ')[0]).ToString();
                    }
                }
                return "";
            }
        }
    }
}
