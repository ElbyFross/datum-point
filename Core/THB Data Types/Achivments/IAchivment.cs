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

namespace DatumPoint.Types.Achivments
{        
    /// <summary>
    /// Implementing of this interface provide possiblity to add achivment handler to your users.
    /// </summary>
    public interface IAchivment : WpfHandler.Plugins.IPlugin
    {
        /// <summary>
        /// Current state of achivment. (Achived or not).
        /// </summary>
        bool State { get; }

        /// <summary>
        /// Current progress of this achivment.
        /// [0.0f, 1.0f]
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Async check of currnet state.
        /// </summary>
        void CheckAsync();
    }
}
