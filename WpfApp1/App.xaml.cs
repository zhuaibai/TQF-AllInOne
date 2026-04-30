using System.Collections;
using System.Configuration;
using System.Data;
using System.Windows;
using static WpfApp1.ViewModels.MainWindowVM;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public static ResourceDictionary resourceDictionary;

        public static Action<string> ChangeLanguageWithSetting;

        /// <summary>
        /// 切换界面语言
        /// </summary>
        /// <param name="lan"></param>
        /// <returns></returns>
        public static bool UpdateLanguage(string lan)
        {
            bool res = false;

            // 获取配置
            string requestedLanguage = $"Dictionary\\{lan}.xaml";
            resourceDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));

            if (resourceDictionary != null)
            {
                Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Current.Resources.MergedDictionaries.Add(resourceDictionary);

                // 保存配置
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationManager.AppSettings["Language"] = lan;
                config.Save(ConfigurationSaveMode.Modified);
                //刷新
                ConfigurationManager.RefreshSection("appSettings");

                res = true;
                if (res)
                {
                    ChangeLanguageWithSetting?.Invoke("");
                }
            }

            return res;
        }

        /// <summary>
        /// 通过键来获取值，可实现多语言切换
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetText(string key)
        {
            string res = string.Empty;

            if (resourceDictionary != null)
            {
                var dictionary = resourceDictionary.Cast<DictionaryEntry>();
                var item = dictionary.FirstOrDefault(r => r.Key.ToString() == key);
                if (item.Value != null)
                {
                    res = item.Value.ToString();
                }
                else
                {
                    res = "key搞错了";
                }
            }
            return res;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _cts?.Cancel();
            if (AppServices.CurrentBlueTooth?.IsConnected() == true)
            {
                try
                {
                    await AppServices.CurrentBlueTooth.DisconnectAsync();
                    await Task.Delay(500);
                }
                catch { }
            }
            (AppServices.CurrentBlueTooth as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }

      

    }
