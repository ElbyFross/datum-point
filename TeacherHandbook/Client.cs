﻿// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TeacherHandbook
{
    /// <summary>
    /// Instance of this class will provide client features.
    /// </summary>
    public class Client : UniformClient.BaseClient
    {
        /// <summary>
        /// Reference to current client provider.
        /// Always return not null result.
        /// </summary>
        public static Client Active
        {
            get
            {
                if (active == null)
                    active = new Client();
                return active;
            }

            protected set { active = value; }
        }

        protected static Client active;

        public Client()
        {
            // Set as active.
            Active = this;

            // Loading assemblies from lib direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");
            
            // Loading assemblies from plugins direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Load translation for plugins relative to thread culture.
            LoadXAML_LangDicts(CultureInfo.CurrentCulture, new CultureInfo("en-US"));
        }

        /// <summary>
        /// Make first start of client.
        /// </summary>
        public static void Init()
        {
            _ = Active;
        }

        /// <summary>
        /// Scaning for language dictionaries in XAML files, and load them to Merged dictionaries.
        /// Load relative to new culture if found. Leave previous culture if not.
        /// 
        /// Require files format: *.lang.CULTURE_CODE.xaml, where culture code equal current translation of the app. 
        /// Example: plugin.feed.lang.en-US.xaml
        /// </summary>
        /// <param name="targetCulture">Culture that will be serched with hightest priority.</param>
        /// <param name="secondaryCulture">Culture that will be prefured in case of target not implemented. If also not implemented than will be used first entry.</param>
        public static void LoadXAML_LangDicts(CultureInfo targetCulture, CultureInfo secondaryCulture)
        {
            #region Validate and fix base conditions
            // Validate directory.
            if (!Directory.Exists(UniformClient.Plugins.Constants.PLUGINS_DIR))
            {
                Directory.CreateDirectory(UniformClient.Plugins.Constants.PLUGINS_DIR);
                Console.WriteLine("PLUGINS DIRECTORY NOT FOUND. NEW ONE WAS CREATED.");
            }
            #endregion

            #region Find localization files
            // Load all lang files.
            Regex searchPattern = new Regex(@"\w*.lang.[0-9a-z-]*.xaml", RegexOptions.IgnoreCase);
            var xamlDicts = Directory.EnumerateFiles(UniformClient.Plugins.Constants.PLUGINS_DIR, "*.xaml", SearchOption.AllDirectories)
                .Where(s => searchPattern.IsMatch(s));


            // Detect plugins domains and select more relevant.
            Hashtable pluginDomainsMap = new Hashtable();

            string rootName = null; // Varaiable that avoid allocating of memmory on every loop's step.
            string cultureBufer = null; // Variable that contain temporal culture code.
            int langIndex = 0; // Bufer that avoid allocating for every loop's step.

            // Register every found dictionary.
            foreach (string domain in xamlDicts)
            {
                // Get plugin domain.
                rootName = domain.Substring(domain.LastIndexOf('\\') + 1);
                langIndex = rootName.LastIndexOf(".lang.");

                // Detect file culture.
                cultureBufer = rootName.Substring(langIndex + 6);
                cultureBufer = cultureBufer.Substring(0, cultureBufer.IndexOf('.'));

                rootName = rootName.Substring(0, langIndex);

                // Load map list for this domain.
                if (!(pluginDomainsMap[rootName] is List<DomainContainer> domainMap))
                {
                    // Create new if not found.
                    domainMap = new List<DomainContainer>();
                    pluginDomainsMap.Add(rootName, domainMap);
                }
                // Add data to list.
                domainMap.Add(new DomainContainer() { cultureKey = cultureBufer, pluginDomain = rootName, path = domain });
            }

            // Select most relevant domains.
            List<DomainContainer> relevantDomains = new List<DomainContainer>();
            foreach(string domain in pluginDomainsMap.Keys)
            {
                bool detected = false;
                // Load map list for this domain.
                if (pluginDomainsMap[domain] is List<DomainContainer> domainMap)
                {
                    DomainContainer reservContainer = null;
                    foreach (DomainContainer dc in domainMap)
                    {
                        // If target found.
                        if (dc.cultureKey == targetCulture.Name)
                        {
                            detected = true;
                            relevantDomains.Add(dc);
                            break;
                        }

                        // If found secondary culture contaier then save it.
                        if (dc.cultureKey == secondaryCulture.Name)
                        {
                            reservContainer = dc;
                        }
                    }

                    // Start next domain if found.
                    if (detected) continue;

                    // Apply reserv contaier if found.
                    if(reservContainer != null)
                    {
                        relevantDomains.Add(reservContainer);
                        continue;
                    }

                    // Apply first detected localization file domain.
                    if (domainMap.Count > 0)
                    {
                        relevantDomains.Add(domainMap[0]);
                    }
                }
            }
            #endregion

            #region Change localization.
            // Bufer for dict loading. 
            foreach (DomainContainer domain in relevantDomains)
            {
                // Dict pattern.
                Regex regex = new Regex(@"\w*" + domain.pluginDomain + ".lang.[0-9a-z-]*.xaml", RegexOptions.IgnoreCase);

                // Look for conflict dictionary among loaded.
                ResourceDictionary rdForRemove = null;
                foreach (ResourceDictionary conflict_rd in Application.Current.Resources.MergedDictionaries)
                {
                    // Check os the file if match to patern.
                    if (regex.IsMatch(conflict_rd.Source.OriginalString))
                    {
                        // Set as target for remove.
                        rdForRemove = conflict_rd;
                        break;
                    }
                }

                // Load new as source.
                string formatedPath = domain.path.Replace("\\", "/");
                ResourceDictionary myResourceDictionary = new ResourceDictionary();
                myResourceDictionary.Source =  new Uri("pack://siteoforigin:,,,/" + formatedPath, UriKind.Absolute);

                // Remove conflict dictionary if found and insert new.
                if (rdForRemove != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(rdForRemove);
                    Application.Current.Resources.MergedDictionaries.Remove(rdForRemove);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, myResourceDictionary);
                }
                // Add as new if conflicts not found.
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);

                    Collection<ResourceDictionary> col = Application.Current.Resources.MergedDictionaries;
                }
            }
            #endregion

            // Update culture.
            System.Threading.Thread.CurrentThread.CurrentUICulture = targetCulture;
        }

        class DomainContainer
        {
            public string cultureKey;
            public string pluginDomain;
            public string path;
        }

    }
}