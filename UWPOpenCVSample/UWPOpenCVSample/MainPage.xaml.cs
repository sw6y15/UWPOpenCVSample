using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OpenCvSharp;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPOpenCVSample
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();

            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;
            SampleTitle.Text = FEATURE_NAME;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            ScenarioControl.ItemsSource = scenarios;
            if (Windows.UI.Xaml.Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Called whenever the user changes selection in the scenarios list.  This method will navigate to the respective
        /// sample scenario page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the status block when navigating scenarios.
            NotifyUser(String.Empty, NotifyType.StatusMessage);

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Windows.UI.Xaml.Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        /// <summary>
        /// Display a message to the user.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

            // Raise an event if necessary to enable a screen reader to announce the status update.
            var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }
    }
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return (MainPage.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

    public class Algorithm
    {
        //public List<AlgorithmProperty> algorithmProperties { get; set; }
        public List<AlgorithmProperty> algorithmProperties { get; set; }
        public string AlgorithmName { get; set; }
        public Algorithm()
        {
        }
        public void addProperty(AlgorithmProperty additionParams)
        {
            algorithmProperties.Add(additionParams);
        }
        public void updateProperty(string pName, AlgorithmPropertyType mName, double newValue)
        {
            for (int i = 0; i < algorithmProperties.Count; i++)
            {
                if (algorithmProperties[i].ParameterName == pName)
                {
                    switch (mName)
                    {
                        case AlgorithmPropertyType.currentValue:
                            algorithmProperties[i].CurrentValue = newValue;
                            break;
                        case AlgorithmPropertyType.maxValue:
                            algorithmProperties[i].MaxValue = newValue;
                            break;
                        case AlgorithmPropertyType.minValue:
                            algorithmProperties[i].MinValue = newValue;
                            break;
                        default:
                            algorithmProperties[i].CurrentValue = newValue;
                            break;
                    }
                }
            }
        }
        public void updateCurrentValue(AlgorithmProperty newParam)
        {
            for(int i=0;i<algorithmProperties.Count;i++)
            {
                if (algorithmProperties[i].ParameterName == newParam.ParameterName)
                {
                    algorithmProperties[i].CurrentValue = newParam.CurrentDoubleValue;
                }
            }
        }
        public void updateCurrentValue(string pName, double newValue)
        {
            for (int i = 0; i < algorithmProperties.Count; i++)
            {
                if (algorithmProperties[i].ParameterName == pName)
                {
                    algorithmProperties[i].CurrentValue = newValue;
                }
            }
        }
        public void revertEnable(string ParamName)
        {
            for (int i = 0; i < algorithmProperties.Count; i++)
            {
                if (algorithmProperties[i].ParameterName == ParamName)
                {
                    if (algorithmProperties[i].IsComboBoxEnable)
                    {
                        if(algorithmProperties[i].ComboBoxVisibility== Visibility.Visible)
                        {
                            algorithmProperties[i].ComboBoxVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            algorithmProperties[i].ComboBoxVisibility = Visibility.Visible;
                        }
                    }
                    if(algorithmProperties[i].IsSliderEnable)
                    {
                        if (algorithmProperties[i].SliderVisibility == Visibility.Visible)
                        {
                            algorithmProperties[i].SliderVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            algorithmProperties[i].SliderVisibility = Visibility.Visible;
                        }
                    }

                    if (algorithmProperties[i].DetailsVisibility == Visibility.Visible)
                    {
                        algorithmProperties[i].DetailsVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        algorithmProperties[i].DetailsVisibility = Visibility.Visible;
                    }
                    //if (algorithmProperties[i].IsEnable == false)
                    //{
                    //    algorithmProperties[i].IsEnable = true;
                    //    algorithmProperties[i].isInitialize = false;
                    //    algorithmProperties[i].SettingVisibility = Visibility.Visible; 
                    //}
                    //else
                    //{
                    //    algorithmProperties[i].IsEnable = false;
                    //    algorithmProperties[i].SettingVisibility = Visibility.Collapsed;
                    //}
                }
            }
        }
        public void resetEnable()
        {
            foreach(var ap in algorithmProperties)
            {
                ap.DetailsVisibility = Visibility.Collapsed;
                ap.SliderVisibility = Visibility.Collapsed;
                ap.ComboBoxVisibility = Visibility.Collapsed;
                ap.isInitialize = false;
            }
        }
        public object findParambyName(string ParamName)
        {
            foreach(var ap in algorithmProperties)
            {
                if (ap.ParameterName == ParamName)
                {
                    return ap.CurrentValue;
                }
            }
            return null;
        }
        public static List<AlgorithmProperty> GetObjects(Algorithm algorithm)
        {
            List<AlgorithmProperty> algorithmProperties = new List<AlgorithmProperty>();
            foreach (var algorithmProperty in algorithm.algorithmProperties)
            {
                algorithmProperties.Add(algorithmProperty);
            }
            return algorithmProperties;
        }
        public void SetObjects(Algorithm algorithm)
        {
            algorithmProperties.Clear();
            foreach (var algorithmProperty in algorithm.algorithmProperties)
            {
                algorithmProperties.Add(algorithmProperty);
            }
        }
    }

    public class AlgorithmProperty : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Name of the parameter.
        private string parameterName;
        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                parameterName = value;
                NotifyPropertyChanged("ParameterName");
            }
        }

        // Description of the parameter.
        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        // Value setting panel visibility
        private Visibility sliderVisibility;
        public Visibility SliderVisibility
        {
            get { return sliderVisibility; }
            set
            {
                sliderVisibility = value;
                NotifyPropertyChanged("SliderVisibility");
            }
        }

        // Value setting panel visibility
        private Visibility comboBoxVisibility;
        public Visibility ComboBoxVisibility
        {
            get { return comboBoxVisibility; }
            set
            {
                comboBoxVisibility = value;
                NotifyPropertyChanged("ComboBoxVisibility");
            }
        }

        // Value setting panel visibility
        private Visibility detailsVisibility;
        public Visibility DetailsVisibility
        {
            get { return detailsVisibility; }
            set
            {
                detailsVisibility = value;
                NotifyPropertyChanged("DetailsVisibility");
            }
        }
        // Current value of the parameter
        private double currentValue;
        public object CurrentValue
        {
            get
            {
                if (ParamType== typeof(int))
                {
                    return (int)currentValue;
                }
                else if (ParamType == typeof(double))
                {
                    return currentValue;
                }
                else if (ParamType == typeof(OpenCvSharp.Size))
                {
                    var res = new OpenCvSharp.Size((int)currentValue, (int)currentValue);
                    return res;
                }
                //else if (ParamType == typeof(LineTypes))
                //{
                //    if ((int)currentValue == 0)
                //    {
                //        return LineTypes.Link4;
                //    }
                //    else if ((int)currentValue == 1)
                //    {
                //        return LineTypes.Link8;
                //    }
                //    else if ((int)currentValue == 2)
                //    {
                //        return LineTypes.AntiAlias;
                //    }
                //    else
                //    {
                //        return LineTypes.Link4;
                //    }
                //}
                //else if (ParamType == typeof(BorderTypes))
                //{
                //    if ((int)currentValue == 0)
                //    {
                //        return BorderTypes.Constant;
                //    }
                //    else if ((int)currentValue == 1)
                //    {
                //        return BorderTypes.Replicate;
                //    }
                //    else if ((int)currentValue == 2)
                //    {
                //        return BorderTypes.Reflect;
                //    }
                //    else if ((int)currentValue == 3)
                //    {
                //        return BorderTypes.Wrap;
                //    }
                //    else if ((int)currentValue == 4)
                //    {
                //        return BorderTypes.Reflect101;
                //    }
                //    else if ((int)currentValue == 5)
                //    {
                //        return BorderTypes.Transparent;
                //    }
                //    else if ((int)currentValue == 6)
                //    {
                //        return BorderTypes.Isolated;
                //    }
                //    else
                //    {
                //        return BorderTypes.Default;
                //    }
                //}
                else if (ParamType == typeof(Scalar))
                {
                    return (Scalar)currentValue;
                }
                else if (ParamType == typeof(OpenCvSharp.Point))
                {
                    var res = new OpenCvSharp.Point(currentValue, currentValue);
                    return res;
                }
                else if (ParamType?.BaseType == typeof(Enum))
                {
                    return ParamList[CurrentIntValue];
                }
                else
                {
                    return currentValue;
                }
            }
            set
            {
                currentValue = (double)value;
                CurrentDoubleValue = (double)value;
                CurrentStringValue = CurrentValue.ToString();
                if (ParamType?.BaseType == typeof(Enum))
                {
                    CurrentIntValue = Convert.ToInt32(value);
                }
                else
                {
                    CurrentIntValue = 0;
                }
                NotifyPropertyChanged("CurrentValue");
            }
        }

        private double currentDoubleValue;
        public double CurrentDoubleValue
        {
            get
            {
                return (double)currentDoubleValue;
            }
            set
            {
                currentDoubleValue = value;
                NotifyPropertyChanged("CurrentDoubleValue");
            }
        }

        private string currentStringValue;
        public string CurrentStringValue
        {
            set
            {
                currentStringValue = "Current Value = " + value.ToString();
                NotifyPropertyChanged("CurrentStringValue");
            }
            get
            {
                return currentStringValue;
            }
        }

        private int currentIntValue;
        public int CurrentIntValue
        {
            get
            {
                return currentIntValue;
            }
            set
            {
                currentIntValue = value;
                NotifyPropertyChanged("CurrentIntValue");
            }
        }

        // Maximum value of the parameter
        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                NotifyPropertyChanged("MaxValue");
            }
        }

        // Minimum value of the parameter
        private double minValue;
        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                NotifyPropertyChanged("MinValue");
            }
        }

        private bool isSliderEnable;
        public bool IsSliderEnable
        {
            get { return isSliderEnable; }
            set
            {
                isSliderEnable = value;
                NotifyPropertyChanged("IsSliderEnable");
            }
        }

        private bool isComboBoxEnable;
        public bool IsComboBoxEnable
        {
            get { return isComboBoxEnable; }
            set
            {
                isComboBoxEnable = value;
                NotifyPropertyChanged("IsComboBoxEnable");
            }
        }


        private string tag;
        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                NotifyPropertyChanged("Tag");
            }
        }

        private List<object> comboList;
        public List<object> ComboList
        {
            get { return comboList; }
            set
            {
                comboList = value;
                NotifyPropertyChanged("ComboList");
            }
        }

        private Type paramType;
        public Type ParamType
        {
            get { return paramType; }
            set
            {
                paramType = value;
                NotifyPropertyChanged("ParamType");
            }
        }

        private List<object> paramList;
        public List<object> ParamList
        {
            get { return paramList; }
            set
            {
                paramList = value;
                NotifyPropertyChanged("ParamList");
            }
        }
        // Converter

        // enum val
        public List<string> Selections;
        public int selectIndex;
        public bool isInitialize;

        public AlgorithmProperty(int index, Type type, string name, string description = "The default property desciption.", double max=255, double min=0,double cur=0)
        {
            ParameterName = name;
            Description = description;
            MaxValue = max;
            MinValue = min;
            CurrentValue = cur > max ? max : cur < min ? min : cur;
            ParamType = type;
            if (type.BaseType != typeof(Enum))
            {
                ParamList = null;
                IsComboBoxEnable = false;
                isSliderEnable = true;
            }
            else
            {
                var _enumval = Enum.GetValues(type).Cast<object>();
                ParamList = _enumval.ToList();
                IsComboBoxEnable = true;
                isSliderEnable = false;
            }
            selectIndex = index;
            SliderVisibility = Visibility.Collapsed;
            ComboBoxVisibility = Visibility.Collapsed;
            DetailsVisibility = Visibility.Collapsed;
            isInitialize = false;
            Tag = name;
        }
        public AlgorithmProperty(string name, List<string> selections, string description = "The default property desciption.")
        {
            parameterName = name;
            this.description = description;
            Selections = selections;
            selectIndex = 0;
        }

        public void updateSelectIndex(int idx)
        {
            selectIndex = idx;
        }

        public void resetCurrentValue()
        {
            currentValue = (maxValue + minValue) / 2;
        }

    }
}
