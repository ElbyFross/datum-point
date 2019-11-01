//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatumPoint.Plugins.Social.Schedule
{
    /// <summary>
    /// Interaction logic for ScheduleRow.xaml
    /// </summary>
    public partial class ScheduleRow : UserControl
    {
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register(
         "FontSize", typeof(TimeSpan), typeof(ScheduleRow));

        public static readonly DependencyProperty SubjectLableProperty = DependencyProperty.Register(
         "SubjectLable", typeof(string), typeof(ScheduleRow));

        public static readonly DependencyProperty BuildingLableProperty = DependencyProperty.Register(
         "BuildingLable", typeof(string), typeof(ScheduleRow));

        public static readonly DependencyProperty AuditoryLableProperty = DependencyProperty.Register(
         "AuditoryLable", typeof(string), typeof(ScheduleRow));

        /// <summary>
        /// Time when lesson will start.
        /// </summary>
        public TimeSpan StartTime
        {
            get { return (TimeSpan)this.GetValue(StartTimeProperty); }
            set { this.SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// Title of subject.
        /// </summary>
        public string SubjectLable
        {
            get { return (string)this.GetValue(SubjectLableProperty); }
            set { this.SetValue(SubjectLableProperty, value); }
        }

        /// <summary>
        /// Name of building.
        /// </summary>
        public string BuildingLable
        {
            get { return (string)this.GetValue(BuildingLableProperty); }
            set { this.SetValue(BuildingLableProperty, value); }
        }

        /// <summary>
        /// Name of room\auditory.
        /// </summary>
        public string AuditoryLable
        {
            get { return (string)this.GetValue(AuditoryLableProperty); }
            set { this.SetValue(AuditoryLableProperty, value); }
        }


        public ScheduleRow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Callback that will be calling when user click on auditory button.
        /// Will open auditory scheme and open access to managmet features:
        /// -Place reservation.
        /// -Infoorm about not enought count of places in case of full fulling of auditory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuditoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Open information about building.
        /// Recommended informations:
        /// -Logo.
        /// -Adress.
        /// -Contacts.
        /// -Resposible persons.
        /// -Description.
        /// -Photos.
        /// -Evacuation plans.
        /// -Logistic recommendations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Will open access to managment of data and information relative to that lesson.
        /// -Provides data about teacher
        /// -Allow downloading of the hometask if exist.
        /// -Allow to inform about abscent. 
        /// -Allow to upload documents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubjectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void reservedPlaceButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
