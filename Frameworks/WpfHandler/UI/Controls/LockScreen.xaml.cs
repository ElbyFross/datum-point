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
using System.Collections;
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


namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for LockScreen.xaml
    /// </summary>
    public partial class LockScreen : UserControl
    {
        /// <summary>
        /// Elements that would locked.
        /// </summary>
        protected FrameworkElement[] lockedEllements;

        /// <summary>
        /// How many time will take blur animation.
        /// </summary>
        public TimeSpan lockAnimationDuration = new TimeSpan(0, 0, 0, 0, 300);

        /// <summary>
        /// Size of Gausian blur.
        /// </summary>
        public float blurSize = 20;

        /// <summary>
        /// Method that will has been calling during click on operation cancel button.
        /// </summary>
        public System.Action<object> OperationCancelCallback { get; set; }

        public LockScreen()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Lock screen and enable loading animation. 
        /// </summary>
        /// <param name="message">Message that wold be showed up during lock.</param>
        /// <param name="controls">elements that would be locked.</param>
        public void Lock(string message, params FrameworkElement[] controls)
        {
            if(lockedEllements != null)
            {
                throw new OperationCanceledException("Already locked. Unlock before using.");
            }

            // Buferize shared controls.
            lockedEllements = controls;

            // path to opacity property.
            PropertyPath opacityPropertyPath = new PropertyPath(Control.OpacityProperty);

            // Lock input.
            lockScreen.IsHitTestVisible = true;

            #region Unblock lock UI
            // Cancel button
            lockCancelButton.IsHitTestVisible = true;
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this, lockCancelButton.Name,
                opacityPropertyPath,
                lockAnimationDuration,
                0, 1);

            // Lable
            lockLable.Content = message;
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this, lockLable.Name,
                opacityPropertyPath,
                lockAnimationDuration,
                0, 1);
            #endregion

            // Blur locked elements.
            foreach (FrameworkElement c in controls)
            {
                WpfHandler.UI.Animations.Blur.BlurApply(c, blurSize, lockAnimationDuration, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Unlock screen.
        /// </summary>
        public void Unlock()
        {
            if (lockedEllements == null)
            {
                throw new OperationCanceledException("Panel isn't locked. Lock before using.");
            }
            
            // Path to opacity property.
            PropertyPath opacityPropertyPath = new PropertyPath(Control.OpacityProperty);

            // Unlock input.
            lockScreen.IsHitTestVisible = false;

            #region Block lock UI
            // Cancel button
            lockCancelButton.IsHitTestVisible = false;
            WpfHandler.UI.Animations.Float.FloatAniamtion(this,
                lockCancelButton.Name,
                opacityPropertyPath,
                lockAnimationDuration,
                1, 0);

            // Lock lable
            WpfHandler.UI.Animations.Float.FloatAniamtion(this,
                lockLable.Name,
                opacityPropertyPath,
                lockAnimationDuration,
                1, 0);
            #endregion

            // Unlock elements.
            foreach (FrameworkElement c in lockedEllements)
            {
                WpfHandler.UI.Animations.Blur.BlurDisable(c, lockAnimationDuration, TimeSpan.Zero);
            }

            // Drop buffer.
            lockedEllements = null;
        }

        private void LockCancelCallbackHandler(object sender)
        {
            OperationCancelCallback?.Invoke(sender);
            Unlock();
        }
    }
}
