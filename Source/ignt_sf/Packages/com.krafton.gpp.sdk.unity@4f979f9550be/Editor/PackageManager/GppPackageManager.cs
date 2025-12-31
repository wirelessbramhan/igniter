#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Extension;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Gpp.Editor
{
    [InitializeOnLoad]
    public class GppPackagerManager : IPackageManagerExtension
    {
        // private const string UIDocumentFilePath = "Assets/Editor/PackageManager/GppPackageManager.uxml";
        private const string UIDocumentFilePath = "Packages/com.krafton.gpp.sdk.unity/Editor/PackageManager/GppPackageManager.uxml";
        private GppPackage[] _gppExtensions;
        private PackageInfo[] _registeredPackages;

        private VisualTreeAsset _uiDocument;
        private VisualElement _root;

        private Request _request;
        private const string CorePackageName = "com.krafton.gpp.sdk.unity";
        private const string SteamWorksUrl = "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net#2024.8.0";

        static GppPackagerManager()
        {
            GppPackagerManager packagerManager = new GppPackagerManager
            {
                _registeredPackages = PackageInfo.GetAllRegisteredPackages(),
                _gppExtensions = new GppPackage[]
                {
                    new IAP(),
                    new GoogleAnalytics(),
                    new FirebasePush(),
                    // new Braze(),
                    new GoogleSignIn(),
                    new GooglePlayGames(),
                    new Steam(),
                    new XboxPc(),
                    new XboxConsole(),
                    new Ps5(),
                    new MacAppstore(),
                    new EpicGames()
                }
            };
            PackageManagerExtensions.RegisterExtension(packagerManager);
        }

        public VisualElement CreateExtensionUI()
        {
            _uiDocument = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIDocumentFilePath);
            _root = _uiDocument.CloneTree();
            var packageScrollView = _root.Q<ScrollView>("Packages");
            var mobileContainer = _root.Q<VisualElement>("MobileContainer");
            var pcContainer = _root.Q<VisualElement>("PcContainer");
            var consoleContainer = _root.Q<VisualElement>("ConsoleContainer");

            foreach (var gppPackage in _gppExtensions)
            {
                var package = _uiDocument.CloneTree().Q<VisualElement>("PackageTemplate");
                package.style.display = DisplayStyle.Flex;
                package.viewDataKey = gppPackage.PackageName;
                package.Q<Label>("DisplayName").text = gppPackage.DisplayName;
                package.Q<Label>("PackageName").text = gppPackage.PackageName;
                package.Q<Label>("Description").text = gppPackage.Description;
                var installButton = package.Q<Button>("InstallButton");
                if (ExistPackage(gppPackage.PackageName))
                {
                    installButton.text = "Installed";
                    installButton.SetEnabled(false);
                }
                else
                {
                    installButton.clicked += () =>
                    {
                        List<string> installPackages = new List<string>
                        {
                            gppPackage.Url
                        };
                        if (gppPackage.PackageName.Equals("com.krafton.gpp.sdk.unity.steam", StringComparison.OrdinalIgnoreCase))
                        {
                            installPackages.Add(SteamWorksUrl);
                        }

                        _request = Client.AddAndRemove(installPackages.ToArray());
                        EditorApplication.update += UpdatePackage;
                    };
                }

                switch (gppPackage.SupportPlatform)
                {
                    case PackagePlatform.Mobile:
                        mobileContainer.style.display = DisplayStyle.Flex;
                        mobileContainer.Add(package);
                        break;
                    case PackagePlatform.PC:
                        pcContainer.style.display = DisplayStyle.Flex;
                        pcContainer.Add(package);
                        break;
                    case PackagePlatform.Console:
                        consoleContainer.style.display = DisplayStyle.Flex;
                        consoleContainer.Add(package);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _root.style.display = DisplayStyle.None;
            return _root;
        }

        private bool ExistPackage(string packageName)
        {
            if (_registeredPackages == null)
            {
                RefreshRegisteredPackages();
            }

            return _registeredPackages?.Any(package => package.name == packageName) ?? false;
        }

        private void RefreshRegisteredPackages()
        {
            _registeredPackages = PackageInfo.GetAllRegisteredPackages();
        }

        private void UpdatePackage()
        {
            if (!_request.IsCompleted)
            {
                return;
            }

            RefreshRegisteredPackages();
            EditorApplication.update -= UpdatePackage;
            if (_request.Error != null)
            {
                Debug.Log($"Error: {_request.Error.ToPrettyJsonString()}");
            }
        }

        public void OnPackageAddedOrUpdated(PackageInfo packageInfo)
        {
            if (_gppExtensions.Any(extension => extension.PackageName.Equals(packageInfo.name, StringComparison.OrdinalIgnoreCase)))
            {
                var packageTemplate = _root?.Q<VisualElement>(null, packageInfo.name);
                if (packageTemplate == null)
                {
                    return;
                }

                var installButton = packageTemplate.Q<Button>("InstallButton");
                installButton.text = "Installed";
                installButton.SetEnabled(false);
            }

            if (packageInfo.name.Equals(CorePackageName, StringComparison.OrdinalIgnoreCase))
            {
                PackageInfo.GetAllRegisteredPackages()
                    .Where(x => x.assetPath.Contains("github"))
                    .Where(x => x.name.Contains(CorePackageName, StringComparison.OrdinalIgnoreCase))
                    .Where(x => { return _gppExtensions.Any(extension => extension.PackageName.Equals(x.name, StringComparison.OrdinalIgnoreCase)); })
                    .Select(x => { return _gppExtensions.First(extension => extension.PackageName.Equals(x.name, StringComparison.OrdinalIgnoreCase)); })
                    .ToList()
                    .ForEach(x => Client.Add(x.Url));
            }
        }

        public void OnPackageRemoved(PackageInfo packageInfo)
        {
            try
            {
                var gppPackage = _gppExtensions.First(extension => extension.PackageName.Equals(packageInfo.name, StringComparison.OrdinalIgnoreCase));
                var packageTemplate = _root?.Q<VisualElement>(null, packageInfo.name);
                if (packageTemplate == null)
                {
                    return;
                }

                var installButton = packageTemplate.Q<Button>("InstallButton");
                installButton.text = "Install";
                installButton.SetEnabled(true);
                installButton.clicked += () =>
                {
                    _request = Client.Add(gppPackage.Url);
                    EditorApplication.update += UpdatePackage;
                };
            }
            catch (Exception)
            {
                // Do nothing....
            }
        }

        public void OnPackageSelectionChange(PackageInfo packageInfo)
        {
            if (_root != null && packageInfo != null)
            {
                _root.style.display = packageInfo.name.Equals(CorePackageName, StringComparison.OrdinalIgnoreCase)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }
    }
}

#endif