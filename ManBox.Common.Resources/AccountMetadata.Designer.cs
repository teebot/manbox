﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManBox.Common.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AccountMetadata {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccountMetadata() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ManBox.Common.Resources.AccountMetadata", typeof(AccountMetadata).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-mail invalide.
        /// </summary>
        public static string ErrorEmailFormat {
            get {
                return ResourceManager.GetString("ErrorEmailFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-mail obligatoire.
        /// </summary>
        public static string ErrorEmailRequired {
            get {
                return ResourceManager.GetString("ErrorEmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ouch pas de prénom!.
        /// </summary>
        public static string ErrorFirstNameRequired {
            get {
                return ResourceManager.GetString("ErrorFirstNameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Prénom trop long (twss).
        /// </summary>
        public static string ErrorFirstNameTooLong {
            get {
                return ResourceManager.GetString("ErrorFirstNameTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vous avez oublié de nous laisser un message?.
        /// </summary>
        public static string ErrorMessageRequired {
            get {
                return ResourceManager.GetString("ErrorMessageRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-mail.
        /// </summary>
        public static string LabelEmail {
            get {
                return ResourceManager.GetString("LabelEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Prénom.
        /// </summary>
        public static string LabelFirstName {
            get {
                return ResourceManager.GetString("LabelFirstName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Votre message.
        /// </summary>
        public static string LabelMessage {
            get {
                return ResourceManager.GetString("LabelMessage", resourceCulture);
            }
        }
    }
}
