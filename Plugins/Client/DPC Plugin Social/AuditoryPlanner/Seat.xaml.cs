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

namespace DatumPoint.Plugins.Social.AuditoryPlanner
{
    /// <summary>
    /// Provides UI to certain seat managment.
    /// </summary>
    public partial class Seat : UserControl
    {
        public static readonly DependencyProperty MetaProperty = DependencyProperty.Register(
          "Meta", typeof(Types.AuditoryPlanner.Seat), typeof(Seat));
        
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(
          "Index", typeof(int), typeof(Seat));

        public static readonly DependencyProperty ReservationProperty = DependencyProperty.Register(
          "Reservation", typeof(Types.AuditoryPlanner.SeatReservation), typeof(Seat));

        /// <summary>
        /// Meta data of that place that contains such information like a number.
        /// </summary>
        public Types.AuditoryPlanner.Seat Meta
        {
            get { return (Types.AuditoryPlanner.Seat)this.GetValue(MetaProperty); }
            set 
            {
                // Updating value.
                this.SetValue(MetaProperty, value);

                // Updating displayed lable.
                UpdateIndexLable();
            }
        }

        /// <summary>
        /// Index of that place.
        /// -1 if undefined.
        /// </summary>
        public int Index
        {
            get
            {
                if(Meta != null)
                {
                    return Meta.index;
                }
                return -1;
            }
            set
            {
                // Create meta if not created.
                if (Meta == null)
                {
                    Meta = new Types.AuditoryPlanner.Seat();
                }
                // Update index.
                Meta.index = value;

                // Update lable.
                UpdateIndexLable();
            }
        }

        /// <summary>
        /// Reservation settings appliet to that seat.
        /// </summary>
        public Types.AuditoryPlanner.SeatReservation Reservation
        {
            get { return (Types.AuditoryPlanner.SeatReservation)this.GetValue(ReservationProperty); }
            set 
            {
                // Store value to the property.
                this.SetValue(ReservationProperty, value);

                // Update visual state.
                if (value != null && value.userId != 0)
                {
                    State = Types.AuditoryPlanner.Seat.State.Reserved;
                }
                else
                {
                    State = Types.AuditoryPlanner.Seat.State.Free;
                }
            }
        }

        public Seat()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Callbeck that will be used when user click on seat.
        /// Call interface of seat managment:
        /// -Set place as occupied\free.
        /// -Look who reserved occupied place.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeatManagmentButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Curernt state of the seat.
        /// In casde if defined Reservation object and it contains user id then mark place as reserved.
        /// In other ccase work if llocal state value that could be overrided.
        /// </summary>
        public Types.AuditoryPlanner.Seat.State State
        {
            get
            {
                // If reservation for that seat existed.
                if(Reservation != null)
                {
                    // If user is not exist.
                    if (Reservation.userId == 0) return Types.AuditoryPlanner.Seat.State.Free;
                    // If user defined.
                    else return Types.AuditoryPlanner.Seat.State.Reserved;
                }

                // Return bufer if we work in edit mode without reservations.
                return _State;
            }
            set
            {
                // Update local bufer using for displaying data.
                _State = value;

                // Update seat's meta if conected.
                if(Meta != null) Meta.state = value;
            }
        }
        /// <summary>
        /// Bufer that contains custom step in ccase if user works not with reservation.
        /// </summary>
        private Types.AuditoryPlanner.Seat.State _State;

        /// <summary>
        /// Updating lable that displaying index.
        /// </summary>
        protected void UpdateIndexLable()
        {
            if(Index <= 0)
            {
                indexLable.Content = "-";
            }
            else
            {
                indexLable.Content = Index.ToString();
            }
        }
    }
}
