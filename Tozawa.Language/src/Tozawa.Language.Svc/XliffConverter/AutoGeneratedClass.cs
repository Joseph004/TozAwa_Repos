/*------------------------------------------------------------------------------
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE LanguageORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
------------------------------------------------------------------------------*/

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tozawa.Language.Svc.XliffConverter
{

    /// <remarks/>
    [GeneratedCode("System.Xml", "4.0.30319.34234")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class xliff
    {
        private xliffFile fileField;
        private string headerField;
        private xliffTransunit[] bodyField;
        private float versionField;
        private bool versionFieldSpecified;
        /// <remarks/>
        public xliffFile file
        {
            get
            {
                return fileField;
            }
            set
            {
                fileField = value;
            }
        }
        /// <remarks/>
        public string header
        {
            get
            {
                return headerField;
            }
            set
            {
                headerField = value;
            }
        }
        /// <remarks/>
        [XmlArrayItem("trans-unit", IsNullable = false)]
        public xliffTransunit[] body
        {
            get
            {
                return bodyField;
            }
            set
            {
                bodyField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute]
        public float version
        {
            get
            {
                return versionField;
            }
            set
            {
                versionField = value;
            }
        }
        /// <remarks/>
        [XmlIgnore]
        public bool versionSpecified
        {
            get
            {
                return versionFieldSpecified;
            }
            set
            {
                versionFieldSpecified = value;
            }
        }
    }
    /// <remarks/>
    [GeneratedCode("System.Xml", "4.0.30319.34234")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class xliffFile
    {
        private string sourcelanguageField;
        private string targetlanguageField;
        private string datatypeField;
        private string originalField;
        private DateTime dateField;
        private bool dateFieldSpecified;
        private string valueField;
        /// <remarks/>
        [XmlAttribute("source-language")]
        public string sourcelanguage
        {
            get
            {
                return sourcelanguageField;
            }
            set
            {
                sourcelanguageField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute("target-language")]
        public string targetlanguage
        {
            get
            {
                return targetlanguageField;
            }
            set
            {
                targetlanguageField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute]
        public string datatype
        {
            get
            {
                return datatypeField;
            }
            set
            {
                datatypeField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute]
        public string original
        {
            get
            {
                return originalField;
            }
            set
            {
                originalField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute]
        public DateTime date
        {
            get
            {
                return dateField;
            }
            set
            {
                dateField = value;
            }
        }
        /// <remarks/>
        [XmlIgnore]
        public bool dateSpecified
        {
            get
            {
                return dateFieldSpecified;
            }
            set
            {
                dateFieldSpecified = value;
            }
        }
        /// <remarks/>
        [XmlText]
        public string Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }
    /// <remarks/>
    [GeneratedCode("System.Xml", "4.0.30319.34234")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class xliffTransunit
    {
        private string sourceField;
        private xliffTransunitTarget targetField;
        private sbyte idField;
        private bool idFieldSpecified;
        private string spaceField;
        /// <remarks/>
        public string source
        {
            get
            {
                return sourceField;
            }
            set
            {
                sourceField = value;
            }
        }
        /// <remarks/>
        public xliffTransunitTarget target
        {
            get
            {
                return targetField;
            }
            set
            {
                targetField = value;
            }
        }
        /// <remarks/>
        [XmlAttribute]
        public sbyte id
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }
        /// <remarks/>
        [XmlIgnore]
        public bool idSpecified
        {
            get
            {
                return idFieldSpecified;
            }
            set
            {
                idFieldSpecified = value;
            }
        }
        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return spaceField;
            }
            set
            {
                spaceField = value;
            }
        }
    }
    /// <remarks/>
    [GeneratedCode("System.Xml", "4.0.30319.34234")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class xliffTransunitTarget
    {
        private string stateField;
        private string valueField;
        /// <remarks/>
        [XmlAttribute]
        public string state
        {
            get
            {
                return stateField;
            }
            set
            {
                stateField = value;
            }
        }
        /// <remarks/>
        [XmlText]
        public string Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }
}
