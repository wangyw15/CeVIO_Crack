using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CeVIO;

namespace CeVIO_crack
{
    public class CSActivator : BaseActivator
    {
        protected override string ProductKeyName => "CeVIO";

        protected override string ProductName => "CeVIO Creative Studio (64bit)";
    }
    public class AIActivator : BaseActivator
    {
        protected override string ProductKeyName => "CeVIO_NV";

        protected override string ProductName => "CeVIO AI";
    }
    
    public abstract class BaseActivator
    {
        public string ActivationKey { get; set; } = "00000-00000-00000-00000";
        private readonly byte[] _EmptyData = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        protected abstract string ProductKeyName { get; }
        protected abstract string ProductName { get; }

        public void ActivateProducts(TimeSpan? validateTime = null)
        {
            var packageCodes = GetPackageCodes();
            var mainPackageCode = GetCeVIOProductCode();

            foreach (var code in packageCodes)
            {
                var keyPath = "";
                if (code.ToString() == mainPackageCode)
                {
                    keyPath = $"{LicenseSummary.KeyPath}\\Creative Studio\\Product";
                }
                else
                {
                    keyPath = LicenseSummary.KeyPath + "\\Product\\{" + code.ToString().ToUpper() + "}";
                }

                using (var registryKey = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    var expire = DateTime.Now + (validateTime ?? TimeSpan.FromDays(365));
                    var data = ProductLicense.ScrambleDateTime(expire);
                    registryKey.SetValue(null, _EmptyData);
                    registryKey.SetValue("ProductKey", ActivationKey);
                    registryKey.SetValue("License", data);
                    registryKey.SetValue("Registration", data);
                }
            }
        }

        public IEnumerable<Guid> GetPackageCodes()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"{LicenseSummary.KeyPath}\\Product"))
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
            LicenseSummary.AddFeature(Feature.Full);
            LicenseSummary.AddPackageCodes(GetPackageCodes());
            LicenseSummary.Save();
        }

        public string GetCeVIOProductCode()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"{LicenseSummary.KeyPath}\\Product"))
            {
                foreach(var subKey in registry.GetSubKeyNames())
                {
                    var subRegistry = registry.OpenSubKey(subKey);
                    if (subRegistry.GetValue("ProductName") as string == ProductName)
                    {
                        return new Guid(subKey.Split(' ')[0]).ToString();
                    }
                }
                return "";
            }
        }
    }
}
