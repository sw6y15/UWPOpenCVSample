using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using OpenCvSharp;
using System.Collections.ObjectModel;

namespace UWPOpenCVSample
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "OpenCV C# Sample";

        public List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Camera Example Operations", ClassType=typeof(Scenario1_ExampleOperations)},
            new Scenario() { Title="Image Example Operations", ClassType=typeof(Scenario2_ImageOperations)},
        };

        public List<Algorithm> algorithms = new List<Algorithm>
        {
            new Algorithm()
            {
                AlgorithmName ="Blur",
                algorithmProperties = new List<AlgorithmProperty>()
                {
                    //Cv2.Blur()
                    new AlgorithmProperty(0,typeof(Size),"ksize","A Size object representing the size of the kernel.",480,1,5),
                    new AlgorithmProperty(1,typeof(Point),"anchor","A variable of the type integer representing the anchor point.",5,1,3),
                    new AlgorithmProperty(2,typeof(BorderTypes),"borderType","A variable of the type integer representing the type of the border to be used to the output.",7,0,0),
                }
            },
            new Algorithm()
            {

                AlgorithmName ="HoughLines",
                algorithmProperties = new List<AlgorithmProperty>()
                {
                    //Cv2.HoughLinesP()
                    //Cv2.Line()
                    new AlgorithmProperty(0,typeof(double),"rho","A variable of the type double representing the resolution of the parameter r in pixels.",480,1,1),
                    new AlgorithmProperty(1,typeof(double),"theta","A variable of the type double representing the resolution of the parameter Φ in radians.",157,1,1),
                    new AlgorithmProperty(2,typeof(int),"threshold","A variable of the type integer representing the minimum number of intersections to “detect” a line.",480,1,10),
                    new AlgorithmProperty(3,typeof(double),"minLineLength","The minimum line length. Line segments shorter than that will be rejected. [By default this is 0]",480,0,0),
                    new AlgorithmProperty(4,typeof(double),"maxLineGap","The maximum allowed gap between points on the same line to link them. [By default this is 0]",480,0,0),
                    new AlgorithmProperty(5,typeof(Scalar),"color","Line color.",255,0,0),
                    new AlgorithmProperty(6,typeof(int),"thickness","Line thickness. [By default this is 1]",10,0,1),
                    new AlgorithmProperty(7,typeof(LineTypes),"lineType","Type of the line. [By default this is LineType.Link8]",2,0,1),
                }
            },
            new Algorithm()
            {
                //Cv2.FindContours() 
                //Cv2.DrawContours()
                AlgorithmName ="Contours",
                algorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"threshold1"),
                    new AlgorithmProperty(1,typeof(double),"threshold2"),
                    new AlgorithmProperty(2,typeof(double),"threshold3"),
                }
            },
            new Algorithm()
            {

                AlgorithmName ="Histogram",
                algorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"threshold1"),
                    new AlgorithmProperty(1,typeof(double),"threshold2"),
                    new AlgorithmProperty(2,typeof(double),"threshold3"),
                }
            },
            new Algorithm()
            {

                AlgorithmName ="MotionDetector",
                algorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"threshold1"),
                    new AlgorithmProperty(1,typeof(double),"threshold2"),
                    new AlgorithmProperty(2,typeof(double),"threshold3"),
                }
            },
        };
    }

    public enum AlgorithmPropertyType
    {
        currentValue = 0,
        parameterName,
        description,
        settingVisibility,
        maxValue,
        minValue
    }
    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
