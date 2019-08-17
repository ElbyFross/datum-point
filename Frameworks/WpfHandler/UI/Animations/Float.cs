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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Controls;

namespace WpfHandler.UI.Animations
{
    /// <summary>
    /// Provide base float animations operations.
    /// </summary>
    public static class Float
    {
        /// <summary>
        /// Start float animation.
        /// </summary>
        /// <param name="parent">Object that contain property.</param>
        /// <param name="targetName">A name of the property.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time wold take transit.</param>
        /// <param name="from">Start value,</param>
        /// <param name="to">Finish value.</param>
        public static void FloatAniamtion(
            FrameworkElement parent, 
            string targetName, 
            PropertyPath propertyPath,
            TimeSpan duration,
            float from, float to)
        {
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard();

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new Duration(duration);

            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, targetName);
            Storyboard.SetTargetProperty(animation, propertyPath);
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            storyboard.Begin(parent);
        }
    }
}
