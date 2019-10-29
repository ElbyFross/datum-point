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
using System.Data.Common;
using System.Text;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// Class that redefining base BanInformation's sql table description.
    /// </summary>
    [Serializable]
    [Table("datum-point", "bans")]
    // Override base ban info type to prevent using of different databases.
    [UniformDataOperator.Modifiers.TypeReplacer(typeof(AuthorityController.Data.Personal.BanInformation), typeof(BanInformation), 500)]
    public class BanInformation : AuthorityController.Data.Personal.BanInformation
    {
    }
}
