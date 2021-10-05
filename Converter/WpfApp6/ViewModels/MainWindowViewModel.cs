using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp6.infrastructure.commands;
using WpfApp6.Models;
using WpfApp6.ViewModels.Base;

namespace WpfApp6.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Поля
        static string url = @"https://www.cbr-xml-daily.ru/daily_json.js";
        static string currencyData;                             // Данные валют
        static JToken currency;                                 // Все валюты спаршенные и привидённые к токену
        static List<Currency> ListCurrency;                     // список валют
        static Dictionary<string, double> currencyRatesAndCode; // Словарь для поиска валюты по коду

        static bool isChangedComboBox1 = true;
        static bool isChangedComboBox2 = true;
        #endregion



        #region Основные команды

        private string GetRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.ContentType = "application/json";

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            var content = new StreamReader(stream).ReadToEnd();

            return content;
        }

        private void UpdateCarrencyData(string url)
        {
            currencyData = GetRequest(url);

            ListCurrency = new List<Currency>();
            currencyRatesAndCode = new Dictionary<string, double>();


            var data = JObject.Parse(currencyData);
            currency = data["Valute"];

            foreach (var valute in currency)
            {
                foreach (var item in valute)
                {
                    Currency currency = new Currency();

                    currency.ID = item["ID"].ToString();
                    currency.NumCode = item["NumCode"].ToString();
                    currency.CharCode = item["CharCode"].ToString();
                    currency.Nominal = Convert.ToInt32(item["Nominal"].ToString());
                    currency.Name = item["Name"].ToString();
                    currency.Value = Convert.ToDouble(item["Value"].ToString());
                    currency.Previous = Convert.ToDouble(item["Previous"].ToString());

                    string charCode = item["CharCode"].ToString();
                    double value = Convert.ToDouble(item["Value"].ToString());

                    ListCurrency.Add(currency);
                    currencyRatesAndCode.Add(charCode, value);

                }
            }

            AddDataToTheComboBox();
        }

        private void MyConstruct()
        {
            UpdateCarrencyData(url);

        }

        private void AddDataToTheComboBox()
        {
            List<string> myData = new List<string>();

            foreach (var item in ListCurrency)
            {
                myData.Add(item.Name);
            }
            CurrencyComboBox1 = myData;
            CurrencyComboBox2 = myData;
        }


        #endregion



        #region Кнопки

        #region Кнопка buttonUpdate

        public ICommand ButtonUpdateCommand { get; }

        private void OnButtonUpdateExecuted(object p)
        {

            List<string> myData = new List<string>();

            foreach (var item in ListCurrency)
            {
                string text = string.Format("{0,-40} {1}", item.Name, item.Value);
                myData.Add(text);
            }
            ListBoxUpdate = myData;

        }

        private bool CanButtonUpdateExecute(object p) => true;

        #endregion

        #region Кнопка ButtonDownload

        public ICommand ButtonDownloadCommand { get; }

        private void OnButtonDownloadExecuted(object p)
        {
            UpdateCarrencyData(url);

            List<string> myData = new List<string>();

            foreach (var item in ListCurrency)
            {
                myData.Add(item.CharCode);
            }
            ListBoxDownload = myData;

        }

        private bool CanButtonDownloadExecute(object p) => true;

        #endregion

        #region Кнопка ButtonSearch

        public ICommand ButtonSearchCommand { get; }

        private void OnButtonSearchExecuted(object p)
        {
            string code = TextBoxSearch;

            List<string> myData = new List<string>();

            if (code != null)
            {
                code = code.ToUpper();

                foreach (var valute in currency)
                {
                    foreach (var codeValute in valute)
                    {
                        var charCode = codeValute["CharCode"].ToString();

                        if (charCode == code)
                        {
                            var rates = codeValute["Value"].ToString();

                            myData.Add(rates);

                        }
                    }
                }

                ListBoxSearch = myData;
            }

        }

        private bool CanButtonSearchExecute(object p) => true;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            MyConstruct();


            #region Команды

            ButtonUpdateCommand = new LambdaCommand(OnButtonUpdateExecuted, CanButtonUpdateExecute);
            ButtonDownloadCommand = new LambdaCommand(OnButtonDownloadExecuted, CanButtonDownloadExecute);
            ButtonSearchCommand = new LambdaCommand(OnButtonSearchExecuted, CanButtonSearchExecute);

            #endregion
        }





        #region Свойства


        #region ListBox

        #region ListBoxUpdate

        private List<string> listBoxUpdate;

        public List<string> ListBoxUpdate
        {
            get => listBoxUpdate;
            set => Set(ref listBoxUpdate, value);
        }

        #endregion

        #region ListBoxDownload

        private List<string> listBoxDownload;

        public List<string> ListBoxDownload
        {
            get => listBoxDownload;
            set => Set(ref listBoxDownload, value);
        }

        #endregion

        #region ListBoxSearch

        private List<string> listBoxSearch;

        public List<string> ListBoxSearch
        {
            get => listBoxSearch;
            set => Set(ref listBoxSearch, value);
        }

        #endregion

        #endregion


        #region ComboBox

        #region CurrencyComboBox1

        private List<string> currencyComboBox1;

        public List<string> CurrencyComboBox1
        {
            get => currencyComboBox1;
            set
            {
                if (Equals(textBoxCurrency1, value)) return;

                currencyComboBox1 = value;
                OnPropertyChanged();

                isChangedComboBox1 = true;
            }
        }

        #endregion

        #region CurrencyComboBox2

        private List<string> currencyComboBox2;

        public List<string> CurrencyComboBox2
        {
            get => currencyComboBox2;
            set
            {
                if (Equals(currencyComboBox2, value)) return;

                currencyComboBox2 = value;
                OnPropertyChanged();

                isChangedComboBox2 = true;
            }
        }

        #endregion

        #endregion


        #region TextBox

        #region TextBoxSearch

        private string textBoxSearch;

        public string TextBoxSearch
        {
            get => textBoxSearch;
            set => Set(ref textBoxSearch, value);
        }

        #endregion

        #region TextBoxCurrency1

        private string textBoxCurrency1;

        public string TextBoxCurrency1
        {
            get => textBoxCurrency1;
            set
            {
                if (Equals(textBoxCurrency1, value)) return;

                textBoxCurrency1 = value;
                OnPropertyChanged();


                string currencyName = CurrencyComboBox2.First();


                foreach (var valute in currency)
                {
                    foreach (var codeValute in valute)
                    {
                        var name = codeValute["Name"].ToString();

                        if (currencyName == name)
                        {
                            var valueToConvertString = codeValute["Value"].ToString();
                            double valueToConvert = Convert.ToDouble(valueToConvertString);

                            string myValueString = TextBoxCurrency1;
                            double myValue = Convert.ToDouble(myValueString);

                            double data = myValue / valueToConvert;


                            if (isChangedComboBox1)
                            {
                                isChangedComboBox1 = false;
                                isChangedComboBox2 = false;

                                TextBoxCurrency2 = data.ToString();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region TextBoxCurrency2

        private string textBoxCurrency2;

        public string TextBoxCurrency2
        {
            get => textBoxCurrency2;
            set
            {

                if (Equals(textBoxCurrency2, value)) return;

                textBoxCurrency2 = value;
                OnPropertyChanged();



                string currencyName = CurrencyComboBox1.First();


                foreach (var valute in currency)
                {
                    foreach (var codeValute in valute)
                    {
                        var name = codeValute["Name"].ToString();

                        if (currencyName == name)
                        {
                            var valueToConvertString = codeValute["Value"].ToString();
                            double valueToConvert = Convert.ToDouble(valueToConvertString);

                            string myValueString = TextBoxCurrency2;
                            double myValue = Convert.ToDouble(myValueString);

                            double data = myValue / valueToConvert;

                            if (isChangedComboBox2)
                            {
                                isChangedComboBox1 = false;
                                isChangedComboBox2 = false;

                                TextBoxCurrency2 = data.ToString();
                            }
                        }
                    }
                }


            }
        }








        #endregion

        #endregion



        #endregion
    }
}
