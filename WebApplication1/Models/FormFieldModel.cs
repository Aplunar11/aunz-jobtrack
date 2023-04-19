using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTrack.Models
{
    public class FormFieldModel
    {
        public string Id { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public string TempDataName { get; set; }

        public string Class { get; set; }

        public bool IsReadOnly { get; set; }

        public bool DontDisplay { get; set; }       

        public bool IsRequired { get; set; }

        public int Rows { get; set; }

        public bool IsLeftButton { get; set; }

        public string ButtonText { get; set; }

        public string OnChangeEvent { get; set; }

        public string OnClickEvent { get; set; }

        public FormFieldModel()
        {
            Rows = 3;
        }
    }
}