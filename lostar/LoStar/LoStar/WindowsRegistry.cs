//-----------------------------------------------------------------------
// <copyright file="WindowsRegistry.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Win32;

    /// <summary>
    /// This class contains all the registry access methods.
    /// It is possible to define the subfolder where to look for the registry entries.
    /// The class exposes a number <c>polymorfic</c> static methods named Get and Set, each one has as a first
    /// parameter the key in the GHMU registry sub key and a parameter of the selected type.
    /// There are getters and setters for strings, booleans, integers and lists of strings.
    /// <para />
    /// For instance, to save an integer under the name <c>"IntName"</c>, the following call should be made
    ///     <c>Registry.Set("IntName", 9);</c>
    /// while retrieving the same number should be
    ///     <c>int n;
    ///     Registry.Get("IntName", out n, defaultValue);</c>
    ///     where defaultValue is an integer number returned in n if the key is not present in the registry.
    /// </summary>
    public static class WindowsRegistry
    {
        /// <summary>
        /// All the registry values are stored in the SOFTWARE\LoStar
        /// </summary>
        private static string basePath = "SOFTWARE\\LoStar";

        /// <summary>
        /// Determines if the passed (absolute) key exists.
        /// </summary>
        /// <param name="absoluteKey">Absolute Key, i.e. path of a key from the root of register.</param>
        /// <returns>Returns true if the passed (absolute) key exists.</returns>
        public static bool ExistsAbsoluteKeyClassesRoot(string absoluteKey)
        {
            var subTree = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(absoluteKey);
            return subTree != null;
        }

        #region String getter and setter
        /// <summary>
        /// Gets a value from the registry sub tree of LoStart
        /// If the LoStar sub tree is not present, it creates it
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="defaultValue">default value to associate to the key if not present</param>
        /// <returns>value for the key</returns>
        public static string Get(string key, string defaultValue = null) 
        {
            var lostarRegistry = OpenLostarSubKey();

            if (defaultValue == null)
            {
                return (string)lostarRegistry.GetValue(key);
            }
            else
            {
                return (string)lostarRegistry.GetValue(key, defaultValue);
            }
        }

        /// <summary>
        /// Associates a key to a value in the GMHU sub tree of the registry
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="value">value to associate</param>
        public static void Set(string key, string value)
        {
            var lostarRegistry = OpenLostarSubKey(true);
            lostarRegistry.SetValue(key, value);
        }
        #endregion
        #region bool getter and setter
        /// <summary>
        /// Decodes a boolean value from the registry
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="value">boolean associated to the key</param>
        /// <param name="defValue">default value to be used if no key is present</param>
        /// <returns>String representation of the value</returns>
        public static string Get(string key, out bool value, bool defValue)
        {
            string v = Get(key, defValue ? "1" : "0");
            if (v == "1")
            {
                value = true;
            }
            else
            {
                value = false;
            }

            return v;
        }

        /// <summary>
        /// Associates a boolean value to a registry key
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="value">boolean value to associate</param>
        public static void Set(string key, bool value)
        {
            Set(key, value ? "1" : "0");
        }
        #endregion
        #region int getter and setter
        /// <summary>
        /// Associates an integer value to a registry key
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="value">integer value to associate</param>
        public static void Set(string key, int value)
        {
            Set(key, value.ToString());
        }

        /// <summary>
        /// Decodes an integer value from the registry
        /// </summary>
        /// <param name="key">key in the registry under the GMHU sub tree to search</param>
        /// <param name="value">integer associated to the key</param>
        /// <param name="defValue">default value to be used if no key is present</param>
        /// <returns>String representation of the value</returns>
        public static string Get(string key, out int value, int defValue)
        {
            string v = Get(key, defValue.ToString());
            if (!int.TryParse(v, out value))
            {
                value = defValue;
            }

            return v;
        }
        #endregion
        #region list of strings getter and setter
        /// <summary>
        /// Accesses a composite registry value
        /// </summary>
        /// <param name="key">Registry name.</param>
        /// <returns>List of values attached to the passed key. If no key was present, it returns a void list, i.e. a list containing 0 elements.</returns>
        public static List<string> Gets(string key)
        {
            var lostarRegistry = OpenLostarSubKey();
            object o = lostarRegistry.GetValue(key);
            if (o == null)
            {
                return new List<string>();
            }

            string[] ls = (string[])o;
            return new List<string>(ls);
        }

        /// <summary>
        /// Associates a key to an array of strings in the GMHU sub tree of the registry
        /// </summary>
        /// <param name="key">key in the registry</param>
        /// <param name="values">list of string to associate</param>
        public static void Set(string key, List<string> values)
        {
            var lostarRegistry = OpenLostarSubKey(true);
            string[] vals = values.ToArray();
            lostarRegistry.SetValue(key, vals);
        }
        #endregion

        /// <summary>
        /// Opens the <c>Lostar</c> registry <c>subtree</c> creating it if not existent (default)
        /// </summary>
        /// <param name="writable">True if the registry key must be writable</param>
        /// <param name="createIfDoesNotExist">True if the <c>subtree</c> must be created if it does not exist</param>
        /// <returns>A reference to the opened registry <c>subtree</c>,</returns>
        private static Microsoft.Win32.RegistryKey OpenLostarSubKey(bool writable = false, bool createIfDoesNotExist = true)
        {
            Microsoft.Win32.RegistryKey result = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(basePath, writable);
            if (createIfDoesNotExist && result == null)
            {
                result = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(basePath);
            }

            return result;
        }
    }
}
