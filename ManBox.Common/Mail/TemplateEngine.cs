using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using RazorEngine;

namespace ManBox.Common.Mail
{
    public class TemplateEngine<T>
    {
        private readonly T _templateModel;
        private readonly string _templateFilePath;

        public TemplateEngine(T templateModel, string templateFilePath)
        {
            _templateModel = templateModel;
            _templateFilePath = templateFilePath;
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            var output = LoadTemplate();
            output = Razor.Parse(output, _templateModel);
            return output;
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        /// <returns></returns>
        private string LoadTemplate()
        {
            return File.ReadAllText(_templateFilePath);
        }
    }
}