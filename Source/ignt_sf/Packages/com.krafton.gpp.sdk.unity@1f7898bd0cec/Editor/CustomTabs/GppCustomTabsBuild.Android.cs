#if UNITY_EDITOR && UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Gpp.Config;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Gpp.Build
{
    public class GppCustomTabsBuild_Android : IPreprocessBuildWithReport
    {
        private static readonly string CustomTabActivity = "com.krafton.gppwebview.GppCustomTabActivity";
        private static readonly string ManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        private static readonly string Scheme = "kraftonsdk";

        public int callbackOrder => 9999;

        private static void CreateManifestElement(XmlDocument manifest, XmlNode applicationNode)
        {
            var configFile = GppConfigSO.LoadFromFile();
            XmlElement activityElement = manifest.CreateElement("activity");
            activityElement.SetAttribute("android__theme", "@style/Theme.AppCompat.Light.NoActionBar");
            activityElement.SetAttribute("android__name", CustomTabActivity);
            activityElement.SetAttribute("android__configChanges", "keyboard|keyboardHidden|screenLayout|screenSize|orientation");
            activityElement.SetAttribute("android__label", "@string/app_name");
            activityElement.SetAttribute("android__launchMode", "singleTop");
            activityElement.SetAttribute("android__exported", "true");
            XmlElement intentFilterElement = manifest.CreateElement("intent-filter");
            intentFilterElement.SetAttribute("android__autoVerify", "true");
            XmlElement actionElement = manifest.CreateElement("action");
            actionElement.SetAttribute("android__name", "android.intent.action.VIEW");
            XmlElement categoryBrowsableElement = manifest.CreateElement("category");
            categoryBrowsableElement.SetAttribute("android__name", "android.intent.category.BROWSABLE");
            XmlElement categoryDefaultElement = manifest.CreateElement("category");
            categoryDefaultElement.SetAttribute("android__name", "android.intent.category.DEFAULT");
            intentFilterElement.AppendChild(actionElement);
            intentFilterElement.AppendChild(categoryBrowsableElement);
            intentFilterElement.AppendChild(categoryDefaultElement);
            
            HashSet<string> namespaces = configFile.stages
                .Select(stage => stage.Namespace.ToLower())
                .ToHashSet();

            foreach (string ns in namespaces)
            {
                if (string.IsNullOrWhiteSpace(ns))
                    continue;
                    
                XmlElement dataElement = manifest.CreateElement("data");
                dataElement.SetAttribute("android__scheme", Scheme);
                dataElement.SetAttribute("android__host", ns);
                intentFilterElement.AppendChild(dataElement);
            }
            
            activityElement.AppendChild(intentFilterElement);
            applicationNode.AppendChild(activityElement);
        }
        
        private static void AddQueriesElement(XmlDocument manifest)
        {
            XmlElement manifestRoot = manifest.DocumentElement;

            // 이미 queries 요소가 있는지 확인
            XmlNode queriesNode = manifestRoot.ChildNodes.OfType<XmlNode>()
                .FirstOrDefault(node => node.Name == "queries");

            if (queriesNode == null)
            {
                // queries 요소가 없으면 새로 생성
                queriesNode = manifest.CreateElement("queries");
                manifestRoot.AppendChild(queriesNode);
            }

            // 이미 CustomTabsService 관련 intent가 있는지 확인
            XmlNode intentNode = queriesNode.ChildNodes.OfType<XmlNode>()
                .FirstOrDefault(node => node.Name == "intent" && 
                                        node["action"] != null && 
                                        node["action"].Attributes["android:name"]?.Value == "android.support.customtabs.action.CustomTabsService");

            if (intentNode == null)
            {
                // intent와 action 요소 생성
                XmlElement intentElement = manifest.CreateElement("intent");
                XmlElement actionElement = manifest.CreateElement("action");
                actionElement.SetAttribute("android__name", "android.support.customtabs.action.CustomTabsService");

                // intent 안에 action을 추가하고 queries에 intent 추가
                intentElement.AppendChild(actionElement);
                queriesNode.AppendChild(intentElement);
            }
        }

        private static void SaveManifestFile()
        {
            TextReader manifestReader = new StreamReader(ManifestPath);
            string manifestContent = manifestReader.ReadToEnd();
            manifestReader.Close();

            Regex regex = new Regex("android__");
            manifestContent = regex.Replace(manifestContent, "android:");

            TextWriter manifestWriter = new StreamWriter(ManifestPath);
            manifestWriter.Write(manifestContent);
            manifestWriter.Close();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            XmlDocument manifest = new XmlDocument();
            manifest.Load(ManifestPath);

            XmlElement manifestRoot = manifest.DocumentElement;
            XmlNode applicationNode = null;

            foreach (XmlNode node in manifestRoot.ChildNodes)
            {
                if (node.Name == "application")
                {
                    applicationNode = node;
                    break;
                }
            }

            XmlNode delNode = applicationNode.ChildNodes.OfType<XmlNode>()
                .FirstOrDefault(node => node.Name == "activity" && node.Attributes["android:name"].Value == CustomTabActivity);

            if (delNode != null)
            {
                applicationNode.RemoveChild(delNode);
            }

            CreateManifestElement(manifest, applicationNode);
    
            // <queries> 요소 추가
            AddQueriesElement(manifest);
    
            manifest.Save(ManifestPath);
            SaveManifestFile();
        }
    }
}
#endif