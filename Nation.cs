//-----------------------------------------------------------------------
// <copyright file="Nation.cs" company="Auralia">
//     Copyright (C) 2013 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.NationManager
{
    using System;

    /// <summary>
    /// Represents a nation.
    /// </summary>
    [Serializable]
    public class Nation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Nation"/> class.
        /// </summary>
        /// <param name="name">The nation's name.</param>
        /// <param name="password">The nation's password.</param>
        public Nation(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }

        /// <summary>
        /// Gets or sets the nation's name.
        /// </summary>
        /// <value>The nation's name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the nation's password.
        /// </summary>
        /// <value>The nation's password.</value>
        public string Password
        {
            get;
            set;
        }
    }
}
