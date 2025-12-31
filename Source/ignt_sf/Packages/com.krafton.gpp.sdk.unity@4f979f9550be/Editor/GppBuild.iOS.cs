#if UNITY_IOS && UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Gpp.Build
{
    public class GppBuild
    {
        internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath);
            if (File.Exists(dstPath))
                File.Delete(dstPath);

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }

        [PostProcessBuild(1000)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;
            rootDict.SetString("NSMicrophoneUsageDescription", "This app use mic for voice chat.");

            PlistElementArray urlTypesArray;
            if (rootDict.values.TryGetValue("CFBundleURLTypes", out PlistElement existingUrlTypes))
            {
                urlTypesArray = existingUrlTypes.AsArray();
            }
            else
            {
                urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
            }
            PlistElementDict dict = urlTypesArray.AddDict();
            PlistElementArray schemesArray = dict.CreateArray("CFBundleURLSchemes");

            var urlScheme = GppConfigSO.GetActiveConfigFromFile().Namespace.Replace("_", "");

            schemesArray.AddString($"kraftonsdk{urlScheme}");

            plist.WriteToFile(plistPath);

            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject pBXProject = new PBXProject();
            pBXProject.ReadFromString(File.ReadAllText(projPath));
            string target = pBXProject.GetUnityMainTargetGuid();
            string targetUnityFramework = pBXProject.GetUnityFrameworkTargetGuid();

            string swiftFileName = "Dummy.swift";
            string swiftFilePath = Path.Combine(path, swiftFileName);
            File.WriteAllText(swiftFilePath, "// Dummy Swift file\n");

            pBXProject.AddFileToBuild(target, pBXProject.AddFile(swiftFilePath, swiftFileName));
            pBXProject.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
            pBXProject.AddFileToBuild(targetUnityFramework, pBXProject.AddFile(swiftFilePath, swiftFileName));
            pBXProject.SetBuildProperty(targetUnityFramework, "SWIFT_VERSION", "5.0");

            string bridgingHeaderPath = Path.Combine(path, "Unity-iPhone-Bridging-Header.h");
            if (!File.Exists(bridgingHeaderPath))
            {
                File.WriteAllText(bridgingHeaderPath, "// Unity-iPhone Bridging Header\n");
            }
            pBXProject.SetBuildProperty(target, "SWIFT_OBJC_BRIDGING_HEADER", "Unity-iPhone-Bridging-Header.h");

            pBXProject.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
            pBXProject.SetBuildProperty(targetUnityFramework, "CLANG_ENABLE_MODULES", "YES");

            pBXProject.WriteToFile(projPath);

            var pcm = new ProjectCapabilityManager(projPath, "Unity-iPhone.entitlements", "Unity-iPhone", target);
            pcm.AddKeychainSharing(new string[] { "$(AppIdentifierPrefix)com.krafton.gpp.keychainshare" });
            pcm.WriteToFile();
        }

        public static void GetDirs(string dirPath, ref List<string> dirs)
        {
            foreach (string path in Directory.GetFiles(dirPath))
            {
                if (path.IndexOf(".") != 0 && System.IO.Path.GetExtension(path) != ".meta")
                {
                    dirs.Add(path.Substring(path.IndexOf("iOS")));
                }
            }

            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    GetDirs(path, ref dirs);
                }
            }

        }
    }
}
#endif