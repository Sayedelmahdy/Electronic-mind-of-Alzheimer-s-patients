using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class SecretAndImportantFile
    {
        [Key]
        public int File_Id { get; set; }
        public string FileName { get; set; }
        public string File_Description { get; set; }
        public string DocumentPath { get; set; }
        #region Navigation Prop
        public Patient patient { get; set; }
        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        #endregion
    }
}
