using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Markup;

namespace MLTraderRiskMnter
{
    /// <summary>
    /// App.xaml ªº¤¬°ÊÅÞ¿è
    /// </summary>
    public partial class App : Application
    {
        protected   override   void OnStartup(StartupEventArgs e)
         { 
                base .OnStartup(e); 
                LoadResources(); 
         }

        private void LoadResources()
        {
            try
            {
                ResourceDictionary resDic = null;
                if(!System.IO.Directory.Exists("Resource")){
                    return;
                }
                string[] files = System.IO.Directory.GetFiles("Resource");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine(files[i]);
                    XmlReader XmlRead = XmlReader.Create(files[i]);
                    resDic = (ResourceDictionary)XamlReader.Load(XmlRead);
                    XmlRead.Close();
                    this.Resources.MergedDictionaries.Add(resDic);
                }

            }
            catch (Exception e)
            {

            }
        } 
    }
}
