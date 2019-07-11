using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.AuthorityController
{
    [TestClass]
    public class Regex
    {
        /// <summary>
        /// Validate name in format
        /// Name-name
        /// </summary>
        [TestMethod]
        public void ComplexNameValidation_Type1()
        {
        }

        /// <summary>
        /// Validate name in format
        /// Name'name
        /// </summary>
        [TestMethod]
        public void ComplexNameValidation_Type2()
        {
        }

        /// <summary>
        /// Check base password type.
        /// </summary>
        [TestMethod]
        public void ValidatePassord_Base()
        {
        }

        /// <summary>
        /// Check filter when enabled upper case required.
        /// </summary>
        [TestMethod]
        public void ValidatePassord_UpperCase()
        {
        }

        /// <summary>
        /// Check filter when enabled special symbol required.
        /// </summary>
        [TestMethod]
        public void ValidatePassord_SpecialSymbol()
        {
        }
        
        /// <summary>
        /// Check filter when enabled digits required.
        /// </summary>
        [TestMethod]
        public void ValidatePassord_Digits()
        {

        }
    }
}
