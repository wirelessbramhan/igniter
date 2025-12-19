using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
#endif
using UnityEngine;

namespace Gpp.Extensions
{
    public abstract class GppExtension<TInterface, TGppExtension>
        where TInterface : class
        where TGppExtension : GppExtension<TInterface, TGppExtension>, new()
    {
        private static readonly Lazy<TGppExtension> Lazy = new(() => new TGppExtension());
        protected static TGppExtension Instance => Lazy.Value;

        protected TInterface _impl;
        protected bool _init;

        public static bool CanUse()
        {
            Init();
            return Instance.IsSupportPlatform() && Instance._impl != null;
        }

        public static TInterface Impl()
        {
            Init();
            return Instance._impl;
        }

        private static void Init()
        {
            if (Instance._init)
            {
                return;
            }

            if (!Instance.IsSupportPlatform())
            {
                return;
            }

            Instance._impl = LoadExtension(Instance.TargetClassPath(), Instance.PackageName());
            Instance._init = true;
        }

        protected static TInterface LoadExtension(string targetClassPath, string packageName)
        {
            TInterface extensionImpl = default;
            Type extensionType = Type.GetType($"{targetClassPath}, {packageName}");
            if (extensionType != null && typeof(TInterface).IsAssignableFrom(extensionType))
            {
                extensionImpl = (TInterface)Activator.CreateInstance(extensionType);
            }

            return extensionImpl;
        }

#if UNITY_EDITOR
        public static bool IsPackageInstalled()
        {
            ListRequest listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
            {
                // Wait for the request to complete
            }

            if (listRequest.Status == StatusCode.Success)
            {
                return listRequest.Result.Any(package => package.name == Instance.PackageName());
            }

            Debug.LogError("Failed to list packages: " + listRequest.Error.message);
            return false;
        }
#endif

        protected abstract bool IsSupportPlatform();
        protected abstract string TargetClassPath();
        protected abstract string PackageName();
    }
}