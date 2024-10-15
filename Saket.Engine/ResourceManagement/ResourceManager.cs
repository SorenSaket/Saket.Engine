﻿using Saket.Engine.Graphics;
using Saket.Engine.ResourceManagement.Databases;
using Saket.Engine.ResourceManagement.Loaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Saket.Engine
{
    // Load items from assets folder
    //
    // Embed all required resources

    
    public class ResourceManager
    {
        public List<Database> databases = new List<Database>();

        private Dictionary<Type, ResourceLoader> loaders = new Dictionary<Type, ResourceLoader>();


        /// <summary>
        /// Key: Public facing string key
        /// Value: 
        /// </summary>
        private Dictionary<string, object> resources = new Dictionary<string, object>();


        public void RegisterLoader<T>(ResourceLoader<T> loader)
        {
            if (loaders.ContainsKey(typeof(T)))
                throw new Exception("Loader already exist");

            loaders.Add(typeof(T), loader);
        }


        // Single entry point for loading assets
        public T Load<T>(string asset_name)
        {
            string name = asset_name;

            // wtf
            if(typeof(T) == typeof(Shader))
                name = "shader_" + asset_name;
            else if (typeof(T) == typeof(ImageTexture))
                name = "sheet_" + asset_name;

            // if the asset already exists
            if (resources.ContainsKey(asset_name))
                return (T)resources[asset_name];

            if (loaders.ContainsKey(typeof(T)))
            {
                T obj = ((ResourceLoader<T>)loaders[typeof(T)]).Load(asset_name,this);
                resources.Add(name, obj);
                return obj;
            }
            else
            {
                throw new Exception($"Resource Loader doesn't exsist for type {typeof(T).Name}.");
            }

           
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resource_name"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryLoad<T>(string resource_name, T value)
		{
			
			throw new NotImplementedException();
		}



  /*
        private static TextureSheet? LoadTexture(string textureName)
        {

        }*/





        public bool TryGetStream(string name, out Stream stream)
        {
            for (int i = 0; i < databases.Count; i++)
            {
                if (databases[i].AvaliableResources.Contains(name))
                {
                    stream = databases[i].TryGetStream(name)!;
                    if(stream != null)
                        return true;
                }
            }
            stream = null!;
            return false;
        }

        public static Stream? OpenEmbeddedAssetStream(string name)
        {
            return Assembly.GetEntryAssembly()?.GetManifestResourceStream(name);
        }
        public static string[] GetAllResourceNames()
        {
            return Assembly.GetEntryAssembly()?.GetManifestResourceNames() ?? new string[0];
        }
    }
}