﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Mark_Medicine_Reminder
    {
        [Key]
        public string MarkId { get; set; } = Guid.NewGuid().ToString();
        public Medication_Reminders medication_Reminder { get; set; }
        [ForeignKey(nameof(Medication_Reminders))]
        public int ReminderId { get; set; }
        public bool IsTaken { get; set; }
        public DateTime MarkTime { get; set; }
    }
}
