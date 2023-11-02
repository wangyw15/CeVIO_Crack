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
                if (code == mainPackageCode)
                {
                    keyPath = $"Software\\{ProductKeyName}\\Creative Studio\\Product";
                }
                else
                {
                    keyPath = "Software\\" + ProductKeyName + "\\Product\\{" + code.ToUpper() + "}";
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

        public string[] GetPackageCodes()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"Software\\{ProductKeyName}\\Product"))
            {
                if (registry == null)
                {
                    return null;
                }
                return (from x in registry.GetSubKeyNames()
                        let name = x.Split(' ')[0]
                        select new Guid(name).ToString()).ToArray();
            }
        }

        public string ReadTM()
        {
            using (var registry = Registry.CurrentUser.CreateSubKey("Software\\" + ProductKeyName))
            {
                var encryptedOriginalLicenseData = registry.GetValue("TM") as byte[];

                // read header
                var header = CeVIOAssembly.GetEditorResource<byte[]>("_header");
                
                // decrypt and parse data
                encryptedOriginalLicenseData = encryptedOriginalLicenseData.Skip(header.Length).ToArray<byte>();
                var decrypted = LicenseSummary.encoding.GetString(Cipher.Decrypt(encryptedOriginalLicenseData, Authorizer.HDPrimaryVolumeSerialNo));
                var doc = XElement.Parse(decrypted);
                return doc.ToString();
            }
        }

        public void GenerateTM()
        {
            using (var registry = Registry.CurrentUser.CreateSubKey("Software\\" + ProductKeyName))
            {
                var encryptedOriginalLicenseData = registry.GetValue("TM") as byte[];

                // read header
                var header = CeVIOAssembly.GetEditorResource<byte[]>("_header");

                // decrypt and parse data
                encryptedOriginalLicenseData = encryptedOriginalLicenseData.Skip(header.Length).ToArray<byte>();
                var decrypted = LicenseSummary.encoding.GetString(Cipher.Decrypt(encryptedOriginalLicenseData, Authorizer.HDPrimaryVolumeSerialNo));
                var doc = XElement.Parse(decrypted);

                // set ReleasedFeatures to Full
                var feature = (from x in doc.Elements() where x.Name == "ReleasedFeatures" select x).FirstOrDefault();
                feature.SetAttributeValue("Value", "Full");

                // set trial to NotUse
                var trial = (from x in doc.Elements() where x.Name == "TrialSettings" select x).FirstOrDefault();
                feature.SetAttributeValue("State", "NotUse");

                // authorize all installed packages
                var activatedPackagesElement = (from x in doc.Elements() where x.Name == "ActivatedPackages" select x).FirstOrDefault();
                var activatedPackages = from x in activatedPackagesElement.Elements("Package") select x.Attribute("PackageCode").Value;
                foreach (var packageCode in GetPackageCodes())
                {
                    if (!activatedPackages.Contains(packageCode))
                    {
                        var package = new XElement("Package");
                        package.SetAttributeValue("PackageCode", packageCode);
                        activatedPackagesElement.Add(package);
                    }
                }

                // generate and write TM data
                var encrypted = Cipher.Encrypt(LicenseSummary.encoding.GetBytes(doc.ToString(SaveOptions.DisableFormatting)), Authorizer.HDPrimaryVolumeSerialNo);
                var encryptedFullLicenseData = new List<byte>(header);
                encryptedFullLicenseData.AddRange(encrypted);
                registry.SetValue("TM", encryptedFullLicenseData.ToArray());
            }
        }

        public string GetCeVIOProductCode()
        {
            using (var registry = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\{ProductKeyName}\\Product"))
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
