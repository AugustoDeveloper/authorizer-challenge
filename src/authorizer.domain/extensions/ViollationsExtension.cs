using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Extensions
{
    /// <summary>
    /// Extension class to make easy 
    /// to get all description of a violation or
    /// collection of it
    /// </summary>
    public static class ViolationsExtension
    {
        /// <summary>
        /// Get all descriptions of entire violations
        /// </summary>
        /// <param name="violations"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetViolationDescriptions(this Violations violations)
        {
            return violations.Select(GetViolationDescription).ToArray();
        }

        /// <summary>
        /// Get a description of one violation
        /// </summary>
        /// <param name="violation"></param>
        /// <returns></returns>
        public static string GetViolationDescription(this Violation violation)
        {
            FieldInfo fi = violation.GetType().GetField(violation.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes.Select(c => c.Description).First();
        }
    }
}